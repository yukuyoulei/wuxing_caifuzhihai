using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class GenerateFullUI : EditorWindow
{
    [MenuItem("Tools/Generate Full Battle UI")]
    public static void ShowWindow()
    {
        GetWindow<GenerateFullUI>("Generate Full UI");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Generate Full UI"))
        {
            GenerateFullUIElements();
        }
    }

    private static void GenerateFullUIElements()
    {
        Transform canvas = GameObject.FindObjectOfType<Canvas>()?.transform;
        if (canvas == null)
        {
            Debug.LogError("Canvas not found! Please generate main Canvas first.");
            return;
        }

        // Panels
        Transform battlePanel = canvas.Find("BattleAreaPanel");
        Transform actionPanel = canvas.Find("ActionControlsPanel");
        Transform statusPanel = canvas.Find("PlayerStatusPanel");

        if (battlePanel != null)
        {
            CreatePanel(battlePanel, "PlayerElementPanel");
            CreatePanel(battlePanel, "OpponentElementPanel");
            CreatePanel(battlePanel, "BattleRecordPanel");
        }

        if (actionPanel != null)
        {
            CreateButton(actionPanel, "StartAdventureButton", "开始探险");
            CreateButton(actionPanel, "SkillButton", "技能系统");
            CreateButton(actionPanel, "ReplenishButton", "NPC补充元素");
            CreateButton(actionPanel, "ReturnHomeButton", "返回出生点");
            CreateButton(actionPanel, "ResetButton", "重置游戏");
        }

        if (statusPanel != null)
        {
            CreateText(statusPanel, "PositionText", "(0,0)", 18, new Vector2(0.5f, 0.9f));
            CreatePanel(statusPanel, "ElementInventory");
            CreatePanel(statusPanel, "CurrencyPanel");
        }
    }

    private static void CreatePanel(Transform parent, string name)
    {
        GameObject panelGO = new GameObject(name);
        panelGO.transform.SetParent(parent);
        RectTransform rt = panelGO.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(400, 200);
        panelGO.AddComponent<CanvasRenderer>();
        Image img = panelGO.AddComponent<Image>();
        img.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
    }

    private static void CreateText(Transform parent, string name, string content, int fontSize, Vector2 anchor)
    {
        GameObject textGO = new GameObject(name);
        textGO.transform.SetParent(parent);
        Text txt = textGO.AddComponent<Text>();
        txt.text = content;
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.fontSize = fontSize;
        txt.alignment = TextAnchor.MiddleCenter;
        RectTransform rt = textGO.GetComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(200, 50);
    }

    private static void CreateButton(Transform parent, string name, string label)
    {
        GameObject btnGO = new GameObject(name);
        btnGO.transform.SetParent(parent);
        Button btn = btnGO.AddComponent<Button>();
        Image img = btnGO.AddComponent<Image>();
        img.color = Color.white;

        GameObject txtGO = new GameObject("Text");
        txtGO.transform.SetParent(btnGO.transform);
        Text txt = txtGO.AddComponent<Text>();
        txt.text = label;
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.alignment = TextAnchor.MiddleCenter;

        RectTransform rt = btnGO.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(160, 40);
    }
}