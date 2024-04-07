import IInvoiceOwner from "./invoice-owner.model";

export default interface IInvoice {
  id: number;
  invoiceNumber: string;
  date: string;
  amount: number;
  tax: number;
  totalAmount: number;
  notes?: string;
  customer?: IInvoiceOwner
  supplier?: IInvoiceOwner
}
