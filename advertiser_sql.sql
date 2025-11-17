-- Step 1: Add columns to the Advertisers table
ALTER TABLE Advertisers
ADD 
    CompanyLogo VARCHAR(100) NULL,
    Status VARCHAR(10) NOT NULL DEFAULT 'Unpaid',
    VisibleMode VARCHAR(10) NOT NULL DEFAULT 'Offline',
    StatusUpdatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    ActivityState VARCHAR(10) NOT NULL DEFAULT 'Active';

		-- Step 2: Create trigger to update StatusUpdatedDate when Status is changed
	CREATE TRIGGER UpdateAdvertiserStatusDate
ON Advertisers
AFTER UPDATE
AS
BEGIN
    -- Check if the 'Status' column was actually updated
    IF UPDATE(Status)
    BEGIN
        -- Update the StatusUpdatedDate only when the Status has changed
        UPDATE a
        SET StatusUpdatedDate = GETDATE()
        FROM Advertisers a
        INNER JOIN inserted i ON a.AdvertiserId = i.AdvertiserId
        WHERE a.AdvertiserId = i.AdvertiserId;
    END
END;

-- Add the SQL Agent Job for Advertisers
EXEC msdb.dbo.sp_add_job
    @job_name = N'AdvertiserStatusUpdateJob',
    @enabled = 1;

-- Add job step
EXEC msdb.dbo.sp_add_jobstep
    @job_name = N'AdvertiserStatusUpdateJob',
    @step_name = N'UpdateAdvertiserStatusStep',
    @subsystem = N'TSQL',
    @command = N'
        DECLARE @currentDate DATETIME = GETDATE();

        -- Update Status to Unpaid based on PaymentType
        UPDATE Advertisers
        SET Status = ''Unpaid''
        WHERE Status = ''Paid''
          AND (
              (PaymentType = ''Monthly'' AND StatusUpdatedDate <= DATEADD(DAY, -30, @currentDate)) OR
              (PaymentType = ''Quarterly'' AND StatusUpdatedDate <= DATEADD(DAY, -90, @currentDate))
          );

        -- Update VisibleMode
        UPDATE Advertisers
        SET VisibleMode = CASE
            WHEN Status = ''Paid'' THEN ''Public''
            ELSE ''Offline''
        END;

        -- Set ActivityState to Disable after expiry if Unpaid
        UPDATE Advertisers
        SET ActivityState = ''Disable''
        WHERE Status = ''Unpaid''
          AND (
              (PaymentType = ''Monthly'' AND StatusUpdatedDate <= DATEADD(DAY, -30, @currentDate)) OR
              (PaymentType = ''Quarterly'' AND StatusUpdatedDate <= DATEADD(DAY, -90, @currentDate))
          );

        -- Ensure ActivityState is Active if Paid
        UPDATE Advertisers
        SET ActivityState = ''Active''
        WHERE Status = ''Paid'';
    ',
    @database_name = N'RadioCabDB'; -- Replace with your DB name

-- Create schedule
EXEC msdb.dbo.sp_add_schedule
    @schedule_name = N'DailyAdvertiserStatusUpdate',
    @enabled = 1,
    @freq_type = 4,
    @freq_interval = 1,
    @active_start_time = 010000;

-- Attach schedule
EXEC msdb.dbo.sp_attach_schedule
    @job_name = N'AdvertiserStatusUpdateJob',
    @schedule_name = N'DailyAdvertiserStatusUpdate';

