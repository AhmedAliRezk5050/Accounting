import { Component } from '@angular/core';
import BalanceSheetModalType from "../../models/balance-sheet-modal-type";
import {AlertService} from "../../../shared/services/alert.service";
import {ModalService} from "../../../shared/services/modal.service";
import {BalanceSheetService} from "../../services/balance-sheet.service";
import TrialBalanceModalType from "../../../trial-balance/models/trial-balance-modal-type";
import {FormControl, FormGroup, Validators} from "@angular/forms";

@Component({
  selector: 'app-balance-sheet-modal',
  templateUrl: './balance-sheet-modal.component.html',
  styleUrls: ['./balance-sheet-modal.component.scss']
})
export class BalanceSheetModalComponent {
  balanceSheetModalType = BalanceSheetModalType
  submitBtnEnabled = true;


  constructor(
    public alertService: AlertService,
    public balanceSheetService: BalanceSheetService,
    public modalService: ModalService
  ) {
  }

  fromDate = new FormControl('', [Validators.required]);
  toDate = new FormControl('');
  searchTrialBalanceForm = new FormGroup({
    fromDate: this.fromDate,
    toDate: this.toDate,
  });

  searchBalanceSheet() {
    this.submitBtnEnabled = false;
    this.alertService.alertShown = true;
    this.alertService.alertMsg = "Searching. please wait...";
    this.alertService.alertColor = 'blue';

    this.balanceSheetService.fetchBalanceSheet(this.fromDate.value!, this.toDate.value!).subscribe({
      next: t => {
        t.assetsAccountsInfoList = t.assetsAccountsInfoList.filter(a => a.level <= 3 && a.balance != 0);
        t.liabilitiesAccountsInfoList = t.liabilitiesAccountsInfoList.filter(a => a.level <= 3 && a.balance != 0);
        t.equityAccountsInfoList = t.equityAccountsInfoList.filter(a => a.level <= 3 && a.balance != 0);
        this.balanceSheetService.balanceSheetFetched.emit(t);
        this.modalService.toggleModal("balanceSheet", false)
      },
      error: err => {
        if (err.error === 'No results found') {
          this.alertService.alertMsg = err.error;
        } else {
          this.alertService.alertMsg = "Searching failed. Try again later";
        }
        this.alertService.alertColor = 'red';
        this.submitBtnEnabled = true;
        console.log(err)
      }
    })
  }

  getBalanceSheetPdf() {
    this.submitBtnEnabled = false;
    this.alertService.alertShown = true;
    this.alertService.alertMsg = "Getting Pdf. please wait...";
    this.alertService.alertColor = 'blue';

    this.balanceSheetService.getBalanceSheetPdf(this.fromDate.value!, this.toDate.value!).subscribe({
      next: response => {
        const blob = response.body;
        const url = URL.createObjectURL(blob!);
        window.open(url)
        this.modalService.toggleModal("balanceSheet", false)
      },
      error: err => {
        if (err.error === 'No results found') {
          this.alertService.alertMsg = err.error;
        } else {
          this.alertService.alertMsg = "Getting Pdf failed. Try again later";
        }
        this.alertService.alertColor = 'red';
        this.submitBtnEnabled = true;
        console.log(err)
      }
    })
  }

  onSubmit = () => {
    switch (this.balanceSheetService.balanceSheetModalType) {
      case BalanceSheetModalType.Search:
        this.searchBalanceSheet();
        break;
      case BalanceSheetModalType.Pdf:
        this.getBalanceSheetPdf();
        break;
    }
  }
}
