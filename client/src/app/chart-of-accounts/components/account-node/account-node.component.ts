import {Component, Input} from '@angular/core';
import IAccount from "../../../shared/models/account.model";
import AppLanguage from "../../../shared/models/app-language.enum";
import {TranslationService} from "../../../services/translation.service";

@Component({
  selector: 'app-account-node',
  templateUrl: './account-node.component.html',
  styleUrls: ['./account-node.component.scss']
})
export class AccountNodeComponent {
  @Input() account!: IAccount;

  constructor(private translationService: TranslationService) {
  }

  get isEnglish() {
    return this.translationService.getCurrentLanguage() === AppLanguage.EN
  }
}
