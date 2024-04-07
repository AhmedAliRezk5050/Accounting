import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SuppliersHomeComponent } from './suppliers-home.component';

describe('SuppliersHomeComponent', () => {
  let component: SuppliersHomeComponent;
  let fixture: ComponentFixture<SuppliersHomeComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [SuppliersHomeComponent]
    });
    fixture = TestBed.createComponent(SuppliersHomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
