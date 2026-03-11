
-- Akshit's Tables for Student Features

CREATE TABLE [dbo].[Enrollment] (
    [enrollmentid] INT IDENTITY(1,1) NOT NULL,
    [userid] INT NOT NULL,
    [courseid] INT NOT NULL,
    [enrolldate] DATETIME NOT NULL,
    [isactive] BIT DEFAULT(1) NOT NULL,
    PRIMARY KEY CLUSTERED ([enrollmentid] ASC),
    CONSTRAINT [FK_Enrollment_User] FOREIGN KEY ([userid]) REFERENCES [dbo].[User]([userid]),
    CONSTRAINT [FK_Enrollment_Course] FOREIGN KEY ([courseid]) REFERENCES [dbo].[Course]([courseid])
);

CREATE TABLE [dbo].[LessonProgress] (
    [progressid] INT IDENTITY(1,1) NOT NULL,
    [userid] INT NOT NULL,
    [lessonid] INT NOT NULL,
    [iscompleted] BIT DEFAULT(0) NOT NULL,
    [completedtime] DATETIME NULL,
    PRIMARY KEY CLUSTERED ([progressid] ASC),
    CONSTRAINT [FK_LessonProgress_User] FOREIGN KEY ([userid]) REFERENCES [dbo].[User]([userid]),
    CONSTRAINT [FK_LessonProgress_Lesson] FOREIGN KEY ([lessonid]) REFERENCES [dbo].[Lesson]([lessonid])
);

CREATE TABLE [dbo].[StudentPoints] (
    [pointsid] INT IDENTITY(1,1) NOT NULL,
    [userid] INT NOT NULL,
    [totalpoints] INT DEFAULT(0) NOT NULL,
    [badge] NVARCHAR(100) NULL,
    [lastupdated] DATETIME NOT NULL,
    PRIMARY KEY CLUSTERED ([pointsid] ASC),
    CONSTRAINT [FK_StudentPoints_User] FOREIGN KEY ([userid]) REFERENCES [dbo].[User]([userid])
);

-- Test data
INSERT INTO Course (ownerid, coursename, description, price, creationtime, category, status)
VALUES 
(1, 'Introduction to C#', 'Learn C# basics', 0, GETDATE(), 'Programming', 1),
(1, 'Web Development', 'HTML, CSS, JavaScript', 0, GETDATE(), 'Web', 1);