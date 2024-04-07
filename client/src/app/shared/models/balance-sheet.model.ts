import IBalanceSheetAccountInfo from "./balance-sheet-account-info.model";

export default interface IBalanceSheet {
  assetsAmount: number;
  liabilitiesAmount: number;
  equityAmount: number;
  assetsAccountsInfoList: IBalanceSheetAccountInfo[];
  liabilitiesAccountsInfoList: IBalanceSheetAccountInfo[];
  equityAccountsInfoList: IBalanceSheetAccountInfo[];
  fromDate: string;
  toDate: string;
  netIncome: number
}
