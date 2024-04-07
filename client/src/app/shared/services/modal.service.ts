import {EventEmitter, Injectable} from '@angular/core';
import ConfirmModalInfo from "../models/confirm-modal-info";
import ConfirmModalType from "../models/confirm-modal-type";
import GeneralLedgerModalType from "../../general-ledger/models/general-ledger-modal-type";

@Injectable({
  providedIn: 'root'
})
export class ModalService {
  private modals: string[] = []

  confirmModalSubmitted = new EventEmitter<{type: string, id: string}>()

  confirmModalInfo: ConfirmModalInfo = {
    type: ConfirmModalType.Success,
    confirmBtnText: '',
    message: '',
    title: '',
    info: {
      type: '',
      id: ''
    }
  }

  modalVisibilityChanged = new EventEmitter<boolean>()

  constructor() {
  }

  isModalVisible = (id: string): boolean => !!this.modals.find(m => m === id)

  toggleModal = (id: string, status: boolean) => {
    if (status) {
      this.modals.push(id)
    } else {
      this.modals = this.modals.filter(m => m !== id);
    }
    this.modalVisibilityChanged.emit(status)
  }


}
