export default interface IPagedResponseModel<T> {
  items: T[];
  totalCount: number
}
