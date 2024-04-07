import {Component, OnInit, ViewChild} from '@angular/core';
import {NodeDirective} from "../../../shared/directives/node.directive";
import {SuppliersService} from "../../services/suppliers.service";
import IAccount from "../../../shared/models/account.model";
import {AccountNodeComponent} from "../../../chart-of-accounts/components/account-node/account-node.component";

@Component({
  selector: 'app-suppliers-home',
  templateUrl: './suppliers-home.component.html',
  styleUrls: ['./suppliers-home.component.scss']
})
export class SuppliersHomeComponent implements OnInit{
  @ViewChild(NodeDirective, { static: true }) nodeDirective!: NodeDirective;

  constructor(private suppliersService: SuppliersService) {
  }

  ngOnInit(): void {
    this.suppliersService.getSupplierAccounts().subscribe({
      next: accounts => {
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
}
