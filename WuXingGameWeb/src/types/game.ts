// 五行元素类型
export enum ElementType {
  METAL = 'metal', // 金
  WOOD = 'wood',   // 木
  WATER = 'water', // 水
  FIRE = 'fire',   // 火
  EARTH = 'earth'  // 土
}

// 货币类型
export enum CurrencyType {
  YIN = 'yin',   // 阴
  YANG = 'yang'  // 阳
}

// 技能类型（对应五行元素）
export interface Skill {
  element: ElementType;
  yinLevel: number;  // 阴属性等级（增强克制效果）
  yangLevel: number; // 阳属性等级（减免被克制效果）
}

// 技能集合
export interface SkillSet {
  [ElementType.METAL]: Skill;
  [ElementType.WOOD]: Skill;
  [ElementType.WATER]: Skill;
  [ElementType.FIRE]: Skill;
  [ElementType.EARTH]: Skill;
}

// 技能升级成本
export interface SkillUpgradeCost {
  elementCost: number;
  currencyCost: number;
}

// 元素数量
export interface ElementCount {
  [ElementType.METAL]: number;
  [ElementType.WOOD]: number;
  [ElementType.WATER]: number;
  [ElementType.FIRE]: number;
  [ElementType.EARTH]: number;
}

// 货币数量
export interface CurrencyCount {
  [CurrencyType.YIN]: number;
  [CurrencyType.YANG]: number;
}

// 玩家位置
export interface Position {
  x: number;
  y: number;
}

// 对手元素
export interface OpponentElement {
  type: ElementType;
  quantity: number;
  revealed: boolean;
  originalQuantity: number;
}

// 战斗回合结果
export interface BattleRound {
  playerElement: ElementType;
  opponentElement: OpponentElement;
  playerQuantity: number;
  opponentQuantity: number;
  playerWin: boolean;
  index: number;
}

// 游戏状态
export interface GameState {
  position: Position;
  elements: ElementCount;
  currency: CurrencyCount;
  skills: SkillSet;
  isInBattle: boolean;
  battleRounds: BattleRound[];
  opponentElements: OpponentElement[];
  selectedPlayerElement: ElementType | null;
  currentRoundIndex: number;
  battleResult: 'pending' | 'win' | 'lose' | null;
  canUseCurrency: boolean;
}

// 方向
export enum Direction {
  LEFT_UP = 'left-up',
  LEFT_DOWN = 'left-down',
  RIGHT_UP = 'right-up',
  RIGHT_DOWN = 'right-down'
}

// 元素显示信息
export interface ElementInfo {
  type: ElementType;
  name: string;
  color: string;
  icon: string;
}
