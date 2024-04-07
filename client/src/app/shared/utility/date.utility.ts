import {DateTime} from "luxon";

export default class DateUtility {
  public static IsoToUtc(dateAsString: string): DateTime {
    return DateTime.fromISO(dateAsString, {zone: Intl.DateTimeFormat().resolvedOptions().timeZone}).toUTC()
  }
}
