using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CreateBattleUI : EditorWindow
{
    [MenuItem("Tools/Create Battle UI")]
    public static void ShowWindow()
    {
        GetWindow<CreateBattleUI>("Create Battle UI");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Generate Battle UI"))
        {
            GenerateUI();
        }
    }

    private static void GenerateUI()
    {
        Canvas canvas = GameObject.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        CreatePanel(canvas.transform, "BattleAreaPanel");
        CreatePanel(canvas.transform, "ActionControlsPanel");
        CreatePanel(canvas.transform, "PlayerStatusPanel");
    }

    private static void CreatePanel(Transform parent, string name)
    {
        GameObject panelGO = new GameObject(name);
        panelGO.transform.SetParent(parent);
        RectTransform rt = panelGO.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        panelGO.AddComponent<CanvasRenderer>();
        Image img = panelGO.AddComponent<Image>();
        img.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
    }
}