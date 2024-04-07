import IUpdateEntryDetailsRequestBody from "./update-entry-details-request-body";

export default interface IUpdateEntryRequestBody {
  description: string;
  entryDate: string;
  isOpening: boolean
  entryDetails: IUpdateEntryDetailsRequestBody[]
}
