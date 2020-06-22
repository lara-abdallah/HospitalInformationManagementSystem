using HospitalInformationManagementSystem.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;

namespace HospitalInformationManagementSystem.Controllers
{
    public class DoctorController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }


        // GET: Doctor
        [Authorize(Roles = "Doctor")]
        public ActionResult Index(string message)
        {
            var date = DateTime.Now.Date;
            ViewBag.Messege = message;
            var user = User.Identity.GetUserId();
            var doctor = db.Doctors.Single(c => c.ApplicationUserId == user);
            var model = new AdminViewModel
            {
                Departments = db.Department.ToList(),
                Doctors = db.Doctors.ToList(),
                Patients = db.Patients.ToList(),
                ActiveAppointments = db.Appointments.Where(c => c.DoctorId == doctor.Id).Where(c => c.Status).Where(c => c.AppointmentDate >= date).ToList(),
                PendingAppointments = db.Appointments.Where(c => c.DoctorId == doctor.Id).Where(c => c.Status == false).Where(c => c.AppointmentDate >= date).ToList(),
                Announcements = db.Announcements.Where(c => c.AnnouncementFor == "Doctor").ToList()
            };
            return View(model);
        }

        [Authorize(Roles = "Doctor")]
        public ActionResult ScheduleDetail()
        {
            string user = User.Identity.GetUserId();
            var doctor = db.Doctors.Single(c => c.ApplicationUserId == user);
            var schedule = db.Schedules.Single(c => c.DoctorId == doctor.Id);
            return View(schedule);
        }

        [Authorize(Roles = "Doctor")]
        public ActionResult EditSchedule(int id)
        {
            var schedule = db.Schedules.Single(c => c.Id == id);
            return View(schedule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSchedule(int id, Schedule model)
        {
            var schedule = db.Schedules.Single(c => c.Id == id);
            schedule.AvailableEndDay = model.AvailableEndDay;
            schedule.AvailableEndTime = model.AvailableEndTime;
            schedule.AvailableStartDay = model.AvailableStartDay;
            schedule.AvailableStartTime = model.AvailableStartTime;
            schedule.Status = model.Status;
            schedule.TimePerPatient = model.TimePerPatient;
            db.SaveChanges();
            return RedirectToAction("ScheduleDetail");
        }

        [Authorize(Roles = "Doctor")]
        public ActionResult AddAppointment()
        {
            var appointment = new AppointmentViewModel
            {
                Appointment = new Appointment(),
                Patients = db.Patients.ToList()
            };
            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAppointment(AppointmentViewModel model)
        {
            string user = User.Identity.GetUserId();
            var appointments = new AppointmentViewModel
            {
                Appointment = model.Appointment,
                Patients = db.Patients.ToList()
            };
            if (model.Appointment.AppointmentDate >= DateTime.Now.Date)
            {
                var doctor = db.Doctors.Single(c => c.ApplicationUserId == user);
                var appointment = new Appointment();
                appointment.PatientId = model.Appointment.PatientId;
                appointment.DoctorId = doctor.Id;
                appointment.AppointmentDate = model.Appointment.AppointmentDate;
                appointment.Problem = model.Appointment.Problem;
                appointment.Status = model.Appointment.Status;

                db.Appointments.Add(appointment);
                db.SaveChanges();

                if (model.Appointment.Status == true)
                {
                    return RedirectToAction("ActiveAppointments");
                }
                else
                {
                    return RedirectToAction("PendingAppointments");
                }
            }
            ViewBag.Messege = "Please Enter the Date greater than today or equal!!";

            return View(appointments);
        }

        [Authorize(Roles = "Doctor")]
        public ActionResult ActiveAppointments()
        {
            var user = User.Identity.GetUserId();
            var doctor = db.Doctors.Single(c => c.ApplicationUserId == user);
            var date = DateTime.Now.Date;
            var appointment = db.Appointments.Include(c => c.Doctor).Include(c => c.Patient).Where(c => c.DoctorId == doctor.Id).Where(c => c.Status == true).Where(c => c.AppointmentDate >= date).ToList();
            return View(appointment);
        }

        public ActionResult PendingAppointments()
        {
            var user = User.Identity.GetUserId();
            var doctor = db.Doctors.Single(c => c.ApplicationUserId == user);
            var date = DateTime.Now.Date;
            var appointment = db.Appointments.Include(c => c.Doctor).Include(c => c.Patient).Where(c => c.DoctorId == doctor.Id).Where(c => c.Status == false).Where(c => c.AppointmentDate >= date).ToList();
            return View(appointment);
        }

        [Authorize(Roles = "Doctor")]
        public ActionResult EditAppointment(int id)
        {
            var appointment = new AppointmentViewModel
            {
                Appointment = db.Appointments.Single(c => c.Id == id),
                Patients = db.Patients.ToList()
            };
            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAppointment(int id, AppointmentViewModel model)
        {
            var appointments = new AppointmentViewModel
            {
                Appointment = model.Appointment,
                Patients = db.Patients.ToList()
            };
            if (model.Appointment.AppointmentDate >= DateTime.Now.Date)
            {
                var appointment = db.Appointments.Single(c => c.Id == id);
                appointment.PatientId = model.Appointment.PatientId;
                appointment.AppointmentDate = model.Appointment.AppointmentDate;
                appointment.Problem = model.Appointment.Problem;
                appointment.Status = model.Appointment.Status;
                db.SaveChanges();
                if (model.Appointment.Status == true)
                {
                    return RedirectToAction("ActiveAppointments");
                }
                else
                {
                    return RedirectToAction("PendingAppointments");
                }
            }
            ViewBag.Messege = "Please Enter the Date greater than today or equal!!";

            return View(appointments);
        }

        [Authorize(Roles = "Doctor")]
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
            if (appointment.Status)
            {
                return RedirectToAction("ActiveAppointments");
            }
            else
            {
                return RedirectToAction("PendingAppointments");
            }
        }

        [Authorize(Roles = "Doctor")]
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