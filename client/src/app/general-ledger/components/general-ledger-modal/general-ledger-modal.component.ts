import {Component, Input} from '@angular/core';
import {ChartOfAccountsService} from "../../../chart-of-accounts/services/chart-of-accounts.service";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {AlertService} from "../../../shared/services/alert.service";
import {GeneralLedgerService} from "../../services/general-ledger.service";
import {ModalService} from "../../../shared/services/modal.service";
import IGeneralLedger from 'src/app/shared/models/general-ledger';
import GeneralLedgerModalType from "../../models/general-ledger-modal-type";
import {TranslationService} from "../../../services/translation.service";

@Component({
  selector: 'app-general-ledger-modal',
  templateUrl: './general-ledger-modal.component.html',
  styleUrls: ['./general-ledger-modal.component.scss']
})
export class GeneralLedgerModalComponent {
  submitBtnEnabled = true;

  generalLedgerModalType = GeneralLedgerModalType

  constructor(
    private treeService: ChartOfAccountsService,
    public alertService: AlertService,
    public generalLedgerService: GeneralLedgerService,
    private modalService: ModalService,
    private translationService: TranslationService
  ) {
  }

  searchedAccounts: { text: string, value: string }[] = []
  selectedAccountId?: string

  searchAccounts(term: string) {
    this.treeService.searchAccount(term).subscribe(fetchedAccounts => {
      this.searchedAccounts = fetchedAccounts.map(a => {
        const accountName = this.isEnglish ? a.englishName : a.arabicName
        return {
          text: a.code.toString() + " -- " + accountName,
          value: a.id
        }
      })
    })
  }

  fromDate = new FormControl('', [Validators.required]);
  toDate = new FormControl('');

  searchGeneralLedgerForm = new FormGroup({
    fromDate: this.fromDate,
    toDate: this.toDate,
  });

  onSubmit = () => {
    switch (this.generalLedgerService.generalLedgerModalType) {
      case GeneralLedgerModalType.Search:
        this.searchGeneralLedger();
        break;
    }
  }

  searchGeneralLedger() {
    this.submitBtnEnabled = false;
    this.alertService.alertShown = true;
    this.alertService.alertMsg = "Searching. please wait...";
    this.alertService.alertColor = 'blue';
    this.generalLedgerService
      .getGeneralLedger(this.selectedAccountId!, this.fromDate.value!, this.toDate.value!)
      .subscribe({
        next: response => {
          this.generalLedgerService.generalLedgerFetched.emit({
            data: response.data,
            accountCode: response.accountCode,
            accountArabicName: response.accountArabicName,
            accountEnglishName: response.accountEnglishName,
            totalCredit: response.totalCredit,
            totalDebit: response.totalDebit,
            totalBalance: response.totalBalance,
            fromDate: response.fromDate,
            toDate: response.toDate,
            accountId: response.accountId
          })
          this.modalService.toggleModal("generalLedger", false)
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

  get isEnglish() {
    return this.translationService.isEnglishLanguage()
  }

}
