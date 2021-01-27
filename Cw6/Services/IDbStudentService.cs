using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cw6.DTOs;
using Cw6.Models;
using Cw6.Services;

namespace Cw6.Services
{
    public interface IDbStudentService
    {
        public IEnumerable<Student> GetAllStudents();
        public GetSingleStudentResponse GetStudent(string indexNumber);
        public EnrollmentResult EnrollStudent(EnrollStudentRequest newStudent);
        public Enrolled PromoteStudents(PromoteStudentsRequest promoteStudentsRequest);
    }
}