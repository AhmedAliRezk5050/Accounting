import IExpensesAccountBalance from "./expenses-account-balance.model";

export default interface IIncomeStatement{
  revenuesAmount: number;
  revenueReturnsAmount: number;
  operatingExpensesAmount: number;
  administrativeExpensesAmount: number;
  generalExpensesAmount: number;
  sellingExpensesAmount: number;
  financingExpensesAmount: number;
  capitalRevenuesAmount: number;
  accidentCompensationRevenuesAmount: number;
  netIncome: number;
  fromDate: string;
  toDate: string;
}
