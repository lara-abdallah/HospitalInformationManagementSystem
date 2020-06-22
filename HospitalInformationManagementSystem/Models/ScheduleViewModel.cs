using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalInformationManagementSystem.Models
{
    public class ScheduleViewModel
    {
        public Schedule Schedule { get; set; }
        public IEnumerable<Doctor> Doctors { get; set; }
    }
}