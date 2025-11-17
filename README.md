# **RadioCabs.in – Online Radio Taxi Directory**

## **Introduction**
RadioCabs.in is a complete online directory for Radio Taxi companies. It allows users to
find and explore taxi services, drivers, and company listings by city. The platform providing a personalized, safe, and cost-effective transport experience.

---

## **Project Requirements**
- Companies register to advertise and list their services.  
- Users can access company info, search drivers, and explore services.  
- Payments for registrations, drivers, and advertisements can be made **Monthly or Quarterly**:

| Service | Monthly | Quarterly |
|---------|---------|-----------|
| Registration | $15 | $40 |
| Driver | $10 | $25 |
| Advertisement | $15 | $40 |

- Two logins:
  - **Registered Unit:** View own data, payment status, and orders.  
  - **User:** Search data, provide feedback, and register for services.

---

## **Functional Requirements**
### **Home Page**
- Introduction and purpose of the website.  
- Menus: **Listing, Drivers, Services, Advertise, Feedback**.  
- Authentication section for logins.

### **Listing Page**
- Register Radio Cab companies.  
- Search registered companies.  
- Registration form includes: Company Name, ID, Password, Contact, Address, Membership Type, Payment Type.

### **Drivers Page**
- Driver registration and search.  
- Form includes: Driver Name, ID, Contact, City, Experience, Description, Payment Type.

### **Advertise Page**
- Place advertising orders.  
- Form includes: Company info, Contact, Description, Payment Type.

### **Feedback Page**
- Collect user feedback: Complaint, Suggestion, Compliment.  
- Form includes: Name, Mobile, Email, City, Type, Description.  

**Note:** Every registration form has **Submit** and **Reset** buttons.

---

## **Technologies Used**
- **Frontend:** HTML5, CSS, JavaScript, Bootstrap  
- **Backend:** .NET
- **Database:** SQL Server
- **Browser Compatibility:** Chrome, Firefox, Edge, Safari  

---

## **Achievement**
- Successfully implemented all registration forms, search functionality, and payment system.  
- Organized User roles with proper data management.  

---

## **How to Run**

1.Clone the Project

Open in Visual Studio
Open the project in Visual Studio 2022 (or your preferred version).

2. Update Connection String

Open appsettings.json in the project root.

Locate the ConnectionStrings section:

"ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=RadioCabDB;Trusted_Connection=True;TrustServerCertificate=True;"
}


Replace YOUR_SERVER with your SQL Server instance name.

If using SQL Server authentication, modify to:

"DefaultConnection": "Server=YOUR_SERVER;Database=RadioCabDB;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;TrustServe

4. Configure Email Settings 

In appsettings.json, update the EmailSettings section according to your system:

"EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-app-password",
    "FromEmail": "your-email@gmail.com",
    "ReplyTo": "reply@example.com"
}


Replace with your actual email credentials.

Make sure “Less Secure Apps” or “App Password” is enabled for Gmail.


Restore Packages
Visual Studio should automatically restore NuGet packages.
If not, open Package Manager Console and run:

Update-Package -reinstall


Update the Database
Ensure that the migrations are included in the project.
In Package Manager Console, run:

Update-Database


This will create all required tables and radiocab database in your SQL Server database.

Run SQL Commands 
in sql server, files are given for company_sql, advertiser_sql and drivers_sql in my project
run the commands of these files on radiocabDB tables in sql server.

Start the Application
Press F5 or click Start in Visual Studio to run the project.

---
## **Screen Recording of the project**

demo part 1  https://drive.google.com/file/d/1E1a4zPgZEdpjebskOqEzugIth4mWa8hG/view?usp=drive_link
part 2 https://drive.google.com/file/d/1Icay40wR6oxPo44ndiZrjKljdyiFPiHd/view?usp=sharing
