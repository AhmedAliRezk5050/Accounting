import {EventEmitter, Injectable} from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import IEntry from "../../shared/models/entry.model";
import IEntryDetailRequest from "../models/entry-detail-request";
import IUpdateEntryRequestBody from "../models/update-entry-request-body";
import {environment} from "../../../environments/environment";
import {BehaviorSubject} from "rxjs";
import IPagedResponseModel from "../../shared/models/paged-response.model";
import {LoadingSpinnerService} from "../../shared/services/loading-spinner.service";

@Injectable({
  providedIn: 'root'
})
export class EntriesService {
  public entriesFetchedSubject = new BehaviorSubject<IPagedResponseModel<IEntry>>
  ({items: [], totalCount: 0});

  constructor(private httpClient: HttpClient) {
  }

  private getEntries = (pageNumber: number, pageSize: number) => {
    return this.httpClient.get<IPagedResponseModel<IEntry>>(`${environment.apiUrl}/api/entries?PageNumber=${pageNumber}&&PageSize=${pageSize}`);
  }

  fetchEntry = (id: string) => {
    return this.httpClient.get<IEntry>(`${environment.apiUrl}/api/entries/${id}`);
  }

  addEntry = (entry: any) => {
    return this.httpClient.post(`${environment.apiUrl}/api/entries`, entry);
  }

  deleteEntry = (id: string) => {
    return this.httpClient.delete(`${environment.apiUrl}/api/entries/${id}`);
  }
  postEntry = (id: string) => {
    return this.httpClient.post(`${environment.apiUrl}/api/entries/${id}/post`, {});
  }

  updateEntry = (updates: IUpdateEntryRequestBody, id: string) => {
    return this.httpClient.put(`${environment.apiUrl}/api/entries/${id}`, updates);
  }

  getEntryPdf = (entryId: string) => {
    return this.httpClient.get(`${environment.apiUrl}/api/entries/GetEntryPdf/${entryId}`, {
      observe: 'response',
      responseType: 'blob'
    })
  }

  fetchEntries = (pageNumber = 1, pageSize = 10) => {
    this.getEntries(pageNumber, pageSize).subscribe({
      next: res => {
        res.items = res.items.map(e => {
          e.entryDate = this.formatDate(e.entryDate)
          return e;
        });
        this.entriesFetchedSubject.next(res);
      },
      complete: () => {
      }
    })
  }

  formatDate(date: string) {
    const dateObj = new Date(date);

    const year = dateObj.getFullYear();
    const month = String(dateObj.getMonth() + 1).padStart(2, '0');
    const day = String(dateObj.getDate()).padStart(2, '0');

    const formattedDate = `${year}-${month}-${day}`;

    return formattedDate;
  }
}
