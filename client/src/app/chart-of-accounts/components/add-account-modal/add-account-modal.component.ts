import {Component, ComponentRef, OnInit, ViewChild, ViewContainerRef} from '@angular/core';
import {AlertService} from "../../../shared/services/alert.service";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {ChartOfAccountsService} from "../../services/chart-of-accounts.service";
import {ModalService} from "../../../shared/services/modal.service";
import {NodeDirective} from "../../../shared/directives/node.directive";
import {TranslationService} from "../../../services/translation.service";

@Component({
  selector: 'app-add-account-modal',
  templateUrl: './add-account-modal.component.html',
  styleUrls: ['./add-account-modal.component.scss']
})
export class AddAccountModalComponent implements OnInit {
  @ViewChild(NodeDirective, {static: true}) nodeDirective!: NodeDirective;
  viewContainerRef?: ViewContainerRef
  submitBtnEnabled = true;
  searchedAccounts: { text: string, value: string }[] = []

  constructor(
    public alertService: AlertService,
    public treeService: ChartOfAccountsService,
    private modalService: ModalService,
    private translationService: TranslationService
    ) {
  }

  lastSelectedAccountId?: string

  ngOnInit(): void {
  }

  arabicName = new FormControl('', [Validators.required])
  englishName = new FormControl('', [Validators.required])
  currency = new FormControl(null)

  addAccountForm = new FormGroup({
    arabicName: this.arabicName,
    englishName: this.englishName,
    currency: this.currency,
  });

  addAccount() {
    this.submitBtnEnabled = false;
    this.alertService.alertColor = "blue";
    this.alertService.alertShown = true;
    this.alertService.alertMsg = "Creating account, please wait";
    const parentId = this.lastSelectedAccountId ?? this.treeService.operatingAccount?.id;
    this.treeService.addAccount({
      parentId,
      arabicName: this.arabicName.value!,
      englishName: this.englishName.value!,
      currency: this.currency.value!,
    })
      .subscribe({
        next: _ => {
          this.alertService.alertColor = 'green';
          this.alertService.alertMsg = 'Account created successfully.';
          setTimeout(() => {
            this.alertService.alertShown = false;
            this.modalService.toggleModal('addAccount', false);
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

  searchAccounts(term: string) {
    this.treeService.searchAccount(term).subscribe(fetchedAccounts => {

      this.searchedAccounts = fetchedAccounts.map(a => {
        const text = this.isEnglish ? a.englishName : a.arabicName;
        return {
          text,
          value: a.id
        }
      })
      console.log(this.searchedAccounts)
    })
  }

  get isEnglish() {
    return this.translationService.isEnglishLanguage()
  }
}
