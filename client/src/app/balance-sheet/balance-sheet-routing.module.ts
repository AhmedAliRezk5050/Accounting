import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {BalanceSheetHomeComponent} from "./components/balance-sheet-home/balance-sheet-home.component";
import {NotFoundComponent} from "../shared/components/not-found/not-found.component";
import {authGuard} from "../guards/auth.guard";

const routes: Routes = [
  {
    path: 'balance-sheet',
    component: BalanceSheetHomeComponent,
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
export class BalanceSheetRoutingModule { }
