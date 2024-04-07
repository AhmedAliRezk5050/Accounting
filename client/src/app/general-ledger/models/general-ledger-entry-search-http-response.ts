export default interface IGeneralLedgerEntrySearchHttpResponse {
  accountCode: number
  accountArabicName: string
  accountEnglishName: string
  date: string
  entryId: string
  description: string
  debit: number
  credit: number
  balance: number
}
