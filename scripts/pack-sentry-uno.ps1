# Script pour générer les packages NuGet nécessaires pour Sentry.Uno
# Usage: .\scripts\pack-sentry-uno.ps1 [-OutputPath <chemin>] [-VersionSuffix <suffixe>] [-Configuration Release|Debug] [-SkipNativeBuild]

param(
    [string]$OutputPath = "$PSScriptRoot\..\packages",
    [string]$VersionSuffix = "local",
    [string]$Configuration = "Release",
    [switch]$SkipNativeBuild
)

$ErrorActionPreference = "Stop"

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Génération des packages NuGet Sentry.Uno" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Créer le dossier de sortie s'il n'existe pas
if (-not (Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
    Write-Host "Dossier de sortie créé: $OutputPath" -ForegroundColor Green
}

# Obtenir la version depuis Directory.Build.props
$rootProps = "$PSScriptRoot\..\Directory.Build.props"
$versionPrefix = (Select-String -Path $rootProps -Pattern 'VersionPrefix.*(\d+\.\d+\.\d+)' | ForEach-Object { $_.Matches.Groups[1].Value })
if (-not $versionPrefix) {
    $versionPrefix = "6.0.0"
    Write-Host "VersionPrefix non trouvée, utilisation de la version par défaut: $versionPrefix" -ForegroundColor Yellow
}

$fullVersion = if ($VersionSuffix) { "$versionPrefix-$VersionSuffix" } else { $versionPrefix }
Write-Host "Version complète: $fullVersion" -ForegroundColor Green
Write-Host ""

# Construire les bibliothèques natives si nécessaire (pour Sentry uniquement)
if (-not $SkipNativeBuild) {
    $nativeLibPath = "$PSScriptRoot\..\src\Sentry\Platforms\Native\sentry-native"
    $needsNativeBuild = $false

    # Vérifier si les natives Windows sont nécessaires et manquantes
    if ($IsWindows) {
        $winX64Path = Join-Path $nativeLibPath "win-x64\sentry-native.lib"
        if (-not (Test-Path $winX64Path)) {
            $needsNativeBuild = $true
            Write-Host "Bibliothèques natives manquantes détectées." -ForegroundColor Yellow
        }
    }

    if ($needsNativeBuild) {
        Write-Host "Construction des bibliothèques natives sentry-native..." -ForegroundColor Yellow
        Write-Host "  Cela peut prendre quelques minutes..." -ForegroundColor Gray

        $buildNativeScript = Join-Path $PSScriptRoot "build-sentry-native.ps1"
        if (-not (Test-Path $buildNativeScript)) {
            Write-Host "ERREUR: Script de build natif non trouvé: $buildNativeScript" -ForegroundColor Red
            Write-Host "  Utilisez -SkipNativeBuild pour ignorer cette étape (packages sans support natif)" -ForegroundColor Yellow
            exit 1
        }

        # Vérifier que cmake est installé
        $cmakeAvailable = Get-Command cmake -ErrorAction SilentlyContinue
        if (-not $cmakeAvailable) {
            Write-Host "AVERTISSEMENT: cmake n'est pas installé. Les bibliothèques natives ne peuvent pas être construites." -ForegroundColor Yellow
            Write-Host "  Installez cmake ou utilisez -SkipNativeBuild pour continuer sans natives." -ForegroundColor Yellow
            Write-Host ""
            $response = Read-Host "Voulez-vous continuer sans les bibliothèques natives? (O/N)"
            if ($response -ne "O" -and $response -ne "o") {
                exit 1
            }
            $SkipNativeBuild = $true
        }
        else {
            Push-Location $PSScriptRoot\..
            try {
                $nativeBuildResult = & $buildNativeScript 2>&1
                if ($LASTEXITCODE -ne 0) {
                    Write-Host "ERREUR: Échec de la construction des bibliothèques natives" -ForegroundColor Red
                    $nativeBuildResult | Write-Host
                    Write-Host ""
                    Write-Host "Vous pouvez utiliser -SkipNativeBuild pour ignorer cette étape" -ForegroundColor Yellow
                    exit 1
                }
                Write-Host "  ✓ Bibliothèques natives construites avec succès" -ForegroundColor Green
            }
            finally {
                Pop-Location
            }
            Write-Host ""
        }
    }
    else {
        Write-Host "Bibliothèques natives déjà présentes, construction ignorée." -ForegroundColor Green
        Write-Host ""
    }
}

# Ordre de packaging (dépendances d'abord)
$projectsToPack = @(
    @{ Name = "Sentry"; Path = "src\Sentry\Sentry.csproj" },
    @{ Name = "Sentry.Extensions.Logging"; Path = "src\Sentry.Extensions.Logging\Sentry.Extensions.Logging.csproj" },
    @{ Name = "Sentry.Uno"; Path = "src\Sentry.Uno\Sentry.Uno.csproj" }
)

$packagedProjects = @()

foreach ($project in $projectsToPack) {
    $projectPath = Join-Path $PSScriptRoot ".." $project.Path
    $projectPath = [System.IO.Path]::GetFullPath($projectPath)

    if (-not (Test-Path $projectPath)) {
        Write-Host "ERREUR: Projet non trouvé: $projectPath" -ForegroundColor Red
        exit 1
    }

    Write-Host "Packaging $($project.Name)..." -ForegroundColor Yellow

    # Construire d'abord pour s'assurer que tout compile
    # Pour Sentry.Uno, construire aussi les dépendances explicitement
    Write-Host "  Building $($project.Name)..." -ForegroundColor Gray
    if ($project.Name -eq "Sentry.Uno") {
        # Construire Sentry.Uno avec toutes ses dépendances
        # Ne pas utiliser --no-incremental pour éviter les problèmes de nettoyage
        # Utiliser /p:SkipNativeBuild=true si on skip les natives pour éviter les erreurs de nettoyage
        $buildProps = @()
        if ($SkipNativeBuild) {
            $buildProps += "/p:CI_PUBLISHING_BUILD=false"
        }
        $buildResult = dotnet build $projectPath -c $Configuration --nologo -v minimal $buildProps 2>&1
    }
    else {
        $buildResult = dotnet build $projectPath -c $Configuration --nologo -v minimal 2>&1
    }

    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERREUR: Échec du build de $($project.Name)" -ForegroundColor Red
        $buildResult | Write-Host
        exit 1
    }

    # Packager
    Write-Host "  Packing $($project.Name)..." -ForegroundColor Gray

    # Construire les propriétés MSBuild
    $msbuildProps = @(
        "Version=$fullVersion",
        "PackageOutputPath=$OutputPath",
        "IncludeSymbols=true",
        "SymbolPackageFormat=snupkg"
    )

    # Si on skip les natives pour Sentry, ne pas inclure les natives dans le package
    if ($SkipNativeBuild -and $project.Name -eq "Sentry") {
        $msbuildProps += "CI_PUBLISHING_BUILD=false"
        Write-Host "    (Packaging sans bibliothèques natives)" -ForegroundColor Gray
    }

    # Construire les arguments dotnet pack
    $packArgs = @(
        "pack",
        $projectPath,
        "-c", $Configuration,
        "--nologo",
        "-v", "minimal"
    )

    # Pour Sentry.Uno, ne pas utiliser --no-build car il a des dépendances complexes
    # qui doivent être construites pour tous les TFMs
    if ($project.Name -ne "Sentry.Uno") {
        $packArgs += "--no-build"
    }
    else {
        Write-Host "    (Construction des dépendances pendant le packaging)" -ForegroundColor Gray
    }

    # Ajouter les propriétés MSBuild
    foreach ($prop in $msbuildProps) {
        $packArgs += "-p:$prop"
    }

    $packResult = & dotnet $packArgs 2>&1

    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERREUR: Échec du packaging de $($project.Name)" -ForegroundColor Red
        $packResult | Write-Host
        exit 1
    }

    # Trouver le fichier .nupkg généré
    $nupkgFile = Get-ChildItem -Path $OutputPath -Filter "$($project.Name).$fullVersion.nupkg" -ErrorAction SilentlyContinue
    if ($nupkgFile) {
        $sizeKB = [math]::Round($nupkgFile.Length / 1KB, 2)
        Write-Host "  ✓ Package créé: $($nupkgFile.Name) ($sizeKB KB)" -ForegroundColor Green

        # Trouver aussi le .snupkg si présent
        $snupkgFile = Get-ChildItem -Path $OutputPath -Filter "$($project.Name).$fullVersion.snupkg" -ErrorAction SilentlyContinue
        if ($snupkgFile) {
            $snupkgSizeKB = [math]::Round($snupkgFile.Length / 1KB, 2)
            Write-Host "  ✓ Symboles créés: $($snupkgFile.Name) ($snupkgSizeKB KB)" -ForegroundColor Green
        }

        $packagedProjects += $project.Name
    }
    else {
        Write-Host "  ⚠ Avertissement: Fichier .nupkg non trouvé pour $($project.Name)" -ForegroundColor Yellow
    }

    Write-Host ""
}

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Packages générés avec succès!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Packages créés:" -ForegroundColor Cyan
foreach ($proj in $packagedProjects) {
    Write-Host "  - $proj" -ForegroundColor White
}
Write-Host ""
Write-Host "Emplacement: $OutputPath" -ForegroundColor Cyan
Write-Host ""
Write-Host "Pour créer un dépôt NuGet local, exécutez:" -ForegroundColor Yellow
Write-Host "  dotnet nuget add source `"$OutputPath`" --name SentryUnoLocal" -ForegroundColor White
Write-Host ""
Write-Host "Ou pour utiliser un dossier spécifique:" -ForegroundColor Yellow
Write-Host "  dotnet nuget add source `"<chemin-complet>`" --name SentryUnoLocal" -ForegroundColor White
Write-Host ""
