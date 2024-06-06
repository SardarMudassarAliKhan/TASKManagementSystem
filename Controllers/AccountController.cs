using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TASKManagementSystem_BAL.IService;
using TASKManagementSystem_BAL.ViewModels;

namespace TASKManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IProfileService profileService;

        public AccountController(IAccountService accountService, IProfileService profileService)
        {
            _accountService = accountService;
            this.profileService = profileService;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Roles");
            }
            return View();
        }

        [HttpPost, ActionName("Login")]
        public async Task<IActionResult> LoginPost(LoginViewModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.LoginAsync(loginModel);
                if (result.Succeeded)
                {
                    // Handle session management and role-based redirection
                    var user = await _accountService.FindByEmailAsync(loginModel.Email);
                    var roles = await _accountService.GetRolesAsync(user);
                    var primaryRole = roles.FirstOrDefault();
                    var profile = await profileService.GetProfileByUserNameAsync(loginModel.Email);

                    HttpContext.Session.SetString("ProfileId", profile.ProfileId.ToString());
                    HttpContext.Session.SetString("ProfileRole", primaryRole ?? "Member");
                    HttpContext.Session.SetString("ProfileImage", !string.IsNullOrWhiteSpace(profile.DisplayImageUrl) ? profile.DisplayImageUrl : "favicon.ico");

                    switch (primaryRole)
                    {
                        case "Developer":
                            return RedirectToAction("Developer", "DashBoard");
                        case "Manager":
                            return RedirectToAction("Manager", "DashBoard");
                        case "TeamLead":
                            return RedirectToAction("Index", "DashBoard");
                        default:
                            return RedirectToAction("Admin", "DashBoard");
                    }
                }
            }

            ModelState.AddModelError("", "Failed to login");
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost, ActionName("Register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterPost(RegisterViewModel registerModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.RegisterAsync(registerModel);
                var profile = await profileService.GetProfileByUserNameAsync(registerModel.Email);
                if (result.Succeeded)
                {
                    HttpContext.Session.SetString("ProfileId", profile.ProfileId.ToString());
                    HttpContext.Session.SetString("ProfileImage", "favicon.ico");
                    return RedirectToAction("Index", "Roles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _accountService.ChangePasswordAsync(model);
            if (result.Succeeded)
            {
                ViewBag.Message = "Your password has been updated";
            }
            return View();
        }

        public IActionResult VerifyContact()
        {
            var profile = profileService.GetProfileByUserNameAsync(User.Identity.Name).Result;
            var model = new VerifyContactViewModel
            {
                Email = profile.UserName,
                PhoneNumber = profile.PhoneNumber,
                Status = profile.ContactVerified
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyContact(VerifyContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                var profile = await _accountService.VerifyContactAsync(model);
                if (profile != null)
                {
                    ViewBag.Message = "Contact Verified";
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError("", "You entered the wrong code. Please enter the code sent to your email.");
                    return View(model);
                }
            }
            return View(model);
        }

        public async Task<string> ConfirmContact()
        {
            var email = User.Identity.Name;
            return await _accountService.ConfirmContactAsync(email);
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.ForgotPasswordAsync(model);
                if (result.Succeeded)
                {
                    return View("ForgotPasswordConfirmation");
                }
            }
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult ResetPassword(string email, string code)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.ResetPasswordAsync(model);
                if (result.Succeeded)
                {
                    return RedirectToAction("ResetPasswordConfirmation", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}
