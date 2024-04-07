import {Component} from '@angular/core';
import {FormArray, FormControl, FormGroup, Validators} from "@angular/forms";
import {AlertService} from "../../../shared/services/alert.service";
import {TranslationService} from "../../../services/translation.service";
import {CustomersService} from "../../services/customers.service";

@Component({
  selector: 'app-add-customer',
  templateUrl: './add-customer.component.html',
  styleUrls: ['./add-customer.component.scss']
})
export class AddCustomerComponent {
  constructor(
              public alertService: AlertService,
              private translationService: TranslationService,
              private customersService: CustomersService
  ) {}

  arabicName = new FormControl('', [Validators.required])
  englishName = new FormControl('', [Validators.required])
  phoneNumber = new FormControl('', [Validators.required])
  bankAccountNumber = new FormControl('', [Validators.required])
  bankName = new FormControl('', [Validators.required])
  taxNumber = new FormControl('', [Validators.required])


  addCustomerForm = new FormGroup({
    arabicName: this.arabicName,
    englishName: this.englishName,
    phoneNumber: this.phoneNumber,
    bankAccountNumber: this.bankAccountNumber,
    bankName: this.bankName,
    taxNumber: this.taxNumber
  });


  addCustomer() {
    debugger
    this.alertService.alertShown = true;
    this.alertService.alertColor = 'blue';
    this.alertService.alertMsg = this.translationService.getTranslatedWord('adding');

    this.customersService.addCustomer({
      arabicName: this.arabicName.value,
      englishName: this.englishName.value,
      phoneNumber: this.phoneNumber.value,
      bankAccountNumber: this.bankAccountNumber.value,
      bankName: this.bankName.value,
      taxNumber: this.taxNumber.value
    }).subscribe({
      next: _ => {
        this.alertService.alertColor = 'green';
        this.alertService.alertMsg = this.translationService.getTranslatedWord('success-add');
        this.addCustomerForm.reset();

        setTimeout(() => {
          this.alertService.alertColor = '';
          this.alertService.alertMsg = '';
          this.alertService.alertShown = false;
        }, 1000);
      },
      error: err => {
        this.alertService.alertColor = 'red';
        this.alertService.alertMsg = this.translationService.getTranslatedWord('fail-add');
      }
    })
  }
}
