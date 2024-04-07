import {Component, OnInit} from '@angular/core';
import {IncomeStatementService} from "../../services/income-statement.service";
import {ModalService} from "../../../shared/services/modal.service";
import IncomeStatementModalType from "../../models/income-statement-modal-type";
import IIncomeStatement from "../../models/income-statement.model";
import {AlertService} from "../../../shared/services/alert.service";
import BalanceSheetModalType from "../../../balance-sheet/models/balance-sheet-modal-type";

@Component({
  selector: 'app-income-statement-home',
  templateUrl: './income-statement-home.component.html',
  styleUrls: ['./income-statement-home.component.scss']
})
export class IncomeStatementHomeComponent implements OnInit {

  incomeStatement?: IIncomeStatement
  btnDisabled = false

  constructor(
    public incomeStatementService: IncomeStatementService,
    public modalService: ModalService,
    public alertService: AlertService
  ) {
  }

  ngOnInit(): void {
    this.incomeStatementService.incomeStatementFetched.subscribe(i => this.incomeStatement = i);
  }


  openIncomeStatementModal() {
    this.incomeStatementService.incomeStatementModalType = IncomeStatementModalType.Search;
    this.modalService.toggleModal('income-statement', true);
  }

  printIncomeStatement() {
    this.btnDisabled = true;
    this.incomeStatementService.getIncomeStatementPdf(this.incomeStatement?.fromDate!, this.incomeStatement?.toDate!).subscribe({
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
