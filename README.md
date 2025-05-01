# Projet de Recherche : Labyrinthe VR et Étude sur le Cybermalaise

## Description du Projet

Ce projet a pour objectif de développer une expérience immersive en réalité virtuelle (VR) où l'utilisateur navigue dans un labyrinthe 3D généré aléatoirement. L'étude vise à évaluer l'impact de différentes rétroactions vibrotactiles sur les symptômes de cybermalaise au fil du temps, en utilisant le **Simulator Sickness Questionnaire (SSQ)**.

## Fonctionnalités Principales

- **Génération de Labyrinthe 3D** : Un labyrinthe est généré aléatoirement à chaque session, avec des cellules et des murs configurables.
- **Rétroactions Vibrotactiles** : Plusieurs modes de vibrations sont proposés pour évaluer leur impact sur le cybermalaise :
  - Aucun retour
  - Vibration aléatoire
  - Vibration rythmique
  - Vibration dépendante de la vitesse
- **Questionnaires SSQ** : Des questionnaires sont placés au début, au milieu et à la fin du labyrinthe pour collecter les réponses des utilisateurs.
- **Personnalisation des Murs** : Les utilisateurs peuvent choisir entre différents types de murs (Noir et Blanc, RGB).
- **Gestion des Sessions** : Suivi des sessions utilisateur, y compris la durée et les réponses aux questionnaires.

## Structure du Projet

### Fichiers et Scripts Principaux

- **`MazeGenerator.cs`** : Génère le labyrinthe 3D aléatoire et place les objets interactifs (questionnaires, panneaux d'information).
- **`MazeCell.cs`** : Gère les cellules du labyrinthe, y compris les murs et leur type.
- **`QuestionnaireManager.cs`** : Gère l'affichage et la collecte des réponses des questionnaires SSQ.
- **`UserSessionManager.cs`** : Gère les sessions utilisateur, y compris les vibrations, le suivi du temps et les interactions.
- **`VRHaptics.cs`** : Implémente les différents modes de rétroactions vibrotactiles.
- **`TextBoxController.cs`** : Gère les panneaux d'information affichés dans le labyrinthe.
- **`FreeCamera.cs`** : Permet à l'utilisateur de naviguer librement dans l'environnement VR.
- **`VRRayCastUI.cs`** : Gère les interactions avec les éléments d'interface utilisateur en VR.

### Ressources

- **Matériaux** : Différents matériaux pour les murs du labyrinthe (Noir et Blanc, RGB).
- **Préfabs** :
  - **Labyrinthe** : Cellules et murs.
  - **Questionnaires** : Objets interactifs pour collecter les réponses.
  - **Panneaux d'information** : Fournissent des instructions ou des messages à l'utilisateur.

## Instructions d'Utilisation

1. **Configuration Initiale** :
   - Assurez-vous que les dépendances Unity nécessaires (XR Toolkit, TMP, etc.) sont installées.
   - Configurez les contrôleurs VR et les paramètres de la scène.

2. **Lancement de l'Expérience** :
   - Lancez la scène principale contenant le labyrinthe.
   - L'utilisateur peut naviguer dans le labyrinthe à l'aide des contrôleurs VR.

3. **Interaction avec les Questionnaires** :
   - Les questionnaires SSQ apparaissent à des points spécifiques (début, milieu, fin).
   - Les réponses sont sauvegardées dans un fichier JSON pour analyse ultérieure.

4. **Personnalisation** :
   - Les utilisateurs peuvent choisir le type de mur avant de commencer.
   - Les vibrations sont automatiquement configurées en fonction du mode sélectionné.

## Objectifs de l'Étude

- **Évaluer le Cybermalaise** : Mesurer les symptômes de cybermalaise à l'aide des réponses au SSQ.
- **Analyser l'Impact des Vibrations** : Comparer les différents modes de rétroactions vibrotactiles pour identifier leur influence sur le confort de l'utilisateur.
- **Améliorer l'Immersion** : Offrir une expérience VR engageante et personnalisable.

## Technologies Utilisées

- **Unity** : Moteur de jeu pour le développement de l'expérience VR.
- **C#** : Langage de programmation pour les scripts.
- **XR Toolkit** : Gestion des interactions VR.
- **TMP (TextMeshPro)** : Affichage des textes dans l'environnement VR.

## Auteur

Ce projet a été développé par Victor Vieux-Melchior sous la supervision du professeur Pascal E. Fortin dans le cadre d'une étude de recherche sur le cybermalaise en réalité virtuelle pour [l'UQAC](https://www.uqac.ca/)

## Remarques

- Les données utilisateur sont sauvegardées localement dans un fichier JSON.
- Le projet est conçu pour être extensible, permettant l'ajout de nouveaux modes de vibrations ou de types de murs.
