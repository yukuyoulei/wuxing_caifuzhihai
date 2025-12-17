using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameManager gameManager;
    public BattleAreaController battleAreaController;
    public WebSocketClient webSocketClient;
    
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
    
    void OnStartAdventureClicked()
    {
        Debug.Log("Start Adventure clicked");
        if (gameManager != null)
        {
            gameManager.StartAdventure();
        }
    }
    
    void OnReplenishClicked()
    {
        Debug.Log("Replenish Elements clicked");
        if (gameManager != null)
        {
            gameManager.ReplenishElements();
        }
    }
    
    void OnReturnHomeClicked()
    {
        Debug.Log("Return Home clicked");
        if (gameManager != null)
        {
            gameManager.ReturnToSpawn();
        }
    }
    
    void OnResetClicked()
    {
        Debug.Log("Reset Game clicked");
        if (gameManager != null)
        {
            gameManager.ResetGame();
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
    
    void OnUseCurrencyClicked(CurrencyType currencyType)
    {
        Debug.Log("Use " + currencyType + " Currency clicked");
        // TODO: Implement currency usage logic
        // This would typically involve selecting a battle round to reverse
    }
    
    // WebSocket event handlers
    void OnBattleResultReceived(string message)
    {
        Debug.Log("Battle result received: " + message);
        // TODO: Update UI with battle result
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