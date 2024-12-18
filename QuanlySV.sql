CREATE DATABASE QuanlySV

ON
(NAME = QuanlySV_dat,
	FILENAME = 'E:\C_Sharp\LeMinhNhat_2280602198\QuanlySV.mdf',
	SIZE = 10,
	MAXSIZE = 50,
	FILEGROWTH = 5)
LOG ON 
	(NAME = QuanlySV_log,
	FILENAME = 'E:\C_Sharp\LeMinhNhat_2280602198\QuanlySV.ldf',
	SIZE = 5MB,
	MAXSIZE = 25 MB,
	FILEGROWTH = 5 MB);

CREATE TABLE LOP (
	MaLop char(3) Primary Key,
	TenLop nvarchar(30) not null
	);

CREATE TABLE SINHVIEN (
	MaSV char(6) Primary Key,
	HoTenSV nvarchar(40),
	NgaySinh date,
	MaLop char(3)
	);

ALTER TABLE SINHVIEN ADD CONSTRAINT FK_SV FOREIGN KEY (MaLop) REFERENCES LOP(MaLop)