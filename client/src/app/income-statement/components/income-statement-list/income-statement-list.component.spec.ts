import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IncomeStatementListComponent } from './income-statement-list.component';

describe('IncomeStatementListComponent', () => {
  let component: IncomeStatementListComponent;
  let fixture: ComponentFixture<IncomeStatementListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ IncomeStatementListComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(IncomeStatementListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
