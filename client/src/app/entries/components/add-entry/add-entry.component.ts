import {Component} from '@angular/core';
import {ChartOfAccountsService} from "../../../chart-of-accounts/services/chart-of-accounts.service";
import IAccount from "../../../shared/models/account.model";
import {FormArray, FormControl, FormGroup, Validators} from "@angular/forms";
import {EntriesService} from "../../services/entries.service";
import {AlertService} from "../../../shared/services/alert.service";
import {TranslationService} from "../../../services/translation.service";

@Component({
  selector: 'app-add-entry',
  templateUrl: './add-entry.component.html',
  styleUrls: ['./add-entry.component.scss']
})
export class AddEntryComponent {
  searchInputItems: { text: string, value: string }[] = []
  searchedAccounts: IAccount[] = []
  accountIdSelected?: boolean

  date = new FormControl('', [Validators.required])
  description = new FormControl('', [Validators.required])
  isOpening = new FormControl(false)
  entryDetails = new FormArray([], Validators.minLength(2));


  addEntryForm = new FormGroup({
    date: this.date,
    description: this.description,
    isOpening: this.isOpening,
    entryDetails: this.entryDetails
  });

  constructor(
    private chartOfAccountsService: ChartOfAccountsService,
    private entriesService: EntriesService,
    public alertService: AlertService,
    private translationService: TranslationService
  ) {
  }

  ngOnInit() {
  }

  get totalCredit(): number {
    const total = this.entryDetails.controls.reduce((total, control: FormControl) => {
      const creditValue = control.get('credit')?.value;
      return total + (creditValue ? +creditValue : 0);
    }, 0);
    return +total.toFixed(2);
  }

  get totalDebit(): number {
    const total =  this.entryDetails.controls.reduce((total, control: FormControl) => {
      const debitValue = control.get('debit')?.value;
      return total + (debitValue ? +debitValue : 0);
    }, 0);

    return +total.toFixed(2);
  }

  get isBalancedEntry() {
    return this.totalCredit == this.totalDebit;
  }

  get unbalancedEntryAmount() {
    return Math.abs(this.totalCredit - this.totalDebit).toFixed(2);
  }

  addEntryDetail() {
    const entryDetailFormGroup = new FormGroup({
      debit: new FormControl(0),
      credit: new FormControl(0),
      description: new FormControl(null),
      accountId: new FormControl(null, [Validators.required])
    });
    this.entryDetails.push(entryDetailFormGroup as never);
  }

  removeEntryDetail(index: number) {
    this.entryDetails.removeAt(index);
  }

  addEntry = () => {
    this.alertService.alertShown = true;
    this.alertService.alertColor = 'blue';
    this.alertService.alertMsg = this.translationService.getTranslatedWord('adding');
    this.entriesService.addEntry({
      description: this.description.value,
      entryDate: this.date.value!,
      isOpening: this.isOpening.value,
      entryDetails: this.addEntryForm.get('entryDetails')?.value
    }).subscribe({
      next: _ => {
        this.alertService.alertColor = 'green';
        this.alertService.alertMsg = this.translationService.getTranslatedWord('success-add');
        this.addEntryForm.reset();
        while (this.entryDetails.length !== 0) {
          this.entryDetails.removeAt(0);
        }
        this.isOpening.setValue(false);
        setTimeout(() => {
          this.alertService.alertColor = '';
          this.alertService.alertMsg = '';
          this.alertService.alertShown = false;
        }, 1000);
      },
      error: err => {
        this.alertService.alertColor = 'red';
        this.alertService.alertMsg = this.translationService.getTranslatedWord('fail-add');
      }
    })
  }


  searchAccounts(term: string) {
    this.chartOfAccountsService.searchAccount(term, true).subscribe(fetchedAccounts => {
      this.searchedAccounts = fetchedAccounts;
      this.searchInputItems = fetchedAccounts.map(a => ({
        text: a.code.toString() + " -- " + a.arabicName,
        value: a.id
      }))
    })
  }

  onAccountSelect(id: string, index: number) {
    this.entryDetails.controls.forEach((x: FormControl, i) => {
      if (index === i) {
        x.get('accountId')?.setValue(id)
      }
    });
  }
}
