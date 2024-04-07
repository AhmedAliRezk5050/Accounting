import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {TrialBalanceHomeComponent} from "./components/trial-balance-home/trial-balance-home.component";
import {NotFoundComponent} from "../shared/components/not-found/not-found.component";
import {authGuard} from "../guards/auth.guard";

const routes: Routes = [
  {
    path: 'trial-balance',
    component: TrialBalanceHomeComponent,
    canActivate: [authGuard]
  },
  {
    path: '404',
    component: NotFoundComponent
  },

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TrialBalanceRoutingModule { }
