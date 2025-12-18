using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SetupGameUI : EditorWindow
{
    [MenuItem("Tools/WuXing Game/Setup Game UI")]
    public static void ShowWindow()
    {
        GetWindow<SetupGameUI>("Setup Game UI");
    }

    private void OnGUI()
    {
        GUILayout.Label("Setup WuXing Game UI", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Create Main Canvas"))
        {
            CreateMainCanvas();
        }
        
        if (GUILayout.Button("Setup UI Components"))
        {
            SetupUIComponents();
        }
        
        if (GUILayout.Button("Setup All"))
        {
            CreateMainCanvas();
            SetupUIComponents();
        }
    }

    private static void CreateMainCanvas()
    {
        // Check if canvas already exists
        Canvas existingCanvas = FindObjectOfType<Canvas>();
        if (existingCanvas != null)
        {
            Debug.Log("Canvas already exists in the scene");
            return;
        }

        // Create canvas
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasGO.AddComponent<GraphicRaycaster>();
        
        // Create EventSystem if it doesn't exist
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
        }
        
        Debug.Log("Main Canvas created successfully");
    }

    private static void SetupUIComponents()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas not found in the scene. Please create a canvas first.");
            return;
        }

        // Create main game manager if it doesn't exist
        if (FindObjectOfType<GameManager>() == null)
        {
            GameObject gameManagerGO = new GameObject("GameManager");
            gameManagerGO.AddComponent<GameManager>();
            gameManagerGO.AddComponent<BattleSystem>();
            gameManagerGO.AddComponent<WebSocketClient>();
            gameManagerGO.AddComponent<UIController>();
            gameManagerGO.AddComponent<UIManager>();
        }

        Debug.Log("UI components setup successfully");
    }
}