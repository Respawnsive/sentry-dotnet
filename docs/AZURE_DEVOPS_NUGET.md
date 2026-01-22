# Guide Azure DevOps : Créer une source NuGet privée pour Sentry.Uno

Ce guide vous explique comment créer un feed Azure Artifacts (source NuGet privée) dans Azure DevOps et publier vos packages Sentry.Uno personnalisés.

## 📋 Prérequis

- Un compte Azure DevOps (dev.azure.com)
- Une organisation Azure DevOps
- Un projet dans votre organisation
- [Azure Artifacts Credential Provider](https://github.com/microsoft/artifacts-credprovider) installé
- [.NET SDK](https://dotnet.microsoft.com/download) installé (2.1.400+)

## 🚀 Étape 1 : Créer un feed Azure Artifacts

### 1.1 Accéder à Azure DevOps

1. Connectez-vous à [dev.azure.com](https://dev.azure.com)
2. Sélectionnez votre **organisation**
3. Naviguez vers votre **projet**

### 1.2 Créer le feed

1. Dans le menu de gauche, cliquez sur **Artifacts** (ou **Packages**)
2. Cliquez sur **Create Feed** (Créer un feed)
3. Remplissez le formulaire :
   - **Name** : Donnez un nom à votre feed (ex: `SentryUnoCustom`)
   - **Visibility** : 
     - **Private** : Seuls les utilisateurs autorisés peuvent voir les packages
     - **Public** : Accessible à tous sur Internet
   - **Scope** :
     - **Project** : Le feed est limité à votre projet (recommandé)
     - **Organization** : Le feed est accessible à toute l'organisation
   - **Include packages from common public sources** : Cochez cette case si vous voulez inclure les packages de nuget.org
4. Cliquez sur **Create**

![Création d'un feed Azure Artifacts](https://learn.microsoft.com/en-us/azure/devops/artifacts/media/create-new-feed-azure-devops.png)

## 🔐 Étape 2 : Installer Azure Artifacts Credential Provider

L'Azure Artifacts Credential Provider permet l'authentification automatique avec Azure DevOps.

### Windows (PowerShell)

```powershell
# Télécharger et installer le credential provider
iex "& { $(irm https://aka.ms/install-artifacts-credprovider.ps1) } -AddNetfx"
```

### macOS/Linux

```bash
# Télécharger et installer le credential provider
sh -c "$(curl -fsSL https://aka.ms/install-artifacts-credprovider.sh)"
```

**Vérification :**

```powershell
# Vérifier que le credential provider est installé
dotnet nuget list source
```

## 🔗 Étape 3 : Configurer votre projet pour se connecter au feed

### 3.1 Obtenir l'URL du feed

1. Dans Azure DevOps, allez dans **Artifacts**
2. Sélectionnez votre feed dans le menu déroulant
3. Cliquez sur **Connect to Feed** (Se connecter au feed)
4. Sélectionnez **dotnet** dans la section NuGet
5. **Copiez l'URL** affichée dans la section **Project setup**

L'URL ressemble à :
- **Feed au niveau projet** : `https://pkgs.dev.azure.com/<ORGANIZATION_NAME>/<PROJECT_NAME>/_packaging/<FEED_NAME>/nuget/v3/index.json`
- **Feed au niveau organisation** : `https://pkgs.dev.azure.com/<ORGANIZATION_NAME>/_packaging/<FEED_NAME>/nuget/v3/index.json`

### 3.2 Créer le fichier nuget.config

Créez un fichier `nuget.config` à la racine de votre projet Sentry (à côté de `Sentry.sln`).

**Pour un feed au niveau projet :**

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="SentryUnoCustom" value="https://pkgs.dev.azure.com/<ORGANIZATION_NAME>/<PROJECT_NAME>/_packaging/<FEED_NAME>/nuget/v3/index.json" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
  </packageSources>
</configuration>
```

**Pour un feed au niveau organisation :**

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="SentryUnoCustom" value="https://pkgs.dev.azure.com/<ORGANIZATION_NAME>/_packaging/<FEED_NAME>/nuget/v3/index.json" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
  </packageSources>
</configuration>
```

**Remplacez :**
- `<ORGANIZATION_NAME>` : Le nom de votre organisation Azure DevOps
- `<PROJECT_NAME>` : Le nom de votre projet (si feed au niveau projet)
- `<FEED_NAME>` : Le nom de votre feed

**Exemple concret :**

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="SentryUnoCustom" value="https://pkgs.dev.azure.com/MyCompany/MyProject/_packaging/SentryUnoCustom/nuget/v3/index.json" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
  </packageSources>
</configuration>
```

### 3.3 Authentification automatique

L'Azure Artifacts Credential Provider s'authentifiera automatiquement avec vos identifiants Azure DevOps lorsque vous utiliserez `dotnet` ou `nuget`.

**Première connexion :**

Lors de la première utilisation, une fenêtre de connexion s'ouvrira pour vous authentifier avec Azure DevOps.

## 📦 Étape 4 : Générer les packages NuGet

Utilisez le script fourni pour générer vos packages :

```powershell
# Depuis la racine du repository Sentry
.\scripts\pack-sentry-uno.ps1 -OutputPath ".\packages"
```

Cela génère les packages suivants dans `.\packages` :
- `Sentry.6.0.0-local.nupkg`
- `Sentry.Extensions.Logging.6.0.0-local.nupkg`
- `Sentry.Uno.6.0.0-local.nupkg`

## 🚀 Étape 5 : Publier les packages dans Azure Artifacts

### 5.1 Publier un package unique

```powershell
# Publier Sentry (dépendance de base)
dotnet nuget push ".\packages\Sentry.6.0.0-local.nupkg" `
    --source "SentryUnoCustom" `
    --api-key AZ

# Publier Sentry.Extensions.Logging
dotnet nuget push ".\packages\Sentry.Extensions.Logging.6.0.0-local.nupkg" `
    --source "SentryUnoCustom" `
    --api-key AZ

# Publier Sentry.Uno
dotnet nuget push ".\packages\Sentry.Uno.6.0.0-local.nupkg" `
    --source "SentryUnoCustom" `
    --api-key AZ
```

**Note :** Le paramètre `--api-key` est requis mais vous pouvez utiliser n'importe quelle chaîne (ex: `AZ`, `key`, etc.).

### 5.2 Publier tous les packages automatiquement

Créez un script PowerShell pour publier tous les packages :

```powershell
# scripts/publish-to-azure-devops.ps1
param(
    [string]$FeedName = "SentryUnoCustom",
    [string]$PackagesPath = ".\packages",
    [string]$ApiKey = "AZ"
)

$packages = @(
    "Sentry.6.0.0-local.nupkg",
    "Sentry.Extensions.Logging.6.0.0-local.nupkg",
    "Sentry.Uno.6.0.0-local.nupkg"
)

foreach ($package in $packages) {
    $packagePath = Join-Path $PackagesPath $package
    if (Test-Path $packagePath) {
        Write-Host "Publishing $package..." -ForegroundColor Yellow
        dotnet nuget push $packagePath --source $FeedName --api-key $ApiKey
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ $package published successfully" -ForegroundColor Green
        } else {
            Write-Host "✗ Failed to publish $package" -ForegroundColor Red
        }
    } else {
        Write-Host "⚠ Package not found: $packagePath" -ForegroundColor Yellow
    }
}
```

**Utilisation :**

```powershell
.\scripts\publish-to-azure-devops.ps1 -FeedName "SentryUnoCustom"
```

## ✅ Étape 6 : Vérifier la publication

1. Dans Azure DevOps, allez dans **Artifacts**
2. Sélectionnez votre feed
3. Vous devriez voir vos 3 packages listés :
   - Sentry
   - Sentry.Extensions.Logging
   - Sentry.Uno

## 🔧 Étape 7 : Utiliser les packages dans un autre projet

### 7.1 Configurer le projet cible

Dans votre autre projet, créez un fichier `nuget.config` à la racine :

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
    <add key="SentryUnoCustom" value="https://pkgs.dev.azure.com/<ORGANIZATION_NAME>/<PROJECT_NAME>/_packaging/<FEED_NAME>/nuget/v3/index.json" />
  </packageSources>
</configuration>
```

### 7.2 Ajouter le package

**Via CLI :**

```powershell
cd "C:\VotreAutreProjet"
dotnet add package Sentry.Uno --version 6.0.0-local --source SentryUnoCustom
```

**Via fichier .csproj :**

```xml
<ItemGroup>
  <PackageReference Include="Sentry.Uno" Version="6.0.0-local" />
</ItemGroup>
```

### 7.3 Restaurer les packages

```powershell
dotnet restore
```

## 🔐 Authentification avec Personal Access Token (PAT)

Si vous avez besoin d'authentifier depuis un autre environnement ou une autre organisation :

### Créer un PAT

1. Dans Azure DevOps, cliquez sur votre **profil** (en haut à droite)
2. Sélectionnez **Personal access tokens**
3. Cliquez sur **New Token**
4. Configurez le token :
   - **Name** : Donnez un nom descriptif
   - **Organization** : Sélectionnez votre organisation
   - **Expiration** : Définissez une date d'expiration
   - **Scopes** : Sélectionnez **Packaging** > **Read & write**
5. Cliquez sur **Create**
6. **Copiez le token** (vous ne pourrez plus le voir après)

### Utiliser le PAT

```powershell
# Ajouter la source avec authentification PAT
dotnet nuget add source `
    https://pkgs.dev.azure.com/<ORGANIZATION_NAME>/<PROJECT_NAME>/_packaging/<FEED_NAME>/nuget/v3/index.json `
    --name SentryUnoCustom `
    --username <USER_NAME> `
    --password <PERSONAL_ACCESS_TOKEN> `
    --configfile nuget.config

# Publier avec cette source
dotnet nuget push ".\packages\Sentry.Uno.6.0.0-local.nupkg" `
    --source SentryUnoCustom `
    --api-key AZ
```

## 🔄 Mise à jour des packages

Quand vous modifiez le code et voulez publier une nouvelle version :

```powershell
# 1. Générer les nouveaux packages avec une nouvelle version
.\scripts\pack-sentry-uno.ps1 -VersionSuffix "custom-1.1"

# 2. Publier les nouveaux packages
.\scripts\publish-to-azure-devops.ps1 -FeedName "SentryUnoCustom"

# 3. Dans votre autre projet, mettre à jour la version
dotnet add package Sentry.Uno --version 6.0.0-custom-1.1 --source SentryUnoCustom
```

## 🎯 Bonnes pratiques

1. **Versioning** : Utilisez un système de versionnement cohérent (SemVer recommandé)
2. **Sécurité** : Ne commitez jamais les PAT dans le contrôle de version
3. **Permissions** : Limitez les permissions du feed selon vos besoins
4. **Retention** : Configurez une politique de rétention pour éviter l'accumulation de packages
5. **Documentation** : Documentez les versions et les changements dans votre feed

## 🐛 Dépannage

### Erreur : "Unable to load the service index"

- Vérifiez que l'URL du feed est correcte
- Vérifiez que vous êtes authentifié : `az devops login`
- Vérifiez que le credential provider est installé

### Erreur : "Response status code does not indicate success: 401"

- Vérifiez vos permissions sur le feed
- Vérifiez que votre PAT n'a pas expiré
- Réauthentifiez-vous avec `az devops login`

### Erreur : "Package already exists"

- Les packages avec la même version ne peuvent pas être republiés
- Utilisez une nouvelle version ou supprimez l'ancienne version du feed

## 📚 Ressources supplémentaires

- [Documentation officielle Azure Artifacts](https://learn.microsoft.com/en-us/azure/devops/artifacts/)
- [Guide NuGet avec dotnet CLI](https://learn.microsoft.com/en-us/azure/devops/artifacts/nuget/dotnet-exe)
- [Gestion des Personal Access Tokens](https://learn.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate)
