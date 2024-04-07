import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { IncomeStatementRoutingModule } from './income-statement-routing.module';
import { IncomeStatementHomeComponent } from './components/income-statement-home/income-statement-home.component';
import {SharedModule} from "../shared/shared.module";
import {ReactiveFormsModule} from "@angular/forms";
import { IncomeStatementModalComponent } from './components/income-statement-modal/income-statement-modal.component';
import { IncomeStatementListComponent } from './components/income-statement-list/income-statement-list.component';


@NgModule({
  declarations: [
    IncomeStatementHomeComponent,
    IncomeStatementModalComponent,
    IncomeStatementListComponent,
  ],
  imports: [
    CommonModule,
    IncomeStatementRoutingModule,
    SharedModule,
    ReactiveFormsModule
  ],
  exports: [
    IncomeStatementModalComponent
  ]
})
export class IncomeStatementModule { }
