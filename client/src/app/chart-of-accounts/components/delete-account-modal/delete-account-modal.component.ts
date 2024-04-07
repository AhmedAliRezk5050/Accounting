import {Component, OnInit} from '@angular/core';
import IAccount from "../../../shared/models/account.model";
import {ChartOfAccountsService} from "../../services/chart-of-accounts.service";
import {ModalService} from "../../../shared/services/modal.service";
import {AlertService} from "../../../shared/services/alert.service";

@Component({
  selector: 'app-delete-account-modal',
  templateUrl: './delete-account-modal.component.html',
  styleUrls: ['./delete-account-modal.component.scss']
})
export class DeleteAccountModalComponent implements OnInit {
  account?: IAccount

  constructor(
    public chartOfAccountsService: ChartOfAccountsService,
    public modalService: ModalService,
    public alertService: AlertService
  ) {
  }

  ngOnInit(): void {
    this.account = this.chartOfAccountsService.operatingAccount
  }

  deleteAccount() {
    if (this.account) {
      this.alertService.alertMsg = "Deleting account, please wait."
      this.alertService.alertShown = true;
      this.chartOfAccountsService.deleteAccount(this.account.id).subscribe({
        next: response => {
          this.alertService.alertMsg = "Account deleted successfully."
          this.alertService.alertColor = 'green';
          setTimeout(() => {
            this.alertService.alertShown = false;
            this.modalService.toggleModal('deleteAccount', false);
            this.chartOfAccountsService.crudExecuted.emit();
          }, 1000);
        },
        error: err => {
          this.alertService.alertMsg = "Account deletion failed."
          this.alertService.alertColor = 'red';
          console.log(err)
        }
      })
    }
  }
}
