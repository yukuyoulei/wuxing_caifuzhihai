import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { SkillCard } from './SkillCard';
import { useGame } from '@/contexts/GameContext';
import { getAllElements } from '@/lib/game-utils';
import { Zap } from 'lucide-react';
import { CurrencyType } from '@/types/game';

export function SkillPanel() {
  const { gameState, upgradeSkill } = useGame();
  const elements = getAllElements();
  
  return (
    <Card className="w-full">
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <Zap className="w-5 h-5" />
          技能系统
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        <div className="grid grid-cols-1 gap-4">
          {elements.map(element => (
            <SkillCard
              key={element}
              skill={gameState.skills[element]}
              elementQuantity={gameState.elements[element]}
              yinCurrency={gameState.currency[CurrencyType.YIN]}
              yangCurrency={gameState.currency[CurrencyType.YANG]}
              onUpgrade={upgradeSkill}
            />
          ))}
        </div>
        
        <div className="p-4 bg-muted rounded-lg space-y-2">
          <h4 className="font-semibold text-sm">技能说明</h4>
          <ul className="text-xs text-muted-foreground space-y-1">
            <li>• 阴属性：增强克制效果，让对手元素减少更多</li>
            <li>• 阳属性：减免被克制效果，让对手元素增加更少</li>
            <li>• 升级需要消耗对应元素和阴阳货币</li>
            <li>• 等级越高，升级成本越高</li>
          </ul>
        </div>
      </CardContent>
    </Card>
  );
}
