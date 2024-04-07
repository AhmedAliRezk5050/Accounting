import ConfirmModalType from "./confirm-modal-type";

export default interface ConfirmModalInfo {
  type: ConfirmModalType;
  message: string;
  title: string;
  confirmBtnText: string,
  info: {type: string, id: string}
}
