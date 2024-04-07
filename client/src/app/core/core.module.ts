import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {NavComponent} from "./nav/nav.component";
import {RouterModule} from "@angular/router";
import {LanguageSwitcherComponent} from './language-switcher/language-switcher.component';
import {SharedModule} from "../shared/shared.module";
import { SidebarComponent } from './sidebar/sidebar.component';


@NgModule({
  declarations: [NavComponent, LanguageSwitcherComponent, SidebarComponent],
  imports: [
    CommonModule,
    RouterModule,
    SharedModule
  ],
  exports: [NavComponent, SidebarComponent]
})
export class CoreModule {
}
