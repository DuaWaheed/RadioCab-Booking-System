// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


//const counters = document.querySelectorAll('.counter');

//counters.forEach(counter => {
//    const updateCounter = () => {
//        const target = +counter.getAttribute('data-target');
//        const count = +counter.innerText;

//        const increment = target / 800; // adjust speed

//        if (count < target) {
//            counter.innerText = Math.ceil(count + increment);
//            setTimeout(updateCounter, 10);
//        } else {
//            counter.innerText = target + "+";
//        }
//    };

//    updateCounter();
//});
let countersStarted = false;

function startCounter() {
    const counters = document.querySelectorAll(".counter");
    counters.forEach(counter => {
        const target = +counter.getAttribute("data-target");
        let count = 0;

        const increment = target / 90;

        const updateCount = () => {
            if (count < target) {
                count += increment;
                counter.innerText = Math.ceil(count);
                requestAnimationFrame(updateCount);
            } else {
                counter.innerText = target + "+";
            }
        };
        updateCount();
    });
}

function isInViewport(element) {
    const rect = element.getBoundingClientRect();
    return (
        rect.top >= 0 &&
        rect.bottom <= (window.innerHeight || document.documentElement.clientHeight)
    );
}

window.addEventListener('scroll', () => {
    const counterSection = document.querySelector(".counter-container");

    if (!countersStarted && isInViewport(counterSection)) {
        startCounter();
        countersStarted = true;
    }
});



    let mybutton = document.getElementById("scrollTopBtn");

    window.onscroll = function () {
    if (document.body.scrollTop > 100 || document.documentElement.scrollTop > 100) {
        mybutton.style.display = "block";
    } else {
        mybutton.style.display = "none";
    }
  };

    function topFunction() {
        window.scrollTo({ top: 0, behavior: 'smooth' });
  }


let slides = document.querySelectorAll(".testimonial-slide");
let currentIndex = 0;

function showSlide(index) {
    slides.forEach(slide => slide.classList.remove("active"));
    slides[index].classList.add("active");
}

function autoSlide() {
    currentIndex = (currentIndex + 1) % slides.length;
    showSlide(currentIndex);
}

setInterval(autoSlide, 3000);


//function selectRole(role) {
//    document.getElementById("dropdownMenuButton1").textContent = role;
//}


document.querySelector('.open-popup-btn').addEventListener('click', () => {
    document.getElementById('popup').style.display = 'flex';
});

// Close Popup
document.getElementById('closePopup').addEventListener('click', () => {
    document.getElementById('popup').style.display = 'none';
});

// Tabs Logic
const tabLinks = document.querySelectorAll('.tab-link');
const tabContents = document.querySelectorAll('.tab-content');

tabLinks.forEach(button => {
    button.addEventListener('click', () => {
        const targetTab = button.getAttribute('data-tab');

        tabLinks.forEach(btn => btn.classList.remove('active'));
        button.classList.add('active');

        tabContents.forEach(content => {
            content.classList.remove('active');
            if (content.id === targetTab) {
                content.classList.add('active');
            }
        });
    });
});


   
