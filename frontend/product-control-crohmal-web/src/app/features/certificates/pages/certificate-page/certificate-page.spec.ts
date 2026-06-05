import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CertificatePage } from './certificate-page';

describe('CertificatePage', () => {
  let component: CertificatePage;
  let fixture: ComponentFixture<CertificatePage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CertificatePage],
    }).compileComponents();

    fixture = TestBed.createComponent(CertificatePage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
