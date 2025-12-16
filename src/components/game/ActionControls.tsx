import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { useGame } from '@/contexts/GameContext';
import { Compass, Home, RefreshCw, RotateCcw, Sparkles } from 'lucide-react';
import { useState } from 'react';
import { SkillDialog } from './SkillDialog';

export function ActionControls() {
  const { gameState, startAdventure, returnToSpawn, replenishElements, resetGame, canStartAdventure: checkCanStartAdventure } = useGame();
  const { isInBattle, position, battleResult } = gameState;
  const [skillDialogOpen, setSkillDialogOpen] = useState(false);
  
  const isAtSpawn = position.x === 0 && position.y === 0;
  const canStartAdventureNow = !isInBattle && checkCanStartAdventure();
  const canReturnToSpawn = !isAtSpawn && !isInBattle;
  const needsReplenishment = !checkCanStartAdventure();
  
  return (
    <>
      <Card className="w-full">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Compass className="w-5 h-5" />
            操作面板
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-3">
          <Button
            className="w-full"
            size="lg"
            onClick={startAdventure}
            disabled={!canStartAdventureNow}
          >
            <Compass className="w-4 h-4 mr-2" />
            开始探险
          </Button>
          
          {needsReplenishment && !isInBattle && (
            <div className="text-xs text-destructive bg-destructive/10 p-2 rounded-md">
              ⚠️ 所有元素必须至少有10个才能开始探险
            </div>
          )}
          
          <Button
            className="w-full"
            size="lg"
            variant="secondary"
            onClick={() => setSkillDialogOpen(true)}
          >
            <Sparkles className="w-4 h-4 mr-2" />
            技能系统
          </Button>
          
          {isAtSpawn && (
            <Button
              className="w-full"
              size="lg"
              variant="secondary"
              onClick={replenishElements}
            >
              <RefreshCw className="w-4 h-4 mr-2" />
              NPC补充元素
            </Button>
          )}
          
          {canReturnToSpawn && (
            <Button
              className="w-full"
              size="lg"
              variant="outline"
              onClick={returnToSpawn}
            >
              <Home className="w-4 h-4 mr-2" />
              返回出生点
            </Button>
          )}
          
          <div className="pt-4 border-t border-border">
            <Button
              className="w-full"
              size="sm"
              variant="destructive"
              onClick={resetGame}
            >
              <RotateCcw className="w-4 h-4 mr-2" />
              重置游戏
            </Button>
          </div>
          
          {isInBattle && battleResult === 'pending' && (
            <div className="mt-4 p-3 bg-muted rounded-lg text-sm space-y-2">
              <p className="font-semibold">游戏提示：</p>
              <ul className="list-disc list-inside space-y-1 text-muted-foreground">
                <li>金克木，木克土</li>
                <li>土克水，水克火</li>
                <li>火克金</li>
                <li>克制关系会影响对手数量</li>
                <li>需要赢2局以上才能成功</li>
              </ul>
            </div>
          )}
        </CardContent>
      </Card>
      
      <SkillDialog open={skillDialogOpen} onOpenChange={setSkillDialogOpen} />
    </>
  );
}
