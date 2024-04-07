import {Component, Input} from '@angular/core';
import IIncomeStatement from "../../models/income-statement.model";
import {TranslationService} from "../../../services/translation.service";

@Component({
  selector: 'app-income-statement-list',
  templateUrl: './income-statement-list.component.html',
  styleUrls: ['./income-statement-list.component.scss']
})
export class IncomeStatementListComponent {
  @Input() incomeStatement!: IIncomeStatement


  constructor(private translationService: TranslationService) {}

  get isEnglish() {
    return this.translationService.isEnglishLanguage();
  }
}
