import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { EntriesRoutingModule } from './entries-routing.module';
import { EntriesHomeComponent } from './components/entries-home/entries-home.component';
import {SharedModule} from "../shared/shared.module";
import {ReactiveFormsModule} from "@angular/forms";
import {AddEntryComponent} from "./components/add-entry/add-entry.component";
import { EditEntryComponent } from './components/edit-entry/edit-entry.component';


@NgModule({
  declarations: [
    EntriesHomeComponent,
    AddEntryComponent,
    EditEntryComponent
  ],
    exports: [
    ],
  imports: [
    CommonModule,
    SharedModule,
    EntriesRoutingModule,
    ReactiveFormsModule
  ]
})
export class EntriesModule { }
