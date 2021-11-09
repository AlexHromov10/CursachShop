using AkiraShop2.Models;
using AkiraShop2.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AkiraShop2.Entities;

namespace AkiraShop2.Areas.Admin.Controllers
{
    public class UserAuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public UserAuthController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            loginModel.LoginInValid = "true";

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginModel.Email,
                                                                      loginModel.Password,
                                                                      loginModel.RememberMe,
                                                                      lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    loginModel.LoginInValid = "";
                }
                else
                {
                    ModelState.AddModelError(string.Empty,"Неверная попытка входа");
                }
            }
            return PartialView("_UserLoginPartial",loginModel);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();

            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index","Home");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUser(RegistrationModel registrationModel)
        {
            registrationModel.RegistrationInValid = "true";

            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = registrationModel.Email,
                    Email = registrationModel.Email,
                    PhoneNumber = registrationModel.PhoneNumber,
                    FirstName = registrationModel.FirstName,
                    LastName = registrationModel.LastName,
                    Address = registrationModel.Address,
                    PostCode = registrationModel.PostCode
                };

                var result = await _userManager.CreateAsync(user, registrationModel.Password);

                if (result.Succeeded)
                {
                    registrationModel.RegistrationInValid = "";

                    Order Cart = new Order { UserOrderId = user.Id, Status = "CART" };
                    _context.Add(Cart);
                    await _context.SaveChangesAsync();


                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return PartialView("_UserRegistrationPartial", registrationModel);
                }
                AddErrorsToModelState(result);

            }
            return PartialView("_UserRegistrationPartial", registrationModel);
        }

        //Проверка на существующий email 
        [AllowAnonymous]
        public async Task<bool> UserNameExists(string userName)
        {
            bool userNameExists = await _context.Users.AnyAsync(u => u.UserName.ToUpper() == userName.ToUpper());

            if (userNameExists)
            {
                return true;
            }
            return false;
        }

        private void AddErrorsToModelState(IdentityResult result)
        {
            foreach (var errors in result.Errors)
            {
                ModelState.AddModelError(string.Empty, errors.Description);
            }
            
        }







    }
}
