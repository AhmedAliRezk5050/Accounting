import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {EntriesRoutingModule} from "./entries/entries-routing.module";
import {GeneralLedgerRoutingModule} from "./general-ledger/general-ledger-routing.module";
import {TrialBalanceRoutingModule} from "./trial-balance/trial-balance-routing.module";
import {IncomeStatementRoutingModule} from "./income-statement/income-statement-routing.module";
import {BalanceSheetRoutingModule} from "./balance-sheet/balance-sheet-routing.module";
import {ChartOfAccountsRoutingModule} from "./chart-of-accounts/chart-of-accounts-routing.module";
import {CustomersRoutingModule} from "./customers/customers-routing.module";
import {SuppliersRoutingModule} from "./suppliers/suppliers-routing.module";

const routes: Routes = [
  {
    path: 'coa',
    loadChildren: () => ChartOfAccountsRoutingModule,
  },
  {
    path: 'entries',
    loadChildren: () => EntriesRoutingModule,
  },
  {
    path: 'general-ledger',
    loadChildren: () => GeneralLedgerRoutingModule,
  },
  {
    path: 'trial-balance',
    loadChildren: () => TrialBalanceRoutingModule,
  },
  {
    path: 'income-statement',
    loadChildren: () => IncomeStatementRoutingModule,
  },
  {
    path: 'balance-sheet',
    loadChildren: () => BalanceSheetRoutingModule,
  },
  {
    path: 'customers',
    loadChildren: () => CustomersRoutingModule,
  },
  {
    path: 'suppliers',
    loadChildren: () => SuppliersRoutingModule,
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
