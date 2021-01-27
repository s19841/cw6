using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cw6.DTOs
{
    public class PromoteStudentsRequest
    {
        [Required(ErrorMessage = "Studies name is mandatory to promote the student!")]
        [MaxLength(100)]
        public string Studies { get; set; }

        [Required(ErrorMessage = "Semester number is mandatory to promote the student!")]
        public int Semester { get; set; }
    }
}