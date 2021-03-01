import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiPaths } from 'src/app/constants/api.constants';
import { StockModel } from 'src/app/models/stock/stock.model';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class StockRestService {

  constructor(private http: HttpClient) { }

  getStocks(): Observable<StockModel[]> {
    return this.http.get<StockModel[]>(environment.backEndUrl + ApiPaths.Stocks);
  }

  getStockById(id: string): Observable<StockModel> {
    return this.http.get<StockModel>(environment.backEndUrl + ApiPaths.Stocks + `/${id}`);
  }
}
