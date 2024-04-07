import {EventEmitter, Injectable} from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import IBalanceSheet from "../../shared/models/balance-sheet.model";
import BalanceSheetModalType from "../models/balance-sheet-modal-type";
import {environment} from "../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class BalanceSheetService {
  balanceSheetFetched = new EventEmitter<IBalanceSheet>();
  balanceSheetModalType?: BalanceSheetModalType
  constructor(private httpClient: HttpClient) {
  }

  fetchBalanceSheet = (fromDate: string, toDate?: string) => {
    let params = new HttpParams();

    params = params.append('fromDate', fromDate);

    if (toDate) {
      params = params.append('toDate', toDate);
    }

    return this.httpClient.get<IBalanceSheet>(`${environment.apiUrl}/api/entries/GetBalanceSheet`, {
      params: params
    })
  }

  getBalanceSheetPdf = (fromDate: string, toDate?: string) => {
    let params = new HttpParams();

    params = params.append('fromDate', fromDate);

    if (toDate) {
      params = params.append('toDate', toDate);
    }

    return this.httpClient.get(`${environment.apiUrl}/api/entries/GetBalanceSheetPdf`, {
      params: params,
      observe: 'response',
      responseType: 'blob'
    })
  }
}
