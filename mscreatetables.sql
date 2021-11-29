-- create database consid
-- go
use considsimon;
GO
-- SQLINES DEMO *** , create user & grant access to it, in the final seed script

-- SQLINES LICENSE FOR EVALUATION USE ONLY
if (not exists(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Category')) 
BEGIN
PRINT 'NO TABLE NAMED CATEGORY FOUND'
CREATE TABLE Category(ID INT CHECK (ID > 0) NOT NULL IDENTITY(1, 1), CategoryName NVARCHAR(100) NOT NULL, PRIMARY KEY(ID))

-- SQLINES LICENSE FOR EVALUATION USE ONLY
CREATE TABLE LibraryItem(ID INT CHECK (ID > 0) NOT NULL IDENTITY(1, 1),CategoryID INT CHECK (CategoryID > 0) NOT NULL,Title NVARCHAR(100) NOT NULL,Author NVARCHAR(100) NOT NULL,Pages INT CHECK (Pages > 0),RunTimeMinutes INT CHECK (RunTimeMinutes > 0),IsBorrowable BINARY(1) NOT NULL,Borrower NVARCHAR(100) NOT NULL,BorrowDate DATE,Type NVARCHAR(30) NOT NULL,PRIMARY KEY(ID),CONSTRAINT FK_CategoryLibraryItem FOREIGN KEY(CategoryID) REFERENCES Category(ID))

INSERT INTO Category (CategoryName)
VALUES
	('Computer Science'),
	('Art'), 
	('Programming'), 
	('Horror'), 
	('Drama');
END
GO

-- insert books
-- Programming books
-- SQLINES LICENSE FOR EVALUATION USE ONLY
DECLARE @programming AS INT
SELECT @programming = [ID]
	FROM Category
	WHERE CategoryName = 'Programming'
PRINT @programming 

DECLARE @cs AS INT
SELECT @cs = [ID]
	FROM Category
	WHERE CategoryName = 'Computer Science'
PRINT @cs

DECLARE @horror AS INT
SELECT @horror = [ID]
	FROM Category
	WHERE CategoryName = 'Horror'
PRINT @horror

-- SELECT @programming := ID FROM Category where CategoryName = 'Programming';
-- SQLINES LICENSE FOR EVALUATION USE ONLY
-- SELECT @horror := ID FROM Category where CategoryName = 'Horror';
-- SQLINES LICENSE FOR EVALUATION USE ONLY
-- SELECT @cs := ID FROM Category where CategoryName = 'Computer Science';
-- SQLINES LICENSE FOR EVALUATION USE ONLY
-- SELECT @art := ID FROM Category where CategoryName = 'Art';

-- SQLINES LICENSE FOR EVALUATION USE ONLY
-- SELECT @programming;
-- SQLINES LICENSE FOR EVALUATION USE ONLY
-- SELECT @horror;
-- SQLINES LICENSE FOR EVALUATION USE ONLY
-- select @cs;
-- SQLINES LICENSE FOR EVALUATION USE ONLY
-- select @art;

INSERT INTO [LibraryItem]([CategoryID], [Title], [Author], [Pages], [IsBorrowable], [Borrower], [Type])
VALUES
	(@programming, 'The C++ Programming Language', 'Bjarne Strostroup', 1376, 1, '', 'book'),
	(@programming, 'Effective C++', 'Scott Meyers', 297, 1, '', 'book'),
	(@cs, 'Computer Security, 3rd Edition', 'Dieter Gollman', 436, 1,'', 'book'),
	(@cs, 'Computer Architecture: A Quantitative Approach', 'David A. Patterson', 856, 1,'', 'book'),
	(@cs, 'Mikroprocessorteknik', 'Per Foyer', 276, 1, '', 'book'),
	(@programming, 'The Linux Programming Interface', 'Michael Kerrisk', 1506, 1, '', 'book');

-- SQLINES DEMO *** books
INSERT INTO LibraryItem([CategoryID], [Title], [Author], [Pages], [IsBorrowable], [Borrower],[Type])
VALUES
	(@cs, 'Intel 64 and IA-32 architectures software developer''s manual volume 1: Basic architecture', 'Intel', 482, 0, '', 'reference book'),
	(@cs, 'Intel 64 and IA-32 architectures optimization reference manual', 'Intel', 868, 0, '', 'reference book');
-- insert DVD:s
INSERT INTO LibraryItem([CategoryID], [Title], [Author], [RunTimeMinutes], [IsBorrowable], [Borrower],[Type])
VALUES
	(@horror, 'Event Horizon', 'Philip Eisner', 96, 1, '', 'dvd'),
	(@horror, 'Alien: Covenant', 'Dan O''Bannon', 122, 1, '', 'dvd'),
	(@horror, 'Predators', 'Alex Litvak', 107, 1, '', 'dvd'),
	(@horror, 'Pontypool', 'Tony Burgess', 93, 1, '', 'dvd');
go
