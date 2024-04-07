import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {IncomeStatementHomeComponent} from "./components/income-statement-home/income-statement-home.component";
import {NotFoundComponent} from "../shared/components/not-found/not-found.component";
import {authGuard} from "../guards/auth.guard";

const routes: Routes = [
  {
    path: 'income-statement',
    component: IncomeStatementHomeComponent,
    canActivate: [authGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class IncomeStatementRoutingModule { }
