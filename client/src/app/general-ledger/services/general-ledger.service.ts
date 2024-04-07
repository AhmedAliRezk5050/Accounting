import { EventEmitter, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from "@angular/common/http";
import IGeneralLedgerSearchHttpResponse from "../models/general-ledger-search-http-response.model";
import IGeneralLedger from 'src/app/shared/models/general-ledger';
import GeneralLedgerModalType from "../models/general-ledger-modal-type";
import {environment} from "../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class GeneralLedgerService {
  generalLedgerFetched = new EventEmitter<IGeneralLedger | null>()

  generalLedgerModalType?: GeneralLedgerModalType

  constructor(private httpClient: HttpClient) {
  }

  getGeneralLedger = (accountId: string, fromDate: string, toDate?: string) => {
    let httpParams = new HttpParams()
    httpParams = httpParams.append('fromDate', fromDate)
    if (toDate) {
      httpParams = httpParams.append('toDate', toDate)
    }
    return this.httpClient.get<IGeneralLedger>(`${environment.apiUrl}/api/entries/GetGeneralLedger/${accountId}`, {
      params: httpParams
    })
  }

  getGeneralLedgerPdf = (accountId: number, fromDate: string, toDate?: string) => {
    let httpParams = new HttpParams()
    httpParams = httpParams.append('fromDate', fromDate)
    if (toDate) {
      httpParams = httpParams.append('toDate', toDate)
    }
    return this.httpClient.get(`${environment.apiUrl}/api/entries/GetGeneralLedgerPdf/${accountId}`, {
      params: httpParams,
      observe: 'response',
      responseType: 'blob'
    })
  }

  getGeneralLedgerExcel = (accountId: string, fromDate: string, toDate?: string) => {
    let httpParams = new HttpParams()
    httpParams = httpParams.append('fromDate', fromDate)
    if (toDate) {
      httpParams = httpParams.append('toDate', toDate)
    }
    return this.httpClient.get(`${environment.apiUrl}/api/entries/GetGeneralLedgerExcel/${accountId}`, {
      params: httpParams,
      observe: 'response',
      responseType: 'blob'
    })
  }
}
