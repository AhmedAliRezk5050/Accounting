import {
  AfterContentInit,
  AfterViewInit,
  Component,
  ContentChild,
  ContentChildren,
  OnInit,
  QueryList,
  ViewChild,
  ViewChildren
} from '@angular/core';
import IEntry from "../../../shared/models/entry.model";
import {EntriesService} from "../../services/entries.service";
import {ActivatedRoute, Router} from "@angular/router";
import {HttpClient} from "@angular/common/http";
import IAccount from "../../../shared/models/account.model";
import {FormArray, FormControl, FormGroup, Validators} from "@angular/forms";
import {AlertService} from "../../../shared/services/alert.service";
import {TranslationService} from "../../../services/translation.service";
import {ChartOfAccountsService} from "../../../chart-of-accounts/services/chart-of-accounts.service";
import {
  DataListSearchInputComponent
} from "../../../shared/components/data-list-search-input/data-list-search-input.component";

@Component({
  selector: 'app-edit-entry',
  templateUrl: './edit-entry.component.html',
  styleUrls: ['./edit-entry.component.scss']
})
export class EditEntryComponent implements OnInit, AfterViewInit {
  @ViewChildren(DataListSearchInputComponent) dataListSearchInputComponents?: QueryList<DataListSearchInputComponent>
  entry?: IEntry
  searchInputItems: { text: string, value: string }[] = []
  searchedAccounts: IAccount[] = []
  accountIdSelected?: boolean
  entryId?: string

  date = new FormControl('', [Validators.required])
  description = new FormControl('', [Validators.required])
  isOpening = new FormControl(false)
  entryDetails = new FormArray([], Validators.minLength(2));

  editEntryForm = new FormGroup({
    date: this.date,
    description: this.description,
    isOpening: this.isOpening,
    entryDetails: this.entryDetails
  });

  constructor(
    private entriesService: EntriesService,
    private route: ActivatedRoute,
    private http: HttpClient,
    private router: Router,
    public alertService: AlertService,
    private translationService: TranslationService,
    private chartOfAccountsService: ChartOfAccountsService
  ) {
  }

  get totalCredit(): number {
    const total = this.entryDetails.controls.reduce((total, control: FormControl) => {
      const creditValue = control.get('credit')?.value;
      return total + (creditValue ? +creditValue : 0);
    }, 0);
    return +total.toFixed(2);
  }

  get totalDebit(): number {
    const total = this.entryDetails.controls.reduce((total, control: FormControl) => {
      const debitValue = control.get('debit')?.value;
      return total + (debitValue ? +debitValue : 0);
    }, 0);

    return +total.toFixed(2);
  }

  ngOnInit(): void {

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

  editEntry = () => {
    if (!this.entryId) {
      return;
    }
    this.alertService.alertShown = true;
    this.alertService.alertColor = 'blue';
    this.alertService.alertMsg = this.translationService.getTranslatedWord('adding');
    this.entriesService.updateEntry({
      description: this.description.value!,
      entryDate: this.date.value!,
      isOpening: this.isOpening.value!,
      entryDetails: this.editEntryForm.get('entryDetails')?.value!,
    }, this.entryId).subscribe({
      next: _ => {
        this.alertService.alertColor = 'green';
        this.alertService.alertMsg = this.translationService.getTranslatedWord('success-edit');

        setTimeout(() => {
          this.alertService.alertShown = false;
          this.router.navigate(['entries']);
        }, 1000);
      },
      error: err => {
        this.alertService.alertColor = 'red';
        this.alertService.alertMsg = this.translationService.getTranslatedWord('fail-edit');
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

  ngAfterViewInit(): void {
    this.route.params.subscribe(params => {
      const id = params['id'];
      if (id) {
        this.entryId = id;
        this.entriesService
          .fetchEntry(id)
          .subscribe({
            next: data => {
              if (!data) {
                this.router.navigate(['404'], {replaceUrl: true});
              }
              this.entry = data;

              // this.editEntryForm = new FormGroup({
              //   date: this.entry.entryDate.,
              //   description: this.description,
              //   isOpening: this.isOpening,
              //   entryDetails: this.entryDetails,
              // });
              this.date.setValue(this.entry.entryDate.split('T')[0]);
              this.description.setValue(this.entry.description);
              this.isOpening.setValue(this.entry.isOpening);
              this.entry.entryDetails.forEach((x, i) => {
                const entryDetailFormGroup = new FormGroup({
                  debit: new FormControl(x.debit),
                  credit: new FormControl(x.credit),
                  description: new FormControl(x.description),
                  accountId: new FormControl(x.account.id, [Validators.required])
                });
                this.entryDetails.push(entryDetailFormGroup as never);
                setTimeout(() => {
                  if (this.dataListSearchInputComponents) {
                    this.entry?.entryDetails.forEach((details, i) => {
                      const searchComponent = this.dataListSearchInputComponents?.get(i);
                      if (searchComponent) {
                        searchComponent.onValueSelected({text: details.account.arabicName, value: details.account.id})
                      }
                    })
                  }
                }, 1);
              })

            },
            error: e => {
              this.router.navigate(['404'], {replaceUrl: true});
            }
          });
      }
    });
  }
}
