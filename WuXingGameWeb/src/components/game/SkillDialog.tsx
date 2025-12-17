import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { SkillPanel } from '@/components/skill/SkillPanel';

interface SkillDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function SkillDialog({ open, onOpenChange }: SkillDialogProps) {
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>技能系统</DialogTitle>
          <DialogDescription>
            使用元素和货币升级你的技能，提升战斗能力
          </DialogDescription>
        </DialogHeader>
        <SkillPanel />
      </DialogContent>
    </Dialog>
  );
}
