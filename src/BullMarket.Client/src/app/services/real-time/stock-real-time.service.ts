import { Injectable } from '@angular/core';
import * as SignalR from '@microsoft/signalr';
import { environment } from 'src/environments/environment';
import { HubMethods, HubPaths } from 'src/app/constants/hub.constants';

@Injectable({
  providedIn: 'root'
})
export class StockRealTimeService {
  private url: string = environment.backEndUrl + HubPaths.Stocks;
  connection: SignalR.HubConnection;
  
  constructor() { }

  startConnection() {
    this.connection = new SignalR.HubConnectionBuilder()
      .withAutomaticReconnect([1, 5, 10, 20])
      .withUrl(this.url, {
        accessTokenFactory: () => {
          return localStorage.getItem("token");
        }
      })
      .build();

    this.connection
      .start()
      .then(() => console.log('Connection started!'))
      .catch(err => console.log('Connection failed! ' + err));
  }

  addComment(content: string, stockId: string): Promise<any> {
    return this.connection
      .invoke(HubMethods.AddComment, { commentContent: content, stockId: stockId });
  }
}
