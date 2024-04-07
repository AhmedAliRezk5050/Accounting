import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SuppliersRoutingModule } from './suppliers-routing.module';
import { SuppliersHomeComponent } from './components/suppliers-home/suppliers-home.component';
import { AddSupplierComponent } from './components/add-supplier/add-supplier.component';
import {SharedModule} from "../shared/shared.module";


@NgModule({
  declarations: [
    SuppliersHomeComponent,
    AddSupplierComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    SuppliersRoutingModule
  ]
})
export class SuppliersModule { }
