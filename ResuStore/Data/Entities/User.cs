using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ResuStore.Data.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public int Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Image Url")]
        public string ImageUrl { get; set; }
    }
}