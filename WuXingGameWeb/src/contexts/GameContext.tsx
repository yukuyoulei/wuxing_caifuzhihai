import React, { createContext, useContext, useState, useEffect, type ReactNode } from 'react';
import {
  ElementType,
  CurrencyType,
  Direction,
  type GameState,
  type ElementCount,
  type CurrencyCount,
  type Position,
  type OpponentElement,
  type BattleRound,
  type SkillSet
} from '@/types/game';
import {
  calculateDistance,
  randomInt,
  randomChoice,
  getAllElements,
  generateOpponentQuantity,
  generateCurrencyAmount,
  doesCounter,
  isCounteredBy,
  initializeSkills,
  calculateSkillUpgradeCost,
  applyCounterEffect,
  ELEMENT_INFO
} from '@/lib/game-utils';
import { useToast } from '@/hooks/use-toast';

interface GameContextType {
  gameState: GameState;
  startAdventure: () => void;
  selectPlayerElement: (element: ElementType) => void;
  revealOpponentElement: (index: number) => void;
  useCurrencyToReverse: (roundIndex: number, currencyType: CurrencyType) => void;
  returnToSpawn: () => void;
  replenishElements: () => void;
  resetGame: () => void;
  selectWinningRound: (roundIndex: number) => void;
  upgradeSkill: (element: ElementType, upgradeType: 'yin' | 'yang') => void;
  canStartAdventure: () => boolean;
}

const GameContext = createContext<GameContextType | undefined>(undefined);

const INITIAL_ELEMENTS: ElementCount = {
  [ElementType.METAL]: 0,
  [ElementType.WOOD]: 0,
  [ElementType.WATER]: 0,
  [ElementType.FIRE]: 0,
  [ElementType.EARTH]: 0
};

const INITIAL_CURRENCY: CurrencyCount = {
  [CurrencyType.YIN]: 0,
  [CurrencyType.YANG]: 0
};

const INITIAL_POSITION: Position = { x: 0, y: 0 };

const STORAGE_KEY = 'wuxing_game_state';

