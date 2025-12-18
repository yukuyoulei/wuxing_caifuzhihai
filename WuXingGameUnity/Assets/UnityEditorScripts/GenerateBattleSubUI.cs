using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GenerateBattleSubUI : EditorWindow
{
    [MenuItem("Tools/Generate Battle SubUI")]
    public static void ShowWindow()
    {
        GetWindow<GenerateBattleSubUI>("Generate Battle SubUI");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Generate Battle SubUI"))
        {
            GenerateSubUI();
        }
    }

    private static void GenerateSubUI()
    {
        Transform battlePanel = GameObject.Find("BattleAreaPanel")?.transform;
        if (battlePanel == null)
        {
            Debug.LogError("BattleAreaPanel not found! Please generate main UI first.");
            return;
        }

        CreatePanel(battlePanel, "PlayerElementPanel");
        CreatePanel(battlePanel, "OpponentElementPanel");
        CreatePanel(battlePanel, "BattleRecordPanel");

        // 示例添加Text
        CreateText(battlePanel, "BattleTitle", "战斗区域", 24, new Vector2(0.5f, 1f));
        // 示例添加按钮
        CreateButton(battlePanel, "SampleButton", "点击操作");
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
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
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
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.alignment = TextAnchor.MiddleCenter;

        RectTransform rt = btnGO.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(160, 40);
    }
}