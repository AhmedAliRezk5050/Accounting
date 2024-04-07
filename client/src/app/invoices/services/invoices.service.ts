import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {environment} from "../../../environments/environment";
import IInvoice from "../../shared/models/invoice.model";
import IPagedResponseModel from "../../shared/models/paged-response.model";

@Injectable({
  providedIn: 'root'
})
export class InvoicesService {
  constructor(private httpClient: HttpClient) {
  }

  getCustomersInvoices = () => {
    return this.httpClient.get<IPagedResponseModel<IInvoice>>(`${environment.apiUrl}/api/invoices/GetCustomersInvoices`)
  }

  getSuppliersInvoices = () => {
    return this.httpClient.get<IPagedResponseModel<IInvoice>>(`${environment.apiUrl}/api/invoices/GetSuppliersInvoices`)
  }

  addCustomerInvoice = (customerInvoice: any) => {
    return this.httpClient.post(`${environment.apiUrl}/api/invoices/AddCustomerInvoice`, customerInvoice);
  }

  addSupplierInvoice = (supplierInvoice: any) => {
    return this.httpClient.post(`${environment.apiUrl}/api/invoices/AddSupplierInvoice`, supplierInvoice);
  }
}
