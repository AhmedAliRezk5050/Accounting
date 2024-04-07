import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {authGuard} from "../guards/auth.guard";
import {NotFoundComponent} from "../shared/components/not-found/not-found.component";
import {CustomersHomeComponent} from "./components/customers-home/customers-home.component";
import {AddCustomerComponent} from "./components/add-customer/add-customer.component";

const routes: Routes = [
  {
    path: 'customers',
    component: CustomersHomeComponent,
    canActivate: [authGuard]
  },
  {
    path: 'add-customer',
    component: AddCustomerComponent,
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
export class CustomersRoutingModule { }
