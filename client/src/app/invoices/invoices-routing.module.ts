import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {authGuard} from "../guards/auth.guard";
import {CustomersInvoicesHomeComponent} from "./components/customers-invoices-home/customers-invoices-home.component";
import {SuppliersInvoicesHomeComponent} from "./components/suppliers-invoices-home/suppliers-invoices-home.component";
import {AddCustomerInvoiceComponent} from "./components/add-customer-invoice/add-customer-invoice.component";
import {AddSupplierInvoiceComponent} from "./components/add-supplier-invoice/add-supplier-invoice.component";

const routes: Routes = [
  {
    path: 'invoices/customers',
    component: CustomersInvoicesHomeComponent,
    canActivate: [authGuard]
  },
  {
    path: 'invoices/suppliers',
    component: SuppliersInvoicesHomeComponent,
    canActivate: [authGuard]
  },
  {
    path: 'invoices/customers/add-invoice',
    component: AddCustomerInvoiceComponent,
    canActivate: [authGuard]
  },
  {
    path: 'invoices/suppliers/add-invoice',
    component: AddSupplierInvoiceComponent,
    canActivate: [authGuard]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class InvoicesRoutingModule { }
