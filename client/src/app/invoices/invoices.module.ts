import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { InvoicesRoutingModule } from './invoices-routing.module';
import { CustomersInvoicesHomeComponent } from './components/customers-invoices-home/customers-invoices-home.component';
import { SuppliersInvoicesHomeComponent } from './components/suppliers-invoices-home/suppliers-invoices-home.component';
import { AddCustomerInvoiceComponent } from './components/add-customer-invoice/add-customer-invoice.component';
import { AddSupplierInvoiceComponent } from './components/add-supplier-invoice/add-supplier-invoice.component';
import {SharedModule} from "../shared/shared.module";


@NgModule({
  declarations: [
    CustomersInvoicesHomeComponent,
    SuppliersInvoicesHomeComponent,
    AddCustomerInvoiceComponent,
    AddSupplierInvoiceComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    InvoicesRoutingModule
  ]
})
export class InvoicesModule { }
