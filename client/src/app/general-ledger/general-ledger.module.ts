import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { GeneralLedgerRoutingModule } from './general-ledger-routing.module';
import { GeneralLedgerHomeComponent } from './components/general-ledger-home/general-ledger-home.component';
import { GeneralLedgerEntriesList } from './components/general-ledger-entries-list/general-ledger-entries-list';
import { GeneralLedgerModalComponent } from './components/general-ledger-modal/general-ledger-modal.component';
import {ReactiveFormsModule} from "@angular/forms";


@NgModule({
  declarations: [
    GeneralLedgerHomeComponent,
    GeneralLedgerEntriesList,
    GeneralLedgerModalComponent
  ],
  exports: [
    GeneralLedgerModalComponent
  ],
  imports: [
    CommonModule,
    GeneralLedgerRoutingModule,
    ReactiveFormsModule
  ]
})
export class GeneralLedgerModule { }
