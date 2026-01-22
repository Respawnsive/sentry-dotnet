# Guide de Test - Sentry.Samples.Uno

Ce guide vous permet de tester toutes les fonctionnalités Sentry implémentées dans le sample Uno Platform.

## Prérequis

1. **Application en mode Debug** : Assurez-vous que `options.Debug = true` est activé dans `App.xaml.cs`
2. **Dashboard Sentry** : Ouvrez votre projet Sentry dans un navigateur : https://sentry.io/organizations/[votre-org]/projects/[votre-projet]/
3. **Console de l'application** : Surveillez les logs de l'application (Visual Studio Output, Console, etc.)

---

## 1. Page Principale (MainPage)

### Actions à effectuer :
1. **Capture Message** : Cliquez sur "Capture message"
2. **Add Breadcrumb** : Cliquez sur "Add breadcrumb" (plusieurs fois)
3. **Capture Exception** : Cliquez sur "Capture exception"

### Vérifications :

#### Dans la Console/Logs de l'application :
- ✅ Messages de debug Sentry avec `[Sentry]` préfixe
- ✅ Messages indiquant l'envoi d'événements
- ✅ Messages de breadcrumbs ajoutés

#### Dans le Dashboard Sentry :
1. **Allez dans "Issues"** :
   - ✅ Vous devriez voir un événement "Hello from Sentry.Samples.Uno" (niveau Info)
   - ✅ Vous devriez voir un événement "Sample exception from Sentry.Samples.Uno" (niveau Error)

2. **Cliquez sur l'événement avec l'exception** :
   - ✅ Vérifiez l'onglet "Breadcrumbs" : vous devriez voir les breadcrumbs ajoutés
   - ✅ Vérifiez l'onglet "Tags" : vous devriez voir les tags configurés dans `ConfigureScope` :
     - `Application: Sentry.Samples.Uno`
     - `Framework: Uno Platform`
     - `AppVersion: 1.0.0`
     - `Platform: Uno`
   - ✅ Vérifiez l'onglet "User" : vous devriez voir les informations utilisateur configurées
   - ✅ Vérifiez l'onglet "Contexts" : vous devriez voir le contexte "device"

---

## 2. Page Fingerprinting

### Actions à effectuer :
1. **Custom Fingerprint - Group by Exception Type** : Cliquez sur le bouton
2. **Custom Fingerprint - Group by Message** : Cliquez sur le bouton
3. **Custom Fingerprint - Multiple Values** : Cliquez sur le bouton
4. **Default Fingerprint (Auto)** : Cliquez sur le bouton

### Vérifications :

#### Dans la Console/Logs :
- ✅ Messages de debug indiquant l'envoi des événements

#### Dans le Dashboard Sentry :
1. **Allez dans "Issues"** :
   - ✅ Les événements avec le même fingerprint personnalisé devraient être **groupés ensemble**
   - ✅ Les événements avec des fingerprints différents devraient être dans des **groupes séparés**

2. **Cliquez sur un événement** :
   - ✅ Vérifiez l'onglet "Tags" : vous devriez voir les tags ajoutés (ex: `Component: PaymentProcessor`, `ErrorCode: PAY001`)
   - ✅ Vérifiez la section "Fingerprint" dans les détails de l'événement

---

## 3. Page Structured Logs

### Actions à effectuer :
1. **Log Information (Breadcrumb)** : Cliquez sur le bouton
2. **Log Warning (Breadcrumb)** : Cliquez sur le bouton
3. **Log Error (Event)** : Cliquez sur le bouton
4. **Log Critical (Event)** : Cliquez sur le bouton
5. **Structured Log with Properties** : Cliquez sur le bouton
6. **Log with Exception** : Cliquez sur le bouton

### Vérifications :

#### Dans la Console/Logs :
- ✅ Les logs Information et Warning apparaissent dans la console locale
- ✅ Les logs Error et Critical déclenchent l'envoi d'événements à Sentry

#### Dans le Dashboard Sentry :
1. **Allez dans "Issues"** :
   - ✅ Vous devriez voir des événements pour les logs Error et Critical
   - ✅ Les logs Information et Warning apparaissent comme **breadcrumbs** dans les événements

2. **Cliquez sur un événement de log Error/Critical** :
   - ✅ Vérifiez l'onglet "Breadcrumbs" : vous devriez voir les logs Information/Warning précédents
   - ✅ Vérifiez les détails du log : les propriétés structurées devraient être visibles (ex: `{Action}`, `{UserId}`, `{Resource}`)

3. **Allez dans "Logs" (si disponible dans votre plan Sentry)** :
   - ✅ Vous devriez voir tous les logs structurés avec leurs attributs

---

## 4. Page Tracing

