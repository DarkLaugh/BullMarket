import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HubEvents } from 'src/app/constants/hub.constants';
import { PriceState, StockModel } from 'src/app/models/stock/stock.model';
import { StockChangedModel } from 'src/app/models/stock/stock-changed.model';
import { StockRealTimeService } from 'src/app/services/real-time/stock-real-time.service';
import { StockRestService } from 'src/app/services/rest/stock-rest.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  stocks: StockModel[] = [];
  PriceState = PriceState; 

  constructor(
    private restService: StockRestService,
    private realTimeService: StockRealTimeService,
    private router: Router) { }

  ngOnInit(): void {
    this.restService
      .getStocks()
      .subscribe(res => this.stocks = res.map(s => {
        s.priceState = PriceState.Default;
        return s;
      }));

    this.realTimeService
      .connection
      .on(HubEvents.StockUpdate, (data: StockChangedModel) => {
        let stock = this.stocks.find(s => s.symbol === data.symbol);

        stock.priceState = PriceState.Default;

        if(stock.price !== data.price) {
          stock.priceState = data.price > stock.price ? PriceState.Up : PriceState.Down;
          stock.price = data.price;
        }
      });
  }

  selectStock(stock: StockModel) {
    this.router.navigate(['stock', stock.id]);
  }
}
