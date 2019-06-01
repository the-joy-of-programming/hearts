import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ScridgetsComponent } from './scridgets.component';

describe('ScridgetsComponent', () => {
  let component: ScridgetsComponent;
  let fixture: ComponentFixture<ScridgetsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ScridgetsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ScridgetsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
