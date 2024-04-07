import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {authGuard} from "../guards/auth.guard";
import {DataManagementHomeComponent} from "./components/data-management-home/data-management-home.component";

const routes: Routes = [
  {
    path: 'data-management',
    component: DataManagementHomeComponent,
    canActivate: [authGuard]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DataManagementRoutingModule { }
