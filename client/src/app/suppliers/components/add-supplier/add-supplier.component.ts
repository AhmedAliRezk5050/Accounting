import { Component } from '@angular/core';
import {AlertService} from "../../../shared/services/alert.service";
import {TranslationService} from "../../../services/translation.service";
import {SuppliersService} from "../../services/suppliers.service";
import {FormControl, FormGroup, Validators} from "@angular/forms";

@Component({
  selector: 'app-add-supplier',
  templateUrl: './add-supplier.component.html',
  styleUrls: ['./add-supplier.component.scss']
})
export class AddSupplierComponent {
  constructor(
    public alertService: AlertService,
    private translationService: TranslationService,
    private suppliersService: SuppliersService
  ) {}

  arabicName = new FormControl('', [Validators.required])
  englishName = new FormControl('', [Validators.required])
  phoneNumber = new FormControl('', [Validators.required])
  bankAccountNumber = new FormControl('', [Validators.required])
  bankName = new FormControl('', [Validators.required])
  taxNumber = new FormControl('', [Validators.required])
  supplierType = new FormControl('', [Validators.required])

  addSupplierForm = new FormGroup({
    arabicName: this.arabicName,
    englishName: this.englishName,
    phoneNumber: this.phoneNumber,
    bankAccountNumber: this.bankAccountNumber,
    bankName: this.bankName,
    taxNumber: this.taxNumber,
    SupplierType: this.supplierType
  });

  addSupplier() {
    this.alertService.alertShown = true;
    this.alertService.alertColor = 'blue';
    this.alertService.alertMsg = this.translationService.getTranslatedWord('adding');

    this.suppliersService.addSupplier({
      arabicName: this.arabicName.value,
      englishName: this.englishName.value,
      phoneNumber: this.phoneNumber.value,
      bankAccountNumber: this.bankAccountNumber.value,
      bankName: this.bankName.value,
      taxNumber: this.taxNumber.value,
      SupplierType: this.supplierType.value,

    }).subscribe({
      next: _ => {
        this.alertService.alertColor = 'green';
        this.alertService.alertMsg = this.translationService.getTranslatedWord('success-add');
        this.addSupplierForm.reset();

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
