import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomersInvoicesHomeComponent } from './customers-invoices-home.component';

describe('CustomersInvoicesHomeComponent', () => {
  let component: CustomersInvoicesHomeComponent;
  let fixture: ComponentFixture<CustomersInvoicesHomeComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CustomersInvoicesHomeComponent]
    });
    fixture = TestBed.createComponent(CustomersInvoicesHomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
