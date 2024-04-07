import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UsersManagementRoutingModule } from './users-management-routing.module';
import { UsersComponent } from './components/users/users.component';
import {TranslateModule} from "@ngx-translate/core";
import {FontAwesomeModule} from "@fortawesome/angular-fontawesome";
import { ManagePermissionsComponent } from './components/manage-permissions/manage-permissions.component';
import {SharedModule} from "../shared/shared.module";


@NgModule({
  declarations: [
    UsersComponent,
    ManagePermissionsComponent
  ],
  imports: [
    CommonModule,
    UsersManagementRoutingModule,
    TranslateModule,
    FontAwesomeModule,
    SharedModule
  ]
})
export class UsersManagementModule { }
