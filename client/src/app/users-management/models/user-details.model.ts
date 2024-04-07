import {ClaimDetailsModel} from "./claim-details.model";

export interface UserDetailsModel {
  id: string;
  userName: string;
  claimDetailsList: ClaimDetailsModel[]
}
