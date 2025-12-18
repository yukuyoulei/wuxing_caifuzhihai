using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public BattleAreaController battleAreaController;
    
    // UI References
    public Text positionText;
    public Text currencyText;
    public Text[] elementTexts; // 5个元素文本显示
    
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
        
        UpdateUI();
    }
    
    void UpdateUI()
    {
        // 更新位置显示
        if (positionText != null)
        {
            positionText.text = $"位置: ({gameState.position.x}, {gameState.position.y})";
        }
        
        // 更新货币显示
        if (currencyText != null)
        {
            currencyText.text = $"阴: {gameState.currency.yin} 阳: {gameState.currency.yang}";
        }
        
        // 更新元素显示
        if (elementTexts.Length >= 5)
        {
            elementTexts[0].text = $"{GameUtils.GetElementIcon(ElementType.METAL)} {GameUtils.GetElementName(ElementType.METAL)}: {gameState.elements.metal}";
            elementTexts[1].text = $"{GameUtils.GetElementIcon(ElementType.WOOD)} {GameUtils.GetElementName(ElementType.WOOD)}: {gameState.elements.wood}";
            elementTexts[2].text = $"{GameUtils.GetElementIcon(ElementType.WATER)} {GameUtils.GetElementName(ElementType.WATER)}: {gameState.elements.water}";
            elementTexts[3].text = $"{GameUtils.GetElementIcon(ElementType.FIRE)} {GameUtils.GetElementName(ElementType.FIRE)}: {gameState.elements.fire}";
            elementTexts[4].text = $"{GameUtils.GetElementIcon(ElementType.EARTH)} {GameUtils.GetElementName(ElementType.EARTH)}: {gameState.elements.earth}";
        }
    }
    
    // 获取游戏状态
    public GameState GetGameState()
    {
        return gameState;
    }
    
    // 设置游戏状态
    public void SetGameState(GameState newState)
    {
        gameState = newState;
        UpdateUI();
    }
    
    // 开始探险
    public void StartAdventure()
    {
        if (battleAreaController != null)
        {
            battleAreaController.StartAdventure();
        }
        
        UpdateUI();
    }
    
    // 补充元素
    public void ReplenishElements()
    {
        bool replenished = false;
        List<ElementType> elements = GameUtils.GetAllElements();
        
        foreach (ElementType element in elements)
        {
            if (gameState.elements.GetElementCount(element) < 10)
            {
                gameState.elements.SetElementCount(element, 10);
                replenished = true;
            }
        }
        
        if (replenished)
        {
            Debug.Log("所有元素已补充至10个");
        }
        else
        {
            Debug.Log("所有元素都已充足");
        }
        
        UpdateUI();
    }
    
    // 返回出生点
    public void ReturnToSpawn()
    {
        gameState.position = new Position(0, 0);
        gameState.isInBattle = false;
        gameState.battleRounds.Clear();
        gameState.opponentElements.Clear();
        gameState.selectedPlayerElement = null;
        gameState.currentRoundIndex = 0;
        gameState.battleResult = BattleResult.NONE;
        gameState.canUseCurrency = false;
        
        Debug.Log("返回出生点");
        UpdateUI();
    }
    
    // 重置游戏
    public void ResetGame()
    {
        gameState = new GameState();
        
        // 初始化玩家元素（每个元素10个）
        gameState.elements.SetElementCount(ElementType.METAL, 10);
        gameState.elements.SetElementCount(ElementType.WOOD, 10);
        gameState.elements.SetElementCount(ElementType.WATER, 10);
        gameState.elements.SetElementCount(ElementType.FIRE, 10);
        gameState.elements.SetElementCount(ElementType.EARTH, 10);
        
        Debug.Log("游戏已重置");
        UpdateUI();
    }
    
    // 使用货币扭转局势
    public void UseCurrencyToReverse(int roundIndex, CurrencyType currencyType)
    {
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
            // 处理胜利逻辑
        }
        else
        {
            Debug.Log("即使扭转这一局，你仍然没有赢得战斗");
        }
        
        UpdateUI();
    }
}