#!/usr/bin/env pwsh

# Build script for REPOCosmeticsDisplay mod using .NET SDK
# No Visual Studio required!

param(
    [switch]$Debug = $false,
    [switch]$Clean = $false,
    [switch]$Deploy = $false
)

# Configuration
$ProjectDir = Join-Path $PSScriptRoot "REPOCosmeticsDisplay"
$OutputDir = Join-Path $ProjectDir "bin\Release"
$RepoPath = "C:\Users\cleme\Desktop\Jeux\steam\steamapps\common\REPO"
$PluginsDir = Join-Path $RepoPath "BepInEx\plugins"

# Colors for output
function Write-Success { Write-Host $args -ForegroundColor Green }
function Write-Error-Custom { Write-Host $args -ForegroundColor Red }
function Write-Info { Write-Host $args -ForegroundColor Cyan }

# Create output directory
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir | Out-Null
}

Write-Info "Building REPOCosmeticsDisplay mod..."
Write-Info "Using .NET SDK C# Compiler (no Visual Studio needed!)"
Write-Info ""

# Verify REPO installation
if (-not (Test-Path $RepoPath)) {
    Write-Error-Custom "ERROR: REPO installation not found at: $RepoPath"
    Write-Error-Custom "Please update the `$RepoPath variable in this script."
    exit 1
}

# Get C# compiler path from Unity MonoBleedingEdge
$unityBinPath = "C:\Program Files\Unity\Hub\Editor\2022.3.60f1\Editor\Data\MonoBleedingEdge\bin"
$cscPath = Join-Path $unityBinPath "csc"
$mcsPath = Join-Path $unityBinPath "mcs"

# Try csc first, then mcs
$compilerPath = $null
if (Test-Path $cscPath) {
    $compilerPath = $cscPath
} elseif (Test-Path $mcsPath) {
    $compilerPath = $mcsPath
} else {
    Write-Error-Custom "ERROR: C# compiler (csc or mcs) not found!"
    Write-Error-Custom "Expected at: $unityBinPath"
    exit 1
}

$cscPath = $compilerPath

Write-Info "Using compiler: $cscPath"

# Define source files
$SourceFiles = @(
    "$ProjectDir\Properties\AssemblyInfo.cs"
    "$ProjectDir\CosmeticsManager.cs"
    "$ProjectDir\UIManager.cs"
    "$ProjectDir\GameHooks.cs"
    "$ProjectDir\REPOCosmeticsMod.cs"
)

# Verify all source files exist
$MissingFiles = $SourceFiles | Where-Object { -not (Test-Path $_) }
if ($MissingFiles) {
    Write-Error-Custom "ERROR: Missing source files:"
    $MissingFiles | ForEach-Object { Write-Error-Custom "  - $_" }
    exit 1
}

# Define references
$References = @(
    "$RepoPath\BepInEx\core\BepInEx.dll"
    "$RepoPath\BepInEx\core\0Harmony.dll"
    "$RepoPath\REPO_Data\Managed\Assembly-CSharp.dll"
    "$RepoPath\REPO_Data\Managed\UnityEngine.dll"
    "$RepoPath\REPO_Data\Managed\UnityEngine.CoreModule.dll"
    "$RepoPath\REPO_Data\Managed\UnityEngine.IMGUIModule.dll"
    "$RepoPath\REPO_Data\Managed\UnityEngine.UIModule.dll"
    "$RepoPath\REPO_Data\Managed\UnityEngine.UI.dll"
)

# Verify all references exist
$MissingRefs = $References | Where-Object { -not (Test-Path $_) }
if ($MissingRefs) {
    Write-Error-Custom "ERROR: Missing assembly references:"
    $MissingRefs | ForEach-Object { Write-Error-Custom "  - $_" }
    exit 1
}

# Build compiler arguments
$CompilerArgs = @(
    "/target:library"
    "/out:$(Join-Path $OutputDir 'REPOCosmeticsDisplay.dll')"
    "/unsafe"
)

# Add references
$References | ForEach-Object { $CompilerArgs += "/reference:$_" }

# Add source files
$CompilerArgs += $SourceFiles

# Compile
Write-Info "Compiling..."
& $cscPath @CompilerArgs

# Check result
if ($LASTEXITCODE -eq 0) {
    $DllPath = Join-Path $OutputDir "REPOCosmeticsDisplay.dll"
    Write-Success ""
    Write-Success "BUILD SUCCESSFUL!"
    Write-Success "DLL created: $DllPath"
    Write-Success ""
    Write-Info "DLL size: $(((Get-Item $DllPath).Length / 1KB).ToString('F1')) KB"
} else {
    Write-Error-Custom ""
    Write-Error-Custom "BUILD FAILED (Exit code: $LASTEXITCODE)"
    exit $LASTEXITCODE
}
