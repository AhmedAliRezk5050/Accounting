import {Injectable} from '@angular/core';
import {environment} from "../../../environments/environment";
import {HttpClient} from "@angular/common/http";
import {UserDetailsModel} from "../models/user-details.model";
import {map, Observable} from "rxjs";
import PermissionStatusModel from "../models/permission-status.model";
import {ClaimDetailsModel} from "../models/claim-details.model";

@Injectable({
  providedIn: 'root'
})
export class UsersManagementService {

  constructor(private httpClient: HttpClient) {
  }

  getUsers() {
    return this.httpClient.get<UserDetailsModel[]>(`${environment.apiUrl}/api/users/GetUsers`)
  }

  getUser(id: string) {
    return this.httpClient.get<UserDetailsModel>(`${environment.apiUrl}/api/users/GetUser/${id}`)
  }

  getAllPermissions(): Observable<PermissionStatusModel[]> {
    return this.httpClient
      .get<string[]>(`${environment.apiUrl}/api/users/GetAllPermissions`)
      .pipe(map(permissions => permissions.map(p => ({
        value: p,
        permissionSelected: false
      }))));
  }

  updatePermissionStatus(permissions: PermissionStatusModel[], claims: ClaimDetailsModel[]): PermissionStatusModel[] {
    return permissions.map(permission => {
      const hasClaim = claims.some(claim => claim.value === permission.value);
      return {
        ...permission,
        permissionSelected: hasClaim
      };
    });
  }
  setUserPermissions(id: string, updatedPermissions: PermissionStatusModel[]) {
    return this.httpClient
      .post(`${environment.apiUrl}/api/users/SetUserPermissions/${id}`, updatedPermissions)
  }
}
