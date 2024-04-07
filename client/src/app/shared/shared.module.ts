import {FaIconLibrary, FontAwesomeModule} from "@fortawesome/angular-fontawesome";

import {
  faBagShopping as fasBagShopping,
  faShoppingCart as fasShoppingCart,
  faStar as fasStar,
  faArrowRight as fasArrowRight,
  faArrowLeft as fasArrowLeft,
  faArrowDown as fasArrowDown,
  faPlus as fasPlus,
  faBookOpen as fasBookOpen,
  faPen as fasPen,
  faTrash as fasTrash,
  faPlusCircle as fasPlusCircle,
  faMagnifyingGlass as fasMagnifyingGlass,
  faPaperPlane as fasPaperPlane,
  faPrint as fasPrint,
  faFileExcel as fasFileExcel,
  faUserGear as fasUserGear,
  faFileLines as fasFileLines
} from "@fortawesome/free-solid-svg-icons";
import {
  faStar as farStar
} from "@fortawesome/free-regular-svg-icons";
import {
  faOpencart as fabOpencart,
  faReact as fabReact
} from "@fortawesome/free-brands-svg-icons";

import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import { ModalComponent } from './components/modal/modal.component';
import { TabComponent } from './components/tab/tab.component';
import { TabsContainerComponent } from './components/tabs-container/tabs-container.component';
import { FormInputComponent } from './components/forms/form-input/form-input.component';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import { AlertComponent } from './components/alert/alert.component';
import { DataListSearchInputComponent } from './components/data-list-search-input/data-list-search-input.component';
import { FormInputErrorsComponent } from './components/forms/form-input-errors/form-input-errors.component';
import { NegativeToParenthesesPipe } from './pipes/negative-to-parentheses.pipe';
import { ConfirmationModalComponent } from './components/confirmation-modal/confirmation-modal.component';
import {TranslateLoader, TranslateModule} from "@ngx-translate/core";
import {HttpClient} from "@angular/common/http";
import {HttpLoaderFactory} from "../app.module";
import { DropDownComponent } from './components/drop-down/drop-down.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { ToFixedPipe } from './pipes/to-fixed.pipe';
import { CommaPipe } from './pipes/comma.pipe';
import { PaginationComponent } from './components/pagination/pagination.component';
import { LoadingSpinnerComponent } from './components/loading-spinner/loading-spinner.component';
import {NodeDirective} from "./directives/node.directive";

@NgModule({
  declarations: [
    ModalComponent,
    TabComponent,
    TabsContainerComponent,
    FormInputComponent,
    AlertComponent,
    DataListSearchInputComponent,
    FormInputErrorsComponent,
    NegativeToParenthesesPipe,
    ConfirmationModalComponent,
    DropDownComponent,
    NotFoundComponent,
    ToFixedPipe,
    CommaPipe,
    PaginationComponent,
    LoadingSpinnerComponent,
    NodeDirective
  ],
  imports: [
    CommonModule,
    FontAwesomeModule,
    ReactiveFormsModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      }
    }),
    FormsModule,
  ],
  exports: [
    CommonModule,
    FontAwesomeModule,
    ModalComponent,
    TabComponent,
    TabsContainerComponent,
    FormInputComponent,
    AlertComponent,
    DataListSearchInputComponent,
    FormInputErrorsComponent,
    NegativeToParenthesesPipe,
    CommaPipe,
    ConfirmationModalComponent,
    TranslateModule,
    DropDownComponent,
    ToFixedPipe,
    ReactiveFormsModule,
    PaginationComponent,
    LoadingSpinnerComponent,
    NodeDirective
  ]
})
export class SharedModule {
  constructor(library: FaIconLibrary) {
    library.addIcons(
      fasStar,
      fasBagShopping,
      fasShoppingCart,
      fasArrowRight,
      fasArrowLeft,
      fasArrowDown,
      fasBookOpen,
      fasPlus,
      farStar,
      fabReact,
      fabOpencart,
      fasPen,
      fasTrash,
      fasPlusCircle,
      fasMagnifyingGlass,
      fasPaperPlane,
      fasPrint,
      fasFileExcel,
      fasUserGear,
      fasFileLines
      );
  }
}
