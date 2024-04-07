import IEntryDetails from "./entry-details.model";

export default interface IEntry {
  id: string
  totalDebit: number;
  totalCredit: number;
  description: string;
  entryDetails: IEntryDetails[]
  entryDate: string;
  isPosted: boolean
  isOpening: boolean
}
