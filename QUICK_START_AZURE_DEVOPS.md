# 🚀 Guide rapide : Azure DevOps Artifacts pour Sentry.Uno

## Étape 1 : Créer le feed dans Azure DevOps

1. Allez sur [dev.azure.com](https://dev.azure.com)
2. Ouvrez votre projet
3. Cliquez sur **Artifacts** > **Create Feed**
4. Nommez-le (ex: `SentryUnoCustom`)
5. Choisissez **Project** scope
6. Cliquez sur **Create**

## Étape 2 : Installer le Credential Provider

```powershell
# Windows PowerShell
iex "& { $(irm https://aka.ms/install-artifacts-credprovider.ps1) } -AddNetfx"
```

## Étape 3 : Configurer nuget.config

Créez `nuget.config` à la racine de votre projet Sentry :

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="SentryUnoCustom" value="https://pkgs.dev.azure.com/<VOTRE_ORG>/<VOTRE_PROJET>/_packaging/SentryUnoCustom/nuget/v3/index.json" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
```

**Remplacez :**
- `<VOTRE_ORG>` : Votre nom d'organisation Azure DevOps
- `<VOTRE_PROJET>` : Votre nom de projet

## Étape 4 : Générer et publier les packages

```powershell
# Générer les packages
.\scripts\pack-sentry-uno.ps1

# Publier vers Azure Artifacts
.\scripts\publish-to-azure-devops.ps1 -FeedName "SentryUnoCustom"
```

## Étape 5 : Utiliser dans un autre projet

Dans votre autre projet, créez `nuget.config` :

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="SentryUnoCustom" value="https://pkgs.dev.azure.com/<VOTRE_ORG>/<VOTRE_PROJET>/_packaging/SentryUnoCustom/nuget/v3/index.json" />
  </packageSources>
</configuration>
```

Puis ajoutez le package :

```powershell
dotnet add package Sentry.Uno --version 6.0.0-local --source SentryUnoCustom
```

## 📚 Documentation complète

Pour plus de détails, consultez [`docs/AZURE_DEVOPS_NUGET.md`](docs/AZURE_DEVOPS_NUGET.md).
