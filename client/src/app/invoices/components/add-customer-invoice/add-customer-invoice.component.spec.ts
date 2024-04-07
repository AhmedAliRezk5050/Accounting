import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddCustomerInvoiceComponent } from './add-customer-invoice.component';

describe('AddCustomerInvoiceComponent', () => {
  let component: AddCustomerInvoiceComponent;
  let fixture: ComponentFixture<AddCustomerInvoiceComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AddCustomerInvoiceComponent]
    });
    fixture = TestBed.createComponent(AddCustomerInvoiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
