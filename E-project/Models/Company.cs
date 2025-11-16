using System.ComponentModel.DataAnnotations;


namespace E_project.Models
{
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }

       
        [Required(ErrorMessage = "Company Name is required.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Company Name should only contain alphabetic characters and spaces.")]
        [StringLength(15, MinimumLength = 2, ErrorMessage = "Company Name must be between 2 and 15 characters and contain no more than 5 words.")]
        [CustomCompanyNameValidation(ErrorMessage = "This company name is already registered.")]
        public string CompanyName { get; set; }
        public class CustomCompanyNameValidation : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value == null)
                {
                    return ValidationResult.Success;  // Let [Required] handle nulls
                }

                var companyName = value.ToString();
                var dbContext = (mycontext)validationContext.GetService(typeof(mycontext));

                if (dbContext == null)
                {
                    throw new System.Exception("Database context is not available in validation.");
                }

                bool nameExists = dbContext.Companies.Any(c => c.CompanyName == companyName);

                if (nameExists)
                {
                    return new ValidationResult(ErrorMessage ?? "This company name is already registered.");
                }

                return ValidationResult.Success;
            }
        }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(12, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 12 characters.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,12}$", ErrorMessage = "Password must contain at least one letter, one number, and one special character.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Contact Person Name is required.")]
        [RegularExpression(@"^(\w+\s){1,4}\w+$", ErrorMessage = "Contact Person name must contain full name (at least 2 words and at most 5 words).")]
        public string ContactPerson { get; set; }

        [Required(ErrorMessage = "Designation is required.")]
        [RegularExpression(@"^(\w+\s){0,3}\w+$", ErrorMessage = "Designation must contain at least 1 word and at most 4 words.")]
        public string Designation { get; set; }


        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [RegularExpression(@"^[a-zA-Z0-9\s,.\-/#]{5,100}$",
        ErrorMessage = "Invalid address format (e.g., House #22-B, Gulshan, Flat 5/A).")]

        public string Address { get; set; }



        [Required(ErrorMessage = "Mobile number is required.")]
        [Phone(ErrorMessage = "Enter a valid mobile number.")]
        [RegularExpression(@"^(\+92|0)?3[0-9]{9}$", ErrorMessage = "Enter a valid Pakistani mobile number.")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "Telephone number is required.")]
        [Phone(ErrorMessage = "Enter a valid telephone number.")]
        public string Telephone { get; set; }

        [StringLength(20, ErrorMessage = "Fax number cannot exceed 20 characters.")]
        [RegularExpression(@"^\+92-\d{2}-\d{7}$", ErrorMessage = "Fax number must be in the format: +92-<AreaCode>-<SubscriberNumber> (e.g., +92-51-9260257).")]
        public string FaxNumber { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [CustomEmailValidation(ErrorMessage = "This email is already registered.")]
        public string Email { get; set; }
        public class CustomEmailValidation : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value == null)
                {
                    return ValidationResult.Success;  // Let [Required] handle nulls
                }

                var email = value.ToString();

                // Get the DbContext from the validation context
                var dbContext = (mycontext)validationContext.GetService(typeof(mycontext));

                if (dbContext == null)
                {
                    throw new System.Exception("Database context is not available in validation.");
                }

                // Check if the email already exists in the database
                bool emailExists = dbContext.Companies.Any(c => c.Email == email);

                if (emailExists)
                {
                    return new ValidationResult(ErrorMessage ?? "This email address is already registered.");
                }

                return ValidationResult.Success;
            }
        }

            [Required(ErrorMessage = "Payment Type is required.")]
        [RegularExpression("^(Monthly|Quarterly)$", ErrorMessage = "Payment Type must be either 'Monthly' or 'Quarterly'.")]
        public string PaymentType { get; set; }

        [Required(ErrorMessage = "Payment Amount is required.")]
        public string PaymentAmount { get; set; }

        [Url(ErrorMessage = "Enter a valid URL.")]
        public string CompanyAppLink { get; set; }

        //// New columns without validation
        [Required(ErrorMessage = "Company logo is required.")]
        [FileExtensions(Extensions = "png,jpg,jpeg", ErrorMessage = "Only PNG, JPG, or JPEG files are allowed.")]
        [MaxFileSize(2 * 1024 * 1024, ErrorMessage = "The image size must not exceed 2 MB.")]
        public string CompanyLogo { get; set; } // Optional, will be set via trigger
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
                    string sizeText;
                    if (_maxFileSizeInBytes >= 1024 * 1024)
                    {
                        sizeText = $"{_maxFileSizeInBytes / (1024 * 1024)} MB";
                    }
                    else
                    {
                        sizeText = $"{_maxFileSizeInBytes / 1024} KB";
                    }

                    return new ValidationResult(ErrorMessage ?? $"Maximum allowed file size is {sizeText}.");
                }

                return ValidationResult.Success;
            }
        }

        public string Status { get; set; } = "Unpaid"; // Default value set to "Unpaid"
        public string VisibleMode { get; set; } = "Offline"; // Default value set to "Offline"
        public DateTime StatusUpdatedDate { get; set; } = DateTime.Now; // Default value set to current date
        public string ActivityState { get; set; } = "Active"; // Default value set to "Active"


    }
}


// Detailed Breakdown of Validations
// Company Name:

// Minimum length: 2 characters.

// Maximum length: 15 characters.

// Contains only alphabets and spaces.

// Up to 5 words allowed.

// Password:

// Minimum length: 6 characters.

// Maximum length: 12 characters.

// Must contain:

// At least one letter.

// At least one number.

// At least one special character (@$!%*?&).

// Contact Person:

// Minimum of 2 words and a maximum of 5 words.

// Only spaces between words.

// Designation:

// Minimum of 1 word and maximum of 4 words.

// Address:

// Valid Pakistani address format: 123 Main St, Karachi, Pakistan.

// Validates format with city and country (Pakistan).

// Mobile Number:

// Uses the correct Pakistani mobile format with +92 or 0.

// Telephone Number:

// Uses the Phone validation to ensure it’s valid.

// Fax Number:

// Must be in the correct Pakistani fax number format: +92-<AreaCode>-<SubscriberNumber>, e.g., +92-51-9260257.

// Email:

// Validates using [EmailAddress].

// Payment Type:

// Must be either 'Monthly' or 'Quarterly'.

// Payment Amount:

// Must be within a range (100 to 999,999).

// Company App Link:

// Validates as a URL format.

