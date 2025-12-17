using UnityEngine;
using System.Collections.Generic;

public class BattleSystem : MonoBehaviour
{
    public GameManager gameManager;
    public WebSocketClient webSocketClient;
    
    private GameState gameState;
    
    void Start()
    {
        if (gameManager != null)
        {
            gameState = gameManager.GetGameState();
        }
    }
    
    /// <summary>
    /// 开始战斗
    /// </summary>
    public void StartBattle()
    {
        if (gameState == null) return;
        
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
        List<ElementType> selectedElements = shuffledElements.GetRange(0, Mathf.Min(3, shuffledElements.Count));
        
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
        
        // 通知服务器开始战斗
        if (webSocketClient != null)
        {
            // TODO: 发送战斗开始消息到服务器
        }
        
        Debug.Log("战斗开始！");
    }
    
    /// <summary>
    /// 玩家选择元素
    /// </summary>
    public void SelectPlayerElement(ElementType elementType)
    {
        if (gameState == null) return;
        
        int elementCount = gameState.elements.GetElementCount(elementType);
        
        if (elementCount <= 0)
        {
            Debug.Log("元素不足: " + GameUtils.GetElementName(elementType));
            return;
        }
        
        gameState.selectedPlayerElement = elementType;
        Debug.Log("玩家选择元素: " + GameUtils.GetElementName(elementType) + " (数量: " + elementCount + ")");
        
        // 通知服务器元素选择
        if (webSocketClient != null)
        {
            // TODO: 发送元素选择消息到服务器
        }
    }
    
    /// <summary>
    /// 揭示对手元素
    /// </summary>
    public void RevealOpponentElement(int index)
    {
        if (gameState == null) return;
        
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
        
        // 添加战斗记录
        string resultText = playerWin ? "胜利" : "失败";
        Debug.Log($"第{gameState.currentRoundIndex + 1}回合: 我方{GameUtils.GetElementName(gameState.selectedPlayerElement.Value)}({playerQuantity}) vs 对方{GameUtils.GetElementName(opponent.type)}({finalOpponentQuantity}) - {resultText}");
        
        // 清除选中状态
        gameState.selectedPlayerElement = null;
        gameState.currentRoundIndex++;
        
        // 检查是否完成所有回合
        if (gameState.battleRounds.Count >= 3)
        {
            CheckBattleResult();
        }
        
        // 通知服务器揭示元素
        if (webSocketClient != null)
        {
            // TODO: 发送揭示元素消息到服务器
        }
    }
    
    /// <summary>
    /// 检查战斗结果
    /// </summary>
    void CheckBattleResult()
    {
        if (gameState == null) return;
        
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
    
    /// <summary>
    /// 处理战斗胜利
    /// </summary>
    void HandleBattleWin()
    {
        if (gameState == null) return;
        
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
        
        // 通知服务器战斗胜利
        if (webSocketClient != null)
        {
            // TODO: 发送战斗胜利消息到服务器
        }
    }
    
    /// <summary>
    /// 处理战斗失败
    /// </summary>
    void HandleBattleLose()
    {
        if (gameState == null) return;
        
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
        
        // 通知服务器战斗失败
        if (webSocketClient != null)
        {
            // TODO: 发送战斗失败消息到服务器
        }
    }
    
    /// <summary>
    /// 使用货币扭转局势
    /// </summary>
    public void UseCurrencyToReverse(int roundIndex, CurrencyType currencyType)
    {
        if (gameState == null) return;
        
        if (roundIndex < 0 || roundIndex >= gameState.battleRounds.Count)
        {
            Debug.Log("无效的回合索引");
            return;
        }
        
        BattleRound round = gameState.battleRounds[roundIndex];
        if (round.playerWin)
        {
            Debug.Log("这一局你已经赢了");
            return;
        }
        
        int deficit = round.opponentQuantity - round.playerQuantity;
        if (gameState.currency.GetCurrencyCount(currencyType) < deficit)
        {
            Debug.Log($"需要 {deficit} {currencyType} 货币");
            return;
        }
        
        // 扭转结果
        gameState.battleRounds[roundIndex] = new BattleRound(
            round.playerElement,
            round.opponentElement,
            round.playerQuantity,
            round.opponentQuantity,
            true, // 现在赢了
            round.index
        );
        
        // 扣除货币
        int currentCurrency = gameState.currency.GetCurrencyCount(currencyType);
        gameState.currency.SetCurrencyCount(currencyType, currentCurrency - deficit);
        
        // 重新检查战斗结果
        int wins = 0;
        foreach (var r in gameState.battleRounds)
        {
            if (r.playerWin) wins++;
        }
        
        bool battleWon = wins >= 2;
        gameState.battleResult = battleWon ? BattleResult.WIN : BattleResult.LOSE;
        gameState.canUseCurrency = !battleWon;
        
        if (battleWon)
        {
            Debug.Log("扭转成功，战斗胜利！");
            HandleBattleWin(); // 重新处理胜利逻辑
        }
        else
        {
            Debug.Log("即使扭转这一局，你仍然没有赢得战斗");
        }
        
        // 通知服务器货币使用
        if (webSocketClient != null)
        {
            // TODO: 发送货币使用消息到服务器
        }
    }
    
    /// <summary>
    /// Fisher-Yates 洗牌算法
    /// </summary>
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