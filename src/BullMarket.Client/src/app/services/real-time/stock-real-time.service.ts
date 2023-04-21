import { Injectable } from '@angular/core';
import * as SignalR from '@microsoft/signalr';
import { environment } from 'src/environments/environment';
import { HubMethods, HubPaths } from 'src/app/constants/hub.constants';
import { AuthService } from '../auth/auth.service';

@Injectable({
  providedIn: 'root',
})
export class StockRealTimeService {
  private url: string = environment.backEndUrl + HubPaths.Stocks;
  connection: SignalR.HubConnection;

  constructor(private authService: AuthService) {}

  startConnection() {
    this.connection = new SignalR.HubConnectionBuilder()
      .withAutomaticReconnect([1, 5, 10, 20])
      .withUrl(this.url, {
        accessTokenFactory: () => {
          let identityToken = localStorage.getItem('token');
          let oauth2AccessToken = this.authService.accessToken;
          if (identityToken) return identityToken;
          return oauth2AccessToken;
        },
      })
      .build();

    return this.connection
      .start()
      .then(() => console.log('Connection started!'))
      .catch((err) => console.log('Connection failed! ' + err));
  }

  addComment(content: string, stockId: string): Promise<any> {
    return this.connection.invoke(HubMethods.AddComment, {
      commentContent: content,
      stockId: stockId,
    });
  }
}
