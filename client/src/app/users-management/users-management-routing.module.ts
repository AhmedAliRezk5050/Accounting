import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {UsersComponent} from "./components/users/users.component";
import {authGuard} from "../guards/auth.guard";
import {ManagePermissionsComponent} from "./components/manage-permissions/manage-permissions.component";

const routes: Routes = [
  {
    path: 'users-management/users',
    component: UsersComponent,
    canActivate: [authGuard]
  },
  {
    path: 'users-management/users/manage-permissions/:id',
    component: ManagePermissionsComponent,
    canActivate: [authGuard]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UsersManagementRoutingModule {
}
