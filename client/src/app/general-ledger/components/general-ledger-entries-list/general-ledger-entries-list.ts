import {Component, Input} from '@angular/core';
import IGeneralLedger from "../../../shared/models/general-ledger";

@Component({
  selector: 'app-general-ledger-entries-list',
  templateUrl: './general-ledger-entries-list.html',
  styleUrls: ['./general-ledger-entries-list.scss']
})
export class GeneralLedgerEntriesList {
  @Input() generalLedger!: IGeneralLedger
}
