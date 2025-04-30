# Documentation détaillée du projet

## Fichier : `Assets/VRRayCastUI.cs`

### Description générale
Ce script gère les interactions entre les rayons des contrôleurs VR et les éléments d'interface utilisateur (UI). Il utilise des `XRRayInteractor` pour détecter les collisions avec des éléments interactifs, comme des boutons, et déclenche les actions associées.

### Fonctions

#### `Update()`
- **Description** : Appelée à chaque frame, cette fonction gère les interactions des rayons des deux contrôleurs (gauche et droit) avec les éléments interactifs.
- **Détails** :
  - Appelle la fonction `HandleRaycastInteraction` pour chaque contrôleur (`leftRayInteractor` et `rightRayInteractor`).
  - Permet de détecter les interactions en temps réel.

#### `HandleRaycastInteraction(XRRayInteractor rayInteractor)`
- **Description** : Gère l'interaction d'un contrôleur VR avec les éléments interactifs détectés par le rayon.
- **Paramètres** :
  - `rayInteractor` : L'interacteur de rayon (gauche ou droit) utilisé pour détecter les collisions.
- **Détails** :
  - Vérifie si le rayon interagit avec un objet en utilisant `TryGetCurrent3DRaycastHit`.
  - Si une collision est détectée :
    - Visualise le rayon avec `Debug.DrawLine` (optionnel, pour le débogage).
    - Vérifie si l'objet touché a le tag `UIElement`.
    - Si l'objet est un bouton (`Button`), déclenche son événement `onClick`.

---

## Fichier : `Assets/VRHaptics.cs`

### Description générale
Ce script gère les retours haptiques (vibrations) des contrôleurs VR en fonction de différents modes (aléatoire, rythmique, dépendant de la vitesse, etc.).

### Fonctions

#### `SetHapticFeedback()`
- **Description** : Configure le mode de vibration en fonction du type de vibration défini dans `UserSessionManager`.
- **Détails** :
  - Récupère le type de vibration depuis `UserSessionManager.Instance.VibrationType`.
  - Appelle `SetVibrationMode` pour activer le mode correspondant.

#### `SetVibrationMode(int mode)`
- **Description** : Définit le mode de vibration et démarre la routine correspondante.
- **Paramètres** :
  - `mode` : Le mode de vibration (1 = aucun retour, 2 = aléatoire, 3 = rythmique, 4 = dépendant de la vitesse).
- **Détails** :
  - Arrête toute routine de vibration en cours.
  - Démarre la routine appropriée (`RandomVibration`, `FixedRhythmicVibration`, ou `SpeedBasedVibration`).

#### `SendHapticFeedback(HapticImpulsePlayer haptic, float amplitude, float duration)`
- **Description** : Envoie un retour haptique à un contrôleur.
- **Paramètres** :
  - `haptic` : Le contrôleur haptique (gauche ou droit).
  - `amplitude` : L'intensité de la vibration.
  - `duration` : La durée de la vibration.

#### `RandomVibration()`
- **Description** : Routine de vibration aléatoire.
- **Détails** :
  - Génère des vibrations de durées aléatoires entre `randomMinDuration` et `randomMaxDuration`.
  - Pause entre les vibrations définie par `randomPause`.

#### `FixedRhythmicVibration()`
- **Description** : Routine de vibration rythmique fixe.
- **Détails** :
  - Envoie des vibrations avec une intensité fixe toutes les secondes.

#### `SpeedBasedVibration()`
- **Description** : Routine de vibration basée sur la vitesse de déplacement.
- **Détails** :
  - Calcule la vitesse de déplacement du joueur.
  - Ajuste l'intensité des vibrations en fonction de la vitesse.

#### `OnEnable()` et `OnDisable()`
- **Description** : Abonnent et désabonnent la fonction `OnSceneLoaded` à l'événement de chargement de scène.

#### `OnSceneLoaded(Scene scene, LoadSceneMode mode)`
- **Description** : Réinitialise le retour haptique lorsque la scène est chargée.

---

## Fichier : `Assets/UserSessionManager.cs`

### Description générale
Ce script gère les sessions utilisateur, y compris les types de vibrations, les temps de session, et les interactions avec les questionnaires.

### Fonctions

#### `Awake()`
- **Description** : Initialise le singleton `UserSessionManager` et génère un identifiant utilisateur unique si nécessaire.

