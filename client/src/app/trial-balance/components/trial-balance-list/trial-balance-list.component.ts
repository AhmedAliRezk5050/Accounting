import {Component, Input} from '@angular/core';
import ITrialBalanceRecord from "../../../shared/models/trial-balance-record.model";
import ITrialBalance from "../../../shared/models/trial-balance.model";
import {TranslationService} from "../../../services/translation.service";

@Component({
  selector: 'app-trial-balance-list',
  templateUrl: './trial-balance-list.component.html',
  styleUrls: ['./trial-balance-list.component.scss']
})
export class TrialBalanceListComponent {
  @Input() trialBalance!: ITrialBalance


  constructor(private translationService: TranslationService) {
  }

  get isEnglish() {
    return this.translationService.isEnglishLanguage()
  }
}
