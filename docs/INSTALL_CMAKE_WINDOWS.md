# Guide d'installation de CMake et prérequis pour construire sentry-native

Ce guide vous explique comment installer CMake et tous les outils nécessaires pour construire les bibliothèques natives `sentry-native` sur Windows.

## 📋 Prérequis nécessaires

Pour construire `sentry-native` sur Windows, vous avez besoin de :

1. **CMake** (3.10 ou supérieur)
2. **Visual Studio Build Tools** ou **Visual Studio** (avec charge de travail C++)
3. **Git** (pour les submodules)

## 🚀 Méthode 1 : Installation via Visual Studio Installer (Recommandé)

Cette méthode installe CMake et le compilateur C++ en une seule fois.

### Étape 1 : Télécharger Visual Studio Installer

1. Allez sur [https://visualstudio.microsoft.com/downloads/](https://visualstudio.microsoft.com/downloads/)
2. Téléchargez **Visual Studio Installer** (ou **Build Tools for Visual Studio** si vous ne voulez pas l'IDE complet)

### Étape 2 : Installer les composants nécessaires

1. Lancez **Visual Studio Installer**
2. Si vous avez déjà Visual Studio installé, cliquez sur **Modifier**
3. Sinon, cliquez sur **Installer** pour une nouvelle installation

4. Sélectionnez la charge de travail **Desktop development with C++** :
   - ✅ **Desktop development with C++**
   - Dans les **Installation details**, assurez-vous que les composants suivants sont sélectionnés :
     - ✅ **MSVC v143 - VS 2022 C++ x64/x86 build tools** (ou version plus récente)
     - ✅ **Windows 10 SDK** (ou Windows 11 SDK)
     - ✅ **CMake tools for Windows** (important !)

5. Cliquez sur **Installer** ou **Modifier**

![Visual Studio Installer avec Desktop development with C++](https://learn.microsoft.com/en-us/cpp/get-started/media/vs2022-installer-workloads.png)

### Étape 3 : Vérifier l'installation

Ouvrez une **nouvelle** invite PowerShell (pour que les variables d'environnement soient mises à jour) :

```powershell
# Vérifier CMake
cmake --version

# Vérifier le compilateur MSVC
cl
```

Vous devriez voir quelque chose comme :
```
cmake version 3.28.0
CMake suite maintained and supported by Kitware (kitware.com/cmake).
```

## 🔧 Méthode 2 : Installation manuelle de CMake

Si vous préférez installer CMake séparément (sans Visual Studio) :

### Option A : Installateur Windows (Recommandé)

1. Allez sur [https://cmake.org/download/](https://cmake.org/download/)
2. Téléchargez **Windows x64 Installer** (`.msi`)
3. Exécutez l'installateur
4. **Important** : Cochez **"Add CMake to system PATH"** pendant l'installation
5. Cliquez sur **Install**

### Option B : Via winget (Windows Package Manager)

```powershell
winget install Kitware.CMake
```

### Option C : Via Chocolatey

```powershell
choco install cmake
```

### Vérifier l'installation

Ouvrez une **nouvelle** invite PowerShell :

```powershell
cmake --version
```

## 🛠️ Installer Visual Studio Build Tools (si nécessaire)

Si vous n'avez pas Visual Studio installé, vous pouvez installer uniquement les outils de build :

### Étape 1 : Télécharger Build Tools

1. Allez sur [https://visualstudio.microsoft.com/downloads/#build-tools-for-visual-studio-2022](https://visualstudio.microsoft.com/downloads/#build-tools-for-visual-studio-2022)
2. Téléchargez **Build Tools for Visual Studio 2022**

### Étape 2 : Installer les composants

1. Lancez l'installateur
2. Sélectionnez **Desktop development with C++**
3. Dans **Installation details**, assurez-vous que :
   - ✅ **MSVC v143 - VS 2022 C++ x64/x86 build tools** est sélectionné
   - ✅ **Windows 10 SDK** (ou Windows 11 SDK) est sélectionné
4. Cliquez sur **Install**

## ✅ Vérification complète de l'installation

Ouvrez une **nouvelle** invite PowerShell et exécutez :

```powershell
# Vérifier CMake
cmake --version

# Vérifier le compilateur MSVC
# Note: Vous devez d'abord initialiser l'environnement Visual Studio
& "C:\Program Files\Microsoft Visual Studio\2022\BuildTools\VC\Auxiliary\Build\vcvars64.bat"
cl

# Vérifier Git
git --version
```

## 🎯 Configuration de l'environnement pour PowerShell

Pour utiliser le compilateur MSVC depuis PowerShell, vous devez initialiser l'environnement Visual Studio. Créez un script PowerShell pour faciliter cela :

### Créer un script d'initialisation

Créez un fichier `init-vs-env.ps1` :

```powershell
# init-vs-env.ps1
# Initialise l'environnement Visual Studio pour PowerShell

$vsPath = "C:\Program Files\Microsoft Visual Studio\2022"
$buildToolsPath = "$vsPath\BuildTools"
$communityPath = "$vsPath\Community"
$professionalPath = "$vsPath\Professional"
$enterprisePath = "$vsPath\Enterprise"

# Trouver le chemin Visual Studio installé
$vcvarsPath = $null
if (Test-Path "$buildToolsPath\VC\Auxiliary\Build\vcvars64.bat") {
    $vcvarsPath = "$buildToolsPath\VC\Auxiliary\Build\vcvars64.bat"
}
elseif (Test-Path "$communityPath\VC\Auxiliary\Build\vcvars64.bat") {
    $vcvarsPath = "$communityPath\VC\Auxiliary\Build\vcvars64.bat"
}
elseif (Test-Path "$professionalPath\VC\Auxiliary\Build\vcvars64.bat") {
    $vcvarsPath = "$professionalPath\VC\Auxiliary\Build\vcvars64.bat"
}
elseif (Test-Path "$enterprisePath\VC\Auxiliary\Build\vcvars64.bat") {
    $vcvarsPath = "$enterprisePath\VC\Auxiliary\Build\vcvars64.bat"
}

if ($vcvarsPath) {
    Write-Host "Initialisation de l'environnement Visual Studio..." -ForegroundColor Green
    cmd /c "`"$vcvarsPath`" && set" | ForEach-Object {
        if ($_ -match "^([^=]+)=(.*)$") {
            [System.Environment]::SetEnvironmentVariable($matches[1], $matches[2])
        }
    }
    Write-Host "Environnement Visual Studio initialisé." -ForegroundColor Green
}
else {
    Write-Host "Visual Studio Build Tools non trouvés." -ForegroundColor Yellow
}
```

**Utilisation :**

```powershell
# Dans PowerShell, avant de construire sentry-native
. .\init-vs-env.ps1
cmake --version
cl
```

## 🧪 Tester l'installation complète

Une fois tout installé, testez la construction de sentry-native :

```powershell
# Depuis la racine du repository Sentry
cd C:\WS\GitHub\Sentry

# Initialiser l'environnement Visual Studio (si nécessaire)
# . .\init-vs-env.ps1  # Décommentez si vous utilisez le script ci-dessus

# Construire les bibliothèques natives
.\scripts\build-sentry-native.ps1

# Ou utiliser le script de packaging qui construira automatiquement
.\scripts\pack-sentry-uno.ps1
```

## 🐛 Dépannage

### Erreur : "cmake n'est pas reconnu"

**Solution :**
1. Vérifiez que CMake est dans le PATH : `$env:PATH -split ';' | Select-String cmake`
2. Redémarrez PowerShell après l'installation
3. Ajoutez manuellement CMake au PATH si nécessaire :
   ```powershell
   $env:PATH += ";C:\Program Files\CMake\bin"
   ```

### Erreur : "cl n'est pas reconnu"

**Solution :**
1. Initialisez l'environnement Visual Studio (voir section ci-dessus)
2. Ou utilisez le **Developer Command Prompt for VS** au lieu de PowerShell standard

### Erreur : "CMake Error: Could not find CMAKE_C_COMPILER"

**Solution :**
1. Assurez-vous que Visual Studio Build Tools est installé
2. Initialisez l'environnement Visual Studio avant d'exécuter CMake
3. Vérifiez que le compilateur MSVC est disponible : `where cl`

### Erreur lors de la construction : "LINK : fatal error LNK1104"

**Solution :**
1. Fermez tous les processus qui pourraient utiliser les fichiers (Visual Studio, autres builds)
2. Réessayez la construction

## 📚 Ressources supplémentaires

- [Documentation officielle CMake](https://cmake.org/documentation/)
- [Installation C++ dans Visual Studio](https://learn.microsoft.com/en-us/cpp/build/vscpp-step-0-installation)
- [CMake dans Visual Studio](https://learn.microsoft.com/en-us/cpp/build/cmake-projects-in-visual-studio)
- [Build Tools pour Visual Studio](https://visualstudio.microsoft.com/downloads/#build-tools-for-visual-studio-2022)

## ✅ Checklist d'installation

- [ ] CMake installé et dans le PATH (`cmake --version`)
- [ ] Visual Studio Build Tools ou Visual Studio installé
- [ ] Charge de travail "Desktop development with C++" installée
- [ ] Windows SDK installé
- [ ] Git installé (`git --version`)
- [ ] Test de construction réussi (`.\scripts\build-sentry-native.ps1`)

Une fois tous ces éléments installés, vous pouvez utiliser `.\scripts\pack-sentry-uno.ps1` sans l'option `-SkipNativeBuild` pour générer des packages avec support natif complet.
