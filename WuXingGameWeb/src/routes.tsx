import GamePage from './pages/GamePage';
import type { ComponentType } from 'react';

interface RouteConfig {
  name: string;
  path: string;
  element: ComponentType;
  visible?: boolean;
}

const routes: RouteConfig[] = [
  {
    name: '五行探险记',
    path: '/',
    element: GamePage
  }
];

export default routes;
