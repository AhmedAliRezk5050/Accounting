export default interface ITrialBalanceRecord {
  accountCode: number;
  accountArabicName: string;
  accountEnglishName: string;
  openingDebit: number;
  openingCredit: number;
  debit: number;
  credit: number;
  endingDebit: number;
  endingCredit: number;
}
