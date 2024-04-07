import {Component, OnInit} from '@angular/core';
import {TrialBalanceService} from "../../services/trial-balance.service";
import ITrialBalance from "../../../shared/models/trial-balance.model";
import {ModalService} from "../../../shared/services/modal.service";
import TrialBalanceModalType from "../../models/trial-balance-modal-type";
import {AlertService} from "../../../shared/services/alert.service";

@Component({
  selector: 'app-trial-balance-home',
  templateUrl: './trial-balance-home.component.html',
  styleUrls: ['./trial-balance-home.component.scss']
})
export class TrialBalanceHomeComponent implements OnInit {
  trialBalance?: ITrialBalance
  btnDisabled = false
  constructor(
    public trialBalanceService: TrialBalanceService,
    public modalService: ModalService,
    public alertService: AlertService
    ) {
  }

  ngOnInit(): void {
    this.trialBalanceService.trialBalanceFetched.subscribe(t => this.trialBalance = t)
  }

  openTrialBalanceModal() {
    this.trialBalanceService.trialBalanceModalType = TrialBalanceModalType.Search;
    this.modalService.toggleModal('trialBalance', true);
  }

  printTrialBalance() {
    this.btnDisabled = true;
    this.trialBalanceService.getTrialBalancePdf(this.trialBalance?.fromDate!, this.trialBalance?.toDate!).subscribe({
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