### Actions à effectuer :
1. **OpenTelemetry Transaction** : Cliquez sur le bouton
   - Attendez quelques secondes pour que la transaction se termine
2. **OpenTelemetry HTTP Request** : Cliquez sur le bouton
   - Attendez que la requête HTTP se termine
3. **Custom Sentry Transaction** : Cliquez sur le bouton
4. **Custom Sentry Span** : Cliquez sur le bouton
5. **Nested Spans** : Cliquez sur le bouton

### Vérifications :

#### Dans la Console/Logs :
- ✅ Messages de debug indiquant le démarrage et la fin des transactions/spans
- ✅ Pour les requêtes HTTP : messages indiquant l'instrumentation automatique

#### Dans le Dashboard Sentry :
1. **Allez dans "Performance"** :
   - ✅ Vous devriez voir des transactions pour chaque action effectuée
   - ✅ Les transactions OpenTelemetry devraient avoir l'origine `auto.otel`
   - ✅ Les transactions personnalisées devraient avoir l'origine `manual`

2. **Cliquez sur une transaction** :
   - ✅ Vérifiez la timeline : vous devriez voir les spans imbriqués
   - ✅ Pour "OpenTelemetry HTTP Request" : vous devriez voir un span HTTP automatique
   - ✅ Pour "Nested Spans" : vous devriez voir la hiérarchie des spans (parent → child → inner)
   - ✅ Vérifiez les tags sur les spans (ex: `span.tag: value`)

3. **Vérifiez les métriques** :
   - ✅ Durée des transactions
   - ✅ Nombre de spans par transaction

---

## 5. Page Profiling

### Actions à effectuer :
1. **Start Profiled Transaction** : Cliquez sur le bouton
   - Attendez quelques secondes (le traitement CPU prend du temps)
2. **Profiled Transaction with Exception** : Cliquez sur le bouton

### Vérifications :

#### Dans la Console/Logs :
- ✅ Messages indiquant le démarrage des transactions profilées
- ✅ Pour la transaction avec exception : message d'erreur

#### Dans le Dashboard Sentry :
1. **Allez dans "Performance"** :
   - ✅ Vous devriez voir les transactions avec l'indicateur de profiling (icône de graphique)

2. **Cliquez sur une transaction profilée** :
   - ✅ Vérifiez l'onglet "Profiling" ou "Profile" :
     - Vous devriez voir un graphique de performance
     - Les fonctions les plus coûteuses devraient être visibles
     - Pour la transaction avec exception : l'exception devrait être visible dans le profil

3. **Vérifiez les métriques de profiling** :
   - ✅ Temps CPU utilisé
   - ✅ Nombre d'appels de fonctions
   - ✅ Stack traces des fonctions les plus lentes

---

## 6. Page User Feedback

### Actions à effectuer :
1. Remplissez le formulaire :
   - **Name** : Entrez votre nom (optionnel)
   - **Email** : Entrez votre email (optionnel)
   - **Message** : Entrez un message de feedback
2. **Submit Feedback** : Cliquez sur le bouton
3. Remplissez à nouveau le formulaire et cliquez sur **Submit Feedback with Screenshot**

### Vérifications :

#### Dans la Console/Logs :
- ✅ Message de confirmation que le feedback a été envoyé

#### Dans le Dashboard Sentry :
1. **Allez dans "User Feedback"** :
   - ✅ Vous devriez voir les feedbacks soumis
   - ✅ Les détails devraient inclure : nom, email, message, timestamp

2. **Cliquez sur un feedback** :
   - ✅ Vérifiez que toutes les informations sont présentes
   - ✅ Pour le feedback avec screenshot : vérifiez l'attachement (si implémenté)

---

## 7. Vérifications Globales de Configuration

### Dans le Dashboard Sentry :

1. **Allez dans "Settings" → "Projects" → [Votre Projet] → "Client Keys (DSN)"** :
   - ✅ Vérifiez que le DSN utilisé correspond à celui dans `App.xaml.cs`

2. **Allez dans "Settings" → "Projects" → [Votre Projet] → "Release Tracking"** :
   - ✅ Vérifiez que les releases sont trackées (Release: `sentry-samples-uno@1.0.0`)

3. **Allez dans "Settings" → "Projects" → [Votre Projet] → "Debug Files"** :
   - ✅ Vérifiez que les symboles (Debug Information Files) sont uploadés
   - ✅ Les fichiers `.pdb` devraient être présents pour chaque build

4. **Vérifiez les événements récents** :
   - ✅ Tous les événements devraient avoir les tags configurés dans `ConfigureScope`
   - ✅ Tous les événements devraient avoir les informations utilisateur configurées
   - ✅ Tous les événements devraient avoir le contexte "device"

---

## 8. Tests de Fonctionnalités Avancées

### Test du BeforeSend (Filtrage d'événements) :

