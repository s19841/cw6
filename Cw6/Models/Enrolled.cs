using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw6.Models
{
    public class Enrolled
    {
        public int IdEnrollment { get; set; }
        public int Semester { get; set; }
        public int IdStudy { get; set; }
        public string StudiesName { get; set; }
        public DateTime StartDate { get; set; }
    }
}