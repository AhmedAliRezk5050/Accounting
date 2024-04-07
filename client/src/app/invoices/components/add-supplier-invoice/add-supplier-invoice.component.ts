import {Component} from '@angular/core';
import IAccount from "../../../shared/models/account.model";
import {AlertService} from "../../../shared/services/alert.service";
import {TranslationService} from "../../../services/translation.service";
import {InvoicesService} from "../../services/invoices.service";
import {ChartOfAccountsService} from "../../../chart-of-accounts/services/chart-of-accounts.service";
import {FormControl, FormGroup, Validators} from "@angular/forms";

@Component({
  selector: 'app-add-supplier-invoice',
  templateUrl: './add-supplier-invoice.component.html',
  styleUrls: ['./add-supplier-invoice.component.scss']
})
export class AddSupplierInvoiceComponent {

  searchInputItems: { text: string, value: string }[] = []
  searchedAccounts: IAccount[] = []
  accountIdSelected?: boolean

  constructor(
    public alertService: AlertService,
    private translationService: TranslationService,
    private invoicesService: InvoicesService,
    private chartOfAccountsService: ChartOfAccountsService
  ) {
  }

  invoiceNumber = new FormControl('', [Validators.required])
  date = new FormControl('', [Validators.required])
  amount = new FormControl('', [Validators.required])
  tax = new FormControl('', [Validators.required])
  totalAmount = new FormControl('', [Validators.required])
  notes = new FormControl('')
  supplierId = new FormControl('', [Validators.required])


  addSupplierInvoiceForm = new FormGroup({
    invoiceNumber: this.invoiceNumber,
    date: this.date,
    amount: this.amount,
    tax: this.tax,
    totalAmount: this.totalAmount,
    notes: this.notes,
    supplierId: this.supplierId,
  });

  addSupplierInvoice() {
    this.alertService.alertShown = true;
    this.alertService.alertColor = 'blue';
    this.alertService.alertMsg = this.translationService.getTranslatedWord('adding');

    this.invoicesService.addSupplierInvoice({
      invoiceNumber: this.invoiceNumber.value,
      date: this.date.value,
      amount: this.amount.value,
      tax: this.tax.value,
      totalAmount: this.totalAmount.value,
      notes: this.notes.value,
      supplierId: this.supplierId.value,
    }).subscribe({
      next: _ => {
        this.alertService.alertColor = 'green';
        this.alertService.alertMsg = this.translationService.getTranslatedWord('success-add');
        this.addSupplierInvoiceForm.reset();

        setTimeout(() => {
          this.alertService.alertColor = '';
          this.alertService.alertMsg = '';
          this.alertService.alertShown = false;
        }, 1000);
      },
      error: err => {
        this.alertService.alertColor = 'red';
        this.alertService.alertMsg = this.translationService.getTranslatedWord('fail-add');
      }
    });
  }

  searchAccounts(term: string) {
    this.chartOfAccountsService.searchAccount(term, true).subscribe(fetchedAccounts => {
      debugger
      this.searchedAccounts = fetchedAccounts.filter(a => a.code.toString().startsWith('20101'));
      this.searchInputItems = this.searchedAccounts.map(a => ({
        text: a.code.toString() + " -- " + a.arabicName,
        value: a.supplier?.id.toString() ?? ''
      }))
    })
  }

  onAccountSelect(id: string) {
    // this.entryDetails.controls.forEach((x: FormControl, i) => {
    //   if (index === i) {
    //     x.get('accountId')?.setValue(id)
    //     const w = x.get('accountId')
    //   }
    // });
    debugger
    debugger
    this.supplierId.setValue(id)
  }
}
