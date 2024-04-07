import {Component, OnInit} from '@angular/core';
import {ModalService} from "../../../shared/services/modal.service";
import {BalanceSheetService} from "../../services/balance-sheet.service";
import IBalanceSheet from "../../../shared/models/balance-sheet.model";
import BalanceSheetModalType from "../../models/balance-sheet-modal-type";
import {AlertService} from "../../../shared/services/alert.service";

@Component({
  selector: 'app-balance-sheet-home',
  templateUrl: './balance-sheet-home.component.html',
  styleUrls: ['./balance-sheet-home.component.scss']
})
export class BalanceSheetHomeComponent implements OnInit {
  balanceSheet?: IBalanceSheet
  btnDisabled = false
  constructor(
    public balanceSheetService: BalanceSheetService,
    public modalService: ModalService,
    public alertService: AlertService) {
  }

  ngOnInit(): void {
    this.balanceSheetService.balanceSheetFetched.subscribe(t => this.balanceSheet = t)
  }

  openBalanceSheetModal() {
    this.balanceSheetService.balanceSheetModalType = BalanceSheetModalType.Search;
    this.modalService.toggleModal('balanceSheet', true);
  }

  printBalanceSheet() {
    this.btnDisabled = true;
    this.balanceSheetService.getBalanceSheetPdf(this.balanceSheet?.fromDate!, this.balanceSheet?.toDate!).subscribe({
      next: response => {
        this.btnDisabled = false;
        const blob = response.body;
        const url = URL.createObjectURL(blob!);
        window.open(url)
        this.alertService.alertShown = true;
        this.alertService.alertColor = 'green';
        this.alertService.alertMsg = 'Getting Pdf succeeded.';

        setTimeout(() => {
          this.alertService.alertShown = false;
        }, 1000)
      },
      error: err => {
        this.btnDisabled = false;
        if (err.error === 'No results found') {
          this.alertService.alertMsg = err.error;
        } else {
          this.alertService.alertMsg = "Getting Pdf failed. Try again later";
        }
        this.alertService.alertColor = 'red';

        setTimeout(() => {
          this.alertService.alertShown = false;
        }, 1000)

      }
    })
  }
}
