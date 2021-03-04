using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication.Constants;

namespace WebApplication.Pages
{
    public class LoginModel : PageModel
    {
        private const string PasswordForTest = "password";
        private static readonly List<string> AllowedUserNames = new()
        {
            "user1", "user2"
        };

        [BindProperty]
        [Required, DisplayName("User")]
        public string UserName { get; set; }

        [BindProperty]
        [Required]
        public string Password { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool IsLoggedOut { get; set; }

        // ReSharper disable once UnusedMember.Global
        public async Task<IActionResult> OnGet()
        {
            if (IsLoggedOut || User.Identity != null && !User.Identity.IsAuthenticated)
            {
                return Page();
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Login", new { IsLoggedOut = true, ReturnUrl });
        }

        // ReSharper disable once UnusedMember.Global
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!(AllowedUserNames.Contains(UserName) && PasswordForTest.Equals(Password)))
            {
                ModelState.AddModelError($"{nameof(UserName)}", "Wrong username and/or password");
                return Page();
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, UserName)
            };

            if (UserName.Equals(AllowedUserNames[0]))
            {
                claims.Add(new Claim(ClaimTypes.Role, RoleConstants.CanAccessCustomerList, ClaimValueTypes.String));
            }

            await HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));

            if (!string.IsNullOrEmpty(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }

            return RedirectToPage("/Index");
        }
    }
}
