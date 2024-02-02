using BookWormsOnline.Model;
using BookWormsOnline.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BookWormsOnline.Pages
{
	public class RegisterModel : PageModel
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly SignInManager<ApplicationUser> signInManager;

		[BindProperty]
		public Register RModel { get; set; }

		public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
		}

		public void OnGet()
		{ }

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
                if (!Regex.IsMatch(RModel.MobileNo, @"^\d{8}$"))
                {
                    ModelState.AddModelError(nameof(RModel.MobileNo), "Mobile number must be 8 digits.");
                    return Page();
                }

                // Additional validation for credit card number
                if (!Regex.IsMatch(RModel.CreditCardNo, @"^[345]\d{15}$"))
                {
                    ModelState.AddModelError(nameof(RModel.CreditCardNo), "Invalid credit card number.");
                    return Page();
                }

                // Additional validation for shipping and billing addresses
                if (RModel.ShippingAddr.Length < 10 || RModel.BillingAddr.Length < 10)
                {
                    ModelState.AddModelError("", "Shipping and billing addresses must be at least 10 characters long.");
                    return Page();
                }
                var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
				var protector = dataProtectionProvider.CreateProtector("MySecretKey");
				var user = new ApplicationUser()
				{
					FirstName = RModel.FirstName,
					LastName = RModel.LastName,
					CreditCardNo = protector.Protect(RModel.CreditCardNo),
					MobileNo = RModel.MobileNo,
					BillingAddr = RModel.BillingAddr,
					ShippingAddr = RModel.ShippingAddr,
					UserName = RModel.Email,
					Email = RModel.Email
				};
				// Check if the email is unique before creating a new user
				var existingUser = await userManager.FindByEmailAsync(RModel.Email);
				if (existingUser != null)
				{
					ModelState.AddModelError(nameof(RModel.Email), "Email is already in use.");
					return Page();
				}

				// Check password complexity
				var passwordComplexityError = CheckPasswordComplexity(RModel.Password);
				if (!string.IsNullOrEmpty(passwordComplexityError))
				{
					ModelState.AddModelError(nameof(RModel.Password), passwordComplexityError);
					return Page();
				}

				

				var result = await userManager.CreateAsync(user, RModel.Password);
				if (result.Succeeded)
				{
					await signInManager.SignInAsync(user, false);
					return RedirectToPage("Index");
				}

				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
			}

			return Page();
		}

        private string CheckPasswordComplexity(string password)
        {
            const int MinLength = 12;

            if (password.Length < MinLength)
            {
                return $"Password must be at least {MinLength} characters long.";
            }

            if (!Regex.IsMatch(password, "[a-z]"))
            {
                return "Password must contain at least one lowercase letter.";
            }

            if (!Regex.IsMatch(password, "[A-Z]"))
            {
                return "Password must contain at least one uppercase letter.";
            }

            if (!Regex.IsMatch(password, "[0-9]"))
            {
                return "Password must contain at least one digit.";
            }

            if (!Regex.IsMatch(password, "[^a-zA-Z0-9]"))
            {
                return "Password must contain at least one special character.";
            }

            if (IsPasswordTooCommon(password))
            {
                return "Password is too common. Choose a more unique password.";
            }

            return null; // Password meets complexity requirements
        }

        private bool IsPasswordTooCommon(string password)
        {
            // List of common passwords (replace with a more comprehensive list)
            var commonPasswords = new List<string>
    {
        "password", "123456", "qwerty", "admin", "letmein", "welcome", "123abc"
    };

            // Convert the password to lowercase for case-insensitive comparison
            return commonPasswords.Contains(password.ToLower());
        }
    }
}
