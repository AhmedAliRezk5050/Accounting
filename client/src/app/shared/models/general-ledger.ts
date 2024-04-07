import IGeneralLedgerEntry from "./general-ledger-entry";

export default interface IGeneralLedger {
  fromDate: string
  toDate: string
  accountCode: number;
  accountArabicName: string;
  accountEnglishName: string;
  data: IGeneralLedgerEntry[]
  totalDebit: number;
  totalCredit: number;
  totalBalance: number;
  accountId: number;
}