export function GameProvider({ children }: { children: ReactNode }) {
  const { toast } = useToast();
  
  const [gameState, setGameState] = useState<GameState>(() => {
    const saved = localStorage.getItem(STORAGE_KEY);
    if (saved) {
      try {
        const parsed = JSON.parse(saved);
        if (!parsed.skills) {
          parsed.skills = initializeSkills();
        }
        return parsed;
      } catch {
        return {
          position: INITIAL_POSITION,
          elements: INITIAL_ELEMENTS,
          currency: INITIAL_CURRENCY,
          skills: initializeSkills(),
          isInBattle: false,
          battleRounds: [],
          opponentElements: [],
          selectedPlayerElement: null,
          currentRoundIndex: 0,
          battleResult: null,
          canUseCurrency: false
        };
      }
    }
    return {
      position: INITIAL_POSITION,
      elements: INITIAL_ELEMENTS,
      currency: INITIAL_CURRENCY,
      skills: initializeSkills(),
      isInBattle: false,
      battleRounds: [],
      opponentElements: [],
      selectedPlayerElement: null,
      currentRoundIndex: 0,
      battleResult: null,
      canUseCurrency: false
    };
  });

  useEffect(() => {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(gameState));
  }, [gameState]);

  const movePlayer = (): Position => {
    const directions = [Direction.LEFT_UP, Direction.LEFT_DOWN, Direction.RIGHT_UP, Direction.RIGHT_DOWN];
    const direction = randomChoice(directions);
    
    const newPos = { ...gameState.position };
    
    if (direction.includes('left')) {
      newPos.x -= 1;
    } else {
      newPos.x += 1;
    }
    
    if (direction.includes('up')) {
      newPos.y -= 1;
    } else {
      newPos.y += 1;
    }
    
    return newPos;
  };

  const generateOpponentElements = (distance: number): OpponentElement[] => {
    const elements = getAllElements();
    // 从5个元素中随机选择3个
    const shuffled = [...elements].sort(() => Math.random() - 0.5);
    const selectedElements = shuffled.slice(0, 3);
    
    return selectedElements.map(type => {
      const quantity = generateOpponentQuantity(distance);
      return {
        type,
        quantity,
        revealed: false,
        originalQuantity: quantity
      };
    });
  };

  const startAdventure = () => {
    // 检查是否所有元素都至少有10个
    const elements = getAllElements();
    const hasLowElement = elements.some(element => gameState.elements[element] < 10);
    
    if (hasLowElement) {
      toast({
        title: '无法开始探险',
        description: '所有元素必须至少有10个才能开始探险，请先补充元素',
        variant: 'destructive'
      });
      return;
    }
    
    const newPosition = movePlayer();
    const distance = calculateDistance(newPosition);
    const opponents = generateOpponentElements(distance);
    
    setGameState(prev => ({
      ...prev,
      position: newPosition,
      isInBattle: true,
      opponentElements: opponents,
      battleRounds: [],
      selectedPlayerElement: null,
      currentRoundIndex: 0,
      battleResult: 'pending',
      canUseCurrency: false
    }));
    
    toast({
      title: '开始探险',
      description: `移动到坐标 (${newPosition.x}, ${newPosition.y})，距离出生点 ${distance} 步`
    });
  };

  const selectPlayerElement = (element: ElementType) => {
    if (gameState.elements[element] <= 0) {
      toast({
        title: '元素不足',
        description: `你没有足够的${ELEMENT_INFO[element]?.name || element}元素`,
        variant: 'destructive'
      });
      return;
    }
    
    setGameState(prev => ({
      ...prev,
      selectedPlayerElement: element
    }));
    
    toast({
      title: '选择元素',
      description: `你选择了 ${ELEMENT_INFO[element]?.icon} ${ELEMENT_INFO[element]?.name} 元素`
    });
  };

  const calculateBattleResult = (
    playerElement: ElementType,
    playerQuantity: number,
    opponentElement: OpponentElement
  ): { opponentQuantity: number; playerWin: boolean } => {
    const finalOpponentQuantity = applyCounterEffect(
      opponentElement.quantity,
      playerElement,
      opponentElement.type,
      gameState.skills
    );
    
    const playerWin = playerQuantity > finalOpponentQuantity;
    
    return { opponentQuantity: finalOpponentQuantity, playerWin };
  };

  const revealOpponentElement = (index: number) => {
    if (!gameState.selectedPlayerElement) {
      toast({
        title: '请先选择元素',
        description: '你需要先选择一个自己的元素',
        variant: 'destructive'
      });
      return;
    }
    
    const opponent = gameState.opponentElements[index];
    if (opponent.revealed) {
      toast({
        title: '已经揭示',
        description: '这个元素已经被揭示过了',
        variant: 'destructive'
      });
      return;
    }
    
    const playerQuantity = gameState.elements[gameState.selectedPlayerElement];
    const { opponentQuantity, playerWin } = calculateBattleResult(
      gameState.selectedPlayerElement,
      playerQuantity,
      opponent
    );
    
    const newRound: BattleRound = {
      playerElement: gameState.selectedPlayerElement,
      opponentElement: { ...opponent, quantity: opponentQuantity, revealed: true },
      playerQuantity,
      opponentQuantity,
      playerWin,
      index
    };
    
    const newBattleRounds = [...gameState.battleRounds, newRound];
    const newOpponentElements = [...gameState.opponentElements];
    newOpponentElements[index] = { ...opponent, revealed: true, quantity: opponentQuantity };
    
    setGameState(prev => ({
      ...prev,
      battleRounds: newBattleRounds,
      opponentElements: newOpponentElements,
      selectedPlayerElement: null,
      currentRoundIndex: prev.currentRoundIndex + 1
    }));
    
    if (newBattleRounds.length === 3) {
      const wins = newBattleRounds.filter(r => r.playerWin).length;
      const battleWon = wins >= 2;
      
      setGameState(prev => ({
        ...prev,
        battleResult: battleWon ? 'win' : 'lose',
        canUseCurrency: !battleWon
      }));
      
      if (battleWon) {
        handleBattleWin(newBattleRounds);
      } else {
        handleBattleLose(newBattleRounds, wins);
      }
    }
  };

  const handleBattleWin = (rounds: BattleRound[]) => {
    const distance = calculateDistance(gameState.position);
    const currencyAmount = generateCurrencyAmount(distance);
    const currencyType = Math.random() < 0.5 ? CurrencyType.YIN : CurrencyType.YANG;
    
    toast({
      title: '冒险成功！',
      description: `你赢得了战斗！${currencyAmount > 0 ? `获得 ${currencyAmount} ${currencyType === CurrencyType.YIN ? '阴' : '阳'}货币` : ''}`,
    });
    
    if (currencyAmount > 0) {
      setGameState(prev => ({
        ...prev,
        currency: {
          ...prev.currency,
          [currencyType]: prev.currency[currencyType] + currencyAmount
        }
      }));
    }
  };

  const handleBattleLose = (rounds: BattleRound[], wins: number) => {
    const lostElements: { [key in ElementType]?: number } = {};
    
    rounds.forEach(round => {
      if (!round.playerWin) {
        if (!lostElements[round.playerElement]) {
          lostElements[round.playerElement] = 0;
        }
        lostElements[round.playerElement]! += round.playerQuantity;
      }
    });
    
    const newElements = { ...gameState.elements };
    Object.entries(lostElements).forEach(([element, amount]) => {
      newElements[element as ElementType] = Math.max(0, newElements[element as ElementType] - amount);
    });
    
    setGameState(prev => ({
      ...prev,
      elements: newElements
    }));
    
    const lostElementsText = Object.entries(lostElements)
      .map(([element, amount]) => `${ELEMENT_INFO[element as ElementType]?.icon} ${ELEMENT_INFO[element as ElementType]?.name} ${amount}个`)
      .join('、');
    
    toast({
      title: '冒险失败',
      description: `你只赢了 ${wins} 局，失去了失败局次的元素：${lostElementsText}。可以使用货币扭转一局，或直接返回出生点。`,
      variant: 'destructive'
    });
  };

  const selectWinningRound = (roundIndex: number) => {
    const round = gameState.battleRounds[roundIndex];
    if (!round.playerWin) {
      toast({
        title: '无效选择',
        description: '只能选择你赢的局次',
        variant: 'destructive'
      });
      return;
    }
    
    const gainedElement = round.opponentElement.type;
    const gainedQuantity = round.opponentElement.quantity;
    
    setGameState(prev => ({
      ...prev,
      elements: {
        ...prev.elements,
        [gainedElement]: prev.elements[gainedElement] + gainedQuantity
      },
      isInBattle: false,
      battleResult: null
    }));
    
    toast({
      title: '获得元素',
      description: `你获得了 ${gainedQuantity} 个 ${ELEMENT_INFO[gainedElement]?.icon} ${ELEMENT_INFO[gainedElement]?.name} 元素`
    });
  };

  const useCurrencyToReverse = (roundIndex: number, currencyType: CurrencyType) => {
    const round = gameState.battleRounds[roundIndex];
    if (round.playerWin) {
      toast({
        title: '无需扭转',
        description: '这一局你已经赢了',
        variant: 'destructive'
      });
      return;
    }
    
    const deficit = round.opponentQuantity - round.playerQuantity;
    if (gameState.currency[currencyType] < deficit) {
      toast({
        title: '货币不足',
        description: `需要 ${deficit} ${currencyType === CurrencyType.YIN ? '阴' : '阳'}货币`,
        variant: 'destructive'
      });
      return;
    }
    
    const newBattleRounds = [...gameState.battleRounds];
    newBattleRounds[roundIndex] = { ...round, playerWin: true };
    
    const wins = newBattleRounds.filter(r => r.playerWin).length;
    const battleWon = wins >= 2;
    
    setGameState(prev => ({
      ...prev,
      battleRounds: newBattleRounds,
      currency: {
        ...prev.currency,
        [currencyType]: prev.currency[currencyType] - deficit
      },
      battleResult: battleWon ? 'win' : 'lose',
      canUseCurrency: false
    }));
    
    if (battleWon) {
      handleBattleWin(newBattleRounds);
    } else {
      toast({
        title: '扭转失败',
        description: '即使扭转这一局，你仍然没有赢得战斗',
        variant: 'destructive'
      });
    }
  };

  const returnToSpawn = () => {
    setGameState(prev => ({
      ...prev,
      position: INITIAL_POSITION,
      isInBattle: false,
      battleResult: null,
      canUseCurrency: false,
      battleRounds: [],
      opponentElements: [],
      selectedPlayerElement: null,
      currentRoundIndex: 0
    }));
    
    toast({
      title: '返回出生点',
      description: '你已返回坐标 (0, 0)'
    });
  };

  const replenishElements = () => {
    const newElements = { ...gameState.elements };
    let replenished = false;
    
    for (const element of getAllElements()) {
      if (newElements[element] < 10) {
        newElements[element] = 10;
        replenished = true;
      }
    }
    
    if (replenished) {
      setGameState(prev => ({
        ...prev,
        elements: newElements
      }));
      
      toast({
        title: 'NPC补充元素',
        description: '所有元素已补充至10个'
      });
    } else {
      toast({
        title: '无需补充',
        description: '所有元素都已充足'
      });
    }
  };

  const resetGame = () => {
    setGameState({
      position: INITIAL_POSITION,
      elements: INITIAL_ELEMENTS,
      currency: INITIAL_CURRENCY,
      skills: initializeSkills(),
      isInBattle: false,
      battleRounds: [],
      opponentElements: [],
      selectedPlayerElement: null,
      currentRoundIndex: 0,
      battleResult: null,
      canUseCurrency: false
    });
    
    toast({
      title: '游戏重置',
      description: '游戏已重置到初始状态'
    });
  };

  const upgradeSkill = (element: ElementType, upgradeType: 'yin' | 'yang') => {
    const skill = gameState.skills[element];
    const currentLevel = upgradeType === 'yin' ? skill.yinLevel : skill.yangLevel;
    const cost = calculateSkillUpgradeCost(currentLevel);
    
    if (gameState.elements[element] < cost.elementCost) {
      toast({
        title: '元素不足',
        description: `需要 ${cost.elementCost} 个 ${ELEMENT_INFO[element]?.icon} ${ELEMENT_INFO[element]?.name} 元素`,
        variant: 'destructive'
      });
      return;
    }
    
    const currencyType = upgradeType === 'yin' ? CurrencyType.YIN : CurrencyType.YANG;
    if (gameState.currency[currencyType] < cost.currencyCost) {
      toast({
        title: '货币不足',
        description: `需要 ${cost.currencyCost} 个${upgradeType === 'yin' ? '阴' : '阳'}货币`,
        variant: 'destructive'
      });
      return;
    }
    
    const newSkills = { ...gameState.skills };
    newSkills[element] = {
      ...skill,
      [upgradeType === 'yin' ? 'yinLevel' : 'yangLevel']: currentLevel + 1
    };
    
    const newElements = { ...gameState.elements };
    newElements[element] -= cost.elementCost;
    
    const newCurrency = { ...gameState.currency };
    newCurrency[currencyType] -= cost.currencyCost;
    
    setGameState(prev => ({
      ...prev,
      skills: newSkills,
      elements: newElements,
      currency: newCurrency
    }));
    
    toast({
      title: '技能升级成功',
      description: `${ELEMENT_INFO[element]?.icon} ${ELEMENT_INFO[element]?.name} 技能的${upgradeType === 'yin' ? '阴' : '阳'}属性提升至 ${currentLevel + 1} 级`
    });
  };

  const canStartAdventure = () => {
    const elements = getAllElements();
    return elements.every(element => gameState.elements[element] >= 10);
  };

  return (
    <GameContext.Provider
      value={{
        gameState,
        startAdventure,
        selectPlayerElement,
        revealOpponentElement,
        useCurrencyToReverse,
        returnToSpawn,
        replenishElements,
        resetGame,
        selectWinningRound,
        upgradeSkill,
        canStartAdventure
      }}
    >
      {children}
    </GameContext.Provider>
  );
}

export function useGame() {
  const context = useContext(GameContext);
  if (!context) {
    throw new Error('useGame 必须在 GameProvider 内部使用');
  }
  return context;
}
