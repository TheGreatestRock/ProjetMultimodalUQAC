using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class QuestionnaireManager : MonoBehaviour
{
    private Dictionary<string, string> answers = new Dictionary<string, string>();
    private string[] responseLabels = { "4", "3", "2", "1" }; //{ "Severement", "Moderement", "UnPeu", "PasDuTout" };
    private int totalQuestions = 0;
    private Button saveButton;
    public bool IsLastQuestionnaire { get; set; } = false;
    
    [SerializeField]
    private GameObject _ssqObject;


    void Start()
    {
        if (_ssqObject == null)
        {
            Debug.LogWarning("SSQ Object not found");
            return;
        }

        Transform[] questions = _ssqObject.GetComponentsInChildren<Transform>();
        Array.Sort(questions, (x, y) => String.CompareOrdinal(x.name, y.name));

        foreach (Transform question in questions)
        {
            if (question.name.StartsWith("G")) // Identify question groups
            {
                totalQuestions++;
                Button[] buttons = question.GetComponentsInChildren<Button>();

                foreach (Button button in buttons)
                {
                    if (int.TryParse(button.name.Substring(2), out int buttonIndex) && buttonIndex >= 1 && buttonIndex <= 4)
                    {
                        button.onClick.AddListener(() => OnAnswerSelected(question.name, buttonIndex - 1));
                    }
                }
            }

            if (!question.name.StartsWith("Save")) continue;
            saveButton = question.GetComponent<Button>();
            saveButton.onClick.AddListener(SubmitSurvey);
            saveButton.interactable = false;
            Debug.Log($"Save button found: {question.name}");
        }
        foreach (var question in questions)
        {
            ResetButtonsOpacity(question.name);
        }
    }
    
    void Update()
    {
        if (_ssqObject)
        {
            _ssqObject.transform.LookAt(Camera.main.transform);
            _ssqObject.transform.Rotate(0, 180, 0);
        }
    }

    public void OnAnswerSelected(string questionID, int buttonIndex)
    {
        UserSessionManager.Instance.StartQuestionnaire();
        answers[questionID] = responseLabels[buttonIndex];
        Debug.Log($"Réponse ajoutée : {questionID} - {responseLabels[buttonIndex]}");

        ResetButtonsOpacity(questionID);

        Button selectedButton = GetSelectedButton(questionID, buttonIndex);
        if (selectedButton != null)
        {
            SetButtonOpacity(selectedButton, 0.8f, true);
        }

        CheckSurveyCompletion();
    }


    private Button GetSelectedButton(string questionID, int buttonIndex)
    {
        GameObject questionObject = _ssqObject.transform.Find(questionID)?.gameObject;
        if (questionObject != null)
        {
            Button[] buttons = questionObject.GetComponentsInChildren<Button>();
            foreach (Button button in buttons)
            {
                var match = Regex.Match(button.name, @"\d+");
                if (match.Success && int.TryParse(match.Value, out int index) && index-1 == buttonIndex)
                {
                    return button;
                }
            }
        }
        return null;
    }

    private void SetButtonOpacity(Button button, float opacity, bool isSelected = false)
    {
        if (button != null)
        {
            Image img = button.GetComponent<Image>();
            if (img != null)
            {
                Color buttonColor = isSelected ? Color.green : Color.grey;
                buttonColor.a = opacity;
                img.color = buttonColor;
            }
        }
    }


    private void ResetButtonsOpacity(string questionID)
    {
        GameObject questionObject = _ssqObject.transform.Find(questionID)?.gameObject;
        if (questionObject != null)
        {
            foreach (Button button in questionObject.GetComponentsInChildren<Button>())
            {
                SetButtonOpacity(button, 0.25f);
            }
        }
    }

    private void CheckSurveyCompletion()
    {
        if (answers.Count == totalQuestions && saveButton != null)
        {
            saveButton.interactable = true;
        }
        else if (saveButton != null)
        {
            saveButton.interactable = false;
        }
    }

    public void SubmitSurvey()
    {
        if (answers.Count < totalQuestions)
        {
            Debug.LogWarning("Le questionnaire est incomplet !");
            return;
        }

        SaveAnswersToFile();
        Debug.Log("Réponses sauvegardées !");
        UserSessionManager.Instance.EndQuestionnaire();
        if (IsLastQuestionnaire)
        {
            UserSessionManager.Instance.EndSession();
        }
    }

    private void SaveAnswersToFile()
    {
        
        SurveyData data = new SurveyData
        {
            userId = UserSessionManager.Instance.UserId,
            vibrationType = UserSessionManager.Instance.VibrationType, 
            wallType = UserSessionManager.Instance.WallType.ToString(),
            answers = new SerializableDictionary(answers),
            Duration = UserSessionManager.Instance.Duration
        };
    
        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, "survey_answers.json");
    
        // Si le fichier existe déjà, on ajoute les nouvelles réponses
        if (File.Exists(path))
        {
            File.AppendAllText(path, json + "\n"); // Ajoute les données au fichier, avec une nouvelle ligne pour séparer les entrées
            Debug.Log($"Réponses ajoutées au fichier : {path}");
        }
        else
        {
            // Si le fichier n'existe pas, on le crée et écrit les données
            File.WriteAllText(path, json);
            Debug.Log($"Réponses sauvegardées dans le fichier : {path}");
        }

        if (_ssqObject != null)
        {
            _ssqObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("SSQ Object not found when trying to deactivate it");
        }
    }


}

[System.Serializable]
public class SerializableDictionary
{
    public List<string> keys = new List<string>();
    public List<string> values = new List<string>();

    public SerializableDictionary(Dictionary<string, string> dictionary)
    {
        foreach (var pair in dictionary)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public Dictionary<string, string> ToDictionary()
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        for (int i = 0; i < keys.Count; i++)
        {
            dictionary[keys[i]] = values[i];
        }
        return dictionary;
    }
}

[System.Serializable]
public class SurveyData
{
    public string userId;
    public string vibrationType;
    public string wallType;
    public SerializableDictionary answers;
    public TimeSpan Duration;
}
