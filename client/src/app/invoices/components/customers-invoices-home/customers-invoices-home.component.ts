import {Component, OnInit} from '@angular/core';
import IInvoice from "../../../shared/models/invoice.model";
import {InvoicesService} from "../../services/invoices.service";

@Component({
  selector: 'app-customers-invoices-home',
  templateUrl: './customers-invoices-home.component.html',
  styleUrls: ['./customers-invoices-home.component.scss']
})
export class CustomersInvoicesHomeComponent implements OnInit {
  invoices: IInvoice[] = []

  constructor(private invoicesService: InvoicesService) {
  }
  ngOnInit(): void {
    this.invoicesService.getCustomersInvoices().subscribe({
      next: result => {
        result.items.forEach(i => {
        })
        this.invoices = result.items;
      }
    });
  }

}
