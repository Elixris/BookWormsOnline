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

                // Check additional input validations
                if (!IsValidName(RModel.FirstName))
                {
                    ModelState.AddModelError(nameof(RModel.FirstName), "Invalid first name.");
                    return Page();
                }

                if (!IsValidName(RModel.LastName))
                {
                    ModelState.AddModelError(nameof(RModel.LastName), "Invalid last name.");
                    return Page();
                }

                if (!IsValidCreditCard(RModel.CreditCardNo))
                {
                    ModelState.AddModelError(nameof(RModel.CreditCardNo), "Invalid credit card number.");
                    return Page();
                }

                if (!IsValidMobileNo(RModel.MobileNo))
                {
                    ModelState.AddModelError(nameof(RModel.MobileNo), "Invalid mobile number.");
                    return Page();
                }

                if (!IsValidEmail(RModel.Email))
                {
                    ModelState.AddModelError(nameof(RModel.Email), "Invalid email address.");
                    return Page();
                }

                if (!IsValidAddress(RModel.BillingAddr))
                {
                    ModelState.AddModelError(nameof(RModel.BillingAddr), "Invalid billing address.");
                    return Page();
                }

                if (!IsValidAddress(RModel.ShippingAddr))
                {
                    ModelState.AddModelError(nameof(RModel.ShippingAddr), "Invalid shipping address.");
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

            return null; // Password meets complexity requirements
        }

        private bool IsValidName(string name)
        {
            // Valid name should contain only alphabets, minimum 3 characters, and no spaces
            return Regex.IsMatch(name, "^[a-zA-Z]{3,}$");
        }

        private bool IsValidCreditCard(string creditCard)
        {
            // Valid credit card must start with 5, 4, or 3
            return Regex.IsMatch(creditCard, "^[5-4-3]");
        }

        private bool IsValidMobileNo(string mobileNo)
        {
            // Valid mobile no must start with 8, 9, or 6
            return Regex.IsMatch(mobileNo, "^[8-9-6]");
        }

        private bool IsValidEmail(string email)
        {
            // Valid email address format
            return Regex.IsMatch(email, @"^\S+@\S+\.\S+$");
        }

        private bool IsValidAddress(string address)
        {
            // Valid address must be minimum 10 characters and allow all special characters
            return address.Length >= 10;
        }
    }
}
