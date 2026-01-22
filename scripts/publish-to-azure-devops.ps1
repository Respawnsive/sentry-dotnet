# Script pour publier les packages Sentry.Uno vers Azure Artifacts
# Usage: .\scripts\publish-to-azure-devops.ps1 [-FeedName <nom>] [-PackagesPath <chemin>] [-ApiKey <clé>] [-Version <version>]

param(
    [string]$FeedName = "SentryUnoCustom",
    [string]$PackagesPath = ".\packages",
    [string]$ApiKey = "AZ",
    [string]$Version = "6.0.0-local"
)

$ErrorActionPreference = "Stop"

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Publication vers Azure Artifacts" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Vérifier que le dossier existe
if (-not (Test-Path $PackagesPath)) {
    Write-Host "ERREUR: Le dossier de packages n'existe pas: $PackagesPath" -ForegroundColor Red
    Write-Host "Générez d'abord les packages avec: .\scripts\pack-sentry-uno.ps1" -ForegroundColor Yellow
    exit 1
}

# Liste des packages à publier (dans l'ordre des dépendances)
$packages = @(
    "Sentry",
    "Sentry.Extensions.Logging",
    "Sentry.Uno"
)

$publishedCount = 0
$failedCount = 0

foreach ($packageName in $packages) {
    $packageFile = "$packageName.$Version.nupkg"
    $packagePath = Join-Path $PackagesPath $packageFile

    if (-not (Test-Path $packagePath)) {
        Write-Host "⚠ Package non trouvé: $packageFile" -ForegroundColor Yellow
        Write-Host "  Chemin attendu: $packagePath" -ForegroundColor Gray
        $failedCount++
        continue
    }

    Write-Host "Publication de $packageName..." -ForegroundColor Yellow
    Write-Host "  Fichier: $packageFile" -ForegroundColor Gray

    # Publier le package
    $publishResult = dotnet nuget push $packagePath `
        --source $FeedName `
        --api-key $ApiKey `
        --no-symbols `
        2>&1

    if ($LASTEXITCODE -eq 0) {
        $sizeKB = [math]::Round((Get-Item $packagePath).Length / 1KB, 2)
        Write-Host "  ✓ $packageName publié avec succès ($sizeKB KB)" -ForegroundColor Green
        $publishedCount++
    }
    else {
        Write-Host "  ✗ Échec de la publication de $packageName" -ForegroundColor Red

        # Afficher les erreurs pertinentes
        $publishResult | Where-Object { $_ -match "error|Error|ERROR|failed|Failed|FAILED" } | ForEach-Object {
            Write-Host "    $_" -ForegroundColor Red
        }

        # Si c'est une erreur d'authentification, donner des conseils
        if ($publishResult -match "401|Unauthorized|authentication") {
            Write-Host "    → Vérifiez votre authentification Azure DevOps" -ForegroundColor Yellow
            Write-Host "    → Essayez: az devops login" -ForegroundColor Yellow
        }

        # Si le package existe déjà
        if ($publishResult -match "already exists|409") {
            Write-Host "    → Le package existe déjà. Utilisez une nouvelle version ou supprimez l'ancienne." -ForegroundColor Yellow
        }

        $failedCount++
    }

    Write-Host ""
}

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Résumé de la publication" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Publiés avec succès: $publishedCount" -ForegroundColor $(if ($publishedCount -eq $packages.Count) { "Green" } else { "Yellow" })
Write-Host "Échecs: $failedCount" -ForegroundColor $(if ($failedCount -eq 0) { "Green" } else { "Red" })
Write-Host ""

if ($publishedCount -gt 0) {
    Write-Host "Pour vérifier dans Azure DevOps:" -ForegroundColor Cyan
    Write-Host "  1. Allez dans Artifacts > $FeedName" -ForegroundColor White
    Write-Host "  2. Vérifiez que vos packages sont listés" -ForegroundColor White
    Write-Host ""
}

if ($failedCount -gt 0) {
    Write-Host "Pour résoudre les problèmes:" -ForegroundColor Yellow
    Write-Host "  - Vérifiez que le feed '$FeedName' existe dans Azure DevOps" -ForegroundColor White
    Write-Host "  - Vérifiez votre authentification: az devops login" -ForegroundColor White
    Write-Host "  - Vérifiez vos permissions sur le feed" -ForegroundColor White
    Write-Host ""
    exit 1
}
