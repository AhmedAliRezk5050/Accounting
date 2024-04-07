import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddSupplierInvoiceComponent } from './add-supplier-invoice.component';

describe('AddSupplierInvoiceComponent', () => {
  let component: AddSupplierInvoiceComponent;
  let fixture: ComponentFixture<AddSupplierInvoiceComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AddSupplierInvoiceComponent]
    });
    fixture = TestBed.createComponent(AddSupplierInvoiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
