import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HubEvents, HubMethods } from 'src/app/constants/hub.constants';
import { CommentModel } from 'src/app/models/stock/comment.model';
import { StockModel } from 'src/app/models/stock/stock.model';
import { StockRealTimeService } from 'src/app/services/real-time/stock-real-time.service';
import { StockRestService } from 'src/app/services/rest/stock-rest.service';

@Component({
  selector: 'stock',
  templateUrl: './stock.component.html',
  styleUrls: ['./stock.component.scss']
})
export class StockComponent implements OnInit {
  stock: StockModel;
  commentContent: string = '';

  constructor(
    private route: ActivatedRoute,
    private restService: StockRestService,
    private realTimeService: StockRealTimeService) { }

  ngOnInit(): void {
    let id = this.route.snapshot.params['id'];

    this.restService
      .getStockById(id)
      .subscribe(res => this.stock = res);

    this.realTimeService
      .startConnection();

    this.realTimeService
      .connection
      .on(HubEvents.AddedComment, (res: CommentModel) => {
        console.log(res);
      });
  }

  addComment() {
    if(this.commentContent && this.commentContent.length > 0) {
      this.realTimeService
        .addComment(this.commentContent, this.stock.id)
        .then(() => this.commentContent = '')
        .catch(err => console.log(err));
    }
  }
}
