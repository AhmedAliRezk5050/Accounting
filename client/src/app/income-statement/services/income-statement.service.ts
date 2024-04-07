import {EventEmitter, Injectable} from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import IIncomeStatement from "../models/income-statement.model";
import IncomeStatementModalType from "../models/income-statement-modal-type";
import {environment} from "../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class IncomeStatementService {

  incomeStatementFetched = new EventEmitter<IIncomeStatement>();
  incomeStatementModalType?: IncomeStatementModalType
  constructor(private httpClient: HttpClient) { }

  fetchIncomeStatement = (fromDate: string, toDate?: string) => {
    let params = new HttpParams();

    params = params.append('fromDate', fromDate);

    if (toDate) {
      params = params.append('toDate', toDate);
    }

    return this.httpClient.get<IIncomeStatement>(`${environment.apiUrl}/api/entries/GetIncomeStatement`, {
      params: params
    })
  }

  getIncomeStatementPdf = (fromDate: string, toDate?: string) => {
    let params = new HttpParams();

    params = params.append('fromDate', fromDate);

    if (toDate) {
      params = params.append('toDate', toDate);
    }

    return this.httpClient.get(`${environment.apiUrl}/api/entries/GetIncomeStatementPdf`, {
      params: params,
      observe: 'response',
      responseType: 'blob'
    })
  }
}
