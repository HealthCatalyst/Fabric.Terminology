# Fabric.Terminology Notes #

Following are notes for each release: new features and any steps required for upgrading (e.g. other services that need to be installed or upgraded).

## 1.0.x - In Development ##

- Fabric.Identity and Fabric.Authorization integration.

## 0.7.4 - In Development ##

- Fixes issue with Swagger pathing when deployed in a virtual directory
- Updates allowed package repositories in Nuget.Config to be able to reference private Health Catalyst "CatalystShared" packages.  Note, developers outside of HC will not longer be able to build this solution without obtaining certain packages.  This has been discussed in several planning meetings.

## 0.7.3  ##

- Refactor versioning scripts to use csproj files instead of AssemblyInfo using Powershell rather than gulp.

