import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {ChartOfAccountsHomeComponent} from "./components/chart-of-accounts-home/chart-of-accounts-home.component";
import {NotFoundComponent} from "../shared/components/not-found/not-found.component";
import {BanksComponent} from "./components/banks/banks.component";
import {CustomersComponent} from "./components/customers/customers.component";
import {SuppliersComponent} from "./components/suppliers/suppliers.component";
import {authGuard} from "../guards/auth.guard";

const routes: Routes = [
  {
    path: 'coa',
    component: ChartOfAccountsHomeComponent,
    canActivate: [authGuard]
  },
  {
    path: 'banks',
    component: BanksComponent,
    canActivate: [authGuard]
  },
  // {
  //   path: 'customers',
  //   component: CustomersComponent,
  //   canActivate: [authGuard]
  // },
  // {
  //   path: 'suppliers',
  //   component: SuppliersComponent,
  //   canActivate: [authGuard]
  // },

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class ChartOfAccountsRoutingModule { }
