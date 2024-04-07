import {Component, Input, OnInit, ViewChild} from '@angular/core';
import IAccount from "../../../shared/models/account.model";
import {ChartOfAccountsService} from "../../services/chart-of-accounts.service";
import {NodeDirective} from "../../../shared/directives/node.directive";
import {ModalService} from "../../../shared/services/modal.service";
import {TranslationService} from "../../../services/translation.service";
import {IconProp} from "@fortawesome/fontawesome-svg-core";
import AppLanguage from "../../../shared/models/app-language.enum";

@Component({
  selector: 'app-tree-node',
  templateUrl: './tree-node.component.html',
  styleUrls: ['./tree-node.component.scss']
})
export class TreeNodeComponent implements OnInit {
  nodeIsOpen = false
  @Input() account?: IAccount
  @ViewChild(NodeDirective, {static: true}) nodeDirective!: NodeDirective;

  constructor(
    private treeService: ChartOfAccountsService,
    private modalService: ModalService,
    private translationService: TranslationService
  ) {
  }

  onNodeClick = () => {
    this.nodeIsOpen = !this.nodeIsOpen
    const viewContainerRef = this.nodeDirective.viewContainerRef;
    if (!this.nodeIsOpen) {
      viewContainerRef.clear();
      return
    }

    if (this.account && this.nodeIsOpen) {
      this.treeService.fetchAccountsByParent(this.account.id)
        .subscribe(response => {
          response.accounts.forEach(account => {
            const componentRef = viewContainerRef.createComponent(TreeNodeComponent);
            componentRef.instance.account = account;
          })
        })
    }

  }

  addAccount(event: Event) {
    event.stopPropagation();
    this.treeService.operatingAccount = this.account
    this.modalService.toggleModal('addAccount', true);
  }

  viewAccount(event: Event) {
    event.stopPropagation();
    this.treeService.operatingAccount = this.account
    this.modalService.toggleModal('accountDetails', true)
  }

  deleteAccount = (event: Event) => {
    event.stopPropagation();
    this.treeService.operatingAccount = this.account
    this.modalService.toggleModal('deleteAccount', true)
  }

  EditAccount(event: Event) {
    event.stopPropagation();
    this.treeService.operatingAccount = this.account
    this.modalService.toggleModal('editAccount', true)
  }

  ngOnInit(): void {
    this.translationService.languageChanged.subscribe(lang => {
      this.arrowIconProp= ['fas', lang === AppLanguage.EN ? 'arrow-right' : 'arrow-left']
    })
  }

  get isEnglishMode() {
    return this.translationService.isEnglishLanguage()
  }

  arrowIconProp:IconProp = ['fas', this.isEnglishMode ? 'arrow-right' : 'arrow-left']
}
