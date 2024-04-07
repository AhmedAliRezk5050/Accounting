import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IncomeStatementModalComponent } from './income-statement-modal.component';

describe('IncomeStatementModalComponent', () => {
  let component: IncomeStatementModalComponent;
  let fixture: ComponentFixture<IncomeStatementModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ IncomeStatementModalComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(IncomeStatementModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
