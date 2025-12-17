using System;
using System.Collections.Generic;

/// <summary>
/// 玩家位置
/// </summary>
[Serializable]
public class Position
{
    public int x;
    public int y;

    public Position(int x = 0, int y = 0)
    {
        this.x = x;
        this.y = y;
    }

    public int CalculateDistance()
    {
        return Math.Abs(x) + Math.Abs(y);
    }
}

/// <summary>
/// 元素数量
/// </summary>
[Serializable]
public class ElementCount
{
    public int metal;
    public int wood;
    public int water;
    public int fire;
    public int earth;

    public ElementCount(int metal = 0, int wood = 0, int water = 0, int fire = 0, int earth = 0)
    {
        this.metal = metal;
        this.wood = wood;
        this.water = water;
        this.fire = fire;
        this.earth = earth;
    }

    public int GetElementCount(ElementType elementType)
    {
        switch (elementType)
        {
            case ElementType.METAL: return metal;
            case ElementType.WOOD: return wood;
            case ElementType.WATER: return water;
            case ElementType.FIRE: return fire;
            case ElementType.EARTH: return earth;
            default: return 0;
        }
    }

    public void SetElementCount(ElementType elementType, int count)
    {
        switch (elementType)
        {
            case ElementType.METAL: metal = count; break;
            case ElementType.WOOD: wood = count; break;
            case ElementType.WATER: water = count; break;
            case ElementType.FIRE: fire = count; break;
            case ElementType.EARTH: earth = count; break;
        }
    }
}

/// <summary>
/// 货币数量
/// </summary>
[Serializable]
public class CurrencyCount
{
    public int yin;
    public int yang;

    public CurrencyCount(int yin = 0, int yang = 0)
    {
        this.yin = yin;
        this.yang = yang;
    }

    public int GetCurrencyCount(CurrencyType currencyType)
    {
        switch (currencyType)
        {
            case CurrencyType.YIN: return yin;
            case CurrencyType.YANG: return yang;
            default: return 0;
        }
    }

    public void SetCurrencyCount(CurrencyType currencyType, int count)
    {
        switch (currencyType)
        {
            case CurrencyType.YIN: yin = count; break;
            case CurrencyType.YANG: yang = count; break;
        }
    }
}

/// <summary>
/// 对手元素
/// </summary>
[Serializable]
public class OpponentElement
{
    public ElementType type;
    public int quantity;
    public bool revealed;
    public int originalQuantity;

    public OpponentElement(ElementType type, int quantity)
    {
        this.type = type;
        this.quantity = quantity;
        this.originalQuantity = quantity;
        this.revealed = false;
    }
}

/// <summary>
/// 战斗回合结果
/// </summary>
[Serializable]
public class BattleRound
{
    public ElementType playerElement;
    public OpponentElement opponentElement;
    public int playerQuantity;
    public int opponentQuantity;
    public bool playerWin;
    public int index;

    public BattleRound(ElementType playerElement, OpponentElement opponentElement, 
                      int playerQuantity, int opponentQuantity, bool playerWin, int index)
    {
        this.playerElement = playerElement;
        this.opponentElement = opponentElement;
        this.playerQuantity = playerQuantity;
        this.opponentQuantity = opponentQuantity;
        this.playerWin = playerWin;
        this.index = index;
    }
}

/// <summary>
/// 游戏状态
/// </summary>
[Serializable]
public class GameState
{
    public Position position;
    public ElementCount elements;
    public CurrencyCount currency;
    public bool isInBattle;
    public List<BattleRound> battleRounds;
    public List<OpponentElement> opponentElements;
    public ElementType? selectedPlayerElement;
    public int currentRoundIndex;
    public BattleResult battleResult;
    public bool canUseCurrency;

    public GameState()
    {
        position = new Position();
        elements = new ElementCount();
        currency = new CurrencyCount();
        isInBattle = false;
        battleRounds = new List<BattleRound>();
        opponentElements = new List<OpponentElement>();
        selectedPlayerElement = null;
        currentRoundIndex = 0;
        battleResult = BattleResult.NONE;
        canUseCurrency = false;
    }
}