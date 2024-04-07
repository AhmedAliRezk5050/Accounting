import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import IAccount from "../../shared/models/account.model";
import {environment} from "../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class SuppliersService{
  constructor(private httpClient: HttpClient) {
  }

  getSupplierAccounts = () => {
    return this.httpClient.get<IAccount[]>(`${environment.apiUrl}/api/accounts/GetSupplierAccounts`)
  }

  addSupplier = (supplier: any) => {
    debugger
    return this.httpClient.post(`${environment.apiUrl}/api/accounts/AddSupplier`, supplier);
  }
}
