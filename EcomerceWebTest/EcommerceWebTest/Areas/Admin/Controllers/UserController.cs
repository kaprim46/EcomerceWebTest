using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce.DataAccess.Data;
using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models;
using Ecommerce.Models.ViewModel;
using Ecommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stripe.Radar;

namespace EcommerceWebTest.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(ApplicationDbContext db, IUnitOfWork unitOfWork,
                             RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Permission(string id)
        {
            ApplicationUserVM user = new()
            {
                RoleList = _db.Roles.Select(q => new SelectListItem
                {
                    Text = q.Name,
                    Value = q.Name
                }),
                CompanyList = _db.Companies.Select(q => new SelectListItem
                {
                    Text = q.Name,
                    Value = q.CompanyId.ToString()
                }),
                 User = _db.ApplicationUsers.Include(q => q.Company).FirstOrDefault(q => q.Id == id)
            };
            var roleId = _db.UserRoles.FirstOrDefault(q => q.UserId == user.User.Id).RoleId;
            var role = _db.Roles.FirstOrDefault(q => q.Id == roleId).Name;
            user.User.Role = role;
            return View(user);
        }

        [HttpPost]
        public IActionResult Permission(ApplicationUserVM model)
        {
            if (ModelState.IsValid)
            {
                var user = _db.ApplicationUsers.FirstOrDefault(q => q.Id == model.User.Id);
                if (user != null)
                {
                    if (_roleManager.RoleExistsAsync(model.User.Role).GetAwaiter().GetResult())
                    {
                        // Get the current roles for the user
                        var userRoles = _userManager.GetRolesAsync(user).GetAwaiter().GetResult();
                        // Remove all existing roles for the user
                        _userManager.RemoveFromRolesAsync(user, userRoles).GetAwaiter().GetResult();

                        // Add the new role to the user
                       var result = _userManager.AddToRoleAsync(user, model.User.Role).GetAwaiter().GetResult();
                        if (result.Succeeded)
                        {
                            if (model.User.Role != SD.Role_Company)
                            {
                                user.CompanyId = null;
                            }
                            else
                            {
                                user.CompanyId = model.User.CompanyId;
                            }
                        }
                    }
                    _db.SaveChanges();
                
                return RedirectToAction(nameof(Index));
                
                }
            }
            ApplicationUserVM user1 = new()
            {
                RoleList = _db.Roles.ToList().Select(q => new SelectListItem
                {
                    Text = q.Name,
                    Value = q.Name
                }),
                CompanyList = _db.Companies.Select(q => new SelectListItem
                {
                    Text = q.Name,
                    Value = q.CompanyId.ToString()
                }),
                User = _db.ApplicationUsers.FirstOrDefault(q => q.Id == model.User.Id)
            };
            return View(user1);
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> users = _db.ApplicationUsers.Include(q => q.Company).ToList();

            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            foreach (var user in users)
            {
                var roleId = userRoles.FirstOrDefault(q => q.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(q => q.Id == roleId).Name;

                if (user.Company == null)
                {
                    user.Company = new() { Name = "" };
                }
            }
            return Json(new { data = users });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(q => q.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }
            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is currently locked and we need to unlock them
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _db.SaveChanges();
            return Json(new { success = true, message = "Operation Successful" });
        }
    }
    #endregion
}
