using A_MicrosoftAspNetCoreIdentityManagement.Data;
using A_MicrosoftAspNetCoreIdentityManagement.Models;
using A_MicrosoftAspNetCoreIdentityManagement.Services;
using A_MicrosoftAspNetCoreIdentityManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace A_MicrosoftAspNetCoreIdentityManagement.Controllers
{
    public class AccountController : Controller
    {
        private SignInManager<AppUser> _signInManager;
        private UserManager<AppUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;


        public AccountController(AppDbContext context, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Login()
        {
            if (this.User.Identity.IsAuthenticated)
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
                var profile = _context.Profiles.Where(p => p.UserName == loginModel.Email).SingleOrDefault();
                var deactivated = _context.DeactivatedProfiles.Where(e => e.ProfileId == profile.ProfileId).FirstOrDefault();
                if (deactivated != null)
                {
                    ModelState.AddModelError("", $"Your profile has been blocked:{deactivated.Reason}. Please contact administrator admin@artisan.com");
                    return View();
                }
                var result = await _signInManager.PasswordSignInAsync(loginModel.Email, loginModel.Password, loginModel.RememberMe, false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(loginModel.Email);
                    var roles = await _userManager.GetRolesAsync(user);
                    var primaryRole = roles.FirstOrDefault();

                    HttpContext.Session.SetString("ProfileId", profile.ProfileId.ToString());
                    HttpContext.Session.SetString("ProfileRole", primaryRole ?? "Member");
                    if (!string.IsNullOrWhiteSpace(profile.DisplayImageUrl))
                    {
                        HttpContext.Session.SetString("ProfileImage", profile.DisplayImageUrl);

                    }
                    else
                    {
                        HttpContext.Session.SetString("ProfileImage", "favicon.ico");
                    }

                    if(primaryRole == "Vendor")
                    {
                        return RedirectToAction("Index", "Roles");
                    }
                    else if (primaryRole == "Artist")
                    {
                        return RedirectToAction("BuyerDashBoard", "Roles");
                    }
                    else if (primaryRole == "Buyer")
                    {
                        return RedirectToAction("BuyerDashBoard", "Roles");
                    }
                    else if (primaryRole == "Admin")
                    {
                        return RedirectToAction("Index", "Roles");
                    }
                    else if (primaryRole == "Member")
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            ModelState.AddModelError("", "Faild to Login");
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
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
                AppUser user = new AppUser
                {
                    FirstName = registerModel.FirstName,
                    LastName = registerModel.LastName,
                    UserName = registerModel.Email,
                    PhoneNumber = registerModel.PhoneNumber,
                    Email = registerModel.Email
                };

                Profile profile = new Profile
                {
                    UserName = registerModel.Email,
                    FirstName = registerModel.FirstName,
                    LastName = registerModel.LastName,
                    FullName = registerModel.FirstName + " " + registerModel.LastName,
                    PhoneNumber = registerModel.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, registerModel.Password);
                if (result.Succeeded)
                {
                    bool roleExists = await _roleManager.RoleExistsAsync(registerModel.RoleName);
                    if (!roleExists)
                    {
                        await _roleManager.CreateAsync(new IdentityRole(registerModel.RoleName));
                    }

                    if (!await _userManager.IsInRoleAsync(user, registerModel.RoleName))
                    {
                        await _userManager.AddToRoleAsync(user, registerModel.RoleName);
                    }

                    if (!string.IsNullOrWhiteSpace(user.Email))
                    {
                        // Claim[] claim = new Claim(ClaimTypes.GivenName, user.FirstName);
                        Claim[] claims = new Claim[]
                        {
                          new Claim(ClaimTypes.Email, user.Email),
                          new Claim(ClaimTypes.GivenName, user.FirstName),
                          new Claim(ClaimTypes.Surname, user.LastName)
                        };
                        await _userManager.AddClaimsAsync(user, claims);
                    }

                    //Add profile data
                    _context.Profiles.Add(profile);
                    await _context.SaveChangesAsync();

                    var resultSignIn = await _signInManager.PasswordSignInAsync(registerModel.Email, registerModel.Password, registerModel.RememberMe, false);
                    if (resultSignIn.Succeeded)
                    {
                        HttpContext.Session.SetString("ProfileId", profile.ProfileId.ToString());
                        HttpContext.Session.SetString("ProfileImage", "favicon.ico");
                        return RedirectToAction("Index", "Roles");
                    }
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
        public async Task<IActionResult> ChangePassword(ChangePassworViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);

            }
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                ViewBag.Message = "Your password has been updated";
                return View();
            }
            return View();

        }

        public DeactivatedProfile DeactivatedCheck(int id)
        {
            return _context.DeactivatedProfiles.Where(e => e.ProfileId == id).FirstOrDefault();
        }

        public IActionResult VerifyContact()
        {
            var profile = _context.Profiles.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            VerifyContactViewModel model = new VerifyContactViewModel
            {
                Email = profile.UserName,
                PhoneNumber = profile.PhoneNumber,
                Status = profile.ContactVerified
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult VerifyContact(VerifyContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.VerificationCode == "5186")
                {
                    var profile = _context.Profiles.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

                    profile.ContactVerified = "Verified";
                    _context.Profiles.Update(profile);
                    _context.SaveChanges();
                    ViewBag.Message = "Contact Verified";
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError("", $"You entered wrong code.Please enter code sent on your email");
                    return View(model);
                }

            }

            return View(model);
        }

        public async Task<string> ConfirmContact()
        {
            var email = User.Identity.Name;
            await EmailService.SendEmailAsync(new MailRequest() { ToEmail = email, Subject = "Verification Code", Body = "Your Verification Code is:5186" });
            //Send verification code
            return "Verification Code Sent to your email";
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
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { Email = user.Email, Code = code }, protocol: Request.Scheme);
                // await _userManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                await EmailService.SendEmailAsync(new MailRequest() { ToEmail = user.Email, Subject = "Reset Password", Body = "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>" });
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public IActionResult ResetPassword(string email, string code)
        {
            return code == null ? View("Error") : View();
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            AddErrors(result);
            return View();
        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.ToString());
            }
        }


        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}
