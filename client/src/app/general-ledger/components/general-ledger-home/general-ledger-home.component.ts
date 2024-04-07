import {Component, OnInit} from '@angular/core';
import {ModalService} from "../../../shared/services/modal.service";
import {GeneralLedgerService} from "../../services/general-ledger.service";
import IGeneralLedger from "../../../shared/models/general-ledger";
import GeneralLedgerModalType from "../../models/general-ledger-modal-type";
import {TranslationService} from "../../../services/translation.service";
import {AlertService} from "../../../shared/services/alert.service";

@Component({
  selector: 'app-general-ledger-home',
  templateUrl: './general-ledger-home.component.html',
  styleUrls: ['./general-ledger-home.component.scss']
})
export class GeneralLedgerHomeComponent implements OnInit{
  generalLedger: IGeneralLedger | null = null;
  btnDisabled = false;
  constructor(
    public modalService: ModalService,
    private generalLedgerService: GeneralLedgerService,
    private translationService: TranslationService,
    private alertService: AlertService
    ) {
  }

  ngOnInit(): void {
    this.generalLedgerService.generalLedgerFetched.subscribe(response => {
      debugger
      this.generalLedger = response;
    })
  }

  openSearchGeneralLedgerModal = () => {
    this.generalLedgerService.generalLedgerModalType = GeneralLedgerModalType.Search
    this.modalService.toggleModal('generalLedger', true);
  }
  openGeneralLedgerPdfModal = () => {
    this.generalLedgerService.generalLedgerModalType = GeneralLedgerModalType.Pdf;
    this.modalService.toggleModal('generalLedger', true);
  }

  openGeneralLedgerExcelModal = () => {
    this.generalLedgerService.generalLedgerModalType = GeneralLedgerModalType.Excel;
    this.modalService.toggleModal('generalLedger', true);
  }

  get isEnglish() {
    return this.translationService.isEnglishLanguage()
  }

  printGeneralLedger() {
    this.btnDisabled = true;
    this.generalLedgerService.getGeneralLedgerPdf(this.generalLedger?.accountId!, this.generalLedger?.toDate!).subscribe({
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
