import { Component, OnInit } from '@angular/core';
import { HubEvents } from 'src/app/constants/hub.constants';
import { StockModel } from 'src/app/models/stock/stock.model';
import { StockRealTimeService } from 'src/app/services/real-time/stock-real-time.service';
import { StockRestService } from 'src/app/services/rest/stock-rest.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  stocks: StockModel[] = [];

  constructor(
    private restService: StockRestService,
    private realTimeService: StockRealTimeService) { }

  ngOnInit(): void {
    this.restService
      .getStocks()
      .subscribe(res => this.stocks = res);

    this.realTimeService.startConnection();

    this.realTimeService
      .connection
      .on(HubEvents.StockUpdate, data => {
      });
  }

}
