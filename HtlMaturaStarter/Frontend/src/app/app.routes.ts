import { Routes } from '@angular/router';
import { MatchImport } from './match-import/match-import';
import { MatchDetails } from './match-details/match-details';
import { MatchList } from './match-list/match-list';

export const routes: Routes = [
  { path: '', component: MatchImport },
  { path: 'match-list', component: MatchList },
  { path: 'match-details/:id', component: MatchDetails },
  { path: '**', redirectTo: '' }
];
