import {Component, OnInit} from '@angular/core';
import {UserDetailsModel} from "../../models/user-details.model";
import {ActivatedRoute, Router} from "@angular/router";
import {UsersManagementService} from "../../services/users-management.service";
import {FormArray, FormBuilder, FormGroup} from "@angular/forms";
import {of, switchMap, tap} from "rxjs";
import PermissionStatusModel from "../../models/permission-status.model";

@Component({
  selector: 'app-manage-permissions',
  templateUrl: './manage-permissions.component.html',
  styleUrls: ['./manage-permissions.component.scss']
})
export class ManagePermissionsComponent implements OnInit {
  user?: UserDetailsModel;

  permissionsForm!: FormGroup;

  constructor(
    private usersManagementService: UsersManagementService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private router: Router
  ) {
  }

  ngOnInit(): void {
    this.permissionsForm = this.fb.group({
      permissions: new FormArray([])
    });

    this.route.paramMap.pipe(
      switchMap(params => {
        const userId = params.get('id');
        if (userId) {
          return this.usersManagementService.getUser(userId);
        }
        return of(null);  // of is from 'rxjs'
      }),
      tap(fetchedUser => {
        if (fetchedUser) {
          this.user = fetchedUser;
        }
      }),
      switchMap(fetchedUser => {
        if (fetchedUser) {
          return this.usersManagementService.getAllPermissions();
        }
        return of(null);
      })
    ).subscribe(allPermissions => {
      if (allPermissions && this.user) {
        const updatedPermissions = this.usersManagementService.updatePermissionStatus(allPermissions, this.user.claimDetailsList);
        this.setPermissions(updatedPermissions);
      }
    });
  }

  setPermissions(permissions: PermissionStatusModel[]) {
    const permissionsFGs = permissions.map(permission => this.fb.group(permission));
    const permissionsFormArray = this.fb.array(permissionsFGs);
    this.permissionsForm.setControl('permissions', permissionsFormArray);
  }

  get permissions() {
    return this.permissionsForm.get('permissions') as FormArray;
  }

  onSubmit(): void {
    const selectedPermissions: PermissionStatusModel[] = this.permissionsForm.value.permissions
      .filter((permission: { permissionSelected: boolean, value: string }) => permission.permissionSelected)
      .map((permission: { permissionSelected: boolean, value: string }) => permission.value);
    this.usersManagementService.setUserPermissions(this.user!.id, selectedPermissions).subscribe({
      next: _ => {
        this.router.navigate(['/users-management/users']);
      }
    });
  }

}
