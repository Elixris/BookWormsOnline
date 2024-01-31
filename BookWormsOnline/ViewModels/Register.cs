using System.ComponentModel.DataAnnotations;
using BookWormsOnline.CustomAttributes;
using Microsoft.AspNetCore.Http;

namespace BookWormsOnline.ViewModels
{
	public class Register
	{
		[Required]
		public string FirstName { get; set; }

		[Required]
		public string LastName { get; set; }

		[Required, StringLength(16, MinimumLength = 16)]
		public string CreditCardNo { get; set; }

		[Required, StringLength(8, MinimumLength = 8)]
		public string MobileNo { get; set; }

		[Required]
		public string BillingAddr { get; set; }

		[Required]
		public string ShippingAddr { get; set; }

		[Required, EmailAddress]
		public string Email { get; set; }

		[Required, DataType(DataType.Password)]
		[PasswordComplexity(ErrorMessage = "Password does not meet complexity requirements.")]
		public string Password { get; set; }

		[Required, DataType(DataType.Password), Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string CfmPassword { get; set; }

		[Display(Name = "Profile Image")]
		[DataType(DataType.Upload)]
		[AllowedFileExtensions(new[] { ".jpg" }, ErrorMessage = "Only .jpg files are allowed.")]
		public IFormFile? ProfileImage { get; set; }
	}
}
