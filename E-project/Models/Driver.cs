using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace E_project.Models
{
    public class Driver
    {
        [Key]
        public int DriverId { get; set; }


        [Required(ErrorMessage = "Driver Name is required.")]
        [RegularExpression(@"^(\w+\s){1,4}\w+$", ErrorMessage = "Driver name name must contain full name (at least 2 words and at most 5 words).")]
        public string DriverName { get; set; }


        [Required(ErrorMessage = "Password is required.")]
        [StringLength(12, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 12 characters.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,12}$", ErrorMessage = "Password must contain at least one letter, one number, and one special character.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [RegularExpression(@"^[a-zA-Z0-9\s,.\-/#]{5,100}$",
        ErrorMessage = "Invalid address format (e.g., House #22-B, Gulshan, Flat 5/A).")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^(\+92|0)?3[0-9]{9}$", ErrorMessage = "Enter a valid Pakistani mobile number.")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "Telephone number is required.")]
        [Phone(ErrorMessage = "Enter a valid telephone number.")]
        public string Telephone { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [CustomEmailValidation(ErrorMessage = "This email is already registered.")]
        public string Email { get; set; }

        public class CustomEmailValidation : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value == null)
                    return ValidationResult.Success;

                var email = value.ToString();
                var dbContext = (mycontext)validationContext.GetService(typeof(mycontext));

                if (dbContext == null)
                    throw new Exception("Database context is not available in validation.");

                bool emailExists = dbContext.Drivers.Any(d => d.Email == email);

                return emailExists ? new ValidationResult(ErrorMessage) : ValidationResult.Success;
            }
        }

        [Required(ErrorMessage = "Experience is required.")]
        [Range(0, 50, ErrorMessage = "Experience must be between 0 and 50 years.")]
        public int Experience { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 500 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,'-]+$", ErrorMessage = "Description contains invalid characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Payment Type is required.")]
        public string PaymentType { get; set; }

        [Required(ErrorMessage = "Payment Amount is required.")]
        public string PaymentAmount { get; set; }

        [Required(ErrorMessage = "Driver image is required.")]
        [FileExtensions(Extensions = "png,jpg,jpeg", ErrorMessage = "Only PNG, JPG, or JPEG files are allowed.")]
        [MaxFileSize(2 * 1024 * 1024, ErrorMessage = "The image size must not exceed 2 MB.")]
        public string DriverImage { get; set; }

        public class MaxFileSizeAttribute : ValidationAttribute
        {
            private readonly int _maxFileSizeInBytes;

            public MaxFileSizeAttribute(int maxFileSizeInBytes)
            {
                _maxFileSizeInBytes = maxFileSizeInBytes;
            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var file = value as IFormFile;

                if (file != null && file.Length > _maxFileSizeInBytes)
                {
                    string sizeText = _maxFileSizeInBytes >= 1024 * 1024
                        ? $"{_maxFileSizeInBytes / (1024 * 1024)} MB"
                        : $"{_maxFileSizeInBytes / 1024} KB";

                    return new ValidationResult(ErrorMessage ?? $"Maximum allowed file size is {sizeText}.");
                }

                return ValidationResult.Success;
            }
        }

        // Optional metadata fields
        public string Status { get; set; } = "Unpaid";
        public string VisibleMode { get; set; } = "Offline";
        public DateTime StatusUpdatedDate { get; set; } = DateTime.Now;
        public string ActivityState { get; set; } = "Active";
    }
}
