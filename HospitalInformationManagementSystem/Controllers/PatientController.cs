using HospitalInformationManagementSystem.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;


namespace HospitalInformationManagementSystem.Controllers
{
  //Done by Lara Abdallah
    public class PatientController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Patient

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        private ApplicationUserManager _userManager;

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        [Authorize(Roles = "Patient")]
        public ActionResult Index(string message)
        {
            ViewBag.Messege = message;
            string user = User.Identity.GetUserId();
            var patient = db.Patients.Single(c => c.ApplicationUserId == user);
            var date = DateTime.Now.Date;
            var model = new AdminViewModel
            {
                Departments = db.Department.ToList(),
                Doctors = db.Doctors.ToList(),
                Patients = db.Patients.ToList(),
                ActiveAppointments = db.Appointments.Where(c => c.Status).Where(c => c.PatientId == patient.Id).Where(c => c.AppointmentDate >= date).ToList(),
                PendingAppointments = db.Appointments.Where(c => c.Status == false).Where(c => c.PatientId == patient.Id).Where(c => c.AppointmentDate >= date).ToList(),
                Announcements = db.Announcements.Where(c => c.AnnouncementFor == "Patient").ToList()
            };
            return View(model);
        }

        [Authorize(Roles = "Patient")]
        public ActionResult AddAppointment()
        {
            var list = new AppointmentViewModel
            {
                Appointment = new Appointment(),
                Doctors = db.Doctors.ToList()
            };
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAppointment(AppointmentViewModel model)
        {
            var list = new AppointmentViewModel
            {
                Appointment = model.Appointment,
                Doctors = db.Doctors.ToList()
            };
            if (model.Appointment.AppointmentDate >= DateTime.Now.Date)
            {
                string user = User.Identity.GetUserId();
                var patient = db.Patients.Single(c => c.ApplicationUserId == user);
                var appointment = new Appointment();
                appointment.PatientId = patient.Id;
                appointment.DoctorId = model.Appointment.DoctorId;
                appointment.AppointmentDate = model.Appointment.AppointmentDate;
                appointment.Problem = model.Appointment.Problem;
                appointment.Status = false;

                db.Appointments.Add(appointment);
                db.SaveChanges();
                return RedirectToAction("ListOfAppointments");
            }
            ViewBag.Messege = "Please Enter the Date greater than today or equal!";

            return View(list);

        }

        [Authorize(Roles = "Patient")]
        public ActionResult ListOfAppointments()
        {
            string user = User.Identity.GetUserId();
            var patient = db.Patients.Single(c => c.ApplicationUserId == user);
            var appointment = db.Appointments.Include(c => c.Doctor).Where(c => c.PatientId == patient.Id).ToList();
            return View(appointment);
        }

        [Authorize(Roles = "Patient")]
        public ActionResult UpdateProfile(string id)
        {
            id = User.Identity.GetUserId();
            var patient = db.Patients.Single(c => c.ApplicationUserId == id);
            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(string id, Patient model)
        {
            id = User.Identity.GetUserId();
            var patient = db.Patients.Single(c => c.ApplicationUserId == id);
            patient.FirstName = model.FirstName;
            patient.LastName = model.LastName;
            patient.FullName = model.FirstName + " " + model.LastName;
            patient.Address = model.Address;
            patient.BloodGroup = model.BloodGroup;
            patient.DateOfBirth = model.DateOfBirth;
            patient.Gender = model.Gender;
            patient.PhoneNo = model.PhoneNo;
            db.SaveChanges();
            return RedirectToAction("Index", new { Message = "Profile has been successfully updated!" });
        }

        [Authorize(Roles = "Patient")]
        public ActionResult AvailableDoctors()
        {
            var doctor = db.Doctors.Include(c => c.Department).Where(c => c.Status == "Active").ToList();
            return View(doctor);
        }


        [Authorize(Roles = "Patient")]
        public ActionResult DoctorSchedule(int id)
        {
            var schedule = db.Schedules.Include(c => c.Doctor).Single(c => c.DoctorId == id);

            return View(schedule);
        }


        [Authorize(Roles = "Patient")]
        public ActionResult DoctorDetail(int id)
        {
            var doctor = db.Doctors.Include(c => c.Department).Single(c => c.Id == id);
            return View(doctor);
        }

            [Authorize(Roles = "Patient")]
        public ActionResult EditAppointment(int id)
        {
            var appointments = new AppointmentViewModel
            {
                Appointment = db.Appointments.Single(c => c.Id == id),
                Doctors = db.Doctors.ToList()
            };
            return View(appointments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAppointment(int id, AppointmentViewModel model)
        {
            var appointments = new AppointmentViewModel
            {
                Appointment = model.Appointment,
                Doctors = db.Doctors.ToList()
            };
            if (model.Appointment.AppointmentDate >= DateTime.Now.Date)
            {
                var appointment = db.Appointments.Single(c => c.Id == id);
                appointment.DoctorId = model.Appointment.DoctorId;
                appointment.AppointmentDate = model.Appointment.AppointmentDate;
                appointment.Problem = model.Appointment.Problem;
                db.SaveChanges();
                return RedirectToAction("ListOfAppointments");
            }
            ViewBag.Messege = "Please Enter the Date greater than today or equal!!";

            return View(appointments);
        }

        [Authorize(Roles = "Patient")]
        public ActionResult DeleteAppointment(int? id)
        {
            var appointment = db.Appointments.Single(c => c.Id == id);
            return View(appointment);
        }

        [HttpPost, ActionName("DeleteAppointment")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAppointment(int id)
        {
            var appointment = db.Appointments.Single(c => c.Id == id);
            db.Appointments.Remove(appointment);
            db.SaveChanges();
            return RedirectToAction("ListOfAppointments");
        }

        [Authorize(Roles = "Patient")]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                return RedirectToAction("Index", new { Message = "Password has been changed successfully!" });
            }

            ViewBag.Messege = "Password or Something went Wrong";
            AddErrors(result);
            return View(model);
        }
    }
}