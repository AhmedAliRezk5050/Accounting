import {Component, OnInit} from '@angular/core';
import {ModalService} from "../../../shared/services/modal.service";
import {EntriesService} from "../../services/entries.service";
import {AlertService} from "../../../shared/services/alert.service";
import IEntry from "../../../shared/models/entry.model";
import {TranslationService} from "../../../services/translation.service";
import IPagedResponseModel from "../../../shared/models/paged-response.model";

@Component({
  selector: 'app-entries-home',
  templateUrl: './entries-home.component.html',
  styleUrls: ['./entries-home.component.scss']
})
export class EntriesHomeComponent implements OnInit {
  pagedResponse: IPagedResponseModel<IEntry> = {
    items: [],
    totalCount: 0
  };
  btnDisabled = false;

  constructor(
    public modalService: ModalService,
    public entriesService: EntriesService,
    public alertService: AlertService,
    private translationService: TranslationService
  ) {
  }

  ngOnInit(): void {
    this.entriesService.entriesFetchedSubject.subscribe(res => {
      this.pagedResponse = res;
    })

    this.entriesService.fetchEntries();
  }

  deleteEntry(id: string) {
    const deleteConfirmed = confirm("هل انت متاكد من حذف القيد؟");
    if (!deleteConfirmed) return;

    this.alertService.alertShown = true;
    this.alertService.alertColor = 'blue';
    this.alertService.alertMsg = 'Deleting entry. Please wait.';
    this.entriesService.deleteEntry(id).subscribe({
      next: _ => {
        this.alertService.alertColor = 'green';
        this.alertService.alertMsg = 'Deleted entry successfully';
        this.entriesService.fetchEntries();
      },
      error: _ => {
        this.alertService.alertColor = 'red';
        this.alertService.alertMsg = 'Deletion failed. Try again later';
      }
    });

    setTimeout(() => {
      this.alertService.alertShown = false;
    }, 1000)
  }

  postEntry(id: string) {
    const deleteConfirmed = confirm("هل انت متاكد من ترحيل القيد؟");
    if (!deleteConfirmed) return;

    this.alertService.alertShown = true;
    this.alertService.alertMsg = 'Posting entry. Please wait.';
    this.entriesService.postEntry(id).subscribe({
      next: _ => {
        this.alertService.alertColor = 'green';
        this.alertService.alertMsg = 'Posted entry successfully';
        this.entriesService.fetchEntries();
        setTimeout(() => {
          this.modalService.toggleModal('confirm', false)
        }, 1000)
      },
      error: _ => {
        this.alertService.alertColor = 'red';
        this.alertService.alertMsg = 'Posting failed. Try again later';
      }
    });

    setTimeout(() => {
      this.alertService.alertShown = false;
    }, 1000)
  }

  get isEnglish() {
    return this.translationService.isEnglishLanguage()
  }

  printEntry(id: string) {
    this.btnDisabled = true;
    this.entriesService.getEntryPdf(id).subscribe({
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

  protected readonly Math = Math;
}
