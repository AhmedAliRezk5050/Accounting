import {Component, Input} from '@angular/core';
import {FormControl} from "@angular/forms";

@Component({
  selector: 'app-form-input',
  templateUrl: './form-input.component.html',
  styleUrls: ['./form-input.component.scss']
})
export class FormInputComponent {
  @Input() inputFormControl = new FormControl();
  @Input() type = 'text'
  @Input() placeholder = ''
  @Input() name = ''
  @Input() label = ''
  @Input() validationErrorMessage = ''

  @Input() required = true;
  @Input() disabled = false;
}