--manually advertisers insertion
INSERT INTO Advertisers (CompanyName, Password, ContactPerson, Designation, City, Address, Mobile, Telephone, FaxNumber, Email, Description, PaymentType, PaymentAmount, CompanyAppLink, CompanyLogo, Status, VisibleMode, StatusUpdatedDate, ActivityState)
VALUES 
('Tech Nova', 'Password123!', 'Alice Johnson', 'CEO', 'Karachi', '123 Main St, Karachi', '+923001234567', '+922100123456', '+92-21-1234567', 'alice@techinnovations.com', 'Leading tech solutions for businesses.', 'Monthly', '5000', 'http://techinnovations.com', 'techlogo.png', 'Paid', 'Online', GETDATE(), 'Active'),
('Green Earth', 'SecurePass456!', 'John Smith', 'Manager', 'Lahore', '456 Green Lane, Lahore', '+923001234568', '+922100123457', '+92-42-1234568', 'john@greenearth.com', 'Sustainable solutions for a greener future.', 'Quarterly', '12000', 'http://greenearth.com', 'greenlogo.png', 'Paid', 'Online', GETDATE(), 'Active'),
('Luxury Cars', 'LuxuryPass789!', 'Sarah Lee', 'Sales Director', 'Islamabad', '789 Car St, Islamabad', '+923001234569', '+922100123458', '+92-51-1234569', 'sarah@luxurycars.com', 'The best luxury cars available.', 'Monthly', '15000', 'http://luxurycars.com', 'luxurylogo.png', 'Paid', 'Online', GETDATE(), 'Active'),
('Urban Design', 'UrbanPass321!', 'Michael Brown', 'Founder', 'Karachi', '123 Urban Road, Karachi', '+923001234570', '+922100123459', '+92-21-1234570', 'michael@urbandesign.com', 'Creative urban design solutions.', 'Monthly', '8000', 'http://urbandesign.com', 'urbanlogo.png', 'Paid', 'Online', GETDATE(), 'Active'),
('Tech Solutions', 'TechPass234!', 'Emma Davis', 'CTO', 'Lahore', '101 Tech Park, Lahore', '+923001234571', '+922100123460', '+92-42-1234571', 'emma@techsolutions.com', 'Cutting-edge technology services.', 'Quarterly', '20000', 'http://techsolutions.com', 'techsolutionslogo.png', 'Paid', 'Online', GETDATE(), 'Active'),
('Foodie Heaven', 'FoodiePass987!', 'Oliver Wilson', 'Marketing Head', 'Karachi', '202 Food St, Karachi', '+923001234572', '+922100123461', '+92-21-1234572', 'oliver@foodieheaven.com', 'Delicious food delivery services.', 'Monthly', '7000', 'http://foodieheaven.com', 'foodielogo.png', 'Paid', 'Online', GETDATE(), 'Active'),
('Ocean View Hotels', 'OceanPass654!', 'Grace White', 'General Manager', 'Karachi', '303 Ocean Road, Karachi', '+923001234573', '+922100123462', '+92-21-1234573', 'grace@oceanviewhotels.com', 'Luxury hotels with an ocean view.', 'Quarterly', '30000', 'http://oceanviewhotels.com', 'oceanlogo.png', 'Paid', 'Online', GETDATE(), 'Active'),
('Smarte Solutions', 'SmartPass852!', 'Lucas Scott', 'Co-Founder', 'Islamabad', '404 Smart Lane, Islamabad', '+923001234574', '+922100123463', '+92-51-1234574', 'lucas@smarthomesolutions.com', 'Innovative smart home technologies.', 'Monthly', '10000', 'http://smarthomesolutions.com', 'smarthomelogo.png', 'Paid', 'Online', GETDATE(), 'Active'),
('Next Gen ', 'EduPass135!', 'Sophia Clark', 'CEO', 'Lahore', '505 Education Blvd, Lahore', '+923001234575', '+922100123464', '+92-42-1234575', 'sophia@nextgeneducation.com', 'Revolutionizing education with new methods.', 'Quarterly', '15000', 'http://nextgeneducation.com', 'educlogo.png', 'Paid', 'Online', GETDATE(), 'Active'),
('Global Traders', 'TradePass963!', 'David Adams', 'Director', 'Karachi', '606 Trade St, Karachi', '+923001234576', '+922100123465', '+92-21-1234576', 'david@globaltraders.com', 'Connecting markets globally.', 'Monthly', '25000', 'http://globaltraders.com', 'traderslogo.png', 'Paid', 'Online', GETDATE(), 'Active');


--manage table
ALTER TABLE Advertisers
ADD  Description VARCHAR(500);
ALTER TABLE Advertisers
ALTER COLUMN CompanyName VARCHAR(100);  

--execution
SELECT * FROM Advertisers;