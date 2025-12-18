using UnityEngine;

public class GameTester : MonoBehaviour
{
    public GameManager gameManager;
    public BattleSystem battleSystem;
    public WebSocketClient webSocketClient;
    
    void Start()
    {
        Debug.Log("Game Tester initialized");
        
        // Test game utilities
        TestGameUtils();
        
        // Test game state
        TestGameState();
        
        // Test element selection
        TestElementSelection();
    }
    
    void TestGameUtils()
    {
        Debug.Log("=== Testing Game Utilities ===");
        
        // Test element names and icons
        Debug.Log("Metal: " + GameUtils.GetElementName(ElementType.METAL) + " " + GameUtils.GetElementIcon(ElementType.METAL));
        Debug.Log("Wood: " + GameUtils.GetElementName(ElementType.WOOD) + " " + GameUtils.GetElementIcon(ElementType.WOOD));
        Debug.Log("Water: " + GameUtils.GetElementName(ElementType.WATER) + " " + GameUtils.GetElementIcon(ElementType.WATER));
        Debug.Log("Fire: " + GameUtils.GetElementName(ElementType.FIRE) + " " + GameUtils.GetElementIcon(ElementType.FIRE));
        Debug.Log("Earth: " + GameUtils.GetElementName(ElementType.EARTH) + " " + GameUtils.GetElementIcon(ElementType.EARTH));
        
        // Test counter relations
        Debug.Log("Metal counters Wood: " + GameUtils.DoesCounter(ElementType.METAL, ElementType.WOOD));
        Debug.Log("Wood counters Earth: " + GameUtils.DoesCounter(ElementType.WOOD, ElementType.EARTH));
        Debug.Log("Earth counters Water: " + GameUtils.DoesCounter(ElementType.EARTH, ElementType.WATER));
        Debug.Log("Water counters Fire: " + GameUtils.DoesCounter(ElementType.WATER, ElementType.FIRE));
        Debug.Log("Fire counters Metal: " + GameUtils.DoesCounter(ElementType.FIRE, ElementType.METAL));
        
        // Test distance calculation
        Position pos = new Position(3, 4);
        Debug.Log("Distance from (3,4) to origin: " + pos.CalculateDistance());
        
        // Test random generation
        Debug.Log("Random int 1-10: " + GameUtils.RandomInt(1, 10));
        Debug.Log("Opponent quantity at distance 5: " + GameUtils.GenerateOpponentQuantity(5));
        Debug.Log("Currency amount at distance 5: " + GameUtils.GenerateCurrencyAmount(5));
    }
    
    void TestGameState()
    {
        Debug.Log("=== Testing Game State ===");
        
        if (gameManager != null)
        {
            GameState gameState = gameManager.GetGameState();
            
            // Test initial state
            Debug.Log("Initial position: (" + gameState.position.x + ", " + gameState.position.y + ")");
            Debug.Log("Initial metal elements: " + gameState.elements.metal);
            Debug.Log("Initial wood elements: " + gameState.elements.wood);
            Debug.Log("Initial yin currency: " + gameState.currency.yin);
            Debug.Log("Initial yang currency: " + gameState.currency.yang);
            
            // Test setting elements
            gameState.elements.SetElementCount(ElementType.METAL, 15);
            Debug.Log("Updated metal elements: " + gameState.elements.GetElementCount(ElementType.METAL));
        }
    }
    
    void TestElementSelection()
    {
        Debug.Log("=== Testing Element Selection ===");
        
        if (battleSystem != null)
        {
            // Test selecting elements
            battleSystem.SelectPlayerElement(ElementType.METAL);
            battleSystem.SelectPlayerElement(ElementType.WOOD);
            
            // Test revealing opponent elements (will show warnings since no battle started)
            battleSystem.RevealOpponentElement(0);
            battleSystem.RevealOpponentElement(1);
        }
    }
    
    void Update()
    {
        // Test WebSocket connection periodically
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space key pressed - testing WebSocket");
            if (webSocketClient != null)
            {
                webSocketClient.JoinGame("Player1");
            }
        }
        
        // Test starting battle
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("B key pressed - starting battle");
            if (battleSystem != null)
            {
                battleSystem.StartBattle();
            }
        }
        
        // Test replenishing elements
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("R key pressed - replenishing elements");
            if (gameManager != null)
            {
                gameManager.ReplenishElements();
            }
        }
    }
}