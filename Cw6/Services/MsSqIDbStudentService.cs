using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Cw6.DTOs;
using Cw6.DTOs;
using Cw6.Models;

namespace Cw6.Services
{
    public class MsSqlDbStudentService : IDbStudentService
    {
        private const string ConnectionString = "Data Source=db-mssql;Initial Catalog=s19841;Integrated Security=True";

        public IEnumerable<Student> GetAllStudents()
        {
            using var sqlConnection = new SqlConnection(ConnectionString);
            using var sqlCommand = new SqlCommand { Connection = sqlConnection };
            sqlConnection.Open();
            return new MsSqlDbGetStudentService(sqlCommand).GetAllStudents();
        }

        public GetSingleStudentResponse GetStudent(string indexNumber)
        {
            using var sqlConnection = new SqlConnection(ConnectionString);
            using var sqlCommand = new SqlCommand { Connection = sqlConnection };
            sqlConnection.Open();
            return new MsSqlDbGetStudentService(sqlCommand).GetStudent(indexNumber);
        }

        public EnrollmentResult EnrollStudent(EnrollStudentRequest newStudent)
        {
            using var sqlConnection = new SqlConnection(ConnectionString);
            using var sqlCommand = new SqlCommand { Connection = sqlConnection };
            sqlConnection.Open();
            sqlCommand.Transaction = sqlConnection.BeginTransaction();
            try
            {
                var enrollmentResult = new MsSqlDbEnrollmentStudentService(sqlCommand, newStudent).Enroll();
                if (enrollmentResult.Successful) sqlCommand.Transaction.Commit();
                else sqlCommand.Transaction.Rollback();
                return enrollmentResult;
            }
            catch (Exception exception)
            {
                sqlCommand.Transaction.Rollback();
                return new EnrollmentResult { Error = $"Exepion found while adding student - {exception}!" };
            }
        }

        public Enrolled PromoteStudents(PromoteStudentsRequest promoteStudentsRequest)
        {
            using var sqlConnection = new SqlConnection(ConnectionString);
            using var sqlCommand = new SqlCommand("PromoteStudents", sqlConnection)
            { CommandType = CommandType.StoredProcedure };
            sqlCommand.Parameters.AddWithValue("@Studies", promoteStudentsRequest.Studies);
            sqlCommand.Parameters.AddWithValue("@Semester", promoteStudentsRequest.Semester);
            sqlConnection.Open();
            var sqlDataReader = sqlCommand.ExecuteReader();
            if (!sqlDataReader.Read()) return null;
            return new Enrolled
            {
                IdEnrollment = (int)sqlDataReader["IdEnrollment"],
                Semester = (int)sqlDataReader["Semester"],
                IdStudy = (int)sqlDataReader["IdStudy"],
                StudiesName = sqlDataReader["Name"].ToString(),
                StartDate = DateTime.Parse(sqlDataReader["StartDate"].ToString()!)
            };
        }
    }
}