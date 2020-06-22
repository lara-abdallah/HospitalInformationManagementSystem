using HospitalInformationManagementSystem.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using static HospitalInformationManagementSystem.Controllers.ManageController;

namespace HospitalInformationManagementSystem.Controllers
{
    //Done by Zainab Hteit
    public class AdminController : Controller
    {
        
        // GET: Admin
        private ApplicationDbContext db;

        private ApplicationUserManager _userManager;

        //Constructor
        public AdminController()
        {
            db = new ApplicationDbContext();
        }

        //Destructor
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
        }

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

        // GET: Admin
        [Authorize(Roles = "Admin")]
        public ActionResult Index(string message)
        {
            var date = DateTime.Now.Date;
            ViewBag.Messege = message;
            var model = new AdminViewModel
            {
                Departments = db.Department.ToList(),
                Doctors = db.Doctors.ToList(),
                Patients = db.Patients.ToList(),
                Announcements = db.Announcements.ToList(),
                ActiveAppointments =
                    db.Appointments.Where(c => c.Status).Where(c => c.AppointmentDate >= date).ToList(),
                PendingAppointments = db.Appointments.Where(c => c.Status == false)
                    .Where(c => c.AppointmentDate >= date).ToList()
            };
            return View(model);
        }

        //Department Section

        //Department List
        [Authorize(Roles = "Admin")]
        public ActionResult DepartmentList()
        {
            var model = db.Department.ToList();
            return View(model);
        }

