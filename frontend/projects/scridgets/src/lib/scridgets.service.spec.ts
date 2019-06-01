import { TestBed } from '@angular/core/testing';

import { ScridgetsService } from './scridgets.service';

describe('ScridgetsService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: ScridgetsService = TestBed.get(ScridgetsService);
    expect(service).toBeTruthy();
  });
});
