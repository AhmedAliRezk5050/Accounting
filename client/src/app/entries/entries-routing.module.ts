import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {EntriesHomeComponent} from "./components/entries-home/entries-home.component";
import {AddEntryComponent} from "./components/add-entry/add-entry.component";
import {EditEntryComponent} from "./components/edit-entry/edit-entry.component";
import {NotFoundComponent} from "../shared/components/not-found/not-found.component";
import {authGuard} from "../guards/auth.guard";

const routes: Routes = [
  {
    path: 'entries',
    component: EntriesHomeComponent,
    canActivate: [authGuard]
  },
  {
    path: 'entries/add-entry',
    component: AddEntryComponent,
    canActivate: [authGuard]
  },
  {
    path: 'entries/edit-entry/:id',
    component: EditEntryComponent,
    canActivate: [authGuard]
  },

];

@NgModule({

  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class EntriesRoutingModule { }
