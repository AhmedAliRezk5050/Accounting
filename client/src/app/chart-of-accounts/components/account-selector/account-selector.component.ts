import {Component, EventEmitter, Input, Output} from '@angular/core';
import IAccount from "../../../shared/models/account.model";

@Component({
  selector: 'app-account-selector',
  templateUrl: './account-selector.component.html',
  styleUrls: ['./account-selector.component.scss']
})
export class AccountSelectorComponent {
  @Input() accounts: IAccount[] = []

  @Output() accountSelected = new EventEmitter<string>()

  selectAccount(event: Event) {
    this.accountSelected.emit((event.target as HTMLSelectElement).value)
  }
}
