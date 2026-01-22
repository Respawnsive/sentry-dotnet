# 🚀 Guide rapide : Génération et utilisation des packages NuGet Sentry.Uno

## Étape 1 : Installer les prérequis (si nécessaire)

Si vous voulez inclure le support natif (Windows/Linux) dans les packages, vous devez installer :

1. **CMake** : [https://cmake.org/download/](https://cmake.org/download/)
2. **Visual Studio Build Tools** avec la charge de travail "Desktop development with C++"

**Guide complet :** Consultez [`docs/INSTALL_CMAKE_WINDOWS.md`](docs/INSTALL_CMAKE_WINDOWS.md)

**Alternative rapide :** Si CMake n'est pas installé, utilisez `-SkipNativeBuild` pour générer des packages sans support natif (fonctionne toujours pour Android/iOS/Uno Platform).

## Étape 2 : Générer les packages

```powershell
# Depuis la racine du repository Sentry
# Le script construira automatiquement les bibliothèques natives si nécessaire
.\scripts\pack-sentry-uno.ps1

# Ou sans natives (si CMake n'est pas disponible)
.\scripts\pack-sentry-uno.ps1 -SkipNativeBuild
```

Les packages seront générés dans le dossier `.\packages` avec la version `6.0.0-local`.

## Étape 3 : Créer une source NuGet locale

```powershell
# Ajouter le dossier comme source NuGet
dotnet nuget add source ".\packages" --name SentryUnoLocal

# Ou avec un chemin absolu
dotnet nuget add source "C:\WS\GitHub\Sentry\packages" --name SentryUnoLocal
```

## Étape 4 : Utiliser dans votre projet

```powershell
# Dans votre autre projet
cd "C:\VotreAutreProjet"
dotnet add package Sentry.Uno --version 6.0.0-local --source SentryUnoLocal
```

## Alternative : Utiliser NuGet.Config

Créez un fichier `NuGet.Config` à la racine de votre solution :

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="SentryUnoLocal" value="C:\WS\GitHub\Sentry\packages" />
  </packageSources>
</configuration>
```

Ensuite, ajoutez simplement le package dans votre `.csproj` :

```xml
<ItemGroup>
  <PackageReference Include="Sentry.Uno" Version="6.0.0-local" />
</ItemGroup>
```

## Vérification

```powershell
# Vérifier que la source est configurée
dotnet nuget list source

# Vérifier les packages installés
dotnet list package
```

## Mise à jour des packages

Quand vous modifiez le code :

```powershell
# 1. Régénérer les packages avec une nouvelle version
.\scripts\pack-sentry-uno.ps1 -VersionSuffix "custom-1.1"

# 2. Nettoyer le cache NuGet
dotnet nuget locals all --clear

# 3. Mettre à jour dans votre projet
dotnet add package Sentry.Uno --version 6.0.0-custom-1.1 --source SentryUnoLocal
```

## 📚 Documentation complète

Pour plus de détails, consultez `docs/NUGET_PACKAGING.md`.
