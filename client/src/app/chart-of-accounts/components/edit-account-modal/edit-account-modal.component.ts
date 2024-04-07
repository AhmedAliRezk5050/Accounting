import {Component, OnInit} from '@angular/core';
import IAccount from "../../../shared/models/account.model";
import {ChartOfAccountsService} from "../../services/chart-of-accounts.service";
import {AlertService} from "../../../shared/services/alert.service";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {ModalService} from "../../../shared/services/modal.service";

@Component({
  selector: 'app-edit-account-modal',
  templateUrl: './edit-account-modal.component.html',
  styleUrls: ['./edit-account-modal.component.scss']
})
export class EditAccountModalComponent implements OnInit {
  account?: IAccount;
  submitBtnEnabled = true;
  constructor(public treeService: ChartOfAccountsService, public alertService: AlertService, private modalService: ModalService) {
  }

  ngOnInit(): void {
    this.account = this.treeService.operatingAccount;
    this.arabicName.setValue(this.account?.arabicName)
    this.englishName.setValue(this.account?.englishName)
    this.currency.setValue(this.account?.currency)
  }


  arabicName = new FormControl<string | undefined>('')
  englishName = new FormControl<string | undefined>('')
  currency = new FormControl<string | undefined>('')
  // accountDebit = new FormControl(this.account?)

  editAccountForm = new FormGroup({
    arabicName: this.arabicName,
    englishName: this.englishName,
    currency: this.currency
  });


  editAccount() {
    this.submitBtnEnabled = false;
    this.alertService.alertColor = "blue";
    this.alertService.alertShown = true;
    this.alertService.alertMsg = "Updating account, please wait";

    this.treeService.editAccount(this.treeService.operatingAccount!.id, this.editAccountForm.value)
      .subscribe({
        next: response => {
          console.log(response)
          this.alertService.alertColor = 'green';
          this.alertService.alertMsg = 'Account Edited successfully.';
          setTimeout(() => {
            this.alertService.alertShown = false;
            this.modalService.toggleModal('editAccount', false);
            this.treeService.crudExecuted.emit();
          }, 1000);
        },
        error: err => {
          this.alertService.alertColor = 'red';
          this.alertService.alertMsg = 'Failed creating account. Try again later.';
          this.submitBtnEnabled = true;
          console.log(err)
        },
        complete: () => {

        }
      })
  }
}
