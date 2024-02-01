using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using BookWormsOnline.Model;
using Microsoft.AspNetCore.DataProtection;

namespace BookWormsOnline.Pages
{
	[Authorize]
	public class IndexModel : PageModel
	{
		private readonly ILogger<IndexModel> _logger;
		private readonly UserManager<ApplicationUser> _userManager;
        

		public IndexModel(ILogger<IndexModel> logger, UserManager<ApplicationUser> userManager)
		{
			_logger = logger;
			_userManager = userManager;
		}
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DecryptedCreditCard { get; set; }
        public string MobileNo { get; set; }
        public string BillingAddr { get; set; }
        public string ShippingAddr { get; set; }

        public IActionResult OnGet()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = _userManager.FindByIdAsync(userId).Result;

            // Decrypt the credit card number using Unprotect
            var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
            var protector = dataProtectionProvider.CreateProtector("MySecretKey");
            FirstName = user.FirstName;
            LastName = user.LastName;
            DecryptedCreditCard = protector.Unprotect(user.CreditCardNo);
            MobileNo = user.MobileNo;
            BillingAddr = user.BillingAddr;
            ShippingAddr = user.ShippingAddr;

            return Page();
        }
    }
}
