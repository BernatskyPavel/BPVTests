use BPVTests_2017;

create table Categories(CategoryName nvarchar(50) primary key);

create table Users(
	Username nvarchar(30) primary key,
	[Password] nvarchar(30) not null,
	UserType bit not null
);

create table Test(
	TestID int primary key identity,
	TestName nvarchar(50) not null,
	QuestionCount int not null,
	Category nvarchar(50) not null,
	MinSuccess int not null,
);

create table Questions(
	QuestionID int primary key identity,
	QuestionText nvarchar(max) not null,
	QuestionDifficult int not null,
	AnswersCount int not null,
	Picture varbinary(max) null,
	QuestionType int not null,
);

create table Tests(
	TestID int foreign key references Test(TestID) not null,
	QuestionID int foreign key references Questions(QuestionID) not null,
);

create table Answers(
	AnswerID int primary key identity,
	QuestionID int foreign key references Questions(QuestionID) not null,
	AnswerText nvarchar(max) null,
	TorF bit null,
	Picture varbinary(max) null,
	DnD nvarchar(max) null

);