using System;

/// <summary>
/// 五行元素类型
/// </summary>
public enum ElementType
{
    METAL,  // 金
    WOOD,   // 木
    WATER,  // 水
    FIRE,   // 火
    EARTH   // 土
}

/// <summary>
/// 货币类型
/// </summary>
public enum CurrencyType
{
    YIN,   // 阴
    YANG   // 阳
}

/// <summary>
/// 方向
/// </summary>
public enum Direction
{
    LEFT_UP,
    LEFT_DOWN,
    RIGHT_UP,
    RIGHT_DOWN
}

/// <summary>
/// 战斗结果状态
/// </summary>
public enum BattleResult
{
    PENDING,
    WIN,
    LOSE,
    NONE
}