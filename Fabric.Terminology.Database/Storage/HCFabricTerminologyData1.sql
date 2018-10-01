/*
Do not change the database path or name variables.
Any sqlcmd variables will be properly substituted during 
build and deployment.
*/

ALTER DATABASE [$(DatabaseName)]
	ADD FILEGROUP [HCTerminologyData1] 

	GO
ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [HCTerminologyData1File1],
		FILENAME = '$(DataMountPoint)\HC$(DatabaseName)Data1File1.ndf',
		SIZE = 100MB,
		MAXSIZE = 5GB,
		FILEGROWTH = 100MB
	)

TO FILEGROUP [HCTerminologyData1];
GO