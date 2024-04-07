import {Component, Input} from '@angular/core';
import {FormControl} from "@angular/forms";

@Component({
  selector: 'app-form-input-errors',
  templateUrl: './form-input-errors.component.html',
  styleUrls: ['./form-input-errors.component.scss']
})
export class FormInputErrorsComponent {
  @Input() inputFormControl!: FormControl<any>
  @Input() label!: string

}
