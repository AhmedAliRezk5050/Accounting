import {Component, inject, Input, OnInit, ViewChild} from '@angular/core';
import IAccount from "../../../shared/models/account.model";
import {NodeDirective} from "../../../shared/directives/node.directive";
import {ChartOfAccountsService} from "../../services/chart-of-accounts.service";
import {ModalService} from "../../../shared/services/modal.service";

@Component({
  selector: 'app-tree-container',
  templateUrl: './tree-container.component.html',
  styleUrls: ['./tree-container.component.scss']
})
export class TreeContainerComponent{
  chartOfAccountsService = inject(ChartOfAccountsService)
  modalService = inject(ModalService)
  accounts: IAccount[] = []
  @ViewChild(NodeDirective, {static: true}) nodeDirective!: NodeDirective;

  ngOnInit(): void {
    this.modalService.modalVisibilityChanged.subscribe(status => {
      if (!status) {
        if (this.chartOfAccountsService.operatingAccount) {
          this.chartOfAccountsService.operatingAccount = undefined;
        }
      }
    })

    this.chartOfAccountsService.fetchAccountsByLevel(1)
      .subscribe(fetchedAccounts => {
        this.accounts = fetchedAccounts
      })
    this.chartOfAccountsService.crudExecuted.subscribe(() => {
      this.chartOfAccountsService.fetchAccountsByLevel(1)
        .subscribe(fetchedAccounts => {
          this.accounts = fetchedAccounts
        })

    })
  }
}
