import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DataManagementHomeComponent } from './data-management-home.component';

describe('DataManagementHomeComponent', () => {
  let component: DataManagementHomeComponent;
  let fixture: ComponentFixture<DataManagementHomeComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [DataManagementHomeComponent]
    });
    fixture = TestBed.createComponent(DataManagementHomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
