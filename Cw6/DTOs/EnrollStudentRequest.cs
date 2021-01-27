using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cw6.DTOs
{
    public class EnrollStudentRequest
    {
        [Required(ErrorMessage = "Index number is mandatory to add new student!")]
        [MaxLength(100)]
        [RegularExpression("^s[\\d]+$")]
        public string IndexNumber { get; set; }

        [Required(ErrorMessage = "Name is mandatory to add new student!")]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Surname is mandatory to add new student!")]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Birthdate is mandatory to add new student!")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Studies name is mandatory to add new student!")]
        [MaxLength(100)]
        public string Studies { get; set; }
    }
}