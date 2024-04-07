import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';

import {CustomersRoutingModule} from './customers-routing.module';
import {CustomersHomeComponent} from './components/customers-home/customers-home.component';
import {FontAwesomeModule} from "@fortawesome/angular-fontawesome";
import {TranslateModule} from "@ngx-translate/core";
import {AddCustomerComponent} from './components/add-customer/add-customer.component';
import {SharedModule} from "../shared/shared.module";
import {NodeDirective} from "../shared/directives/node.directive";


@NgModule({
  declarations: [
    CustomersHomeComponent,
    AddCustomerComponent,
  ],
  imports: [
    CommonModule,
    SharedModule,
    FontAwesomeModule,
    TranslateModule,
    CustomersRoutingModule,
  ]
})
export class CustomersModule {
}
