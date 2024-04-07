import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import IAccount from "../../shared/models/account.model";
import {environment} from "../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class DataManagementService {
  constructor(private httpClient: HttpClient) {
  }

  getBackupFileNames = () => {
    return this.httpClient.get<string[]>(`${environment.apiUrl}/api/Management/GetBackupFileNames`)
  }

  restoreDatabase = (fileName: string) => {
    return this.httpClient.post(`${environment.apiUrl}/api/Management/restore`, {
      backupFileName: fileName
    })
  }

  backupDatabase = () => {
    return this.httpClient.post(`${environment.apiUrl}/api/Management/Backup`, {})
  }
}
