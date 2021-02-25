import { TestBed } from '@angular/core/testing';

import { StockRestService } from './stock-rest.service';

describe('StockRestService', () => {
  let service: StockRestService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StockRestService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
