import {Component, OnInit} from '@angular/core';
import {ModalService} from "../../../shared/services/modal.service";
import {ChartOfAccountsService} from "../../services/chart-of-accounts.service";
import IAccount from "../../../shared/models/account.model";

@Component({
  selector: 'app-chart-of-accounts-home',
  templateUrl: './chart-of-accounts-home.component.html',
  styleUrls: ['./chart-of-accounts.component.scss']
})
export class ChartOfAccountsHomeComponent {


  constructor(public modalService: ModalService) {
  }

}


