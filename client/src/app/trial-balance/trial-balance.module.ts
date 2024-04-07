import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TrialBalanceRoutingModule } from './trial-balance-routing.module';
import { TrialBalanceHomeComponent } from './components/trial-balance-home/trial-balance-home.component';
import {SharedModule} from "../shared/shared.module";
import { TrialBalanceModalComponent } from './components/trial-balance-modal/trial-balance-modal.component';
import {ReactiveFormsModule} from "@angular/forms";
import {TrialBalanceListComponent} from "./components/trial-balance-list/trial-balance-list.component";


@NgModule({
  declarations: [
    TrialBalanceHomeComponent,
    TrialBalanceModalComponent,
    TrialBalanceListComponent
  ],
  exports: [
    TrialBalanceModalComponent
  ],
  imports: [
    CommonModule,
    TrialBalanceRoutingModule,
    SharedModule,
    ReactiveFormsModule
  ]
})
export class TrialBalanceModule { }
