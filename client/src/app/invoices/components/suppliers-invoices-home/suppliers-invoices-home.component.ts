import {Component, OnInit} from '@angular/core';
import IInvoice from "../../../shared/models/invoice.model";
import {InvoicesService} from "../../services/invoices.service";

@Component({
  selector: 'app-suppliers-invoices-home',
  templateUrl: './suppliers-invoices-home.component.html',
  styleUrls: ['./suppliers-invoices-home.component.scss']
})
export class SuppliersInvoicesHomeComponent implements OnInit{
  invoices: IInvoice[] = []

  constructor(private invoicesService: InvoicesService) {
  }
  ngOnInit(): void {
    this.invoicesService.getSuppliersInvoices().subscribe({
      next: result => {
        this.invoices = result.items;
      }
    });
  }

}
