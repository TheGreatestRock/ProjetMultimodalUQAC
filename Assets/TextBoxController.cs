using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TextBoxController : MonoBehaviour
{
    [Header("Text Fields")]
    public TMP_Text topText;
    public TMP_Text button1Text;
    public TMP_Text button2Text;
    public TMP_Text button3Text;

    [Header("Button References")]
    public Button button1;
    public Button button2;
    public Button button3;
    
    //texts to display
    [Header("Text Content")]
    public string topTextContent = "temp";
    public string button1TextContent = "temp";
    public string button2TextContent = "temp";
    public string button3TextContent = "temp";
    
    [SerializeField]
    private GameObject _canvas;

    public enum ButtonActionType
    {
        None,
        ChangeMaterial,
        ChangeScene,
        Quit,
        Pause
    }

    [System.Serializable]
    public class ButtonAction
    {
        public ButtonActionType actionType;
        public string parameter; // e.g., scene name, material name, etc.
    }

    [Header("Actions")]
    public ButtonAction action1;
    public ButtonAction action2;
    public ButtonAction action3;

    void Start()
    {
        AssignButton(button1, action1);
        AssignButton(button2, action2);
        AssignButton(button3, action3);
        if(topText.text == "temp") topText.text = topTextContent;
        if(button1Text.text == "temp") button1Text.text = button1TextContent;
        if(button2Text.text == "temp") button2Text.text = button2TextContent;
        if(button3Text.text == "temp") button3Text.text = button3TextContent;
    }
    
    void Update()
    {
        if (_canvas)
        {
            _canvas.transform.LookAt(Camera.main.transform);
            _canvas.transform.Rotate(0, 180, 0);
        }
    }

    void AssignButton(Button button, ButtonAction action)
    {
        button.onClick.RemoveAllListeners();
        switch (action.actionType)
        {
            case ButtonActionType.ChangeMaterial:
                button.onClick.AddListener(() => ChangeMaterial(action.parameter));
                break;
            case ButtonActionType.ChangeScene:
                button.onClick.AddListener(() => SceneManager.LoadScene(action.parameter));
                break;
            case ButtonActionType.Quit:
                button.onClick.AddListener(Application.Quit);
                break;
            case ButtonActionType.Pause:
                button.onClick.AddListener(PauseGame);
                break;
        }
    }

    void ChangeMaterial(string newMatName)
    {
        Material newMat = Resources.Load<Material>(newMatName);
        if (newMat == null) return;

        foreach (var obj in Object.FindObjectsByType(System.Type.GetType("MazeCell"), FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            var cell = (MazeCell)obj;
            if (cell != null)
            {
                if (cell._currentWallType == MazeCell.WallType.Start || cell._currentWallType == MazeCell.WallType.End)
                {
                    continue; // Skip start and end cells
                }
                cell.SetCellType(MazeCell.CellType.Default); // Reset to default before applying new material
                cell.SetDefaultMaterial(newMat);    
            }
        }
    }

    void PauseGame()
    {
        // Replace with your actual pause function
        Debug.Log("Pause called");
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }
}
