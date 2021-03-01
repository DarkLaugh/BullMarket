import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { StockModel } from 'src/app/models/stock/stock.model';
import { StockRestService } from 'src/app/services/rest/stock-rest.service';

@Component({
  selector: 'app-stock',
  templateUrl: './stock.component.html',
  styleUrls: ['./stock.component.scss']
})
export class StockComponent implements OnInit {
  stock: StockModel;

  constructor(
    private route: ActivatedRoute,
    private stockService: StockRestService) { }

  ngOnInit(): void {
    let id = this.route.snapshot.params['id'];

    this.stockService
      .getStockById(id)
      .subscribe(res => this.stock = res);
  }
}
