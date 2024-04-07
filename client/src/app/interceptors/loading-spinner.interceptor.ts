import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import {finalize, Observable} from 'rxjs';
import {LoadingSpinnerService} from "../shared/services/loading-spinner.service";

@Injectable()
export class LoadingSpinnerInterceptor implements HttpInterceptor {

  constructor(private loadingSpinnerService: LoadingSpinnerService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    this.loadingSpinnerService.show();

    return next.handle(req).pipe(
      finalize(() => this.loadingSpinnerService.hide())
    );
  }
}
