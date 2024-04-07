import {Component, OnInit} from '@angular/core';
import {ChartOfAccountsService} from "../../services/chart-of-accounts.service";
import IAccount from "../../../shared/models/account.model";
import {TranslationService} from "../../../services/translation.service";
import AppLanguage from "../../../shared/models/app-language.enum";
import {IconProp} from "@fortawesome/fontawesome-svg-core";

@Component({
  selector: 'app-account-details-modal',
  templateUrl: './account-details-modal.component.html',
  styleUrls: ['./account-details-modal.component.scss']
})
export class AccountDetailsModalComponent implements OnInit{
  account?: IAccount
  constructor(
    private treeService: ChartOfAccountsService,
    private translationService: TranslationService
  ) {
  }

  ngOnInit(): void {
    this.account = this.treeService.operatingAccount;
  }

  get isEnglish() {
    return this.translationService.getCurrentLanguage() === AppLanguage.EN
  }

  arrowIconProp = (this.isEnglish ?  ['fas', 'arrow-right'] : ['fas', 'arrow-left']) as IconProp;
}
