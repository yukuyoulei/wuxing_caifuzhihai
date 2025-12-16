import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { ElementIcon } from './ElementIcon';
import { useGame } from '@/contexts/GameContext';
import { getAllElements, ELEMENT_INFO } from '@/lib/game-utils';
import { Swords, HelpCircle, Trophy, XCircle } from 'lucide-react';
import { CurrencyType } from '@/types/game';

export function BattleArea() {
  const { gameState, selectPlayerElement, revealOpponentElement, selectWinningRound, useCurrencyToReverse, returnToSpawn } = useGame();
  const { 
    isInBattle, 
    selectedPlayerElement, 
    opponentElements, 
    battleRounds, 
    elements,
    battleResult,
    canUseCurrency,
    currency
  } = gameState;
  
  if (!isInBattle) {
    return (
      <Card className="w-full h-full flex items-center justify-center">
        <CardContent className="text-center py-12">
          <Swords className="w-16 h-16 mx-auto mb-4 text-muted-foreground" />
          <p className="text-xl text-muted-foreground">
            点击"开始探险"进入战斗
          </p>
        </CardContent>
      </Card>
    );
  }
  
  const wins = battleRounds.filter(r => r.playerWin).length;
  const losses = battleRounds.filter(r => !r.playerWin).length;
  
  return (
    <Card className="w-full">
      <CardHeader>
        <CardTitle className="flex items-center justify-between">
          <span className="flex items-center gap-2">
            <Swords className="w-5 h-5" />
            战斗区域
          </span>
          <span className="text-sm font-normal">
            胜: <span className="text-secondary">{wins}</span> / 
            负: <span className="text-destructive">{losses}</span>
          </span>
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-6">
        {battleResult === 'pending' && (
          <>
            <div>
              <h3 className="text-sm font-semibold mb-3 text-muted-foreground">
                1. 选择你的元素
              </h3>
              <div className="flex gap-4 justify-center flex-wrap">
                {getAllElements().map(element => (
                  <ElementIcon
                    key={element}
                    type={element}
                    quantity={elements[element]}
                    size="md"
                    showQuantity
                    onClick={() => selectPlayerElement(element)}
                    selected={selectedPlayerElement === element}
                    disabled={elements[element] <= 0}
                  />
                ))}
              </div>
            </div>
            
            <div>
              <h3 className="text-sm font-semibold mb-3 text-muted-foreground">
                2. 点击对手元素进行对决
              </h3>
              <div className="flex gap-4 justify-center flex-wrap">
                {opponentElements.map((opponent, index) => (
                  <div key={index} className="relative">
                    {opponent.revealed ? (
                      <ElementIcon
                        type={opponent.type}
                        quantity={opponent.quantity}
                        size="md"
                        showQuantity
                        disabled
                      />
                    ) : (
                      <div
                        className="w-16 h-16 rounded-full border-2 border-muted flex items-center justify-center cursor-pointer hover:scale-110 transition-all duration-300 hover:border-primary"
                        onClick={() => revealOpponentElement(index)}
                      >
                        <HelpCircle className="w-8 h-8 text-muted-foreground" />
                      </div>
                    )}
                  </div>
                ))}
              </div>
            </div>
          </>
        )}
        
        {battleRounds.length > 0 && (
          <div>
            <h3 className="text-sm font-semibold mb-3 text-muted-foreground">
              战斗记录
            </h3>
            <div className="space-y-2">
              {battleRounds.map((round, index) => (
                <div
                  key={index}
                  className={`p-3 rounded-lg border-2 transition-all duration-300 animate-in fade-in slide-in-from-left-4 ${
                    round.playerWin 
                      ? 'border-secondary bg-secondary/10 shadow-[0_0_15px_rgba(var(--secondary),0.3)]' 
                      : 'border-destructive bg-destructive/10'
                  }`}
                  style={{ animationDelay: `${index * 100}ms` }}
                >
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-2">
                      {round.playerWin ? (
                        <Trophy className="w-4 h-4 text-secondary animate-pulse" />
                      ) : (
                        <XCircle className="w-4 h-4 text-destructive" />
                      )}
                      <span className="font-semibold">
                        第 {index + 1} 局
                      </span>
                    </div>
                    <div className="text-sm flex items-center gap-2">
                      <span className="flex items-center gap-1">
                        你的 {ELEMENT_INFO[round.playerElement]?.icon} {ELEMENT_INFO[round.playerElement]?.name} × {round.playerQuantity}
                      </span>
                      <span>vs</span>
                      <span className="flex items-center gap-1">
                        对手 {ELEMENT_INFO[round.opponentElement.type]?.icon} {ELEMENT_INFO[round.opponentElement.type]?.name} × {round.opponentQuantity}
                      </span>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}
        
        {battleResult === 'win' && (
          <div className="space-y-4 animate-in fade-in zoom-in-95 duration-500">
            <div className="text-center p-6 bg-gradient-to-br from-secondary/30 to-secondary/10 rounded-lg border-2 border-secondary shadow-[0_0_30px_rgba(var(--secondary),0.4)] relative overflow-hidden">
              <div className="absolute inset-0 bg-gradient-to-r from-transparent via-secondary/20 to-transparent animate-shimmer" />
              <Trophy className="w-16 h-16 mx-auto mb-3 text-secondary animate-bounce" />
              <p className="text-2xl font-bold text-secondary mb-1">🎉 冒险成功！🎉</p>
              <p className="text-sm text-muted-foreground mt-2">
                选择一个你赢的局次获取对应元素
              </p>
            </div>
            <div className="flex gap-2 justify-center flex-wrap">
              {battleRounds.map((round, index) => (
                round.playerWin && (
                  <Button
                    key={index}
                    onClick={() => selectWinningRound(index)}
                    variant="outline"
                    className="border-secondary hover:bg-secondary/20 hover:scale-105 transition-all duration-200 hover:shadow-lg"
                  >
                    第 {index + 1} 局: {ELEMENT_INFO[round.opponentElement.type]?.icon} {ELEMENT_INFO[round.opponentElement.type]?.name} × {round.opponentQuantity}
                  </Button>
                )
              ))}
            </div>
          </div>
        )}
        
        {battleResult === 'lose' && canUseCurrency && (
          <div className="space-y-4 animate-in fade-in zoom-in-95 duration-500">
            <div className="text-center p-6 bg-gradient-to-br from-destructive/30 to-destructive/10 rounded-lg border-2 border-destructive shadow-lg">
              <XCircle className="w-16 h-16 mx-auto mb-3 text-destructive animate-pulse" />
              <p className="text-2xl font-bold text-destructive mb-1">💔 冒险失败</p>
              <p className="text-sm text-muted-foreground mt-2">
                使用货币扭转一局结果（只能扭转一次）
              </p>
            </div>
            <div className="space-y-2">
              {battleRounds.map((round, index) => (
                !round.playerWin && (
                  <div key={index} className="flex items-center justify-between p-3 bg-muted rounded-lg">
                    <span className="text-sm">
                      第 {index + 1} 局 - 需要 {round.opponentQuantity - round.playerQuantity} 货币
                    </span>
                    <div className="flex gap-2">
                      <Button
                        size="sm"
                        variant="outline"
                        onClick={() => useCurrencyToReverse(index, CurrencyType.YIN)}
                        disabled={currency[CurrencyType.YIN] < (round.opponentQuantity - round.playerQuantity)}
                      >
                        使用阴 ({currency[CurrencyType.YIN]})
                      </Button>
                      <Button
                        size="sm"
                        variant="outline"
                        onClick={() => useCurrencyToReverse(index, CurrencyType.YANG)}
                        disabled={currency[CurrencyType.YANG] < (round.opponentQuantity - round.playerQuantity)}
                      >
                        使用阳 ({currency[CurrencyType.YANG]})
                      </Button>
                    </div>
                  </div>
                )
              ))}
            </div>
            
            <div className="mt-4 pt-4 border-t">
              <Button
                className="w-full"
                variant="secondary"
                onClick={returnToSpawn}
              >
                直接返回出生点（不使用货币）
              </Button>
            </div>
          </div>
        )}
      </CardContent>
    </Card>
  );
}
