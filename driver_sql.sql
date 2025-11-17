-- Step 1: Add columns to the Drivers table
ALTER TABLE Drivers
ADD 
    DriverLogo VARCHAR(100) NULL,
    Status VARCHAR(10) NOT NULL DEFAULT 'Unpaid',
    VisibleMode VARCHAR(10) NOT NULL DEFAULT 'Offline',
    StatusUpdatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    ActivityState VARCHAR(10) NOT NULL DEFAULT 'Active';


		-- Step 2: Create trigger to update StatusUpdatedDate when Status is changed
	CREATE TRIGGER UpdateDriverStatusDate
ON Drivers
AFTER UPDATE
AS
BEGIN
    -- Check if the 'Status' column was actually updated
    IF UPDATE(Status)
    BEGIN
        -- Update the StatusUpdatedDate only when the Status has changed
        UPDATE d
        SET StatusUpdatedDate = GETDATE()
        FROM Drivers d
        INNER JOIN inserted i ON d.DriverId = i.DriverId
        WHERE d.DriverId = i.DriverId;
    END
END;


-- Add the SQL Agent Job for Drivers
EXEC msdb.dbo.sp_add_job
    @job_name = N'DriverStatusUpdateJob',
    @enabled = 1;

-- Add job step
EXEC msdb.dbo.sp_add_jobstep
    @job_name = N'DriverStatusUpdateJob',
    @step_name = N'UpdateDriverStatusStep',
    @subsystem = N'TSQL',
    @command = N'
        DECLARE @currentDate DATETIME = GETDATE();

        -- Update Status to Unpaid based on PaymentType
        UPDATE Drivers
        SET Status = ''Unpaid''
        WHERE Status = ''Paid''
          AND (
              (PaymentType = ''Monthly'' AND StatusUpdatedDate <= DATEADD(DAY, -30, @currentDate)) OR
              (PaymentType = ''Quarterly'' AND StatusUpdatedDate <= DATEADD(DAY, -90, @currentDate))
          );

        -- Update VisibleMode
        UPDATE Drivers
        SET VisibleMode = CASE
            WHEN Status = ''Paid'' THEN ''Public''
            ELSE ''Offline''
        END;

        -- Set ActivityState to Disable after expiry if Unpaid
        UPDATE Drivers
        SET ActivityState = ''Disable''
        WHERE Status = ''Unpaid''
          AND (
              (PaymentType = ''Monthly'' AND StatusUpdatedDate <= DATEADD(DAY, -30, @currentDate)) OR
              (PaymentType = ''Quarterly'' AND StatusUpdatedDate <= DATEADD(DAY, -90, @currentDate))
          );

        -- Ensure ActivityState is Active if Paid
        UPDATE Drivers
        SET ActivityState = ''Active''
        WHERE Status = ''Paid'';
    ',
    @database_name = N'RadioCabDB'; -- Replace with your DB name

-- Create schedule
EXEC msdb.dbo.sp_add_schedule
    @schedule_name = N'DailyDriverStatusUpdate',
    @enabled = 1,
    @freq_type = 4,
    @freq_interval = 1,
    @active_start_time = 010000;

-- Attach schedule
EXEC msdb.dbo.sp_attach_schedule
    @job_name = N'DriverStatusUpdateJob',
    @schedule_name = N'DailyDriverStatusUpdate';


--manually driver insertion
INSERT INTO Drivers (DriverName, Password, Address, City, Mobile, Telephone, Email, Experience, Description, PaymentType, PaymentAmount, DriverImage, Status, VisibleMode, StatusUpdatedDate, ActivityState)
VALUES 
('John Doe', 'hashed_password_1', '1234 Street Name, City A', 'City A', '+923001234567', '021-12345678', 'johndoe@example.com', 5, 'Experienced driver with 5 years on the road.', 'Monthly', '1000', 'driver1.jpg', 'Paid', 'Public', GETDATE(), 'Active'),
('Alice Smith', 'hashed_password_2', '4567 Avenue, City B', 'City B', '+923001234568', '021-12345679', 'alicesmith@example.com', 3, 'Driver with a clean record, specializing in city tours.', 'Monthly', '1200', 'driver2.jpg', 'Paid', 'Public', GETDATE(), 'Active'),
('Bob Brown', 'hashed_password_3', '7890 Road, City C', 'City C', '+923001234569', '021-12345680', 'bobbrown@example.com', 2, 'New driver with great enthusiasm.', 'Quarterly', '1500', 'driver3.jpg', 'Paid', 'Public', GETDATE(), 'Active'),
('Sarah Wilson', 'hashed_password_4', '1357 Park, City D', 'City D', '+923001234570', '021-12345681', 'sarahwilson@example.com', 7, 'Veteran driver with excellent customer service skills.', 'Quarterly', '1400', 'driver4.jpg', 'Paid', 'Public', GETDATE(), 'Active'),
('Michael Johnson', 'hashed_password_5', '2468 Main St, City E', 'City E', '+923001234571', '021-12345682', 'michaeljohnson@example.com', 4, 'Driver with long experience in long-distance transport.', 'Monthly', '1600', 'driver5.jpg', 'Paid', 'Public', GETDATE(), 'Active'),
('Olivia Davis', 'hashed_password_6', '3690 Lakeview Blvd, City F', 'City F', '+923001234572', '021-12345683', 'oliviadavis@example.com', 6, 'Family-friendly driver, always punctual.', 'Monthly', '1100', 'driver6.jpg', 'Paid', 'Public', GETDATE(), 'Active'),
('James Taylor', 'hashed_password_7', '4812 Hilltop Rd, City G', 'City G', '+923001234573', '021-12345684', 'jamestaylor@example.com', 3, 'Reliable driver with a passion for driving.', 'Quarterly', '1300', 'driver7.jpg', 'Paid', 'Public', GETDATE(), 'Active'),
('Emily Clark', 'hashed_password_8', '5923 Ocean Dr, City H', 'City H', '+923001234574', '021-12345685', 'emilyclark@example.com', 5, 'Experienced in city and highway driving.', 'Monthly', '1000', 'driver8.jpg', 'Paid', 'Public', GETDATE(), 'Active'),
('David Lee', 'hashed_password_9', '7034 Sunset Blvd, City I', 'City I', '+923001234575', '021-12345686', 'davidlee@example.com', 8, 'Professional driver with a focus on safety.', 'Quarterly', '1450', 'driver9.jpg', 'Paid', 'Public', GETDATE(), 'Active'),
('Sophia Harris', 'hashed_password_10', '8145 Elm St, City J', 'City J', '+923001234576', '021-12345687', 'sophiaharris@example.com', 4, 'Young and energetic driver, ready for challenges.', 'Monthly', '1250', 'driver10.jpg', 'Paid', 'Public', GETDATE(), 'Active');


--execution
SELECT * FROM Drivers;