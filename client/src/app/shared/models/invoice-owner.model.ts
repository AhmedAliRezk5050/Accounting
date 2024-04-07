import IAccount from "./account.model";

export default interface IInvoiceOwner {
  id: number;
  arabicName: string;
  englishName: string;
  phoneNumber: string;
  bankAccountNumber: string;
  bankName: string;
  taxNumber: string;
  account: IAccount
}
