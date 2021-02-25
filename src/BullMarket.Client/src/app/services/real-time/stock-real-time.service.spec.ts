import { TestBed } from '@angular/core/testing';

import { StockRealTimeService } from './stock-real-time.service';

describe('StockRealTimeService', () => {
  let service: StockRealTimeService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StockRealTimeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
