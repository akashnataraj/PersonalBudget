import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BudgetConfigComponent } from './budget-config.component';

describe('BudgetConfigComponent', () => {
  let component: BudgetConfigComponent;
  let fixture: ComponentFixture<BudgetConfigComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BudgetConfigComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BudgetConfigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
