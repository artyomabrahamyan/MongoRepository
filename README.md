## Purpose: 
---
Common reusable library which provide generic functionality for interaction with Mongo Db.
The result of this library is nuget package

**Platform:** 
NET5.0

**Supported OS-es:** 
Windows, Linux, MacOs

## Steps to build and test
---
**Restore:**
```		 
dotnet restore
```
For more details visit here [here](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-restore?tabs=netcore2x): 
		 
**Build:** 
```
dotnet build $PROJECT_NAME
```
i.e. simply ``` dotnet build ```

For more details visit [here](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-build)

**Test:** 
```
dotnet test $TEST_NAME
```

For more details visit [here](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-test?tabs=netcore21)


**Pack nuget:**
during build it project automatically pack to nuget

for manual pack visit [here](https://docs.microsoft.com/en-us/nuget/create-packages/creating-a-package) 

**Publish nuget**
1. open nuget folder
2. run powershell in that folder
3. run nuget push with specific version, 
```
for example 
nuget push -Source http://192.168.4.10:6547/v3/index.json DataOwl.Common.{VersionNumber}.nupkg
nuget push -Source http://192.168.4.10:6547/v3/index.json DataOwl.Common.IntegrationTests.{VersionNumber}.nupkg
```
