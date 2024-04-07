import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {SharedModule} from "../shared/shared.module";
import {LoginModalComponent} from './components/login-modal/login-modal.component';
import {RegisterModalComponent} from './components/register/register-modal.component';
import {ReactiveFormsModule} from "@angular/forms";
import {TranslateModule} from "@ngx-translate/core";


@NgModule({
  declarations: [
    LoginModalComponent,
    RegisterModalComponent
  ],
  exports: [
    LoginModalComponent,
    RegisterModalComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    TranslateModule
  ]
})
export class AuthModule {}
