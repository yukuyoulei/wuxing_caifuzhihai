import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { ElementIcon } from './ElementIcon';
import { useGame } from '@/contexts/GameContext';
import { getAllElements } from '@/lib/game-utils';
import { CurrencyType } from '@/types/game';
import { MapPin, Coins } from 'lucide-react';

export function PlayerStatus() {
  const { gameState } = useGame();
  const { position, elements, currency } = gameState;
  
  return (
    <Card className="w-full">
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <MapPin className="w-5 h-5" />
          玩家状态
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-6">
        <div>
          <h3 className="text-sm font-semibold mb-2 text-muted-foreground">当前位置</h3>
          <div className="text-2xl font-bold">
            ({position.x}, {position.y})
          </div>
          <div className="text-sm text-muted-foreground mt-1">
            距离出生点: {Math.abs(position.x) + Math.abs(position.y)} 步
          </div>
        </div>
        
        <div>
          <h3 className="text-sm font-semibold mb-3 text-muted-foreground">元素库存</h3>
          <div className="grid grid-cols-2 gap-4">
            {getAllElements().map(element => (
              <ElementIcon
                key={element}
                type={element}
                quantity={elements[element]}
                size="sm"
                showQuantity
              />
            ))}
          </div>
        </div>
        
        <div>
          <h3 className="text-sm font-semibold mb-3 text-muted-foreground flex items-center gap-2">
            <Coins className="w-4 h-4" />
            货币
          </h3>
          <div className="space-y-2">
            <div className="flex items-center justify-between p-2 rounded-lg bg-muted">
              <span className="font-medium" style={{ color: 'hsl(270 50% 60%)' }}>
                阴 ☯
              </span>
              <span className="font-bold">
                {currency[CurrencyType.YIN]}
              </span>
            </div>
            <div className="flex items-center justify-between p-2 rounded-lg bg-muted">
              <span className="font-medium" style={{ color: 'hsl(30 100% 60%)' }}>
                阳 ☯
              </span>
              <span className="font-bold">
                {currency[CurrencyType.YANG]}
              </span>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  );
}
