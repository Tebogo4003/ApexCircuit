-- SECTION C - APEX PREDATOR CIRCUIT
-- Based on ERD: Roles, Users, Events, Categories, Enrolments, Results

CREATE DATABASE ApexCircuitDB;
GO
USE ApexCircuitDB;
GO

-- 1. Roles
CREATE TABLE Roles (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    RoleName VARCHAR(50) NOT NULL UNIQUE
);

-- 2. Users
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    RoleID INT NOT NULL,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);

-- 3. Events
CREATE TABLE Events (
    EventID INT PRIMARY KEY IDENTITY(1,1),
    OrganizerID INT NOT NULL,
    EventName VARCHAR(150) NOT NULL,
    Description VARCHAR(500),
    EventDate DATE NOT NULL,
    Location VARCHAR(100) NOT NULL,
    MaxEntrants VARCHAR(50),
    PostCode VARCHAR(20),
    FOREIGN KEY (OrganizerID) REFERENCES Users(UserID)
);

-- 4. Categories
CREATE TABLE Categories (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    EventID INT NOT NULL,
    CategoryName VARCHAR(100) NOT NULL,
    Description VARCHAR(500),
    EntryFee DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (EventID) REFERENCES Events(EventID)
);

-- 5. Enrolments
CREATE TABLE Enrolments (
    EnrolmentID INT PRIMARY KEY IDENTITY(1,1),
    CategoryID INT NOT NULL,
    UserID INT NOT NULL,
    EventID INT NOT NULL,
    EnrolmentDate DATE NOT NULL,
    Status VARCHAR(50) NOT NULL,
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID),
    FOREIGN KEY (EventID) REFERENCES Events(EventID)
);

-- 6. Results
CREATE TABLE Results (
    ResultID INT PRIMARY KEY IDENTITY(1,1),
    EnrolmentID INT NOT NULL UNIQUE,
    FinishTime TIME,
    Position INT,
    ResultStatus VARCHAR(20),
    FOREIGN KEY (EnrolmentID) REFERENCES Enrolments(EnrolmentID)
);
GO

-- ============================================
-- SAMPLE DATA - MEETS RUBRIC REQUIREMENTS
-- ============================================

-- Roles
INSERT INTO Roles (RoleName) VALUES 
('Organizer'), 
('Participant'), 
('Admin');

-- Users: 2 Organisers + 2 Participants + 1 Admin
INSERT INTO Users (RoleID, FirstName, LastName, Email, PasswordHash) VALUES 
(1, 'Thabo', 'Mokoena', 'thabo@raceday.co.za', 'hash_thabo_2026'),
(1, 'Sarah', 'van Wyk', 'sarah@raceday.co.za', 'hash_sarah_2026'),
(2, 'Lungile', 'Ndlovu', 'lungile.ndlovu@gmail.com', 'hash_lungile_2026'),
(2, 'Johan', 'Pretorius', 'johan.p@gmail.com', 'hash_johan_2026'),
(3, 'System', 'Admin', 'admin@raceday.co.za', 'hash_admin_2026');

-- Events: 3 Events (2 by Thabo, 1 by Sarah)
INSERT INTO Events (OrganizerID, EventName, Description, EventDate, Location, MaxEntrants, PostCode) VALUES 
(1, 'Soweto Marathon', 'Annual road race through the streets of Soweto celebrating community and fitness.', '2026-10-15', 'Soweto, Johannesburg', '5000', '1804'),
(1, 'Cape Town Cycle Tour', 'Scenic cycling event around the Cape Peninsula with stunning ocean views.', '2026-03-08', 'Cape Town Stadium', '35000', '8001'),
(2, 'Comrades Marathon', 'The ultimate human race between Durban and Pietermaritzburg.', '2026-06-14', 'Pietermaritzburg to Durban', '20000', '3201');

-- Categories: Categories for EACH event
-- Soweto Marathon categories
INSERT INTO Categories (EventID, CategoryName, Description, EntryFee) VALUES 
(1, 'Full Marathon', '42.2km road race for elite and amateur runners.', 450.00),
(1, 'Half Marathon', '21.1km road race for intermediate runners.', 350.00),
(1, '10km Fun Run', '10km casual run for families and beginners.', 200.00);

-- Cape Town Cycle Tour categories
INSERT INTO Categories (EventID, CategoryName, Description, EntryFee) VALUES 
(2, 'Elite Cyclists', 'Professional and semi-professional cyclists.', 800.00),
(2, 'Amateur Riders', 'Recreational cyclists completing the full route.', 600.00),
(2, 'Tandem Challenge', 'Pairs riding together on tandem bicycles.', 1000.00);

-- Comrades Marathon categories
INSERT INTO Categories (EventID, CategoryName, Description, EntryFee) VALUES 
(3, 'Up Run', 'Durban to Pietermaritzburg direction.', 700.00),
(3, 'Down Run', 'Pietermaritzburg to Durban direction.', 700.00),
(3, 'Charity Run', 'Running for a registered charity organisation.', 900.00);

-- Enrolments: Sample registrations
INSERT INTO Enrolments (CategoryID, UserID, EventID, EnrolmentDate, Status) VALUES 
(1, 3, 1, '2026-08-01', 'Confirmed'),      -- Lungile -> Soweto Full Marathon
(2, 3, 1, '2026-08-05', 'Confirmed'),      -- Lungile -> Soweto Half Marathon
(4, 4, 2, '2026-01-10', 'Confirmed'),      -- Johan -> Cape Town Elite
(5, 4, 2, '2026-01-12', 'Pending'),        -- Johan -> Cape Town Amateur
(7, 3, 3, '2026-04-20', 'Confirmed'),      -- Lungile -> Comrades Up Run
(8, 4, 3, '2026-04-22', 'Confirmed');      -- Johan -> Comrades Down Run

-- Results: Sample race results
INSERT INTO Results (EnrolmentID, FinishTime, Position, ResultStatus) VALUES 
(1, '02:45:30', 15, 'Completed'),     -- Lungile completed Soweto Marathon
(2, '01:30:15', 42, 'Completed'),     -- Lungile completed Half Marathon
(3, '03:15:00', 8, 'Completed'),      -- Johan completed Cape Town Elite
(5, '05:30:45', 120, 'Completed'),    -- Lungile completed Comrades
(6, '05:45:20', 145, 'Completed');    -- Johan completed Comrades

GO