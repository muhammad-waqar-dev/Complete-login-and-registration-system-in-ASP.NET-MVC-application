CREATE TABLE [dbo].[Student_LoginTB] (
    [StudentID]       INT              IDENTITY (1, 1) NOT NULL,
    [FirstName]       NVARCHAR (50)    NOT NULL,
    [LastName]        NVARCHAR (50)    NOT NULL,
    [DateOfBirth]     DATETIME         NULL,
    [EmailID]         NVARCHAR (254)   NOT NULL,
    [Password]        NVARCHAR (MAX)   NOT NULL,
    [IsEmailVerified] BIT              NOT NULL,
    [ActivationCode]  UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY CLUSTERED ([StudentID] ASC)
);
