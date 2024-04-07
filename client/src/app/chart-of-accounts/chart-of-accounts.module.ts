import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {TreeContainerComponent} from "./components/tree-container/tree-container.component";
import { ChartOfAccountsHomeComponent } from './components/chart-of-accounts-home/chart-of-accounts-home.component';
import {NodeDirective} from "../shared/directives/node.directive";
import {ChartOfAccountsRoutingModule} from "./chart-of-accounts-routing.module";
import {SharedModule} from "../shared/shared.module";
import { AddAccountModalComponent } from './components/add-account-modal/add-account-modal.component';
import {ReactiveFormsModule} from "@angular/forms";
import { AccountSelectorComponent } from './components/account-selector/account-selector.component';
import { AccountDetailsModalComponent } from './components/account-details-modal/account-details-modal.component';
import { DeleteAccountModalComponent } from './components/delete-account-modal/delete-account-modal.component';
import { EditAccountModalComponent } from './components/edit-account-modal/edit-account-modal.component';
import {TreeNodeComponent} from "./components/tree-node/tree-node.component";
import { AccountsListComponent } from './components/accounts-list/accounts-list.component';
import { AccountNodeComponent } from './components/account-node/account-node.component';
import {BanksComponent} from "./components/banks/banks.component";
import { CustomersComponent } from './components/customers/customers.component';
import { SuppliersComponent } from './components/suppliers/suppliers.component';

@NgModule({
  declarations: [
    ChartOfAccountsHomeComponent,
    AddAccountModalComponent,
    AccountSelectorComponent,
    AccountDetailsModalComponent,
    DeleteAccountModalComponent,
    EditAccountModalComponent,
    TreeNodeComponent,
    TreeContainerComponent,
    AccountsListComponent,
    AccountNodeComponent,
    BanksComponent,
    CustomersComponent,
    SuppliersComponent
  ],
    exports: [
        AddAccountModalComponent,
        AccountDetailsModalComponent,
        DeleteAccountModalComponent,
        EditAccountModalComponent
    ],
  imports: [
    CommonModule,
    SharedModule,
    ChartOfAccountsRoutingModule,
    ReactiveFormsModule
  ]
})
export class ChartOfAccountsModule {}
