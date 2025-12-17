import { ElementType } from '@/types/game';
import { ELEMENT_INFO } from '@/lib/game-utils';
import { cn } from '@/lib/utils';

interface ElementIconProps {
  type: ElementType;
  quantity?: number;
  size?: 'sm' | 'md' | 'lg';
  showQuantity?: boolean;
  className?: string;
  onClick?: () => void;
  selected?: boolean;
  disabled?: boolean;
}

export function ElementIcon({
  type,
  quantity = 0,
  size = 'md',
  showQuantity = true,
  className,
  onClick,
  selected = false,
  disabled = false
}: ElementIconProps) {
  const info = ELEMENT_INFO[type];
  
  // 安全检查：如果元素信息不存在，返回空
  if (!info) {
    console.error(`无效的元素类型: ${type}`);
    return null;
  }
  
  const sizeClasses = {
    sm: 'w-12 h-12 text-xl',
    md: 'w-16 h-16 text-2xl',
    lg: 'w-24 h-24 text-4xl'
  };
  
  const elementColorClass = `element-${type}`;
  const borderColorClass = `border-element-${type}`;
  
  return (
    <div
      className={cn(
        'relative flex flex-col items-center gap-1 transition-all duration-300',
        onClick && !disabled && 'cursor-pointer hover:scale-110',
        disabled && 'opacity-50 cursor-not-allowed',
        className
      )}
      onClick={!disabled ? onClick : undefined}
    >
      <div
        className={cn(
          'rounded-full border-2 flex items-center justify-center transition-all duration-300',
          sizeClasses[size],
          borderColorClass,
          selected && 'ring-4 ring-offset-2 ring-offset-background scale-110',
          onClick && !disabled && 'hover:shadow-lg'
        )}
        style={{
          boxShadow: selected ? `0 0 20px ${info.color}` : undefined
        }}
      >
        <span className={cn('select-none', elementColorClass)}>
          {info.icon}
        </span>
      </div>
      
      <div className="text-center">
        <div className={cn('font-bold', elementColorClass)}>
          {info.name}
        </div>
        {showQuantity && (
          <div className="text-sm text-muted-foreground">
            × {quantity}
          </div>
        )}
      </div>
    </div>
  );
}
