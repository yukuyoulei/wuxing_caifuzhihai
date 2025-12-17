using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BattleAreaController : MonoBehaviour
{
    public Transform playerElementPanel;
    public Transform opponentElementPanel;
    public Transform battleRecordPanel;

    public GameObject elementButtonPrefab;
    public GameObject battleRecordPrefab;

    private List<Button> playerButtons = new List<Button>();
    private List<Button> opponentButtons = new List<Button>();
    private List<Text> playerElementTexts = new List<Text>();
    private List<Text> opponentElementTexts = new List<Text>();
    
    private GameState gameState;
    
    void Start()
    {
        // 初始化游戏状态
        gameState = new GameState();
        
        // 初始化玩家元素（每个元素10个）
        gameState.elements.SetElementCount(ElementType.METAL, 10);
        gameState.elements.SetElementCount(ElementType.WOOD, 10);
        gameState.elements.SetElementCount(ElementType.WATER, 10);
        gameState.elements.SetElementCount(ElementType.FIRE, 10);
        gameState.elements.SetElementCount(ElementType.EARTH, 10);
        
        GeneratePlayerElements();
        GenerateOpponentElements();
    }

    void GeneratePlayerElements()
    {
        // 生成5个玩家元素按钮
        List<ElementType> elements = GameUtils.GetAllElements();
        for (int i = 0; i < elements.Count; i++)
        {
            GameObject btnGO = Instantiate(elementButtonPrefab, playerElementPanel);
            btnGO.name = "PlayerElement_" + elements[i].ToString();
            
            Text textComponent = btnGO.GetComponentInChildren<Text>();
            textComponent.text = GameUtils.GetElementIcon(elements[i]) + "\n" + GameUtils.GetElementName(elements[i]) + "\n" + gameState.elements.GetElementCount(elements[i]);
            playerElementTexts.Add(textComponent);
            
            int index = i;
            btnGO.GetComponent<Button>().onClick.AddListener(() => OnPlayerElementSelected(index));
            playerButtons.Add(btnGO.GetComponent<Button>());
        }
    }

    void GenerateOpponentElements()
    {
        // 生成3个对手元素按钮（？表示未揭示）
        for (int i = 0; i < 3; i++)
        {
            GameObject btnGO = Instantiate(elementButtonPrefab, opponentElementPanel);
            btnGO.name = "OpponentElement_" + i;
            
            Text textComponent = btnGO.GetComponentInChildren<Text>();
            textComponent.text = "?";
            opponentElementTexts.Add(textComponent);
            
            int index = i;
            btnGO.GetComponent<Button>().onClick.AddListener(() => OnOpponentElementRevealed(index));
            opponentButtons.Add(btnGO.GetComponent<Button>());
        }
    }

    public void OnPlayerElementSelected(int index)
    {
        List<ElementType> elements = GameUtils.GetAllElements();
        if (index >= 0 && index < elements.Count)
        {
            ElementType selectedElement = elements[index];
            int elementCount = gameState.elements.GetElementCount(selectedElement);
            
            if (elementCount <= 0)
            {
                Debug.Log("元素不足: " + GameUtils.GetElementName(selectedElement));
                return;
            }
            
            gameState.selectedPlayerElement = selectedElement;
            Debug.Log("玩家选择元素: " + GameUtils.GetElementName(selectedElement) + " (数量: " + elementCount + ")");
            
            // 高亮选中的按钮
            for (int i = 0; i < playerButtons.Count; i++)
            {
                if (i == index)
                {
                    playerButtons[i].image.color = Color.yellow;
                }
                else
                {
                    playerButtons[i].image.color = Color.white;
                }
            }
        }
    }

    public void OnOpponentElementRevealed(int index)
    {
        if (gameState.selectedPlayerElement == null)
        {
            Debug.Log("请先选择玩家元素");
            return;
        }
        
        if (index < 0 || index >= gameState.opponentElements.Count)
        {
            Debug.Log("无效的对手索引");
            return;
        }
        
        OpponentElement opponent = gameState.opponentElements[index];
        if (opponent.revealed)
        {
            Debug.Log("对手元素已揭示");
            return;
        }
        
        // 应用克制效果
        int finalOpponentQuantity = GameUtils.ApplyCounterEffect(
            opponent.quantity, 
            gameState.selectedPlayerElement.Value, 
            opponent.type);
        
        // 判断胜负
        int playerQuantity = gameState.elements.GetElementCount(gameState.selectedPlayerElement.Value);
        bool playerWin = playerQuantity > finalOpponentQuantity;
        
        // 创建战斗回合记录
        BattleRound round = new BattleRound(
            gameState.selectedPlayerElement.Value,
            new OpponentElement(opponent.type, finalOpponentQuantity),
            playerQuantity,
            finalOpponentQuantity,
            playerWin,
            index);
        
        gameState.battleRounds.Add(round);
        
        // 更新对手元素状态
        opponent.revealed = true;
        opponent.quantity = finalOpponentQuantity;
        
        // 更新UI
        if (index < opponentElementTexts.Count)
        {
            opponentElementTexts[index].text = GameUtils.GetElementIcon(opponent.type) + "\n" + 
                                              GameUtils.GetElementName(opponent.type) + "\n" + 
                                              opponent.quantity;
        }
        
        // 添加战斗记录
        string resultText = playerWin ? "胜利" : "失败";
        AddBattleRecord($"第{gameState.currentRoundIndex + 1}回合: 我方{GameUtils.GetElementName(gameState.selectedPlayerElement.Value)}({playerQuantity}) vs 对方{GameUtils.GetElementName(opponent.type)}({finalOpponentQuantity}) - {resultText}");
        
        // 清除选中状态
        gameState.selectedPlayerElement = null;
        foreach (var button in playerButtons)
        {
            button.image.color = Color.white;
        }
        
        gameState.currentRoundIndex++;
        
        // 检查是否完成所有回合
        if (gameState.battleRounds.Count >= 3)
        {
            CheckBattleResult();
        }
    }
    
    void CheckBattleResult()
    {
        int wins = 0;
        foreach (var round in gameState.battleRounds)
        {
            if (round.playerWin) wins++;
        }
        
        bool battleWon = wins >= 2;
        gameState.battleResult = battleWon ? BattleResult.WIN : BattleResult.LOSE;
        
        if (battleWon)
        {
            Debug.Log("战斗胜利！");
            HandleBattleWin();
        }
        else
        {
            Debug.Log("战斗失败！");
            HandleBattleLose();
        }
    }
    
    void HandleBattleWin()
    {
        int distance = gameState.position.CalculateDistance();
        int currencyAmount = GameUtils.GenerateCurrencyAmount(distance);
        
        if (currencyAmount > 0)
        {
            // 随机获得阴或阳货币
            CurrencyType currencyType = (CurrencyType)GameUtils.RandomInt(0, 1);
            gameState.currency.SetCurrencyCount(currencyType, 
                gameState.currency.GetCurrencyCount(currencyType) + currencyAmount);
            
            Debug.Log($"获得 {currencyAmount} {currencyType} 货币");
        }
        
        // 可以选择一个获胜回合获得对方元素
        Debug.Log("可以选择一个获胜回合获得对方元素");
    }
    
    void HandleBattleLose()
    {
        // 失败时失去所有失败回合中使用的元素
        Dictionary<ElementType, int> lostElements = new Dictionary<ElementType, int>();
        
        foreach (var round in gameState.battleRounds)
        {
            if (!round.playerWin)
            {
                if (!lostElements.ContainsKey(round.playerElement))
                {
                    lostElements[round.playerElement] = 0;
                }
                lostElements[round.playerElement] += round.playerQuantity;
            }
        }
        
        // 更新玩家元素
        foreach (var kvp in lostElements)
        {
            int currentCount = gameState.elements.GetElementCount(kvp.Key);
            gameState.elements.SetElementCount(kvp.Key, Mathf.Max(0, currentCount - kvp.Value));
        }
        
        gameState.canUseCurrency = true;
        Debug.Log("战斗失败，可以使用货币扭转局势或返回出生点");
    }

    public void AddBattleRecord(string recordText)
    {
        GameObject recordGO = Instantiate(battleRecordPrefab, battleRecordPanel);
        recordGO.GetComponentInChildren<Text>().text = recordText;
    }
    
    // 开始探险
    public void StartAdventure()
    {
        // 检查是否所有元素都至少有10个
        List<ElementType> elements = GameUtils.GetAllElements();
        bool hasLowElement = false;
        
        foreach (ElementType element in elements)
        {
            if (gameState.elements.GetElementCount(element) < 10)
            {
                hasLowElement = true;
                break;
            }
        }
        
        if (hasLowElement)
        {
            Debug.Log("所有元素必须至少有10个才能开始探险，请先补充元素");
            return;
        }
        
        // 移动玩家
        Position newPosition = GameUtils.MovePlayer(gameState.position);
        gameState.position = newPosition;
        int distance = newPosition.CalculateDistance();
        
        Debug.Log($"移动到坐标 ({newPosition.x}, {newPosition.y})，距离出生点 {distance} 步");
        
        // 生成对手元素
        gameState.opponentElements.Clear();
        List<ElementType> allElements = GameUtils.GetAllElements();
        
        // 随机选择3个元素类型
        List<ElementType> shuffledElements = new List<ElementType>(allElements);
        Shuffle(shuffledElements);
        List<ElementType> selectedElements = shuffledElements.GetRange(0, 3);
        
        foreach (ElementType elementType in selectedElements)
        {
            int quantity = GameUtils.GenerateOpponentQuantity(distance);
            gameState.opponentElements.Add(new OpponentElement(elementType, quantity));
        }
        
        // 重置战斗状态
        gameState.isInBattle = true;
        gameState.battleRounds.Clear();
        gameState.selectedPlayerElement = null;
        gameState.currentRoundIndex = 0;
        gameState.battleResult = BattleResult.PENDING;
        gameState.canUseCurrency = false;
        
        // 更新对手元素UI
        for (int i = 0; i < opponentElementTexts.Count; i++)
        {
            if (i < gameState.opponentElements.Count)
            {
                opponentElementTexts[i].text = "?";
            }
            else
            {
                opponentElementTexts[i].text = "";
            }
        }
        
        // 清除战斗记录
        foreach (Transform child in battleRecordPanel)
        {
            Destroy(child.gameObject);
        }
    }
    
    // Fisher-Yates 洗牌算法
    void Shuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}