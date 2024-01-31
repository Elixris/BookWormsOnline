using BookWormsOnline.CustomAttributes;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookWormsOnline.Model
{
	public class ApplicationUser : IdentityUser
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string CreditCardNo { get; set; }
		public string MobileNo { get; set; }
		public string BillingAddr { get; set; }
		public string ShippingAddr { get; set; }
	}
}
