import { ElementType, type ElementInfo, type Position, type SkillUpgradeCost, type Skill, type SkillSet } from '@/types/game';

// 元素信息映射
export const ELEMENT_INFO: Record<ElementType, ElementInfo> = {
  [ElementType.METAL]: {
    type: ElementType.METAL,
    name: '金',
    color: 'hsl(var(--element-metal))',
    icon: '⚔️'
  },
  [ElementType.WOOD]: {
    type: ElementType.WOOD,
    name: '木',
    color: 'hsl(var(--element-wood))',
    icon: '🌳'
  },
  [ElementType.WATER]: {
    type: ElementType.WATER,
    name: '水',
    color: 'hsl(var(--element-water))',
    icon: '💧'
  },
  [ElementType.FIRE]: {
    type: ElementType.FIRE,
    name: '火',
    color: 'hsl(var(--element-fire))',
    icon: '🔥'
  },
  [ElementType.EARTH]: {
    type: ElementType.EARTH,
    name: '土',
    color: 'hsl(var(--element-earth))',
    icon: '🏔️'
  }
};

// 五行相克关系：key 克制 value
export const COUNTER_RELATIONS: Record<ElementType, ElementType> = {
  [ElementType.METAL]: ElementType.WOOD,  // 金克木
  [ElementType.WOOD]: ElementType.EARTH,  // 木克土
  [ElementType.EARTH]: ElementType.WATER, // 土克水
  [ElementType.WATER]: ElementType.FIRE,  // 水克火
  [ElementType.FIRE]: ElementType.METAL   // 火克金
};

// 判断元素A是否克制元素B
export function doesCounter(elementA: ElementType, elementB: ElementType): boolean {
  return COUNTER_RELATIONS[elementA] === elementB;
}

// 判断元素A是否被元素B克制
export function isCounteredBy(elementA: ElementType, elementB: ElementType): boolean {
  return COUNTER_RELATIONS[elementB] === elementA;
}

// 计算两点之间的曼哈顿距离
export function calculateDistance(pos: Position): number {
  return Math.abs(pos.x) + Math.abs(pos.y);
}

// 生成随机整数 [min, max]
export function randomInt(min: number, max: number): number {
  return Math.floor(Math.random() * (max - min + 1)) + min;
}

// 随机选择数组中的一个元素
export function randomChoice<T>(array: T[]): T {
  return array[randomInt(0, array.length - 1)];
}

// 获取所有元素类型
export function getAllElements(): ElementType[] {
  return [
    ElementType.METAL,
    ElementType.WOOD,
    ElementType.WATER,
    ElementType.FIRE,
    ElementType.EARTH
  ];
}

// 根据距离生成对手元素数量
export function generateOpponentQuantity(distance: number): number {
  const baseMin = 5;
  const baseMax = 15;
  const distanceBonus = Math.floor(distance / 2);
  
  return randomInt(baseMin + distanceBonus, baseMax + distanceBonus);
}

// 根据距离生成货币数量
export function generateCurrencyAmount(distance: number): number {
  if (Math.random() < 0.3) return 0; // 30% 概率不获得货币
  
  const baseMin = 1;
  const baseMax = 5;
  const distanceBonus = Math.floor(distance / 3);
  
  return randomInt(baseMin + distanceBonus, baseMax + distanceBonus);
}

// 初始化技能集合
export function initializeSkills(): SkillSet {
  const elements = getAllElements();
  const skills: Partial<SkillSet> = {};
  
  elements.forEach(element => {
    skills[element] = {
      element,
      yinLevel: 0,
      yangLevel: 0
    };
  });
  
  return skills as SkillSet;
}

// 计算技能升级成本
export function calculateSkillUpgradeCost(currentLevel: number): SkillUpgradeCost {
  const baseElementCost = 10;
  const baseCurrencyCost = 3;
  
  return {
    elementCost: baseElementCost + currentLevel * 5,
    currencyCost: baseCurrencyCost + currentLevel * 2
  };
}

// 计算克制效果（考虑技能加成）
export function calculateCounterEffect(skill: Skill, isCountering: boolean): number {
  if (isCountering) {
    const baseReduction = 0.5;
    const yinBonus = skill.yinLevel * 0.05;
    return baseReduction + yinBonus;
  } else {
    const baseMultiplier = 2.0;
    const yangReduction = skill.yangLevel * 0.1;
    return Math.max(1.0, baseMultiplier - yangReduction);
  }
}

// 应用克制效果到对手数量
export function applyCounterEffect(
  opponentQuantity: number,
  playerElement: ElementType,
  opponentElement: ElementType,
  skills: SkillSet
): number {
  const playerSkill = skills[playerElement];
  
  if (doesCounter(playerElement, opponentElement)) {
    const reductionRate = calculateCounterEffect(playerSkill, true);
    return Math.floor(opponentQuantity * (1 - reductionRate));
  } else if (isCounteredBy(playerElement, opponentElement)) {
    const multiplier = calculateCounterEffect(playerSkill, false);
    return Math.floor(opponentQuantity * multiplier);
  }
  
  return opponentQuantity;
}
