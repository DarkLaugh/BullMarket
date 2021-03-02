import { Component, OnInit } from '@angular/core';
import { StockRealTimeService } from './services/real-time/stock-real-time.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'BullMarketUI';
  constructor(private realTimeService: StockRealTimeService) { }

  ngOnInit(): void {
    this.realTimeService.startConnection();
  }
}
