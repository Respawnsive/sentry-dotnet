# Guide de génération et utilisation des packages NuGet Sentry.Uno

Ce guide explique comment générer les packages NuGet nécessaires pour `Sentry.Uno` à partir de votre fork custom et les utiliser dans d'autres projets.

## 📦 Packages générés

Les packages suivants seront générés (dans l'ordre des dépendances) :

1. **Sentry** - SDK de base Sentry
2. **Sentry.Extensions.Logging** - Intégration Microsoft.Extensions.Logging
3. **Sentry.Uno** - Intégration Uno Platform

## 🚀 Génération des packages

### Méthode 1 : Script PowerShell (Recommandé)

```powershell
# Depuis la racine du repository
# Le script construira automatiquement les bibliothèques natives si nécessaire
.\scripts\pack-sentry-uno.ps1
```

**Options disponibles :**

```powershell
# Spécifier un dossier de sortie personnalisé
.\scripts\pack-sentry-uno.ps1 -OutputPath "C:\NuGetPackages\Sentry"

# Spécifier un suffixe de version personnalisé
.\scripts\pack-sentry-uno.ps1 -VersionSuffix "custom-1.0"

# Générer en mode Debug (par défaut: Release)
.\scripts\pack-sentry-uno.ps1 -Configuration Debug

# Ignorer la construction des bibliothèques natives (si cmake n'est pas disponible)
.\scripts\pack-sentry-uno.ps1 -SkipNativeBuild
```

**Note importante :** Le package `Sentry` nécessite des bibliothèques natives (`sentry-native`) pour le support Windows/Linux. Le script tentera de les construire automatiquement si elles sont manquantes. Si vous n'avez pas CMake installé, utilisez `-SkipNativeBuild` pour générer des packages sans support natif (le package fonctionnera toujours pour Android/iOS/Uno Platform).

**Exemple complet :**

```powershell
.\scripts\pack-sentry-uno.ps1 `
    -OutputPath "C:\NuGetPackages\Sentry" `
    -VersionSuffix "custom-1.0" `
    -Configuration Release
```

### Méthode 2 : Commandes manuelles

Si vous préférez générer les packages manuellement :

```powershell
# Définir la version
$version = "6.0.0-local"
$outputPath = ".\packages"

# 1. Sentry (dépendance de base)
dotnet pack .\src\Sentry\Sentry.csproj `
    -c Release `
    -p:Version=$version `
    -p:PackageOutputPath=$outputPath `
    -p:IncludeSymbols=true `
    -p:SymbolPackageFormat=snupkg

# 2. Sentry.Extensions.Logging
dotnet pack .\src\Sentry.Extensions.Logging\Sentry.Extensions.Logging.csproj `
    -c Release `
    -p:Version=$version `
    -p:PackageOutputPath=$outputPath `
    -p:IncludeSymbols=true `
    -p:SymbolPackageFormat=snupkg

# 3. Sentry.Uno
dotnet pack .\src\Sentry.Uno\Sentry.Uno.csproj `
    -c Release `
    -p:Version=$version `
    -p:PackageOutputPath=$outputPath `
    -p:IncludeSymbols=true `
    -p:SymbolPackageFormat=snupkg
```

## 📁 Création d'un dépôt NuGet

### Option 1 : Azure DevOps Artifacts (Recommandé pour équipes)

Azure DevOps Artifacts offre une solution professionnelle pour héberger vos packages NuGet privés.

**Avantages :**
- ✅ Intégration native avec Azure DevOps
- ✅ Gestion des permissions fine
- ✅ Historique des versions
- ✅ Accessible depuis n'importe où
- ✅ Pas de serveur à maintenir

**Guide complet :** Consultez [`docs/AZURE_DEVOPS_NUGET.md`](AZURE_DEVOPS_NUGET.md) pour un guide pas à pas.

**Publication rapide :**

```powershell
# 1. Générer les packages
.\scripts\pack-sentry-uno.ps1

# 2. Publier vers Azure Artifacts
.\scripts\publish-to-azure-devops.ps1 -FeedName "SentryUnoCustom"
```

### Option 2 : Dossier local (Simple)

La méthode la plus simple est d'utiliser un dossier local comme source NuGet :

```powershell
# Ajouter le dossier comme source NuGet
dotnet nuget add source "C:\NuGetPackages\Sentry" --name SentryUnoLocal

# Vérifier que la source a été ajoutée
dotnet nuget list source
```

### Option 2 : BaGet (Serveur NuGet local)

Pour une solution plus robuste avec gestion de versions, utilisez [BaGet](https://github.com/loic-sharma/BaGet) :

```powershell
# Installer BaGet via Docker
docker run -d --name baget `
    -p 5000:80 `
    -v "$(pwd)/baget-data:/var/baget" `
    loicsharma/baget:latest

# Ajouter BaGet comme source NuGet
dotnet nuget add source http://localhost:5000/v3/index.json --name BaGetLocal

# Publier les packages dans BaGet
dotnet nuget push ".\packages\Sentry.6.0.0-local.nupkg" --source BaGetLocal --skip-duplicate
dotnet nuget push ".\packages\Sentry.Extensions.Logging.6.0.0-local.nupkg" --source BaGetLocal --skip-duplicate
dotnet nuget push ".\packages\Sentry.Uno.6.0.0-local.nupkg" --source BaGetLocal --skip-duplicate
```

### Option 3 : NuGet.Server (IIS)

Pour une solution d'entreprise, utilisez NuGet.Server sur IIS. Voir la [documentation officielle](https://learn.microsoft.com/en-us/nuget/hosting-packages/nuget-server).

## 🔧 Utilisation dans un autre projet

### 1. Ajouter la source NuGet locale

```powershell
# Si vous utilisez un dossier local
dotnet nuget add source "C:\NuGetPackages\Sentry" --name SentryUnoLocal

# Si vous utilisez BaGet
dotnet nuget add source http://localhost:5000/v3/index.json --name BaGetLocal
```

### 2. Ajouter le package dans votre projet

**Via CLI :**

```powershell
cd "C:\VotreProjet"
dotnet add package Sentry.Uno --version 6.0.0-local --source SentryUnoLocal
```

**Via fichier .csproj :**

```xml
<ItemGroup>
  <PackageReference Include="Sentry.Uno" Version="6.0.0-local" />
</ItemGroup>
```

**Via NuGet.Config :**

Créez ou modifiez `NuGet.Config` à la racine de votre solution :

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
    <add key="SentryUnoLocal" value="C:\NuGetPackages\Sentry" />
  </packageSources>
</configuration>
```

### 3. Vérifier l'installation

```powershell
# Restaurer les packages
dotnet restore

# Vérifier les packages installés
dotnet list package
```

## 🔄 Mise à jour des packages

Lorsque vous modifiez le code de Sentry.Uno :

1. **Rebuild et repackager :**
   ```powershell
   .\scripts\pack-sentry-uno.ps1 -VersionSuffix "custom-1.1"
   ```

2. **Nettoyer le cache NuGet local :**
   ```powershell
   # Supprimer le cache pour forcer la récupération de la nouvelle version
   dotnet nuget locals all --clear
   ```

3. **Mettre à jour dans votre projet :**
   ```powershell
   dotnet add package Sentry.Uno --version 6.0.0-custom-1.1 --source SentryUnoLocal
   ```

## 📝 Notes importantes

### Versioning

- La version est définie dans `Directory.Build.props` (`VersionPrefix`)
- En mode Debug, un suffixe `-dev` est automatiquement ajouté
- Vous pouvez spécifier un suffixe personnalisé avec `-VersionSuffix`

### Dépendances

Les packages sont générés dans l'ordre des dépendances :
- `Sentry` → `Sentry.Extensions.Logging` → `Sentry.Uno`

Assurez-vous que tous les packages sont dans le même dépôt pour que NuGet puisse résoudre les dépendances.

### Signature des assemblies

Les packages sont signés avec la clé dans `.assets\Sentry.snk`. Si vous modifiez cette clé, vous devrez peut-être ajuster les `InternalsVisibleTo` dans les projets.

### Compatibilité

Les packages générés sont compatibles avec :
- .NET 9.0 et .NET 10.0
- Android (net9.0-android35.0, net10.0-android36.0)
- iOS (net9.0-ios18.0, net10.0-ios26)
- Windows (net9.0-windows10.0.19041.0)

## 🐛 Dépannage

### Erreur : "Package not found"

- Vérifiez que la source NuGet est correctement configurée : `dotnet nuget list source`
- Vérifiez que les packages sont dans le bon dossier
- Nettoyez le cache : `dotnet nuget locals all --clear`

### Erreur : "Unable to resolve dependencies"

- Assurez-vous que tous les packages (Sentry, Sentry.Extensions.Logging, Sentry.Uno) sont dans le même dépôt
- Vérifiez que les versions correspondent

### Erreur lors du packaging

- Assurez-vous que tous les projets compilent correctement : `dotnet build`
- Vérifiez que vous êtes dans le bon répertoire
- Vérifiez les permissions d'écriture sur le dossier de sortie

## 📚 Ressources supplémentaires

- [Documentation NuGet - Sources locales](https://learn.microsoft.com/en-us/nuget/hosting-packages/local-feeds)
- [BaGet - Serveur NuGet local](https://github.com/loic-sharma/BaGet)
- [NuGet.Server - Serveur NuGet pour IIS](https://learn.microsoft.com/en-us/nuget/hosting-packages/nuget-server)