1. **Dans MainPage** : Ajoutez un tag "FilterMe" avant de capturer un message :
   ```csharp
   SentrySdk.CaptureMessage("Test message", scope => {
       scope.SetTag("FilterMe", "true");
   });
   ```

2. **Vérification** :
   - ✅ Dans la console : message de debug indiquant l'envoi
   - ✅ Dans Sentry : cet événement **ne devrait PAS apparaître** (filtré par BeforeSend)

### Test du BeforeBreadcrumb (Filtrage de breadcrumbs) :

1. **Dans MainPage** : Ajoutez un breadcrumb avec "password" dans le message :
   ```csharp
   SentrySdk.AddBreadcrumb("User entered password: 12345");
   ```

2. **Vérification** :
   - ✅ Dans Sentry : ce breadcrumb **ne devrait PAS apparaître** dans les événements suivants

### Test de l'Enriching Events :

1. **Capturez un événement** depuis n'importe quelle page
2. **Vérification dans Sentry** :
   - ✅ Tags personnalisés ajoutés (`AppVersion`, `Platform`)
   - ✅ Fingerprinting personnalisé pour les NullReferenceException

---

## Checklist de Validation Complète

### ✅ Configuration de Base
- [ ] DSN configuré correctement
- [ ] Debug mode activé (logs visibles dans la console)
- [ ] Sample rates configurés (100% en Debug)

### ✅ Fingerprinting
- [ ] Événements groupés selon les fingerprints personnalisés
- [ ] Fingerprints multiples fonctionnent

### ✅ Logs Structurés
- [ ] Logs Information/Warning apparaissent comme breadcrumbs
- [ ] Logs Error/Critical créent des événements
- [ ] Propriétés structurées visibles dans Sentry

### ✅ Tracing
- [ ] Transactions OpenTelemetry visibles dans Performance
- [ ] Spans HTTP automatiques fonctionnent
- [ ] Transactions personnalisées fonctionnent
- [ ] Spans imbriqués visibles

### ✅ Profiling
- [ ] Transactions profilées visibles avec indicateur
- [ ] Profils de performance disponibles
- [ ] Exceptions visibles dans les profils

### ✅ User Feedback
- [ ] Feedbacks soumis visibles dans Sentry
- [ ] Toutes les informations présentes

### ✅ Enriching Events
- [ ] Tags personnalisés présents sur tous les événements
- [ ] Informations utilisateur présentes
- [ ] Contextes personnalisés présents
- [ ] BeforeSend fonctionne (filtrage)
- [ ] BeforeBreadcrumb fonctionne (filtrage)

### ✅ Data Management
- [ ] Debug Information Files uploadés
- [ ] Releases trackées
- [ ] Cache configuré correctement

---

## Dépannage

### Si les événements n'apparaissent pas dans Sentry :
1. Vérifiez la console pour les erreurs de connexion
2. Vérifiez que le DSN est correct
3. Vérifiez que `options.Debug = true` pour voir les messages de debug
4. Vérifiez votre connexion Internet

### Si les logs structurés ne fonctionnent pas :
1. Vérifiez que `options.EnableLogs = true` dans `App.xaml.cs`
2. Vérifiez que le logger est correctement injecté dans `LogsPage`
3. Vérifiez les niveaux de log (`MinimumBreadcrumbLevel`, `MinimumEventLevel`)

### Si le tracing ne fonctionne pas :
1. Vérifiez que `options.UseOpenTelemetry()` est appelé
2. Vérifiez que `services.AddOpenTelemetry().WithTracing(...)` est configuré
3. Vérifiez que `TracesSampleRate > 0`

### Si le profiling ne fonctionne pas :
1. Vérifiez que `ProfilesSampleRate > 0`
2. Vérifiez que les transactions durent assez longtemps (minimum requis par Sentry)
3. Vérifiez que vous êtes sur un plan Sentry qui supporte le profiling

---

## Notes Importantes

- **Mode Debug** : En mode Debug, tous les événements sont envoyés (`SampleRate = 1.0`)
- **Mode Release** : En mode Release, seulement 10% des événements sont envoyés
- **Profiling** : Nécessite un plan Sentry qui supporte le profiling (Team plan ou supérieur)
- **Logs Structurés** : Nécessite un plan Sentry qui supporte les logs (Team plan ou supérieur)
- **Debug Information Files** : Sont automatiquement uploadés si `SentryUploadSymbols` est activé dans le `.csproj`

---

## Support

Si vous rencontrez des problèmes :
1. Vérifiez les logs de debug dans la console
2. Consultez la documentation Sentry : https://docs.sentry.io/platforms/dotnet/guides/maui/
3. Vérifiez les issues GitHub : https://github.com/getsentry/sentry-dotnet/issues
