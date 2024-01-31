// CustomAttributes/AllowedExtensionsAttribute.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace BookWormsOnline.CustomAttributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class AllowedFileExtensionsAttribute : ValidationAttribute
	{
		private readonly string[] _extensions;

		public AllowedFileExtensionsAttribute(string[] extensions)
		{
			_extensions = extensions;
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value is IFormFile file)
			{
				var extension = Path.GetExtension(file.FileName);

				if (Array.IndexOf(_extensions, extension.ToLower()) == -1)
				{
					return new ValidationResult(GetErrorMessage());
				}
			}

			return ValidationResult.Success;
		}

		public string GetErrorMessage()
		{
			return $"Only the following file extensions are allowed: {string.Join(", ", _extensions)}";
		}
	}
}
