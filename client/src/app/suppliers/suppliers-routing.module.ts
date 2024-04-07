import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {CustomersHomeComponent} from "../customers/components/customers-home/customers-home.component";
import {authGuard} from "../guards/auth.guard";
import {AddCustomerComponent} from "../customers/components/add-customer/add-customer.component";
import {NotFoundComponent} from "../shared/components/not-found/not-found.component";
import {SuppliersHomeComponent} from "./components/suppliers-home/suppliers-home.component";
import {AddSupplierComponent} from "./components/add-supplier/add-supplier.component";

const routes: Routes = [
  {
    path: 'suppliers',
    component: SuppliersHomeComponent,
    canActivate: [authGuard]
  },
  {
    path: 'add-supplier',
    component: AddSupplierComponent,
    canActivate: [authGuard]
  },
  {
    path: '404',
    component: NotFoundComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SuppliersRoutingModule { }
