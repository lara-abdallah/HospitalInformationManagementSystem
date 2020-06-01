using HospitalInformationManagementSystem.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalInformationManagementSystem.Controllers
{
  //Done by Lara Abdallah
    public class PatientController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Patient

        [Authorize(Roles = "Patient")]
        public ActionResult Index()
        {
            return View();
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
                return RedirectToAction("Index");
            }
            ViewBag.Messege = "Please Enter the Date greater than today or equal!!";

            return View(list);

        }

        [Authorize(Roles = "Patient")]
        public ActionResult ListOfAppointments()
        {
            string user = User.Identity.GetUserId();
            var patient = db.Patients.Single(c => c.ApplicationUserId == user);
            var appointment = db.Appointments.Where(c => c.PatientId == patient.Id).ToList();
            return View(appointment);
        }
    }
}