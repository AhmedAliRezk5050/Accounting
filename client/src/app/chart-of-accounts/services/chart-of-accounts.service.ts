import {EventEmitter, Injectable, OnInit} from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import IAccount from "../../shared/models/account.model";
import IAddAccount from "../models/add-account";
import {environment} from "../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class ChartOfAccountsService {

  operatingAccount?: IAccount

  crudExecuted = new EventEmitter()

  fetchedAccountsTree: IAccount[] = []

  constructor(private http: HttpClient) {
  }

  fetchAccountsByParent = (id: string) => {
    return this.http.get<{
      parentLevel: number,
      accounts: IAccount[]
    }>(`${environment.apiUrl}/api/accounts/GetAccountsByParent?id=${id}`)
  }

  fetchAccountsByLevel = (level: number) => {
    return this.http.get<IAccount[]>(`${environment.apiUrl}/api/accounts/GetAccountsByLevel?accountLevel=${level}`)
  }

  addAccount = (account: IAddAccount) => {
    return this.http.post(`${environment.apiUrl}/api/accounts/addaccount`, account);
  }

  deleteAccount = (id: string) => {
    return this.http.delete(`${environment.apiUrl}/api/accounts/${id}`);
  }

  editAccount = (id: string, account: any) => {
    return this.http.put(`${environment.apiUrl}/api/accounts/${id}`, account);
  }

  searchAccount = (term: string, subOnly?: boolean) => {
    let params = new HttpParams()
    params = params.append('term', term);
    if (subOnly) {
      params = params.append('subOnly', subOnly);
    }
    return this.http.get<IAccount[]>(`${environment.apiUrl}/api/accounts`, {
      params
    })
  }

  fetchAccountsTree = () => {
    return this.http.get<IAccount[]>(`${environment.apiUrl}/api/accounts/GetAccountsTree`)
  }

  getBankAccounts = () => {
    return this.http.get<IAccount[]>(`${environment.apiUrl}/api/accounts/GetBankAccounts`)
  }

  getCustomerAccounts = () => {
    return this.http.get<IAccount[]>(`${environment.apiUrl}/api/accounts/GetCustomerAccounts`)
  }

  getSupplierAccounts = () => {
    return this.http.get<IAccount[]>(`${environment.apiUrl}/api/accounts/GetSupplierAccounts`)
  }
}
