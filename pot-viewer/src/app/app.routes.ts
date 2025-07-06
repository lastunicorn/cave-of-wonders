import { Routes } from '@angular/router';
import { PotList } from './components/pot-list/pot-list';

export const routes: Routes = [
  { path: '', component: PotList },
  { path: 'pots', component: PotList },
  { path: '**', redirectTo: '' }
];
