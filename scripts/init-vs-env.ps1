# Script pour initialiser l'environnement Visual Studio dans PowerShell
# Usage: . .\scripts\init-vs-env.ps1

$ErrorActionPreference = "Stop"

$vsPath = "C:\Program Files\Microsoft Visual Studio"
$vs2022Path = "$vsPath\2022"
$vs2024Path = "$vsPath\2024"  # Visual Studio 2025 (version 18)

# Chemins possibles pour vcvars64.bat
$possiblePaths = @(
    "$vs2024Path\BuildTools\VC\Auxiliary\Build\vcvars64.bat",
    "$vs2024Path\Community\VC\Auxiliary\Build\vcvars64.bat",
    "$vs2024Path\Professional\VC\Auxiliary\Build\vcvars64.bat",
    "$vs2024Path\Enterprise\VC\Auxiliary\Build\vcvars64.bat",
    "$vs2022Path\BuildTools\VC\Auxiliary\Build\vcvars64.bat",
    "$vs2022Path\Community\VC\Auxiliary\Build\vcvars64.bat",
    "$vs2022Path\Professional\VC\Auxiliary\Build\vcvars64.bat",
    "$vs2022Path\Enterprise\VC\Auxiliary\Build\vcvars64.bat"
)

$vcvarsPath = $null
foreach ($path in $possiblePaths) {
    if (Test-Path $path) {
        $vcvarsPath = $path
        break
    }
}

if (-not $vcvarsPath) {
    Write-Host "ERREUR: Visual Studio Build Tools non trouvés." -ForegroundColor Red
    Write-Host "Assurez-vous que Visual Studio ou Build Tools est installé avec la charge de travail 'Desktop development with C++'." -ForegroundColor Yellow
    exit 1
}

Write-Host "Initialisation de l'environnement Visual Studio..." -ForegroundColor Cyan
Write-Host "  Chemin: $vcvarsPath" -ForegroundColor Gray

# Exécuter vcvars64.bat et capturer les variables d'environnement
$envVars = cmd /c "`"$vcvarsPath`" && set" 2>&1

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERREUR: Échec de l'initialisation de l'environnement Visual Studio" -ForegroundColor Red
    $envVars | Write-Host
    exit 1
}

# Parser et définir les variables d'environnement
$envVars | ForEach-Object {
    if ($_ -match "^([^=]+)=(.*)$") {
        $varName = $matches[1]
        $varValue = $matches[2]
        [System.Environment]::SetEnvironmentVariable($varName, $varValue, "Process")
    }
}

Write-Host "✓ Environnement Visual Studio initialisé" -ForegroundColor Green
Write-Host ""

# Vérifier que le compilateur est disponible
$clPath = Get-Command cl -ErrorAction SilentlyContinue
if ($clPath) {
    Write-Host "Compilateur MSVC disponible: $($clPath.Source)" -ForegroundColor Green
}
else {
    Write-Host "AVERTISSEMENT: Le compilateur 'cl' n'est pas dans le PATH" -ForegroundColor Yellow
}

# Vérifier CMake
$cmakePath = Get-Command cmake -ErrorAction SilentlyContinue
if ($cmakePath) {
    $cmakeVersion = & cmake --version | Select-Object -First 1
    Write-Host "CMake disponible: $cmakeVersion" -ForegroundColor Green
}
else {
    Write-Host "AVERTISSEMENT: CMake n'est pas dans le PATH" -ForegroundColor Yellow
    Write-Host "  Installez CMake ou ajoutez-le au PATH" -ForegroundColor Yellow
}

Write-Host ""