        //Add Department
        [Authorize(Roles = "Admin")]
        public ActionResult AddDepartment()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDepartment(Department model)
        {
            if (db.Department.Any(c => c.Name == model.Name))
            {
                ModelState.AddModelError("Name", "Name already present!");
                return View(model);
            }

            db.Department.Add(model);
            db.SaveChanges();
            return RedirectToAction("DepartmentList");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditDepartment(int id)
        {
            var model = db.Department.SingleOrDefault(c => c.Id == id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDepartment(int id, Department model)
        {
            var department = db.Department.Single(c => c.Id == id);
            department.Name = model.Name;
            department.Description = model.Description;
            department.Status = model.Status;
            db.SaveChanges();
            return RedirectToAction("DepartmentList");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteDepartment(int? id)
        {
            var department = db.Department.Single(c => c.Id == id);
            return View(department);
        }

        [HttpPost, ActionName("DeleteDepartment")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDepartment(int id)
        {
            var department = db.Department.SingleOrDefault(c => c.Id == id);
            db.Department.Remove(department);
            db.SaveChanges();
            return RedirectToAction("DepartmentList");
        }


        [Authorize(Roles = "Admin")]
        public ActionResult AddDoctor()
        {
            var collection = new DoctorViewModel
            {
                ApplicationUser = new RegisterViewModel(),
                Doctor = new Doctor(),
                Departments = db.Department.ToList()
            };
            return View(collection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddDoctor(DoctorViewModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.ApplicationUser.UserName,
                Email = model.ApplicationUser.Email
            };
            var result = await UserManager.CreateAsync(user,"Test@123");
            if (result.Succeeded)
            {
                await UserManager.AddToRoleAsync(user.Id, "Doctor");
                var doctor = new Doctor
                {
                    FirstName = model.Doctor.FirstName,
                    LastName = model.Doctor.LastName,
                    FullName = "Dr. " + model.Doctor.FirstName + " " + model.Doctor.LastName,
                    EmailAddress = model.ApplicationUser.Email,
                    PhoneNo = model.Doctor.PhoneNo,
                    Education = model.Doctor.Education,
                    DepartmentId = model.Doctor.DepartmentId,
                    Specialization = model.Doctor.Specialization,
                    Gender = model.Doctor.Gender,
                    BloodGroup = model.Doctor.BloodGroup,
                    ApplicationUserId = user.Id,
                    DateOfBirth = model.Doctor.DateOfBirth,
                    Address = model.Doctor.Address,
                    Status = model.Doctor.Status,
                   
                };
                db.Doctors.Add(doctor);
                db.SaveChanges();
                return RedirectToAction("ListOfDoctors");
            }

            return HttpNotFound();

        }
         [Authorize(Roles = "Admin")]
        public ActionResult ListOfDoctors()
        {
            var doctor = db.Doctors.Include(c => c.Department).ToList();
            return View(doctor);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditDoctors(int id)
        {
            var collection = new DoctorViewModel
            {
                Departments = db.Department.ToList(),
                Doctor = db.Doctors.Single(c => c.Id == id)
            };
            return View(collection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDoctors(int id, DoctorViewModel model)
        {
            var doctor = db.Doctors.Single(c => c.Id == id);
            doctor.FirstName = model.Doctor.FirstName;
            doctor.LastName = model.Doctor.LastName;
            doctor.FullName = "Dr. " + model.Doctor.FirstName + " " + model.Doctor.LastName;
            doctor.PhoneNo = model.Doctor.PhoneNo;
            doctor.Education = model.Doctor.Education;
            doctor.DepartmentId = model.Doctor.DepartmentId;
            doctor.Specialization = model.Doctor.Specialization;
            doctor.Gender = model.Doctor.Gender;
            doctor.BloodGroup = model.Doctor.BloodGroup;
            doctor.DateOfBirth = model.Doctor.DateOfBirth;
            doctor.Address = model.Doctor.Address;
            doctor.Status = model.Doctor.Status;
            db.SaveChanges();

            return RedirectToAction("ListOfDoctors");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DoctorDetail(int id)
        {
            var doctor = db.Doctors.Include(c => c.Department).SingleOrDefault(c => c.Id == id);
            return View(doctor);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteDoctor(int? id)
        {
            var doctor = db.Doctors.Single(c => c.Id == id);
            return View(doctor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDoctor(int id)
        {
            var doctor = db.Doctors.SingleOrDefault(c => c.Id == id);
            db.Doctors.Remove(doctor);
            db.SaveChanges();
            return RedirectToAction("ListOfDoctors");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ListOfPatients()
        {
            var model = db.Patients.ToList();
            return View(model);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult EditPatient(int id)
        {
            var patient = db.Patients.Single(c => c.Id == id);
            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPatient(int id, Patient model)
        {
            if (!ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var patient = db.Patients.Single(c => c.Id == id);
            patient.FirstName = model.FirstName;
            patient.LastName = model.LastName;
            patient.FullName = model.FirstName + " " + model.LastName;
            patient.Address = model.Address;
            patient.BloodGroup = model.BloodGroup;
            patient.DateOfBirth = model.DateOfBirth;
            patient.EmailAddress = model.EmailAddress;
            patient.Gender = model.Gender;
            patient.PhoneNo = model.PhoneNo;
            db.SaveChanges();
            return RedirectToAction("ListOfPatients");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DeletePatient(int? id)
        {
            var patient = db.Patients.Single(c => c.Id == id);
            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePatient(int id)
        {
            var patient = db.Patients.SingleOrDefault(c => c.Id == id);
            db.Patients.Remove(patient);
            db.SaveChanges();
            return RedirectToAction("ListOfPatients");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult PatientDetail(int id)
        {
            var model = db.Patients.SingleOrDefault(c => c.Id == id);
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult AddSchedule()
        {
            var schedule = new ScheduleViewModel
            {
                Schedule = new Schedule(),
                Doctors = db.Doctors.ToList()
            };
            return View(schedule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSchedule(ScheduleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var schedule = new ScheduleViewModel
                {
                    Schedule = model.Schedule,
                    Doctors = db.Doctors.ToList()
                };
                return View(schedule);
            }

            db.Schedules.Add(model.Schedule);
            db.SaveChanges();
            return RedirectToAction("ListOfSchedules");
        }
        [Authorize(Roles = "Admin")]
        public ActionResult ListOfSchedules()
        {
            var schedule = db.Schedules.Include(c => c.Doctor).ToList();
            return View(schedule);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult EditSchedule(int id)
        {
            var schedule = new ScheduleViewModel
            {
                Schedule = db.Schedules.Single(c => c.Id == id),
                Doctors = db.Doctors.ToList()
            };
            return View(schedule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSchedule(int id, ScheduleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var schedule = db.Schedules.Single(c => c.Id == id);
            schedule.DoctorId = model.Schedule.DoctorId;
            schedule.AvailableEndDay = model.Schedule.AvailableEndDay;
            schedule.AvailableEndTime = model.Schedule.AvailableEndTime;
            schedule.AvailableStartDay = model.Schedule.AvailableStartDay;
            schedule.AvailableStartTime = model.Schedule.AvailableStartTime;
            schedule.Status = model.Schedule.Status;
            schedule.TimePerPatient = model.Schedule.TimePerPatient;
            db.SaveChanges();
            return RedirectToAction("ListOfSchedules");
        }
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteSchedule(int? id)
        {
            var schedule = db.Schedules.Single(c => c.Id == id);
            return View(schedule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSchedule(int id)
        {
            var schedule = db.Schedules.SingleOrDefault(c => c.Id == id);
            db.Schedules.Remove(schedule);
            db.SaveChanges();
            return RedirectToAction("ListOfSchedules");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AddAnnouncement()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAnnouncement(Announcement model)
        {
            if (model.End >= DateTime.Now.Date)
            {
                db.Announcements.Add(model);
                db.SaveChanges();
                return RedirectToAction("ListOfAnnouncement");
            }
            else
            {
                ViewBag.Messege = "Please Enter the Date greater than today!!";
            }

            return View(model);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult ListOfAnnouncement()
        {
            var list = db.Announcements.ToList();
            return View(list);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult EditAnnouncement(int id)
        {
            var announcement = db.Announcements.Single(c => c.Id == id);
            return View(announcement);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAnnouncement(int id, Announcement model)
        {
            var announcement = db.Announcements.Single(c => c.Id == id);
            if (model.End >= DateTime.Now.Date)
            {
                announcement.Announcements = model.Announcements;
                announcement.End = model.End;
                announcement.AnnouncementFor = model.AnnouncementFor;
                db.SaveChanges();
                return RedirectToAction("ListOfAnnouncement");
            }
            else
            {
                ViewBag.Messege = "Please Enter the Date greater than today!!";
            }

            return View(announcement);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteAnnouncement(int? id)
        {
            var announcement = db.Announcements.Single(c => c.Id == id);
            return View(announcement);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAnnouncement(int id)
        {
            var announcement = db.Announcements.SingleOrDefault(c => c.Id == id);
            db.Announcements.Remove(announcement);
            db.SaveChanges();
            return RedirectToAction("ListOfAnnouncement");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AddAppointment()
        {
            var appointment = new AppointmentViewModel
            {
                Appointment = new Appointment(),
                Patients = db.Patients.ToList(),
                Doctors = db.Doctors.ToList()
            };
            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAppointment(AppointmentViewModel model)
        {
            var appointments = new AppointmentViewModel
            {
                Appointment = model.Appointment,
                Patients = db.Patients.ToList(),
                Doctors = db.Doctors.ToList()
            };
            if (model.Appointment.AppointmentDate >= DateTime.Now.Date)
            {
                var appointment = new Appointment();
                appointment.PatientId = model.Appointment.PatientId;
                appointment.DoctorId = model.Appointment.DoctorId;
                appointment.AppointmentDate = model.Appointment.AppointmentDate;
                appointment.Problem = model.Appointment.Problem;
                appointment.Status = model.Appointment.Status;
                db.Appointments.Add(appointment);
                db.SaveChanges();

                if (model.Appointment.Status == true)
                {
                    return RedirectToAction("ListOfAppointments");
                }
                else
                {
                    return RedirectToAction("PendingAppointments");
                }
            }

            ViewBag.Messege = "Please Enter the Date greater than today or equal!!";
            return View(appointments);

        }
        [Authorize(Roles = "Admin")]
        public ActionResult ListOfAppointments()
        {
            var date = DateTime.Now.Date;
            var appointment = db.Appointments.Include(c => c.Doctor).Include(c => c.Patient)
                .Where(c => c.Status == true).Where(c => c.AppointmentDate >= date).ToList();
            return View(appointment);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult PendingAppointments()
        {
            var date = DateTime.Now.Date;
            var appointment = db.Appointments.Include(c => c.Doctor).Include(c => c.Patient)
                .Where(c => c.Status == false).Where(c => c.AppointmentDate >= date).ToList();
            return View(appointment);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditAppointment(int id)
        {
            var appointment = new AppointmentViewModel
            {
                Appointment = db.Appointments.Single(c => c.Id == id),
                Patients = db.Patients.ToList(),
                Doctors = db.Doctors.ToList()
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
                Patients = db.Patients.ToList(),
                Doctors = db.Doctors.ToList()
            };
            if (model.Appointment.AppointmentDate >= DateTime.Now.Date)
            {
                var appointment = db.Appointments.Single(c => c.Id == id);
                appointment.PatientId = model.Appointment.PatientId;
                appointment.DoctorId = model.Appointment.DoctorId;
                appointment.AppointmentDate = model.Appointment.AppointmentDate;
                appointment.Problem = model.Appointment.Problem;
                appointment.Status = model.Appointment.Status;
                db.SaveChanges();
                if (model.Appointment.Status == true)
                {
                    return RedirectToAction("ListOfAppointments");
                }
                else
                {
                    return RedirectToAction("PendingAppointments");
                }
            }
            ViewBag.Messege = "Please Enter the Date greater than today or equal!!";

            return View(appointments);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteAppointment(int? id)
        {
            var appointment = db.Appointments.Single(c => c.Id == id);
            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAppointment(int id)
        {
            var appointment = db.Appointments.Single(c => c.Id == id);
            db.Appointments.Remove(appointment);
            db.SaveChanges();
            if (appointment.Status)
            {
                return RedirectToAction("ListOfAppointments");
            }
            else
            {
                return RedirectToAction("PendingAppointments");
            }
        }
        [Authorize(Roles = "Admin")]
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