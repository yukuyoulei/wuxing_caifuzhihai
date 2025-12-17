using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public Canvas canvas;
    public GameManager gameManager;
    public BattleAreaController battleAreaController;
    
    // UI Panels
    private GameObject battleAreaPanel;
    private GameObject actionControlsPanel;
    private GameObject playerStatusPanel;
    
    // Sub panels
    private GameObject playerElementPanel;
    private GameObject opponentElementPanel;
    private GameObject battleRecordPanel;
    private GameObject elementInventoryPanel;
    private GameObject currencyPanel;
    
    // UI Elements
    private Text positionText;
    private Text[] elementTexts = new Text[5];
    private Text yinCurrencyText;
    private Text yangCurrencyText;
    
    // Buttons
    private Button startAdventureButton;
    private Button replenishButton;
    private Button returnHomeButton;
    private Button resetButton;
    private Button skillButton;
    
    // Element buttons
    private Button[] playerElementButtons = new Button[5];
    private Button[] opponentElementButtons = new Button[3];
    
    void Start()
    {
        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
        }
        
        CreateUI();
        UpdateUI();
    }
    
    void CreateUI()
    {
        // Create main panels
        battleAreaPanel = UIHelper.CreatePanel("BattleAreaPanel", canvas.transform);
        actionControlsPanel = UIHelper.CreatePanel("ActionControlsPanel", canvas.transform);
        playerStatusPanel = UIHelper.CreatePanel("PlayerStatusPanel", canvas.transform);
        
        // Position panels (simple vertical layout)
        RectTransform battleRT = battleAreaPanel.GetComponent<RectTransform>();
        RectTransform actionRT = actionControlsPanel.GetComponent<RectTransform>();
        RectTransform statusRT = playerStatusPanel.GetComponent<RectTransform>();
        
        battleRT.anchorMin = new Vector2(0, 0.5f);
        battleRT.anchorMax = new Vector2(0.5f, 1);
        battleRT.offsetMin = Vector2.zero;
        battleRT.offsetMax = Vector2.zero;
        
        actionRT.anchorMin = new Vector2(0.5f, 0.5f);
        actionRT.anchorMax = new Vector2(1, 1);
        actionRT.offsetMin = Vector2.zero;
        actionRT.offsetMax = Vector2.zero;
        
        statusRT.anchorMin = new Vector2(0, 0);
        statusRT.anchorMax = new Vector2(1, 0.5f);
        statusRT.offsetMin = Vector2.zero;
        statusRT.offsetMax = Vector2.zero;
        
        CreateBattleAreaUI();
        CreateActionControlsUI();
        CreatePlayerStatusUI();
    }
    
    void CreateBattleAreaUI()
    {
        // Create sub panels
        playerElementPanel = UIHelper.CreatePanel("PlayerElementPanel", battleAreaPanel.transform);
        opponentElementPanel = UIHelper.CreatePanel("OpponentElementPanel", battleAreaPanel.transform);
        battleRecordPanel = UIHelper.CreatePanel("BattleRecordPanel", battleAreaPanel.transform);
        
        // Position sub panels
        RectTransform playerRT = playerElementPanel.GetComponent<RectTransform>();
        RectTransform opponentRT = opponentElementPanel.GetComponent<RectTransform>();
        RectTransform recordRT = battleRecordPanel.GetComponent<RectTransform>();
        
        playerRT.anchorMin = new Vector2(0, 0.7f);
        playerRT.anchorMax = new Vector2(1, 1);
        playerRT.offsetMin = Vector2.zero;
        playerRT.offsetMax = Vector2.zero;
        
        opponentRT.anchorMin = new Vector2(0, 0.4f);
        opponentRT.anchorMax = new Vector2(1, 0.7f);
        opponentRT.offsetMin = Vector2.zero;
        opponentRT.offsetMax = Vector2.zero;
        
        recordRT.anchorMin = new Vector2(0, 0);
        recordRT.anchorMax = new Vector2(1, 0.4f);
        recordRT.offsetMin = Vector2.zero;
        recordRT.offsetMax = Vector2.zero;
        
        // Create player element buttons
        List<ElementType> elements = GameUtils.GetAllElements();
        for (int i = 0; i < elements.Count && i < 5; i++)
        {
            GameObject buttonGO = UIHelper.CreateButton(
                "PlayerElement_" + elements[i],
                GameUtils.GetElementIcon(elements[i]) + "\n" + GameUtils.GetElementName(elements[i]),
                playerElementPanel.transform,
                () => OnPlayerElementClicked(i)
            );
            
            playerElementButtons[i] = buttonGO.GetComponent<Button>();
            
            // Position buttons horizontally
            RectTransform buttonRT = buttonGO.GetComponent<RectTransform>();
            buttonRT.anchorMin = new Vector2(0.1f + i * 0.2f, 0.2f);
            buttonRT.anchorMax = new Vector2(0.1f + (i + 1) * 0.2f, 0.8f);
            buttonRT.offsetMin = Vector2.zero;
            buttonRT.offsetMax = Vector2.zero;
        }
        
        // Create opponent element buttons
        for (int i = 0; i < 3; i++)
        {
            GameObject buttonGO = UIHelper.CreateButton(
                "OpponentElement_" + i,
                "?",
                opponentElementPanel.transform,
                () => OnOpponentElementClicked(i)
            );
            
            opponentElementButtons[i] = buttonGO.GetComponent<Button>();
            
            // Position buttons horizontally
            RectTransform buttonRT = buttonGO.GetComponent<RectTransform>();
            buttonRT.anchorMin = new Vector2(0.2f + i * 0.3f, 0.2f);
            buttonRT.anchorMax = new Vector2(0.2f + (i + 1) * 0.3f, 0.8f);
            buttonRT.offsetMin = Vector2.zero;
            buttonRT.offsetMax = Vector2.zero;
        }
    }
    
    void CreateActionControlsUI()
    {
        // Create action buttons
        startAdventureButton = UIHelper.CreateButton(
            "StartAdventureButton",
            "开始探险",
            actionControlsPanel.transform,
            OnStartAdventureClicked
        ).GetComponent<Button>();
        
        replenishButton = UIHelper.CreateButton(
            "ReplenishButton",
            "NPC补充元素",
            actionControlsPanel.transform,
            OnReplenishClicked
        ).GetComponent<Button>();
        
        returnHomeButton = UIHelper.CreateButton(
            "ReturnHomeButton",
            "返回出生点",
            actionControlsPanel.transform,
            OnReturnHomeClicked
        ).GetComponent<Button>();
        
        resetButton = UIHelper.CreateButton(
            "ResetButton",
            "重置游戏",
            actionControlsPanel.transform,
            OnResetClicked
        ).GetComponent<Button>();
        
        skillButton = UIHelper.CreateButton(
            "SkillButton",
            "技能系统",
            actionControlsPanel.transform,
            OnSkillClicked
        ).GetComponent<Button>();
        
        // Position buttons vertically
        Button[] buttons = { startAdventureButton, replenishButton, returnHomeButton, resetButton, skillButton };
        for (int i = 0; i < buttons.Length; i++)
        {
            RectTransform buttonRT = buttons[i].GetComponent<RectTransform>();
            buttonRT.anchorMin = new Vector2(0.1f, 0.8f - i * 0.15f);
            buttonRT.anchorMax = new Vector2(0.9f, 0.9f - i * 0.15f);
            buttonRT.offsetMin = Vector2.zero;
            buttonRT.offsetMax = Vector2.zero;
        }
    }
    
    void CreatePlayerStatusUI()
    {
        // Create position text
        GameObject positionGO = UIHelper.CreateText("PositionText", "位置: (0, 0)", playerStatusPanel.transform, 18);
        positionText = positionGO.GetComponent<Text>();
        
        RectTransform positionRT = positionGO.GetComponent<RectTransform>();
        positionRT.anchorMin = new Vector2(0.1f, 0.8f);
        positionRT.anchorMax = new Vector2(0.9f, 0.9f);
        positionRT.offsetMin = Vector2.zero;
        positionRT.offsetMax = Vector2.zero;
        
        // Create element inventory panel
        elementInventoryPanel = UIHelper.CreatePanel("ElementInventory", playerStatusPanel.transform);
        RectTransform inventoryRT = elementInventoryPanel.GetComponent<RectTransform>();
        inventoryRT.anchorMin = new Vector2(0.1f, 0.4f);
        inventoryRT.anchorMax = new Vector2(0.9f, 0.8f);
        inventoryRT.offsetMin = Vector2.zero;
        inventoryRT.offsetMax = Vector2.zero;
        
        // Create element texts
        List<ElementType> elements = GameUtils.GetAllElements();
        for (int i = 0; i < elements.Count && i < 5; i++)
        {
            GameObject elementGO = UIHelper.CreateText(
                "Element_" + elements[i], 
                GameUtils.GetElementIcon(elements[i]) + " " + GameUtils.GetElementName(elements[i]) + ": 0",
                elementInventoryPanel.transform,
                14
            );
            
            elementTexts[i] = elementGO.GetComponent<Text>();
            
            RectTransform elementRT = elementGO.GetComponent<RectTransform>();
            elementRT.anchorMin = new Vector2(0.1f, 0.8f - i * 0.2f);
            elementRT.anchorMax = new Vector2(0.9f, 1.0f - i * 0.2f);
            elementRT.offsetMin = Vector2.zero;
            elementRT.offsetMax = Vector2.zero;
        }
        
        // Create currency panel
        currencyPanel = UIHelper.CreatePanel("CurrencyPanel", playerStatusPanel.transform);
        RectTransform currencyRT = currencyPanel.GetComponent<RectTransform>();
        currencyRT.anchorMin = new Vector2(0.1f, 0.1f);
        currencyRT.anchorMax = new Vector2(0.9f, 0.4f);
        currencyRT.offsetMin = Vector2.zero;
        currencyRT.offsetMax = Vector2.zero;
        
        // Create currency texts
        GameObject yinGO = UIHelper.CreateText("YinCurrency", "阴: 0", currencyPanel.transform, 16);
        yinCurrencyText = yinGO.GetComponent<Text>();
        
        RectTransform yinRT = yinGO.GetComponent<RectTransform>();
        yinRT.anchorMin = new Vector2(0.1f, 0.6f);
        yinRT.anchorMax = new Vector2(0.9f, 0.9f);
        yinRT.offsetMin = Vector2.zero;
        yinRT.offsetMax = Vector2.zero;
        
        GameObject yangGO = UIHelper.CreateText("YangCurrency", "阳: 0", currencyPanel.transform, 16);
        yangCurrencyText = yangGO.GetComponent<Text>();
        
        RectTransform yangRT = yangGO.GetComponent<RectTransform>();
        yangRT.anchorMin = new Vector2(0.1f, 0.1f);
        yangRT.anchorMax = new Vector2(0.9f, 0.4f);
        yangRT.offsetMin = Vector2.zero;
        yangRT.offsetMax = Vector2.zero;
    }
    
    public void UpdateUI()
    {
        if (gameManager == null) return;
        
        GameState gameState = GetGameState();
        if (gameState == null) return;
        
        // Update position
        if (positionText != null)
        {
            positionText.text = $"位置: ({gameState.position.x}, {gameState.position.y})";
        }
        
        // Update elements
        List<ElementType> elements = GameUtils.GetAllElements();
        for (int i = 0; i < elements.Count && i < 5; i++)
        {
            if (elementTexts[i] != null)
            {
                int count = gameState.elements.GetElementCount(elements[i]);
                elementTexts[i].text = $"{GameUtils.GetElementIcon(elements[i])} {GameUtils.GetElementName(elements[i])}: {count}";
            }
            
            // Update player element buttons
            if (playerElementButtons[i] != null)
            {
                int count = gameState.elements.GetElementCount(elements[i]);
                UIHelper.UpdateButtonText(playerElementButtons[i], $"{GameUtils.GetElementIcon(elements[i])}\n{GameUtils.GetElementName(elements[i])}\n{count}");
            }
        }
        
        // Update currencies
        if (yinCurrencyText != null)
        {
            yinCurrencyText.text = $"阴: {gameState.currency.yin}";
        }
        
        if (yangCurrencyText != null)
        {
            yangCurrencyText.text = $"阳: {gameState.currency.yang}";
        }
    }
    
    GameState GetGameState()
    {
        // This is a simplified approach - in a real implementation, 
        // you would get the actual game state from the GameManager
        return new GameState();
    }
    
    // Button click handlers
    void OnStartAdventureClicked()
    {
        Debug.Log("Start Adventure clicked");
        if (gameManager != null)
        {
            gameManager.StartAdventure();
            UpdateUI();
        }
    }
    
    void OnReplenishClicked()
    {
        Debug.Log("Replenish Elements clicked");
        if (gameManager != null)
        {
            gameManager.ReplenishElements();
            UpdateUI();
        }
    }
    
    void OnReturnHomeClicked()
    {
        Debug.Log("Return Home clicked");
        if (gameManager != null)
        {
            gameManager.ReturnToSpawn();
            UpdateUI();
        }
    }
    
    void OnResetClicked()
    {
        Debug.Log("Reset Game clicked");
        if (gameManager != null)
        {
            gameManager.ResetGame();
            UpdateUI();
        }
    }
    
    void OnSkillClicked()
    {
        Debug.Log("Skill System clicked");
        // TODO: Implement skill system UI
    }
    
    void OnPlayerElementClicked(int index)
    {
        Debug.Log("Player Element " + index + " clicked");
        if (battleAreaController != null)
        {
            battleAreaController.OnPlayerElementSelected(index);
        }
    }
    
    void OnOpponentElementClicked(int index)
    {
        Debug.Log("Opponent Element " + index + " clicked");
        if (battleAreaController != null)
        {
            battleAreaController.OnOpponentElementRevealed(index);
        }
    }
}