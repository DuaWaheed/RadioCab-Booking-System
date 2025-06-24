function showAmount() {
        const type = document.getElementById("paymentType").value; // Get the selected payment type
    const amountField = document.getElementById("paymentAmount"); // Get the input field for the amount
    let amount = "";

    // Set the amount based on the payment type selected
    if (type === "Monthly") {
        amount = "$15";
        } else if (type === "Quarterly") {
        amount = "$40";
        }

    // Update the value of the amount input field
    amountField.value = amount;
      }
    function confirmReset() {
       
        document.querySelector("form").reset();
        
      }
    function togglePassword(fieldId, iconSpan) {
        const field = document.getElementById(fieldId);
    const icon = iconSpan.querySelector("i");

    if (field.type === "password") {
        field.type = "text";
    icon.classList.remove("fa-eye");
    icon.classList.add("fa-eye-slash");
        } else {
        field.type = "password";
    icon.classList.remove("fa-eye-slash");
    icon.classList.add("fa-eye");
        }
      }

    const fileInput = document.getElementById('companyLogo');
    const fileNameSpan = document.getElementById('file-name');

    fileInput.addEventListener('change', function () {
    const fileName = this.files[0] ? this.files[0].name : "No file chosen";
    fileNameSpan.textContent = fileName;
  });



    document.addEventListener("DOMContentLoaded", function () {
    const form = document.querySelector("form");

    form.addEventListener("submit", function (e) {
        e.preventDefault();
    let isValid = true;

    function showError(id, message) {
        const errorDiv = document.getElementById("error-" + id);
    errorDiv.textContent = message;
    errorDiv.classList.add("text-danger");
    isValid = false;
      }

    function clearError(id) {
        const errorDiv = document.getElementById("error-" + id);
    errorDiv.textContent = "";
      }

    function validateRegex(id, regex, message) {
        const field = document.getElementById(id);
    if (!field.value.trim().match(regex)) {
        showError(id, message);
        } else {
        clearError(id);
        }
      }

    validateRegex("companyname", /^[a-zA-Z0-9\s]{2, 50}$/, "Enter a valid company name.");
    validateRegex("password", /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]{6,}$/, "Password must be at least 6 characters and contain letters and numbers.");
    validateRegex("designation", /^[a-zA-Z\s]{2, 50}$/, "Enter a valid designation.");
    validateRegex("address", /^[a-zA-Z0-9\s,.-/\\#&'"\(\)]{5, 100}$/, "Address must be between 5 to 100 characters and can include letters, numbers, spaces, commas, periods, hyphens, slashes, and special characters.");
    validateRegex("mobile", /^03[0-9]{9}$/, "Enter a valid Pakistani mobile number (e.g., 03XXXXXXXXX).");
    validateRegex("telephone", /^[0-9]{6, 15}$/, "Enter a valid telephone number.");
    validateRegex("email", /^[^\s@]+@[^\s@]+\.[^\s@]+$/, "Enter a valid email address.");

    const paymentType = document.getElementById("paymentType");
    if (!paymentType.value) {
        showError("paymentType", "Please select a payment type.");
      } else {
        clearError("paymentType");
      }

    const companyLogo = document.getElementById("companyLogo");
    const logoError = document.getElementById("error-companyLogo");

    // Check if the file input has a file selected
    if (!companyLogo || !companyLogo.files.length) {
        logoError.textContent = "Company logo is required.";
    logoError.classList.add("text-danger");
    isValid = false;
} else {
        // Clear any previous error message
        logoError.textContent = "";

    // Get the selected file
    const file = companyLogo.files[0];

    // Validate file type (png, jpg, jpeg)
    const validExtensions = ['image/png', 'image/jpeg', 'image/jpg'];
    if (!validExtensions.includes(file.type)) {
        logoError.textContent = "Only PNG, JPG, or JPEG files are allowed.";
    logoError.classList.add("text-danger");
    isValid = false;
    }

    // Validate file size (max 5MB)
    else if (file.size > 2 * 1024 * 1024) { // 5MB = 5 * 1024 * 1024 bytes
        logoError.textContent = "File size must be less than 2MB.";
    logoError.classList.add("text-danger");
    isValid = false;
    }
}

    // If form is valid, proceed with the form submission or any other action
    if (isValid) {
        alert("Form submitted successfully!");
    form.submit(); // Submit only if all validations pass
}

    });
  });
