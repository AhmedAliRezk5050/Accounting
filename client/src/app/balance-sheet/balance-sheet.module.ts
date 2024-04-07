import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { BalanceSheetRoutingModule } from './balance-sheet-routing.module';
import {BalanceSheetHomeComponent} from "./components/balance-sheet-home/balance-sheet-home.component";
import {SharedModule} from "../shared/shared.module";
import {TranslateModule} from "@ngx-translate/core";
import {BalanceSheetModalComponent} from "./components/balance-sheet-modal/balance-sheet-modal.component";
import { BalanceSheetListComponent } from './components/balance-sheet-list/balance-sheet-list.component';


@NgModule({
  declarations: [
    BalanceSheetHomeComponent,
    BalanceSheetModalComponent,
    BalanceSheetListComponent
  ],
  exports: [
    BalanceSheetModalComponent
  ],
  imports: [
    CommonModule,
    BalanceSheetRoutingModule,
    SharedModule,
    TranslateModule
  ]
})
export class BalanceSheetModule { }
