import {EventEmitter, Injectable} from '@angular/core';
import ITrialBalance from "../../shared/models/trial-balance.model";
import {HttpClient, HttpParams} from "@angular/common/http";
import TrialBalanceModalType from "../models/trial-balance-modal-type";
import {environment} from "../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class TrialBalanceService {
  trialBalanceFetched = new EventEmitter<ITrialBalance>();
  trialBalanceModalType?: TrialBalanceModalType

  constructor(private httpClient: HttpClient) {
  }

  fetchTrialBalance = (fromDate: string, toDate?: string) => {
    let params = new HttpParams();

    params = params.append('fromDate', fromDate);

    if (toDate) {
      params = params.append('toDate', toDate);
    }

    return this.httpClient.get<ITrialBalance>(`${environment.apiUrl}/api/entries/GetTrialBalance`, {
      params: params
    })
  }

  getTrialBalancePdf = (fromDate: string, toDate?: string) => {
    let params = new HttpParams();

    params = params.append('fromDate', fromDate);

    if (toDate) {
      params = params.append('toDate', toDate);
    }

    return this.httpClient.get(`${environment.apiUrl}/api/entries/GetTrialBalancePdf`, {
      params: params,
      observe: 'response',
      responseType: 'blob'
    })
  }

  getTrialBalanceExcel = (fromDate: string, toDate?: string) => {
    let params = new HttpParams();

    params = params.append('fromDate', fromDate);

    if (toDate) {
      params = params.append('toDate', toDate);
    }

    return this.httpClient.get(`${environment.apiUrl}/api/entries/GetTrialBalanceExcel`, {
      params: params,
      observe: 'response',
      responseType: 'blob'
    })
  }
}
