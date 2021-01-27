using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cw6.DTOs;
using Cw6.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cw6.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IDbStudentService _dbService;

        public EnrollmentsController(IDbStudentService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost]
        public IActionResult Enroll(EnrollStudentRequest newStudentRequest)
        {
            var enrollmentResult = _dbService.EnrollStudent(newStudentRequest);
            if (enrollmentResult.Successful)
                return Created($"/api/students/{newStudentRequest.IndexNumber}", enrollmentResult.Enrolled);
            return BadRequest(enrollmentResult.Error);
        }

        [HttpPost("promotions")]
        public IActionResult PromoteStudents(PromoteStudentsRequest promoteStudentsRequest)
        {
            var newEnrollment = _dbService.PromoteStudents(promoteStudentsRequest);
            if (newEnrollment != null) return Created("/api/students", newEnrollment);
            return BadRequest($"No record of '{promoteStudentsRequest.Studies}' in database " + $"and semester {promoteStudentsRequest.Semester}");
        }
    }
}