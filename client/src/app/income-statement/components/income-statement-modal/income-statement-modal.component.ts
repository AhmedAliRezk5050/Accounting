import {Component} from '@angular/core';
import {AlertService} from "../../../shared/services/alert.service";
import {ModalService} from "../../../shared/services/modal.service";
import {IncomeStatementService} from "../../services/income-statement.service";
import IncomeStatementModalType from "../../models/income-statement-modal-type";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import TrialBalanceModalType from "../../../trial-balance/models/trial-balance-modal-type";

@Component({
  selector: 'app-income-statement-modal',
  templateUrl: './income-statement-modal.component.html',
  styleUrls: ['./income-statement-modal.component.scss']
})
export class IncomeStatementModalComponent {

  incomeStatementModalType = IncomeStatementModalType
  submitBtnEnabled = true;

  constructor(public alertService: AlertService,
              public modalService: ModalService,
              public incomeStatementService: IncomeStatementService) {
  }

  fromDate = new FormControl('', [Validators.required]);
  toDate = new FormControl('');
  searchIncomeStatementForm = new FormGroup({
    fromDate: this.fromDate,
    toDate: this.toDate,
  });

  searchIncomeStatement = () => {
    this.submitBtnEnabled = false;
    this.alertService.alertShown = true;
    this.alertService.alertMsg = "Searching. please wait...";
    this.alertService.alertColor = 'blue';
    this.incomeStatementService
      .fetchIncomeStatement(this.fromDate.value!, this.toDate.value!)
      .subscribe({
        next: i => {
          this.incomeStatementService.incomeStatementFetched.emit(i);
          this.modalService.toggleModal("income-statement", false)
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
      });
  }

  onSubmit = () => {
    switch (this.incomeStatementService.incomeStatementModalType) {
      case IncomeStatementModalType.Search:
        this.searchIncomeStatement();
        break;
      // case TrialBalanceModalType.Pdf:
      //   this.getTrialBalancePdf();
      //   break;
    }
  }

}
