import {Injectable} from '@angular/core';
import {BehaviorSubject} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class LoadingSpinnerService {

  private loadingSubject = new BehaviorSubject<boolean>(false);
  loading$ = this.loadingSubject.asObservable();

  show() {
    if (!this.loadingSubject.value) {
      this.loadingSubject.next(true);
    }
  }

  hide() {
    if (this.loadingSubject.value) {
      this.loadingSubject.next(false);
    }
  }
}
