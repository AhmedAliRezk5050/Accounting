import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {GeneralLedgerHomeComponent} from "./components/general-ledger-home/general-ledger-home.component";
import {SharedModule} from "../shared/shared.module";
import {NotFoundComponent} from "../shared/components/not-found/not-found.component";
import {authGuard} from "../guards/auth.guard";

const routes: Routes = [
  {
    path: 'general-ledger',
    component: GeneralLedgerHomeComponent,
    canActivate: [authGuard]
  },

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule, SharedModule]
})
export class GeneralLedgerRoutingModule { }
