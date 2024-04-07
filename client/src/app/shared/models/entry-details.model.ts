import IAccount from "./account.model";

export default interface IEntryDetails {
  id: string;
  debit: number;
  credit: number;
  account: IAccount;
  description: string;
}
