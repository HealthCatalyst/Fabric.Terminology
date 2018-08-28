/*
Do not change the database path or name variables.
Any sqlcmd variables will be properly substituted during 
build and deployment.
*/
ALTER DATABASE [$(DatabaseName)]
	ADD FILEGROUP [HCFabricTerminologyIndex1]

	GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [HCFabricTerminologyIndex1File1],
		FILENAME = '$(FabricTerminologyDataMountPoint)\HC$(DatabaseName)Index1File1.ndf',
		SIZE = 500MB,
		MAXSIZE = 2GB,
		FILEGROWTH = 100MB
	)

TO FILEGROUP [HCFabricTerminologyIndex1];
GO
