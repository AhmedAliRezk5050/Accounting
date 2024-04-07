import { Injectable } from '@angular/core';
import {Observable} from "rxjs";
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class DropdownService {


  constructor(public http: HttpClient) {
  }

  getDropdownItems(apiEndpoint: string, dataFormat: 'array' | 'object'): Observable<any> {
    return this.http.get<any>(apiEndpoint);
  }
}
