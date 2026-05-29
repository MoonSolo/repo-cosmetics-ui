@echo off
REM Set up Visual Studio environment
call "C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\Tools\VsDevCmd.bat"

REM Run MSBuild
msbuild REPOCosmeticsDisplay.csproj /p:Configuration=Release /p:Platform=AnyCPU

pause
