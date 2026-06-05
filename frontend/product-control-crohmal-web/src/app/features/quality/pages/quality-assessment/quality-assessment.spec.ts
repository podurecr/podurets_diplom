import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QualityAssessment } from './quality-assessment';

describe('QualityAssessment', () => {
  let component: QualityAssessment;
  let fixture: ComponentFixture<QualityAssessment>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [QualityAssessment],
    }).compileComponents();

    fixture = TestBed.createComponent(QualityAssessment);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
