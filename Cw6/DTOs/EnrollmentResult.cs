using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cw6.Models;

namespace Cw6.DTOs
{
    public class EnrollmentResult
    {
        public bool Successful { get; set; }
        public Student Student { get; set; }
        public Enrolled Enrolled { get; set; }
        public string Error { get; set; }
    }
}