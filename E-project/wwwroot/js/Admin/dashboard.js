const allSideMenu = document.querySelectorAll(
    "#sidebar .side-menu.top li a"
);

allSideMenu.forEach((item) => {
    const li = item.parentElement;

    item.addEventListener("click", function () {
        allSideMenu.forEach((i) => {
            i.parentElement.classList.remove("active");
        });
        li.classList.add("active");
    });
});

// TOGGLE SIDEBAR
const menuBar = document.querySelector("#content nav .bx.bx-menu");
const sidebar = document.getElementById("sidebar");

menuBar.addEventListener("click", function () {
    sidebar.classList.toggle("hide");
});




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


// APEXCHART with orange and yellow
var options = {
    series: [{
        name: 'Sales 1',
        data: [31, 40, 28, 51, 42, 109, 100]
    }, {
        name: 'Sales 2',
        data: [11, 32, 45, 32, 34, 52, 41]
    }],
    chart: {
        height: 350,
        type: 'area'
    },
    colors: ['#FFA500', '#FFD700'], // orange, yellow
    dataLabels: {
        enabled: false
    },
    stroke: {
        curve: 'smooth'
    },
    xaxis: {
        type: 'datetime',
        categories: [
            "2018-09-19T00:00:00.000Z",
            "2018-09-19T01:30:00.000Z",
            "2018-09-19T02:30:00.000Z",
            "2018-09-19T03:30:00.000Z",
            "2018-09-19T04:30:00.000Z",
            "2018-09-19T05:30:00.000Z",
            "2018-09-19T06:30:00.000Z"
        ]
    },
    tooltip: {
        x: {
            format: 'dd/MM/yy HH:mm'
        },
    },
};

var chart = new ApexCharts(document.querySelector("#chart"), options);
chart.render();

// PROGRESSBAR
const allProgress = document.querySelectorAll('main .card .progress');

allProgress.forEach(item => {
    item.style.setProperty('--value', item.dataset.value)
})