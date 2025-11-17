﻿-- Step 1: Add columns to the Companies table
ALTER TABLE Companies
ADD 
    CompanyLogo VARCHAR(100) NULL,
    Status VARCHAR(10) NOT NULL DEFAULT 'Unpaid',
    VisibleMode VARCHAR(10) NOT NULL DEFAULT 'Offline',
    StatusUpdatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    ActivityState VARCHAR(10) NOT NULL DEFAULT 'Active';

	-- Step 2: Create trigger to update StatusUpdatedDate when Status is changed
CREATE TRIGGER UpdateStatusDate
ON Companies
AFTER UPDATE
AS
BEGIN
    -- Check if the 'Status' column was actually updated
    IF UPDATE(Status)
    BEGIN
        -- Update the StatusUpdatedDate only when the Status has changed
        UPDATE c
        SET StatusUpdatedDate = GETDATE()
        FROM Companies c
        INNER JOIN inserted i ON c.CompanyId = i.CompanyId
        WHERE c.CompanyId = i.CompanyId;
    END
END;


-- Step 3: Create Daily Scheduled Job (SQL Agent Job) for auto updates
-- The job should be created through SQL Server Agent using the following script

DECLARE @currentDate DATETIME = GETDATE();

-- Update Status to 'Unpaid' based on PaymentType
UPDATE Companies
SET Status = 'Unpaid'
WHERE Status = 'Paid'
  AND (
      (PaymentType = 'Monthly' AND StatusUpdatedDate <= DATEADD(DAY, -30, @currentDate)) OR
      (PaymentType = 'Quarterly' AND StatusUpdatedDate <= DATEADD(DAY, -90, @currentDate))
  );

-- Update VisibleMode to 'Public' if Paid
UPDATE Companies
SET VisibleMode = CASE
    WHEN Status = 'Paid' THEN 'Public'
    ELSE 'Offline'
END;

-- Set ActivityState to 'Disable' after expiry if Unpaid
UPDATE Companies
SET ActivityState = 'Disable'
WHERE Status = 'Unpaid'
  AND (
      (PaymentType = 'Monthly' AND StatusUpdatedDate <= DATEADD(DAY, -30, @currentDate)) OR
      (PaymentType = 'Quarterly' AND StatusUpdatedDate <= DATEADD(DAY, -90, @currentDate))
  );

-- Ensure ActivityState is 'Active' if Status is Paid
UPDATE Companies
SET ActivityState = 'Active'
WHERE Status = 'Paid';

-- Step 4: SQL Agent Job Creation (Automated Scheduling)

-- Create the SQL Agent Job
EXEC msdb.dbo.sp_add_job
    @job_name = N'CompanyStatusUpdateJob', -- Job name
    @enabled = 1;

-- Add a step to the job
EXEC msdb.dbo.sp_add_jobstep
    @job_name = N'CompanyStatusUpdateJob',
    @step_name = N'UpdateCompanyStatusStep',
    @subsystem = N'TSQL',
    @command = N'
        DECLARE @currentDate DATETIME = GETDATE();

        -- Update Status to Unpaid based on PaymentType
        UPDATE Companies
        SET Status = ''Unpaid''
        WHERE Status = ''Paid''
          AND (
              (PaymentType = ''Monthly'' AND StatusUpdatedDate <= DATEADD(DAY, -30, @currentDate)) OR
              (PaymentType = ''Quarterly'' AND StatusUpdatedDate <= DATEADD(DAY, -90, @currentDate))
          );

        -- Update VisibleMode to ''Public'' if Paid
        UPDATE Companies
        SET VisibleMode = CASE
            WHEN Status = ''Paid'' THEN ''Public''
            ELSE ''Offline''
        END;

        -- Set ActivityState to ''Disable'' after expiry if Unpaid
        UPDATE Companies
        SET ActivityState = ''Disable''
        WHERE Status = ''Unpaid''
          AND (
              (PaymentType = ''Monthly'' AND StatusUpdatedDate <= DATEADD(DAY, -30, @currentDate)) OR
              (PaymentType = ''Quarterly'' AND StatusUpdatedDate <= DATEADD(DAY, -90, @currentDate))
          );

        -- Ensure ActivityState is ''Active'' if Status is Paid
        UPDATE Companies
        SET ActivityState = ''Active''
        WHERE Status = ''Paid'';
    ',
    @database_name = N'RadioCabDB'; -- Replace with your actual DB name

-- Create the schedule for the job (e.g., Daily)
EXEC msdb.dbo.sp_add_schedule
    @schedule_name = N'DailyCompanyStatusUpdate',
    @enabled = 1,
    @freq_type = 4, -- Daily
    @freq_interval = 1, -- Every 1 day
    @active_start_time = 010000; -- Start time at 1:00 AM

-- Attach the schedule to the job
EXEC msdb.dbo.sp_attach_schedule
    @job_name = N'CompanyStatusUpdateJob',
    @schedule_name = N'DailyCompanyStatusUpdate';



--✅ What This SQL Server Script Does:
--1. Add Columns (CompanyLogo, Status, VisibleMode, StatusUpdatedDate, ActivityState):
--Adds new columns to your Companies table.

--Sets default values for Status, VisibleMode, and ActivityState.

--2. Triggers:
--SetCompanyLogoBeforeInsert: Automatically sets the CompanyLogo based on whether the CompanyId is odd or even during an insert operation.

