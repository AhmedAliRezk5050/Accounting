import {Component, Input} from '@angular/core';
import IBalanceSheet from "../../../shared/models/balance-sheet.model";
import {TranslationService} from "../../../services/translation.service";

@Component({
  selector: 'app-balance-sheet-list',
  templateUrl: './balance-sheet-list.component.html',
  styleUrls: ['./balance-sheet-list.component.scss']
})
export class BalanceSheetListComponent {
  @Input() balanceSheet!: IBalanceSheet


  constructor(
    private translationService: TranslationService
  ) {
  }

  get isEnglish() {
    return this.translationService.isEnglishLanguage()
  }

  setAccountLevelClass (level: number): string {
    switch (level) {
      case 1:
        return 'bg-indigo-900';
      case 2:
        return 'bg-indigo-500';
      default:
        return '';
    }
  }
}
