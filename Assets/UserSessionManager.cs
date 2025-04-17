using UnityEngine;
using System;
using JetBrains.Annotations;

public class UserSessionManager : MonoBehaviour
{
    public static UserSessionManager Instance { get; private set; }

    public string UserId { get; private set; }
    public string[] PossibleVibrations = {"Aucun retour", "Vibration aléatoire", "Vibration rythmique", "Vibration dépendante de la vitesse"};
    public string VibrationType { get; private set; }
    public MazeCell.WallType WallType = MazeCell.WallType.BlackAndWhite;
    
    //start time, end time, and duration of the session
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public TimeSpan Duration { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persiste entre les scènes

        if (PlayerPrefs.HasKey("UserID"))
        {
            UserId = PlayerPrefs.GetString("UserID");
        }
        else
        {
            UserId = Guid.NewGuid().ToString("N").Substring(0, 8);
            PlayerPrefs.SetString("UserID", UserId);
        }

        Debug.Log("UserSessionManager initialisé - User ID: " + UserId);
        StartSession();
    }

    public void StartSession()
    {
        VibrationType = PossibleVibrations[UnityEngine.Random.Range(0, PossibleVibrations.Length)];
        StartTime = DateTime.Now;
        Duration = TimeSpan.Zero;
    }
    
    public void EndSession()
    {
        // remove the chosen vibration from it's arrays
        PossibleVibrations = Array.FindAll(PossibleVibrations, v => v != VibrationType);
        EndTime = DateTime.Now;
        Duration = EndTime - StartTime;
        Debug.Log($"Session terminée - Durée: {Duration.TotalSeconds} secondes");
    }
}