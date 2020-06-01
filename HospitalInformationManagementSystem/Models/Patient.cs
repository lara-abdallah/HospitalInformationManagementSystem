﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HospitalInformationManagementSystem.Models
{
    //Done by Lara Abdallah
    public class Patient
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Name")]
        public string FullName { get; set; }

        [EmailAddress]
        [Display(Name = "Email Id")]
        public string EmailAddress { get; set; }
        [Display(Name = "Phone No")]
        public string PhoneNo { get; set; }
        [Display(Name = "Blood Group")]
        public string BloodGroup { get; set; }
        public string Gender { get; set; }
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirth { get; set; }
        public string Address { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string ApplicationUserId { get; set; }
    }
}