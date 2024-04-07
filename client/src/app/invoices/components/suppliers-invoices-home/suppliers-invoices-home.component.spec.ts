import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SuppliersInvoicesHomeComponent } from './suppliers-invoices-home.component';

describe('SuppliersInvoicesHomeComponent', () => {
  let component: SuppliersInvoicesHomeComponent;
  let fixture: ComponentFixture<SuppliersInvoicesHomeComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [SuppliersInvoicesHomeComponent]
    });
    fixture = TestBed.createComponent(SuppliersInvoicesHomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
