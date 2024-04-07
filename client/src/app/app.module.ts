import {NgModule} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';

import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {HomeComponent} from './home/home.component';
import {CoreModule} from "./core/core.module";
import {HTTP_INTERCEPTORS, HttpClient, HttpClientModule} from "@angular/common/http";
import {AuthModule} from "./auth/auth.module";
import {ChartOfAccountsModule} from "./chart-of-accounts/chart-of-accounts.module";
import {EntriesModule} from "./entries/entries.module";
import {GeneralLedgerModule} from "./general-ledger/general-ledger.module";
import {SharedModule} from "./shared/shared.module";
import {TrialBalanceModule} from "./trial-balance/trial-balance.module";
import {IncomeStatementModule} from "./income-statement/income-statement.module";
import {TranslateHttpLoader} from '@ngx-translate/http-loader';
import {LanguageInterceptor} from "./interceptors/language.interceptor";
import {BalanceSheetModule} from "./balance-sheet/balance-sheet.module";
import {AuthInterceptor} from "./auth/interceptors/auth.interceptor";
import {UsersManagementModule} from "./users-management/users-management.module";
import {LoadingSpinnerInterceptor} from "./interceptors/loading-spinner.interceptor";
import {CustomersModule} from "./customers/customers.module";
import {SuppliersModule} from "./suppliers/suppliers.module";
import {InvoicesModule} from "./invoices/invoices.module";
import {DataManagementModule} from "./data-management/data-management.module";

export function HttpLoaderFactory(http: HttpClient): TranslateHttpLoader {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    SharedModule,
    CoreModule,
    AuthModule,
    ChartOfAccountsModule,
    EntriesModule,
    GeneralLedgerModule,
    TrialBalanceModule,
    IncomeStatementModule,
    BalanceSheetModule,
    UsersManagementModule,
    CustomersModule,
    SuppliersModule,
    InvoicesModule,
    DataManagementModule,
    AppRoutingModule,
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: LanguageInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: LoadingSpinnerInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
