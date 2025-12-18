using UnityEngine;
using UnityEngine.UI;

public static class UIHelper
{
    /// <summary>
    /// Creates a UI button with text
    /// </summary>
    public static GameObject CreateButton(string name, string text, Transform parent, UnityEngine.Events.UnityAction onClick = null)
    {
        // Create button GameObject
        GameObject buttonGO = new GameObject(name);
        buttonGO.transform.SetParent(parent, false);
        
        // Add Button component
        Button button = buttonGO.AddComponent<Button>();
        button.targetGraphic = buttonGO.AddComponent<Image>();
        
        // Add Text component
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);
        
        Text buttonText = textGO.AddComponent<Text>();
        buttonText.text = text;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.fontSize = 14;
        buttonText.color = Color.black;
        
        // Set font (you may need to adjust this based on your project)
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        
        // Set rect transform
        RectTransform buttonRT = buttonGO.GetComponent<RectTransform>();
        buttonRT.sizeDelta = new Vector2(100, 30);
        
        RectTransform textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;
        
        // Add onClick listener if provided
        if (onClick != null)
        {
            button.onClick.AddListener(onClick);
        }
        
        return buttonGO;
    }
    
    /// <summary>
    /// Creates a UI text element
    /// </summary>
    public static GameObject CreateText(string name, string text, Transform parent, int fontSize = 14)
    {
        GameObject textGO = new GameObject(name);
        textGO.transform.SetParent(parent, false);
        
        Text uiText = textGO.AddComponent<Text>();
        uiText.text = text;
        uiText.fontSize = fontSize;
        uiText.alignment = TextAnchor.MiddleLeft;
        uiText.color = Color.white;
        
        // Set font (you may need to adjust this based on your project)
        uiText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        
        // Set rect transform
        RectTransform textRT = textGO.GetComponent<RectTransform>();
        textRT.sizeDelta = new Vector2(200, 30);
        
        return textGO;
    }
    
    /// <summary>
    /// Creates a UI panel
    /// </summary>
    public static GameObject CreatePanel(string name, Transform parent)
    {
        GameObject panelGO = new GameObject(name);
        panelGO.transform.SetParent(parent, false);
        
        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f); // Dark gray with transparency
        
        RectTransform panelRT = panelGO.GetComponent<RectTransform>();
        panelRT.sizeDelta = new Vector2(300, 200);
        
        return panelGO;
    }
    
    /// <summary>
    /// Updates button text
    /// </summary>
    public static void UpdateButtonText(Button button, string newText)
    {
        if (button != null)
        {
            Text textComponent = button.GetComponentInChildren<Text>();
            if (textComponent != null)
            {
                textComponent.text = newText;
            }
        }
    }
    
    /// <summary>
    /// Updates text component
    /// </summary>
    public static void UpdateText(Text textComponent, string newText)
    {
        if (textComponent != null)
        {
            textComponent.text = newText;
        }
    }
}