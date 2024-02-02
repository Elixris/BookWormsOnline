using BookWormsOnline.Model;
using BookWormsOnline.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Threading.Tasks;

namespace BookWormsOnline.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Login LModel { get; set; }

        private readonly SignInManager<ApplicationUser> signInManager;
        private const string RecaptchaSecretKey = "6LeKsGMpAAAAAMshWfuVh5CoBB8IpcOBt6LmCreM";
        private const string RecaptchaSiteKey = "6LeKsGMpAAAAACgUJ6eN8KfMv6rjnURY2GgPTwUr";
        

        public LoginModel(SignInManager<ApplicationUser> signInManager)
        {
            this.signInManager = signInManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (ModelState.IsValid)
            {
                // Verify reCAPTCHA
                bool isCaptchaValid = await VerifyRecaptcha(Request.Form["g-recaptcha-response"]);

                if (!isCaptchaValid)
                {
                    ModelState.AddModelError("", "reCAPTCHA validation failed. Please try again.");
                    return Page();
                }

                var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password, LModel.RememberMe, false);
                if (identityResult.Succeeded)
                {

                    return RedirectToPage("Index");
                }
                ModelState.AddModelError("", "Username or password incorrect");
            }
            return Page();
        }

        private async Task<bool> VerifyRecaptcha(string recaptchaResponse)
        {
            using (var httpClient = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new[]
                {
            new KeyValuePair<string, string>("secret", RecaptchaSecretKey),
            new KeyValuePair<string, string>("response", recaptchaResponse)
        });

                var response = await httpClient.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    // Deserialize the response and check the 'success' field
                    // Note: You should use a JSON library for proper parsing.
                    return jsonString.Contains("\"success\": true");
                }

                return false;
            }
        }
    }
}
