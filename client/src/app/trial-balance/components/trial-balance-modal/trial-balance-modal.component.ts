import {Component} from '@angular/core';
import {AlertService} from "../../../shared/services/alert.service";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {TrialBalanceService} from "../../services/trial-balance.service";
import {ModalService} from "../../../shared/services/modal.service";
import TrialBalanceModalType from "../../models/trial-balance-modal-type";

@Component({
  selector: 'app-trial-balance-modal',
  templateUrl: './trial-balance-modal.component.html',
  styleUrls: ['./trial-balance-modal.component.scss']
})
export class TrialBalanceModalComponent {
  trialBalanceModalType = TrialBalanceModalType
  submitBtnEnabled = true;

  constructor(
    public alertService: AlertService,
    public trialBalanceService: TrialBalanceService,
    public modalService: ModalService
  ) {
  }

  fromDate = new FormControl('', [Validators.required]);
  toDate = new FormControl('');
  searchTrialBalanceForm = new FormGroup({
    fromDate: this.fromDate,
    toDate: this.toDate,
  });

  searchTrialBalance() {
    this.submitBtnEnabled = false;
    this.alertService.alertShown = true;
    this.alertService.alertMsg = "Searching. please wait...";
    this.alertService.alertColor = 'blue';

    this.trialBalanceService.fetchTrialBalance(this.fromDate.value!, this.toDate.value!).subscribe({
      next: t => {
        this.trialBalanceService.trialBalanceFetched.emit(t);
        this.modalService.toggleModal("trialBalance", false)
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

  onSubmit = () => {
    this.searchTrialBalance();
  }
}
