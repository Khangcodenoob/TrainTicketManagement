/*
  SQL Server script: create database TrainTicketManagementDB.
  Run in SSMS or sqlcmd against your SQL Server instance.
*/

IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = N'TrainTicketManagementDB')
BEGIN
    CREATE DATABASE TrainTicketManagementDB;
END
GO

USE TrainTicketManagementDB;
GO

IF OBJECT_ID(N'dbo.Routes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Routes
    (
        RouteId INT NOT NULL IDENTITY(1, 1) CONSTRAINT PK_Routes PRIMARY KEY,
        DepartureStation NVARCHAR(100) NOT NULL,
        ArrivalStation NVARCHAR(100) NOT NULL,
        DistanceKm DECIMAL(10, 2) NOT NULL CONSTRAINT CK_Routes_DistanceKm CHECK (DistanceKm > 0),
        Status NVARCHAR(30) NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.TrainTrips', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TrainTrips
    (
        TrainTripId INT NOT NULL IDENTITY(1, 1) CONSTRAINT PK_TrainTrips PRIMARY KEY,
        TrainCode NVARCHAR(20) NOT NULL,
        RouteId INT NOT NULL,
        DepartureTime DATETIME2 NOT NULL,
        ArrivalTime DATETIME2 NOT NULL,
        TotalSeats INT NOT NULL CONSTRAINT CK_TrainTrips_TotalSeats CHECK (TotalSeats > 0),
        AvailableSeats INT NOT NULL CONSTRAINT CK_TrainTrips_AvailableSeats CHECK (AvailableSeats >= 0 AND AvailableSeats <= TotalSeats),
        BaseTicketPrice DECIMAL(18, 2) NOT NULL CONSTRAINT CK_TrainTrips_BaseTicketPrice CHECK (BaseTicketPrice > 0),
        Status NVARCHAR(30) NOT NULL,
        CONSTRAINT CK_TrainTrips_ArrivalTime CHECK (ArrivalTime > DepartureTime),
        CONSTRAINT FK_TrainTrips_Routes_RouteId FOREIGN KEY (RouteId) REFERENCES dbo.Routes(RouteId)
    );
END
GO

IF OBJECT_ID(N'dbo.Customers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Customers
    (
        CustomerId INT NOT NULL IDENTITY(1, 1) CONSTRAINT PK_Customers PRIMARY KEY,
        FullName NVARCHAR(100) NOT NULL,
        PhoneNumber NVARCHAR(15) NOT NULL CONSTRAINT UQ_Customers_PhoneNumber UNIQUE,
        Email NVARCHAR(100) NOT NULL CONSTRAINT UQ_Customers_Email UNIQUE,
        IdentityNumber NVARCHAR(20) NOT NULL CONSTRAINT UQ_Customers_IdentityNumber UNIQUE,
        Address NVARCHAR(255) NULL
    );
END
GO

IF OBJECT_ID(N'dbo.Tickets', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Tickets
    (
        TicketId INT NOT NULL IDENTITY(1, 1) CONSTRAINT PK_Tickets PRIMARY KEY,
        TicketCode NVARCHAR(30) NOT NULL CONSTRAINT UQ_Tickets_TicketCode UNIQUE,
        TrainTripId INT NOT NULL,
        CustomerId INT NOT NULL,
        SeatNumber NVARCHAR(10) NOT NULL,
        Price DECIMAL(18, 2) NOT NULL CONSTRAINT CK_Tickets_Price CHECK (Price > 0),
        BookingDate DATETIME2 NOT NULL,
        PaymentStatus NVARCHAR(30) NOT NULL,
        TicketStatus NVARCHAR(30) NOT NULL,
        CONSTRAINT FK_Tickets_TrainTrips_TrainTripId FOREIGN KEY (TrainTripId) REFERENCES dbo.TrainTrips(TrainTripId),
        CONSTRAINT FK_Tickets_Customers_CustomerId FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(CustomerId),
        CONSTRAINT UQ_Tickets_TrainTrip_Seat UNIQUE (TrainTripId, SeatNumber)
    );
END
GO
