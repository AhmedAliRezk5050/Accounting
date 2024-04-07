import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IncomeStatementHomeComponent } from './income-statement-home.component';

describe('IncomeStatementHomeComponent', () => {
  let component: IncomeStatementHomeComponent;
  let fixture: ComponentFixture<IncomeStatementHomeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ IncomeStatementHomeComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(IncomeStatementHomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
