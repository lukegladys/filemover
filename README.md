# musicmover

This can be used to move song files from one folder to another easily.  
The application searches recursively through all directories under the source directory so all song files can be in zip files, nested directories, or in the root top folder.

---

## Deploy and Run Application on Windows

1. Clone repository to `<localfolder>\musicmover`

2. Navigate to repository root and publish the application

    `cd <localfolder>\musicmover`

    `dotnet publish -c Release --sc true -p:PublishSingleFile=true`

3. Navigate to the build output directory and run the application

    `cd <localfolder>\musicmover\src\MusicMover\bin\Release\net7.0\win-x64`

    `.\MusicMover.exe --help` -- This will give you more information about the two arguments (source/triage folder and target folder)