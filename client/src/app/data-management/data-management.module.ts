import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DataManagementRoutingModule } from './data-management-routing.module';
import { DataManagementHomeComponent } from './components/data-management-home/data-management-home.component';
import {FontAwesomeModule} from "@fortawesome/angular-fontawesome";
import {SharedModule} from "../shared/shared.module";
import {TranslateModule} from "@ngx-translate/core";


@NgModule({
  declarations: [
    DataManagementHomeComponent
  ],
    imports: [
        CommonModule,
        DataManagementRoutingModule,
        FontAwesomeModule,
        SharedModule,
        TranslateModule
    ]
})
export class DataManagementModule { }
