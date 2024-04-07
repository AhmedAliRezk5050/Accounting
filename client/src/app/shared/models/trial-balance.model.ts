import ITrialBalanceRecord from "./trial-balance-record.model";

export default interface ITrialBalance {
  trialBalanceQueryResponseList: ITrialBalanceRecord[]
  totalDebit: number;
  totalCredit: number;
  totalOpeningDebit: number;
  totalOpeningCredit: number;
  totalEndingDebit: number;
  totalEndingCredit: number;
  fromDate: string;
  toDate: string;
}
