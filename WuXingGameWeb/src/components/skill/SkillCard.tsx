import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { ElementIcon } from '@/components/game/ElementIcon';
import { type Skill, type ElementType, CurrencyType } from '@/types/game';
import { calculateSkillUpgradeCost, ELEMENT_INFO } from '@/lib/game-utils';
import { TrendingUp, Shield, Sparkles } from 'lucide-react';

interface SkillCardProps {
  skill: Skill;
  elementQuantity: number;
  yinCurrency: number;
  yangCurrency: number;
  onUpgrade: (element: ElementType, upgradeType: 'yin' | 'yang') => void;
}

export function SkillCard({ skill, elementQuantity, yinCurrency, yangCurrency, onUpgrade }: SkillCardProps) {
  const elementInfo = ELEMENT_INFO[skill.element];
  const yinCost = calculateSkillUpgradeCost(skill.yinLevel);
  const yangCost = calculateSkillUpgradeCost(skill.yangLevel);
  
  const canUpgradeYin = elementQuantity >= yinCost.elementCost && yinCurrency >= yinCost.currencyCost;
  const canUpgradeYang = elementQuantity >= yangCost.elementCost && yangCurrency >= yangCost.currencyCost;
  
  const yinEffect = 50 + skill.yinLevel * 5;
  const yangEffect = 200 - skill.yangLevel * 10;
  
  return (
    <Card className="w-full">
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <ElementIcon type={skill.element} size="sm" showQuantity={false} />
          <span>{elementInfo.name}元素技能</span>
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        <div className="flex items-center justify-between p-3 bg-muted rounded-lg">
          <div className="flex items-center gap-2">
            <Sparkles className="w-4 h-4 text-primary" />
            <span className="text-sm font-medium">阴属性（克制加成）</span>
          </div>
          <span className="text-sm font-bold">Lv.{skill.yinLevel}</span>
        </div>
        
        <div className="pl-6 space-y-2">
          <div className="text-sm text-muted-foreground">
            克制效果：减少对手 <span className="font-bold text-primary">{yinEffect}%</span>
          </div>
          <div className="flex items-center gap-2">
            <Button
              size="sm"
              variant="outline"
              onClick={() => onUpgrade(skill.element, 'yin')}
              disabled={!canUpgradeYin}
              className="flex-1"
            >
              <TrendingUp className="w-4 h-4 mr-1" />
              升级
            </Button>
            <div className="text-xs text-muted-foreground">
              需要: {yinCost.elementCost} {elementInfo.icon} + {yinCost.currencyCost} 阴
            </div>
          </div>
        </div>
        
        <div className="flex items-center justify-between p-3 bg-muted rounded-lg">
          <div className="flex items-center gap-2">
            <Shield className="w-4 h-4 text-secondary" />
            <span className="text-sm font-medium">阳属性（抗性加成）</span>
          </div>
          <span className="text-sm font-bold">Lv.{skill.yangLevel}</span>
        </div>
        
        <div className="pl-6 space-y-2">
          <div className="text-sm text-muted-foreground">
            被克制效果：对手增加 <span className="font-bold text-secondary">{yangEffect}%</span>
          </div>
          <div className="flex items-center gap-2">
            <Button
              size="sm"
              variant="outline"
              onClick={() => onUpgrade(skill.element, 'yang')}
              disabled={!canUpgradeYang}
              className="flex-1"
            >
              <TrendingUp className="w-4 h-4 mr-1" />
              升级
            </Button>
            <div className="text-xs text-muted-foreground">
              需要: {yangCost.elementCost} {elementInfo.icon} + {yangCost.currencyCost} 阳
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  );
}
