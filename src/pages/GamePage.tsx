import { GameProvider, useGame } from '@/contexts/GameContext';
import { PlayerStatus } from '@/components/game/PlayerStatus';
import { BattleArea } from '@/components/game/BattleArea';
import { ActionControls } from '@/components/game/ActionControls';
import { Toaster } from '@/components/ui/toaster';

function GameContent() {
  const { gameState } = useGame();
  const { position, isInBattle } = gameState;
  
  // 使用位置和战斗状态作为key，触发场景切换动画
  const sceneKey = `${position.x}-${position.y}-${isInBattle}`;
  
  return (
    <div className="min-h-screen bg-background flex items-center justify-center p-4">
      <div className="w-full max-w-[1400px] mx-auto">
        <div className="space-y-6">
          <header className="text-center space-y-2">
            <h1 className="text-4xl xl:text-5xl font-bold bg-gradient-to-r from-primary via-secondary to-accent bg-clip-text text-transparent">
              五行探险记
            </h1>
            <p className="text-muted-foreground">
              基于五行相克原理的冒险游戏
            </p>
          </header>
          
          <div 
            key={sceneKey}
            className="grid grid-cols-1 xl:grid-cols-12 gap-6 animate-in fade-in slide-in-from-bottom-4 duration-500"
          >
            <div className="xl:col-span-3">
              <PlayerStatus />
            </div>
            
            <div className="xl:col-span-6">
              <BattleArea />
            </div>
            
            <div className="xl:col-span-3">
              <ActionControls />
            </div>
          </div>
          
          <footer className="text-center text-sm text-muted-foreground pt-4">
            <p>© 2025 五行探险记</p>
          </footer>
        </div>
      </div>
      <Toaster />
    </div>
  );
}

export default function GamePage() {
  return (
    <GameProvider>
      <GameContent />
    </GameProvider>
  );
}
