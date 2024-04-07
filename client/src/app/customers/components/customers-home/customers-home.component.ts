import {Component, OnInit, ViewChild} from '@angular/core';
import {CustomersService} from "../../services/customers.service";
import IAccount from "../../../shared/models/account.model";
import {AccountNodeComponent} from "../../../chart-of-accounts/components/account-node/account-node.component";
import {NodeDirective} from "../../../shared/directives/node.directive";

@Component({
  selector: 'app-customers-home',
  templateUrl: './customers-home.component.html',
  styleUrls: ['./customers-home.component.scss']
})
export class CustomersHomeComponent implements OnInit{
  @ViewChild(NodeDirective, { static: true }) nodeDirective!: NodeDirective;
  constructor(private customersService: CustomersService) {
  }

  ngOnInit(): void {
    this.customersService.getCustomerAccounts().subscribe({
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
