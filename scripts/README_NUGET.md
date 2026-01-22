# Scripts de packaging NuGet

## pack-sentry-uno.ps1

Script PowerShell pour générer les packages NuGet nécessaires pour `Sentry.Uno` et ses dépendances.

### Usage

```powershell
# Génération basique (packages dans .\packages)
# Le script construira automatiquement les bibliothèques natives si nécessaire
.\scripts\pack-sentry-uno.ps1

# Avec options personnalisées
.\scripts\pack-sentry-uno.ps1 `
    -OutputPath "C:\NuGetPackages\Sentry" `
    -VersionSuffix "custom-1.0" `
    -Configuration Release

# Ignorer la construction des bibliothèques natives (si cmake n'est pas disponible)
.\scripts\pack-sentry-uno.ps1 -SkipNativeBuild
```

### Prérequis pour les bibliothèques natives

Si vous voulez inclure le support natif (Windows/Linux) dans le package Sentry, vous devez avoir :
- **CMake** installé et disponible dans le PATH
- **Git** pour initialiser les submodules
- **Un compilateur C++** (Visual Studio Build Tools sur Windows, gcc/clang sur Linux/macOS)

Si ces outils ne sont pas disponibles, utilisez `-SkipNativeBuild` pour générer des packages sans support natif (le package fonctionnera toujours pour Android/iOS/Uno Platform).

### Paramètres

- **OutputPath** : Chemin où les packages seront générés (défaut: `.\packages`)
- **VersionSuffix** : Suffixe de version à ajouter (défaut: `local`)
- **Configuration** : Configuration de build (défaut: `Release`)
- **SkipNativeBuild** : Ignorer la construction des bibliothèques natives (packages sans support natif Windows/Linux)

### Packages générés

1. `Sentry.{version}.nupkg` - SDK de base
2. `Sentry.Extensions.Logging.{version}.nupkg` - Intégration logging
3. `Sentry.Uno.{version}.nupkg` - Intégration Uno Platform

Chaque package inclut également un fichier `.snupkg` pour les symboles de débogage.

### Exemple de sortie

```
=========================================
Génération des packages NuGet Sentry.Uno
=========================================

Version complète: 6.0.0-local

Packaging Sentry...
  Building Sentry...
  Packing Sentry...
  ✓ Package créé: Sentry.6.0.0-local.nupkg (245.67 KB)
  ✓ Symboles créés: Sentry.6.0.0-local.snupkg (12.34 KB)

Packaging Sentry.Extensions.Logging...
  Building Sentry.Extensions.Logging...
  Packing Sentry.Extensions.Logging...
  ✓ Package créé: Sentry.Extensions.Logging.6.0.0-local.nupkg (89.12 KB)
  ✓ Symboles créés: Sentry.Extensions.Logging.6.0.0-local.snupkg (5.67 KB)

Packaging Sentry.Uno...
  Building Sentry.Uno...
  Packing Sentry.Uno...
  ✓ Package créé: Sentry.Uno.6.0.0-local.nupkg (156.78 KB)
  ✓ Symboles créés: Sentry.Uno.6.0.0-local.snupkg (8.90 KB)

=========================================
Packages générés avec succès!
=========================================
```

Pour plus de détails, consultez `docs/NUGET_PACKAGING.md`.
