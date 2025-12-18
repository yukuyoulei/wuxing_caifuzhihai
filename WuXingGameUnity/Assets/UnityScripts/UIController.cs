using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIController : MonoBehaviour
{
    public GameManager gameManager;
    public BattleSystem battleSystem;
    public WebSocketClient webSocketClient;
    public UIManager uiManager;
    
    // Action buttons
    public Button startAdventureButton;
    public Button replenishButton;
    public Button returnHomeButton;
    public Button resetButton;
    public Button skillButton;
    
    // Player element buttons (5 elements)
    public Button[] playerElementButtons;
    
    // Opponent element buttons (3 opponents)
    public Button[] opponentElementButtons;
    
    // Currency buttons
    public Button useYinCurrencyButton;
    public Button useYangCurrencyButton;
    
    // Battle round selection for currency usage
    private int selectedBattleRound = -1;
    
    void Start()
    {
        // Bind button events
        if (startAdventureButton != null)
            startAdventureButton.onClick.AddListener(OnStartAdventureClicked);
            
        if (replenishButton != null)
            replenishButton.onClick.AddListener(OnReplenishClicked);
            
        if (returnHomeButton != null)
            returnHomeButton.onClick.AddListener(OnReturnHomeClicked);
            
        if (resetButton != null)
            resetButton.onClick.AddListener(OnResetClicked);
            
        if (skillButton != null)
            skillButton.onClick.AddListener(OnSkillClicked);
            
        // Bind player element buttons
        for (int i = 0; i < playerElementButtons.Length && i < 5; i++)
        {
            int index = i; // Capture variable for closure
            playerElementButtons[i].onClick.AddListener(() => OnPlayerElementClicked(index));
        }
        
        // Bind opponent element buttons
        for (int i = 0; i < opponentElementButtons.Length && i < 3; i++)
        {
            int index = i; // Capture variable for closure
            opponentElementButtons[i].onClick.AddListener(() => OnOpponentElementClicked(index));
        }
        
        // Bind currency buttons
        if (useYinCurrencyButton != null)
            useYinCurrencyButton.onClick.AddListener(() => OnUseCurrencyClicked(CurrencyType.YIN));
            
        if (useYangCurrencyButton != null)
            useYangCurrencyButton.onClick.AddListener(() => OnUseCurrencyClicked(CurrencyType.YANG));
            
        // Bind WebSocket events
        if (webSocketClient != null)
        {
            webSocketClient.OnBattleResultReceived += OnBattleResultReceived;
            webSocketClient.OnPlayerJoined += OnPlayerJoined;
            webSocketClient.OnElementSelected += OnElementSelected;
        }
    }
    
    public void OnStartAdventureClicked()
    {
        Debug.Log("Start Adventure clicked");
        if (battleSystem != null)
        {
            battleSystem.StartBattle();
        }
        
        if (uiManager != null)
        {
            uiManager.UpdateUI();
        }
    }
    
    public void OnReplenishClicked()
    {
        Debug.Log("Replenish Elements clicked");
        if (gameManager != null)
        {
            gameManager.ReplenishElements();
        }
        
        if (uiManager != null)
        {
            uiManager.UpdateUI();
        }
    }
    
    public void OnReturnHomeClicked()
    {
        Debug.Log("Return Home clicked");
        if (gameManager != null)
        {
            gameManager.ReturnToSpawn();
        }
        
        if (uiManager != null)
        {
            uiManager.UpdateUI();
        }
    }
    
    public void OnResetClicked()
    {
        Debug.Log("Reset Game clicked");
        if (gameManager != null)
        {
            gameManager.ResetGame();
        }
        
        if (uiManager != null)
        {
            uiManager.UpdateUI();
        }
    }
    
    public void OnSkillClicked()
    {
        Debug.Log("Skill System clicked");
        // TODO: Implement skill system UI
    }
    
    public void OnPlayerElementClicked(int index)
    {
        Debug.Log("Player Element " + index + " clicked");
        
        // Convert index to ElementType
        List<ElementType> elements = GameUtils.GetAllElements();
        if (index >= 0 && index < elements.Count)
        {
            ElementType elementType = elements[index];
            
            if (battleSystem != null)
            {
                battleSystem.SelectPlayerElement(elementType);
            }
            
            // Highlight selected button
            for (int i = 0; i < playerElementButtons.Length; i++)
            {
                if (playerElementButtons[i] != null)
                {
                    if (i == index)
                    {
                        playerElementButtons[i].image.color = Color.yellow;
                    }
                    else
                    {
                        playerElementButtons[i].image.color = Color.white;
                    }
                }
            }
        }
    }
    
    public void OnOpponentElementClicked(int index)
    {
        Debug.Log("Opponent Element " + index + " clicked");
        if (battleSystem != null)
        {
            battleSystem.RevealOpponentElement(index);
        }
        
        if (uiManager != null)
        {
            uiManager.UpdateUI();
        }
    }
    
    public void OnUseCurrencyClicked(CurrencyType currencyType)
    {
        Debug.Log("Use " + currencyType + " Currency clicked");
        
        // Check if we have a selected battle round
        if (selectedBattleRound >= 0)
        {
            if (battleSystem != null)
            {
                battleSystem.UseCurrencyToReverse(selectedBattleRound, currencyType);
            }
            
            if (uiManager != null)
            {
                uiManager.UpdateUI();
            }
            
            // Reset selection
            selectedBattleRound = -1;
        }
        else
        {
            Debug.Log("请选择要扭转的战斗回合");
        }
    }
    
    // Method to select a battle round for currency usage
    public void SelectBattleRound(int roundIndex)
    {
        selectedBattleRound = roundIndex;
        Debug.Log("选择战斗回合 " + roundIndex + " 用于货币扭转");
    }
    
    // WebSocket event handlers
    void OnBattleResultReceived(string message)
    {
        Debug.Log("Battle result received: " + message);
        // TODO: Update UI with battle result
        
        if (uiManager != null)
        {
            uiManager.UpdateUI();
        }
    }
    
    void OnPlayerJoined(string message)
    {
        Debug.Log("Player joined: " + message);
        // TODO: Update UI for player joining
    }
    
    void OnElementSelected(string message)
    {
        Debug.Log("Element selected: " + message);
        // TODO: Update UI for element selection
        
        if (uiManager != null)
        {
            uiManager.UpdateUI();
        }
    }
    
    void OnDestroy()
    {
        // Unbind WebSocket events
        if (webSocketClient != null)
        {
            webSocketClient.OnBattleResultReceived -= OnBattleResultReceived;
            webSocketClient.OnPlayerJoined -= OnPlayerJoined;
            webSocketClient.OnElementSelected -= OnElementSelected;
        }
    }
}