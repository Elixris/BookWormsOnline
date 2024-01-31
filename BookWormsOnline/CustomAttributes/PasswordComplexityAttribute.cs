// CustomAttributes/PasswordComplexityAttribute.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BookWormsOnline.CustomAttributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class PasswordComplexityAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value is string password)
			{
				var passwordComplexityError = CheckPasswordComplexity(password);
				if (!string.IsNullOrEmpty(passwordComplexityError))
				{
					return new ValidationResult(passwordComplexityError);
				}
			}

			return ValidationResult.Success;
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
	}
}
