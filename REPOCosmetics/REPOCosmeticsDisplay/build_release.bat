@echo off
cd /d "C:\Users\cleme\Documents\!projects\repo-cosmetics-ui\REPOCosmetics\REPOCosmeticsDisplay"
call "C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\Tools\VsDevCmd.bat"
msbuild REPOCosmeticsDisplay.csproj /p:Configuration=Release /p:Platform=AnyCPU
