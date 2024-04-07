import IInvoiceOwner from "./invoice-owner.model";

export default interface IAccount {
  id: string;
  code: number;
  englishName: string;
  arabicName: string;
  currency: string;
  accountLevel: number;
  debit: number;
  credit: number;
  balance: number;
  isMain: boolean;
  hasEntry: boolean;
  parentId?: string;
  subAccounts?: IAccount[];

  customer?: IInvoiceOwner
  supplier?: IInvoiceOwner
}
