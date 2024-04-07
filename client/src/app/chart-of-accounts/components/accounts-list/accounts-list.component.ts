import { AfterViewInit, Component, inject, OnInit, ViewChild } from '@angular/core';
import { ChartOfAccountsService } from "../../services/chart-of-accounts.service";
import { NodeDirective } from "../../../shared/directives/node.directive";
import { AccountNodeComponent } from "../account-node/account-node.component";
import IAccount from "../../../shared/models/account.model";
import { AlertService } from "../../../shared/services/alert.service";
import {TranslationService} from "../../../services/translation.service";
import AppLanguage from "../../../shared/models/app-language.enum";

@Component({
  selector: 'app-accounts-list',
  templateUrl: './accounts-list.component.html',
  styleUrls: ['./accounts-list.component.scss']
})
export class AccountsListComponent implements OnInit {
  @ViewChild(NodeDirective, { static: true }) nodeDirective!: NodeDirective;

  constructor(
    public chartOfAccountsService: ChartOfAccountsService,
    private translationService: TranslationService
  ) {

  }

  ngOnInit(): void {
    this.chartOfAccountsService.fetchAccountsTree().subscribe({
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
