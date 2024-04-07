import {Component, OnInit, ViewChild} from '@angular/core';
import {NodeDirective} from "../../../shared/directives/node.directive";
import {ChartOfAccountsService} from "../../services/chart-of-accounts.service";
import {TranslationService} from "../../../services/translation.service";
import IAccount from "../../../shared/models/account.model";
import AppLanguage from "../../../shared/models/app-language.enum";
import {AccountNodeComponent} from "../account-node/account-node.component";

@Component({
  selector: 'app-banks',
  templateUrl: './banks.component.html',
  styleUrls: ['./banks.component.scss']
})
export class BanksComponent implements OnInit {
  @ViewChild(NodeDirective, { static: true }) nodeDirective!: NodeDirective;

  constructor(
    public chartOfAccountsService: ChartOfAccountsService,
    private translationService: TranslationService
  ) {}

  ngOnInit(): void {
    this.chartOfAccountsService.getBankAccounts().subscribe({
      next: accounts => {
        this.chartOfAccountsService.fetchedAccountsTree = accounts;
        accounts.forEach(a => this.traverseTreeAndAddAccountNode(a))
      },
      error: _ => {
      }
    })
  }
  addAccountNode = (account: IAccount) => {
    const viewContainerRef = this.nodeDirective.viewContainerRef;
    const componentRef = viewContainerRef.createComponent(AccountNodeComponent);
    componentRef.instance.account = account;
  }

  traverseTreeAndAddAccountNode = (account: IAccount) => {
    this.addAccountNode(account);
    if (!account.subAccounts) {
      return;
    }
    account.subAccounts.forEach(a => {
      this.traverseTreeAndAddAccountNode(a)
    })
  }

  get isEnglish() {
    return this.translationService.getCurrentLanguage() === AppLanguage.EN;
  }
}
