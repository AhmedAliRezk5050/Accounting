import {Component, EventEmitter, Output} from '@angular/core';
import {ModalService} from "../../services/modal.service";
import ConfirmModalType from "../../models/confirm-modal-type";
import {AlertService} from "../../services/alert.service";

@Component({
  selector: 'app-confirmation-modal',
  templateUrl: './confirmation-modal.component.html',
  styleUrls: ['./confirmation-modal.component.scss']
})
export class ConfirmationModalComponent {
  confirmModalType = ConfirmModalType

  constructor(public modalService: ModalService, public alertService: AlertService) {}

  confirm = () => {
    const {type, id} = this.modalService.confirmModalInfo.info;
    this.modalService.confirmModalSubmitted.emit({type, id})
  }

  cancel() {
    this.modalService.toggleModal('confirm', false)
  }
}