--Odd CompanyId → 'companylogo.png'

--Even CompanyId → 'companylogoImg.jpeg'

--UpdateStatusDate: Updates the StatusUpdatedDate whenever the Status is changed.

--3. Daily Scheduled Job (using SQL Server Agent):
--This isn't a trigger but a scheduled SQL job that you would set to run every day using SQL Server Agent.

--Actions:

--Automatically updates Status to 'Unpaid' after 30 days (monthly) or 90 days (quarterly).

--Updates VisibleMode to 'Public' if Status is 'Paid'.

--Sets ActivityState to 'Disable' if the company is unpaid and expired.

--Ensures that ActivityState remains 'Active' if the company is paid.

INSERT INTO Companies(
    CompanyName, Password, ContactPerson, Designation, City, Address, 
    Mobile, Telephone, FaxNumber, Email, PaymentType, PaymentAmount, 
    CompanyAppLink, CompanyLogo, Status, VisibleMode, StatusUpdatedDate, ActivityState
)
VALUES
    ('Tech Innovations Ltd.', 'Password@123', 'John Doe', 'CEO', 'Karachi', 'House #22-B, Gulshan, Flat 5/A', 
     '+923012345678', '021-3456789', '+92-51-9260257', 'techinnovations@example.com', 'Monthly', '5000', 
     'http://techinnovations.com', 'techinnovationslogo.png', 'Paid', 'Online', '2025-05-01', 'Active'),
    
    ('Global Solutions Corp.', 'Password@456', 'Jane Smith', 'Manager', 'Lahore', 'Street #15, Model Town', 
     '+923014567890', '042-5678901', '+92-42-9123456', 'globalsolutions@example.com', 'Quarterly', '10000', 
     'http://globalsolutions.com', 'globalsolutionslogo.png', 'Paid', 'Online', '2025-05-01', 'Active'),

    ('Innovative Designs Inc.', 'Password@789', 'Michael Johnson', 'Director', 'Islamabad', 'Building #5, F-10', 
     '+923016789012', '051-6789012', '+92-51-9234567', 'innovativedesigns@example.com', 'Monthly', '7000', 
     'http://innovativedesigns.com', 'innovativedesignslogo.png', 'Paid', 'Online', '2025-05-01', 'Active'),

    ('Creative Labs Pvt. Ltd.', 'Password@321', 'Sara Lee', 'Marketing Head', 'Karachi', 'Sector 12, Karachi', 
     '+923017890123', '021-7654321', '+92-21-9345678', 'creativelabs@example.com', 'Quarterly', '12000', 
     'http://creativelabs.com', 'creativelabslogo.png', 'Paid', 'Online', '2025-05-01', 'Active'),

    ('TechPioneers Ltd.', 'Password@654', 'Robert Brown', 'CTO', 'Lahore', 'Block A, Gulberg', 
     '+923018901234', '042-8765432', '+92-42-9123456', 'techpioneers@example.com', 'Monthly', '9000', 
     'http://techpioneers.com', 'techpioneerslogo.png', 'Paid', 'Online', '2025-05-01', 'Active'),

    ('Future Enterprises Inc.', 'Password@987', 'Lisa White', 'HR Manager', 'Karachi', 'Street 10, Korangi', 
     '+923019012345', '021-9876543', '+92-51-9263456', 'futureenterprises@example.com', 'Quarterly', '15000', 
     'http://futureenterprises.com', 'futureenterpriseslogo.png', 'Paid', 'Online', '2025-05-01', 'Active'),

    ('Digital Vision Ltd.', 'Password@112', 'James Clark', 'Sales Manager', 'Lahore', 'Block C, Mall Road', 
     '+923020123456', '042-5432101', '+92-42-9432101', 'digitalvision@example.com', 'Monthly', '6000', 
     'http://digitalvision.com', 'digitalvisionlogo.png', 'Paid', 'Online', '2025-05-01', 'Active'),

    ('TechWorld Solutions Ltd.', 'Password@334', 'Emily Davis', 'Lead Engineer', 'Islamabad', 'Sector 20, Blue Area', 
     '+923021234567', '051-6543210', '+92-51-9267890', 'techworldsolutions@example.com', 'Quarterly', '11000', 
     'http://techworldsolutions.com', 'techworldsolutionslogo.png', 'Paid', 'Online', '2025-05-01', 'Active'),

    ('NextGen Technologies Ltd.', 'Password@556', 'David Martinez', 'Product Manager', 'Karachi', 'Shahra-e-Faisal', 
     '+923022345678', '021-4321098', '+92-21-9321098', 'nextgentechnologies@example.com', 'Monthly', '8000', 
     'http://nextgentechnologies.com', 'nextgentechnologieslogo.png', 'Paid', 'Online', '2025-05-01', 'Active'),

    ('FutureTech Labs Ltd.', 'Password@778', 'Olivia Harris', 'CTO', 'Lahore', 'Phase 3, DHA', 
     '+923023456789', '042-1234567', '+92-42-9123457', 'futuretechlabs@example.com', 'Quarterly', '13000', 
     'http://futuretechlabs.com', 'futuretechlabslogo.png', 'Paid', 'Online', '2025-05-01', 'Active');

--execution
SELECT * FROM Companies;

UPDATE Companies
SET ActivityState = 'Disable'  -- Replace with the value you want to set
WHERE CompanyId = 11;

DELETE FROM Companies WHERE CompanyId=12;