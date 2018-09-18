/*
Do not change the database path or name variables.
Any sqlcmd variables will be properly substituted during 
build and deployment.
*/
ALTER DATABASE [$(DatabaseName)]
	ADD FILEGROUP [HCTerminologyIndex1]

	GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [HCTerminologyIndex1File1],
		FILENAME = '$(DataMountPoint)\HC$(DatabaseName)Index1File1.ndf',
		SIZE = 500MB,
		MAXSIZE = 2GB,
		FILEGROWTH = 100MB
	)

TO FILEGROUP [HCTerminologyIndex1];
GO
