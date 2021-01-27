using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cw6.DTOs;
using Cw6.Exceptions;
using Cw6.Models;

namespace Cw6.Services
{
    internal class MsSqlDbEnrollmentStudentService
    {
        private const string StudiesFilterNameQuery =
            "SELECT IdStudy, Name FROM Studies WHERE Name = @StudiesName";

        private const string StudentFilterIndexNumberQuery =
            "SELECT IndexNumber FROM Student WHERE IndexNumber = @IndexNumber";

        private const string FirstSemesterEnrollmentForStudiesQuery =
            "SELECT IdEnrollment, Semester, Enrollment.IdStudy, StartDate FROM Enrollment " +
            "INNER JOIN Studies on Studies.IdStudy = Enrollment.IdStudy " +
            "WHERE Studies.Name = @StudiesName AND Semester = 1";

        private const string SelectLastIdEnrollment =
            "SELECT TOP 1 IdEnrollment FROM Enrollment ORDER BY IdEnrollment DESC;";

        private const string InsertFirstSemesterEnrollmentForStudiesQuery =
            "INSERT INTO Enrollment (IdEnrollment, Semester, IdStudy, StartDate) " +
            "SELECT @IdEnrollment, 1, IdStudy, @EnrollmentDate FROM Studies WHERE Studies.Name = @StudiesName";

        private const string InsertStudentQuery =
            "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) " +
            "VALUES (@IndexNumber, @FirstName, @LastName, @BirthDate, @IdEnrollment)";

        private readonly EnrollStudentRequest _enrollRequest;
        private readonly SqlCommand _sqlCommand;
        private string _enrollmentDate;

        public MsSqlDbEnrollmentStudentService(SqlCommand sqlCommand, EnrollStudentRequest enrollRequest)
        {
            _sqlCommand = sqlCommand;
            _enrollRequest = enrollRequest;
        }

        public EnrollmentResult Enroll()
        {
            _enrollmentDate = DateTime.Now.ToLongDateString();
            var studies = GetStudies();
            if (studies == null)
                return new EnrollmentResult { Error = $"Studies {_enrollRequest.Studies} no exists" };
            if (IndexNumberExistsInDatabase())
                return new EnrollmentResult { Error = $"Index number {_enrollRequest.IndexNumber} already exist" };
            var enrolled = GetEnrollmentForNewStudent(studies);
            var newStudent = new Student
            {
                IndexNumber = _enrollRequest.IndexNumber,
                FirstName = _enrollRequest.FirstName,
                LastName = _enrollRequest.LastName,
                BirthDate = _enrollRequest.BirthDate,
                IdEnrollment = enrolled.IdEnrollment
            };
            InsertNewStudentIntoDb(newStudent);
            return new EnrollmentResult
            {
                Successful = true,
                Student = newStudent,
                Enrolled = enrolled
            };
        }

        private Studies GetStudies()
        {
            _sqlCommand.CommandText = StudiesFilterNameQuery;
            _sqlCommand.Parameters.AddWithValue("StudiesName", _enrollRequest.Studies);
            using var sqlDataReader = _sqlCommand.ExecuteReader();
            return sqlDataReader.Read()
                ? new Studies { IdStudy = (int)sqlDataReader["IdStudy"], Name = sqlDataReader["Name"].ToString() }
                : null;
        }

        private bool IndexNumberExistsInDatabase()
        {
            _sqlCommand.CommandText = StudentFilterIndexNumberQuery;
            _sqlCommand.Parameters.AddWithValue("IndexNumber", _enrollRequest.IndexNumber);
            using var sqlDataReader = _sqlCommand.ExecuteReader();
            return sqlDataReader.Read();
        }

        private Enrolled GetEnrollmentForNewStudent(Studies studies)
        {
            _sqlCommand.CommandText = FirstSemesterEnrollmentForStudiesQuery;
            using var sqlDataReader = _sqlCommand.ExecuteReader();
            if (sqlDataReader.Read())
                return new Enrolled
                {
                    IdEnrollment = (int)sqlDataReader["IdEnrollment"],
                    Semester = (int)sqlDataReader["Semester"],
                    IdStudy = (int)sqlDataReader["IdStudy"],
                    StudiesName = _enrollRequest.Studies,
                    StartDate = DateTime.Parse(sqlDataReader["StartDate"].ToString()!)
                };
            return PrepareNewEnrollment(studies);
        }

        private Enrolled PrepareNewEnrollment(Studies studies)
        {
            _sqlCommand.CommandText = InsertFirstSemesterEnrollmentForStudiesQuery;
            var nextIdEnrollment = NextEnrollmentId();
            _sqlCommand.Parameters.AddWithValue("IdEnrollment", nextIdEnrollment);
            _sqlCommand.Parameters.AddWithValue("EnrollmentDate", _enrollmentDate);
            if (_sqlCommand.ExecuteNonQuery() == 0)
                throw new SqlInsertException("Error adding new element to tab \"Enrollment\"!");
            return new Enrolled
            {
                IdEnrollment = nextIdEnrollment,
                Semester = 1,
                IdStudy = studies.IdStudy,
                StartDate = DateTime.Parse(_enrollmentDate)
            };
        }

        private int NextEnrollmentId()
        {
            _sqlCommand.CommandText = SelectLastIdEnrollment;
            using var sqlDataReader = _sqlCommand.ExecuteReader();
            return sqlDataReader.Read() ? (int)sqlDataReader["IdEnrollment"] + 1 : 1;
        }

        private void InsertNewStudentIntoDb(Student newStudent)
        {
            _sqlCommand.CommandText = InsertStudentQuery;
            _sqlCommand.Parameters.AddWithValue("FirstName", _enrollRequest.FirstName);
            _sqlCommand.Parameters.AddWithValue("LastName", _enrollRequest.LastName);
            _sqlCommand.Parameters.AddWithValue("BirthDate", _enrollRequest.BirthDate);
            if (!_sqlCommand.Parameters.Contains("IdEnrollment"))
                _sqlCommand.Parameters.AddWithValue("IdEnrollment", newStudent.IdEnrollment);
            if (_sqlCommand.ExecuteNonQuery() == 0)
                throw new SqlInsertException("Error adding new element to tab \"Enrollment\"!");
        }
    }
}