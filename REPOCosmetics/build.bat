@echo off
setlocal disabledelayedexpansion

set "PROJECT_DIR=%~dp0REPOCosmeticsDisplay"
set "OUTPUT_DIR=%PROJECT_DIR%\bin\Release"
set "REPO_PATH=C:\Users\cleme\Desktop\Jeux\steam\steamapps\common\REPO"
set "MCS=C:\Program Files\Unity\Hub\Editor\2022.3.60f1\Editor\Data\MonoBleedingEdge\bin\mcs"

if not exist "%OUTPUT_DIR%" mkdir "%OUTPUT_DIR%"

echo Building REPOCosmeticsDisplay...

"%MCS%" ^
  -target:library ^
  -out:"%OUTPUT_DIR%\CosmeticsUI.dll" ^
  -reference:"%REPO_PATH%\BepInEx\core\BepInEx.dll" ^
  -reference:"%REPO_PATH%\BepInEx\core\0Harmony.dll" ^
  -reference:"%REPO_PATH%\REPO_Data\Managed\Assembly-CSharp.dll" ^
  -reference:"%REPO_PATH%\REPO_Data\Managed\UnityEngine.dll" ^
  -reference:"%REPO_PATH%\REPO_Data\Managed\UnityEngine.CoreModule.dll" ^
  -reference:"%REPO_PATH%\REPO_Data\Managed\UnityEngine.UI.dll" ^
  -reference:"%REPO_PATH%\REPO_Data\Managed\Unity.TextMeshPro.dll" ^
  -reference:"%REPO_PATH%\REPO_Data\Managed\UnityEngine.InputLegacyModule.dll" ^
  -reference:"%REPO_PATH%\REPO_Data\Managed\netstandard.dll" ^
  "%PROJECT_DIR%\Properties\AssemblyInfo.cs" ^
  "%PROJECT_DIR%\REPOCosmeticsMod.cs"

if %ERRORLEVEL% EQU 0 (
  echo.
  echo BUILD SUCCESS!
  echo DLL: %OUTPUT_DIR%\REPOCosmeticsDisplay.dll
  echo.
  echo Deploying to plugins...
  copy "%OUTPUT_DIR%\REPOCosmeticsDisplay.dll" "%REPO_PATH%\BepInEx\plugins\" /Y
  echo Done.
  pause
) else (
  echo.
  echo BUILD FAILED - Error code: %ERRORLEVEL%
  pause
)