#### `StartSession()`
- **Description** : Démarre une nouvelle session utilisateur.
- **Détails** :
  - Définit un type de vibration aléatoire.
  - Configure les retours haptiques pour tous les objets `VRHaptics` de la scène.

#### `StartQuestionnaire()`
- **Description** : Démarre un questionnaire.
- **Détails** :
  - Enregistre l'heure de début du questionnaire.
  - Met le jeu en pause (`Time.timeScale = 0`).

#### `EndQuestionnaire()`
- **Description** : Termine le questionnaire.
- **Détails** :
  - Calcule la durée du questionnaire.
  - Reprend le jeu (`Time.timeScale = 1`).

#### `EndSession()`
- **Description** : Termine la session utilisateur.
- **Détails** :
  - Calcule la durée totale de la session (sans les questionnaires).
  - Quitte l'application si tous les types de vibrations ont été utilisés.

---

## Fichier : `Assets/MazeGenerator.cs`

### Description générale
Ce script génère un labyrinthe 3D aléatoire et place des objets interactifs (questionnaires, panneaux d'information) à des positions spécifiques.

### Fonctions

#### `Start()`
- **Description** : Initialise la grille du labyrinthe et génère le labyrinthe.
- **Détails** :
  - Crée une grille de cellules (`MazeCell`).
  - Appelle `GenerateMaze` pour générer le labyrinthe.
  - Définit les cellules de départ et de fin avec `SetStartAndEnd`.

#### `SetStartAndEnd()`
- **Description** : Configure les cellules de départ et de fin.
- **Détails** :
  - Place des objets interactifs (questionnaires, panneaux) au-dessus des cellules de départ et de fin.
  - Ajoute un questionnaire au milieu du chemin.

#### `GenerateMaze(MazeCell previousCell, MazeCell currentCell)`
- **Description** : Génère le labyrinthe en supprimant les murs entre les cellules connectées.
- **Détails** :
  - Marque la cellule actuelle comme visitée.
  - Appelle récursivement la fonction pour les cellules voisines non visitées.

#### `FindPath(MazeCell start, MazeCell end)`
- **Description** : Trouve le chemin entre la cellule de départ et la cellule de fin.
- **Détails** :
  - Utilise une recherche en largeur (BFS) pour trouver le chemin.

---

## Fichier : `Assets/MazeCell.cs`

### Description générale
Ce script représente une cellule du labyrinthe, avec des murs et des types de matériaux.

### Fonctions

#### `Visit()`
- **Description** : Marque la cellule comme visitée et désactive le bloc "non visité".

#### `RemoveWall(Direction direction)`
- **Description** : Supprime un mur spécifique de la cellule.

#### `SetCellType(CellType cellType)`
- **Description** : Définit le type de la cellule (départ, fin, ou par défaut) et applique le matériau correspondant.

#### `HasWall(Direction direction)`
- **Description** : Vérifie si un mur spécifique est actif.

---

## Fichier : `Assets/QuestionnaireManager.cs`

### Description générale
Ce script gère l'affichage et la collecte des réponses des questionnaires SSQ.

### Fonctions

#### `Start()`
- **Description** : Initialise les questions et les boutons du questionnaire.
- **Détails** :
  - Associe les boutons aux questions.
  - Configure le bouton de sauvegarde pour soumettre les réponses.

#### `OnAnswerSelected(string questionID, int buttonIndex)`
- **Description** : Enregistre la réponse sélectionnée pour une question.
- **Détails** :
  - Met à jour le dictionnaire des réponses.
  - Vérifie si toutes les questions ont été répondues.

#### `SubmitSurvey()`
- **Description** : Soumet les réponses du questionnaire.
- **Détails** :
  - Sauvegarde les réponses dans un fichier JSON.
  - Désactive l'objet du questionnaire.

---

## Fichier : `Assets/TextBoxController.cs`

### Description générale
Ce script gère les panneaux d'information affichés dans le labyrinthe.

### Fonctions

#### `Start()`
- **Description** : Configure les boutons et le texte du panneau.

#### `ChangeMaterial(string newMatName)`
- **Description** : Change le matériau des murs du labyrinthe.

#### `PauseGame()`
- **Description** : Met le jeu en pause ou le reprend.
