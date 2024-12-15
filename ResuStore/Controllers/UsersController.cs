using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using ResuStore.Data;
using ResuStore.Data.Entities;
using ResuStore.Data.Enums;
using ResuStore.Models;

namespace ResuStore.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext dbContext = new ApplicationDbContext();

        public ActionResult Index()
        {
            List<UserViewModel> users = GetAllUsers();
            return View(users);
        }

        [HttpGet]
        public ActionResult GetUsersJsonData()
        {
            List<UserViewModel> users = GetAllUsers();
            return Json(users, JsonRequestBehavior.AllowGet);
        }

        private List<UserViewModel> GetAllUsers()
        {
            var users = dbContext.Users.ToList();

            var usersVM = new List<UserViewModel>(users.Select(x => new UserViewModel
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                ImageUrl = x.ImageUrl,
                DateOfBirth = null,
                Gender = null,
                Email = null
            })).ToList();

            return usersVM;
        }

        [HttpGet]
        public ActionResult GetUserDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = dbContext.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var userVM = new UserViewModel()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Gender = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Enum.GetName(typeof(Gender), user.Gender).ToLower()),
                DateOfBirth = user.DateOfBirth.ToString("dd/MM/yyyy"),
                ImageUrl = user.ImageUrl
            };

            return Json(userVM, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserViewModel user)
        {
            if (user.ImageFile == null || user.ImageFile.ContentLength == 0)
            {
                ModelState.AddModelError("ImageFile", "Image file is required");
            }
            else
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtention = Path.GetExtension(user.ImageFile.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtention))
                {
                    ModelState.AddModelError("ImageFile", "Invalid file type (Only JPG, PNG, GIF are allowed).");
                }
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(x => x.Key, x => x.Value.Errors.Select(y => y.ErrorMessage).ToArray());
                return Json(new { success = false, errors });
            }

            var uploadDirectory = Server.MapPath("~/Uploads");
            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(user.ImageFile.FileName);
            var filePath = Path.Combine(uploadDirectory, uniqueFileName);

            user.ImageFile.SaveAs(filePath);
            user.ImageUrl = "/Uploads/" + uniqueFileName;

            var userEntity = new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Gender = (int)(Gender)Enum.Parse(typeof(Gender), user.Gender.ToUpper()),
                DateOfBirth = DateTime.Parse(user.DateOfBirth),
                ImageUrl = user.ImageUrl
            };

            dbContext.Users.Add(userEntity);
            dbContext.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(UserViewModel userVM)
        {
            try
            {
                User user = dbContext.Users.Find(userVM.Id);
                var imageFilePath = Server.MapPath("~" + user.ImageUrl);
                dbContext.Users.Remove(user);
                dbContext.SaveChanges();

                if (System.IO.File.Exists(imageFilePath))
                {
                    System.IO.File.Delete(imageFilePath);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = ex.Message });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
