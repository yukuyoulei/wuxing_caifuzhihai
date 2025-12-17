using System;
using System.Collections.Generic;

public static class GameUtils
{
    // 五行相克关系：key 克制 value
    private static readonly Dictionary<ElementType, ElementType> COUNTER_RELATIONS = new Dictionary<ElementType, ElementType>
    {
        { ElementType.METAL, ElementType.WOOD },   // 金克木
        { ElementType.WOOD, ElementType.EARTH },   // 木克土
        { ElementType.EARTH, ElementType.WATER },  // 土克水
        { ElementType.WATER, ElementType.FIRE },   // 水克火
        { ElementType.FIRE, ElementType.METAL }    // 火克金
    };

    /// <summary>
    /// 获取所有元素类型
    /// </summary>
    public static List<ElementType> GetAllElements()
    {
        return new List<ElementType> 
        { 
            ElementType.METAL, 
            ElementType.WOOD, 
            ElementType.WATER, 
            ElementType.FIRE, 
            ElementType.EARTH 
        };
    }

    /// <summary>
    /// 判断元素A是否克制元素B
    /// </summary>
    public static bool DoesCounter(ElementType elementA, ElementType elementB)
    {
        return COUNTER_RELATIONS.ContainsKey(elementA) && COUNTER_RELATIONS[elementA] == elementB;
    }

    /// <summary>
    /// 判断元素A是否被元素B克制
    /// </summary>
    public static bool IsCounteredBy(ElementType elementA, ElementType elementB)
    {
        return COUNTER_RELATIONS.ContainsKey(elementB) && COUNTER_RELATIONS[elementB] == elementA;
    }

    /// <summary>
    /// 生成随机整数 [min, max]
    /// </summary>
    public static int RandomInt(int min, int max)
    {
        Random rand = new Random();
        return rand.Next(min, max + 1);
    }

    /// <summary>
    /// 随机选择数组中的一个元素
    /// </summary>
    public static T RandomChoice<T>(List<T> list)
    {
        if (list == null || list.Count == 0)
            throw new ArgumentException("List cannot be null or empty");
        
        return list[RandomInt(0, list.Count - 1)];
    }

    /// <summary>
    /// 根据距离生成对手元素数量
    /// </summary>
    public static int GenerateOpponentQuantity(int distance)
    {
        int baseMin = 5;
        int baseMax = 15;
        int distanceBonus = distance / 2;

        return RandomInt(baseMin + distanceBonus, baseMax + distanceBonus);
    }

    /// <summary>
    /// 根据距离生成货币数量
    /// </summary>
    public static int GenerateCurrencyAmount(int distance)
    {
        Random rand = new Random();
        if (rand.NextDouble() < 0.3) return 0; // 30% 概率不获得货币

        int baseMin = 1;
        int baseMax = 5;
        int distanceBonus = distance / 3;

        return RandomInt(baseMin + distanceBonus, baseMax + distanceBonus);
    }

    /// <summary>
    /// 移动玩家位置
    /// </summary>
    public static Position MovePlayer(Position currentPosition)
    {
        List<Direction> directions = new List<Direction> 
        { 
            Direction.LEFT_UP, 
            Direction.LEFT_DOWN, 
            Direction.RIGHT_UP, 
            Direction.RIGHT_DOWN 
        };
        
        Direction direction = RandomChoice(directions);
        Position newPos = new Position(currentPosition.x, currentPosition.y);

        if (direction == Direction.LEFT_UP || direction == Direction.LEFT_DOWN)
        {
            newPos.x -= 1;
        }
        else
        {
            newPos.x += 1;
        }

        if (direction == Direction.LEFT_UP || direction == Direction.RIGHT_UP)
        {
            newPos.y -= 1;
        }
        else
        {
            newPos.y += 1;
        }

        return newPos;
    }

    /// <summary>
    /// 应用克制效果到对手数量
    /// </summary>
    public static int ApplyCounterEffect(int opponentQuantity, ElementType playerElement, ElementType opponentElement)
    {
        if (DoesCounter(playerElement, opponentElement))
        {
            // 克制关系，对手数量减半
            double reductionRate = 0.5;
            return (int)Math.Floor(opponentQuantity * (1 - reductionRate));
        }
        else if (IsCounteredBy(playerElement, opponentElement))
        {
            // 被克制关系，对手数量翻倍
            double multiplier = 2.0;
            return (int)Math.Floor(opponentQuantity * multiplier);
        }

        // 无克制关系，数量不变
        return opponentQuantity;
    }

    /// <summary>
    /// 获取元素名称
    /// </summary>
    public static string GetElementName(ElementType elementType)
    {
        switch (elementType)
        {
            case ElementType.METAL: return "金";
            case ElementType.WOOD: return "木";
            case ElementType.WATER: return "水";
            case ElementType.FIRE: return "火";
            case ElementType.EARTH: return "土";
            default: return "";
        }
    }

    /// <summary>
    /// 获取元素图标
    /// </summary>
    public static string GetElementIcon(ElementType elementType)
    {
        switch (elementType)
        {
            case ElementType.METAL: return "⚔️";
            case ElementType.WOOD: return "🌳";
            case ElementType.WATER: return "💧";
            case ElementType.FIRE: return "🔥";
            case ElementType.EARTH: return "🏔️";
            default: return "";
        }
    }
}