using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.Entities
{
    public class Student
    {
        public int StudentId { get; set; }
        public string AdmissionNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ClassName { get; set; }
        public string Section { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string GuardianName { get; set; }
        public string HealthInfo { get; set; }
        public string PhotoUrl { get; set; }
        public int? UserId { get; set; }
    }
}