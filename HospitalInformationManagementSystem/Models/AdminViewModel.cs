using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalInformationManagementSystem.Models
{
    //Done by Zainab Hteit
    public class AdminViewModel
    {
        public IEnumerable<Department> Departments { get; set; }
        public IEnumerable<Doctor> Doctors { get; set; }
        public IEnumerable<Patient> Patients { get; set; }
        public IEnumerable<Appointment> ActiveAppointments { get; set; }
        public IEnumerable<Appointment> PendingAppointments { get; set; }
        public IEnumerable<Announcement> Announcements { get; set; }
    }
}