import {Injectable, OnInit} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import IAccount from "../../shared/models/account.model";
import {environment} from "../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class CustomersService{
  constructor(private httpClient: HttpClient) {
  }

  getCustomerAccounts = () => {
    return this.httpClient.get<IAccount[]>(`${environment.apiUrl}/api/accounts/GetCustomerAccounts`)
  }

  addCustomer = (customer: any) => {
    return this.httpClient.post(`${environment.apiUrl}/api/accounts/AddCustomer`, customer);
  }
}
