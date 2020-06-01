using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalInformationManagementSystem.Models
{
    //Done by Zainab Hteit
    public class DoctorViewModel
    {
        public RegisterViewModel ApplicationUser { get; set; }
        public Doctor Doctor { get; set; }
        public IEnumerable<Department> Departments { get; set; }
    }
}