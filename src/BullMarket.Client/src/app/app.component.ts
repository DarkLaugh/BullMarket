import { Component, OnInit } from '@angular/core';
import { StockRealTimeService } from './services/real-time/stock-real-time.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  title = 'BullMarketUI';
  constructor() {}
}
