export default interface IGeneralLedgerEntry {
  description: string;
  debit: number;
  credit: number;
  balance: number;
  date: string;
  entryId: number
}
