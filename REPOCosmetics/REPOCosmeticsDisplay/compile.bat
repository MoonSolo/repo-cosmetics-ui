@echo off
cd /d "C:\Users\cleme\Documents\!projects\repo-cosmetics-ui\REPOCosmetics\REPOCosmeticsDisplay"

if not exist bin\Release mkdir bin\Release

"C:\Program Files\JetBrains\JetBrains Rider 2024.3.6\tools\MSBuild\Current\Bin\Roslyn\csc.exe" ^
  /target:library ^
  /out:bin\Release\CosmeticsUI.dll ^
  /langversion:7 ^
  /r:"C:\Users\cleme\Documents\!projects\BepInEx\core\0Harmony.dll" ^
  /r:"C:\Users\cleme\Documents\!projects\BepInEx\core\BepInEx.dll" ^
  /r:"T:\steam\steamapps\common\REPO\REPO_Data\Managed\Assembly-CSharp.dll" ^
  /r:"T:\steam\steamapps\common\REPO\REPO_Data\Managed\netstandard.dll" ^
  /r:"T:\steam\steamapps\common\REPO\REPO_Data\Managed\UnityEngine.dll" ^
  /r:"T:\steam\steamapps\common\REPO\REPO_Data\Managed\UnityEngine.CoreModule.dll" ^
  /r:"T:\steam\steamapps\common\REPO\REPO_Data\Managed\UnityEngine.UI.dll" ^
  /r:"T:\steam\steamapps\common\REPO\REPO_Data\Managed\Unity.TextMeshPro.dll" ^
  /r:"T:\steam\steamapps\common\REPO\REPO_Data\Managed\UnityEngine.IMGUIModule.dll" ^
  /r:"T:\steam\steamapps\common\REPO\REPO_Data\Managed\UnityEngine.TextRenderingModule.dll" ^
  CosmeticsManager.cs ^
  UIManager.cs ^
  REPOCosmeticsMod.cs ^
  Properties\AssemblyInfo.cs

if %ERRORLEVEL% EQU 0 (
  echo.
  echo SUCCESS! DLL compiled to: bin\Release\CosmeticsUI.dll
) else (
  echo.
  echo BUILD FAILED
)
