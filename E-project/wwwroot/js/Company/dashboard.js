// add hovered class to selected list item
let list = document.querySelectorAll(".navigation li");

function activeLink() {
    list.forEach((item) => {
        item.classList.remove("hovered");
    });
    this.classList.add("hovered");
}

list.forEach((item) => item.addEventListener("mouseover", activeLink));

// Menu Toggle
let toggle = document.querySelector(".toggle");
let navigation = document.querySelector(".navigation");
let main = document.querySelector(".main");

toggle.onclick = function () {
    navigation.classList.toggle("active");
    main.classList.toggle("active");
};
//profile img dropdown
const profileImg = document.getElementById("profile-img");
const profileDropdown = document.getElementById("profile-dropdown");

// Toggle the dropdown when clicking the profile image
profileImg.addEventListener("click", () => {
    profileDropdown.classList.toggle("show");
});

// Close the dropdown if clicked anywhere outside
window.addEventListener("click", (event) => {
    if (!event.target.closest(".profile")) {
        profileDropdown.classList.remove("show");
    }
});
