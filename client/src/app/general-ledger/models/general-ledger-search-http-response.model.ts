import IGeneralLedgerEntrySearchHttpResponse from "./general-ledger-entry-search-http-response";

export default interface IGeneralLedgerSearchHttpResponse {

  data: IGeneralLedgerEntrySearchHttpResponse[]
  totalDebit: number;
  totalCredit: number;
  totalBalance: number;
  fromDate: string;
  toDate: string;
}
