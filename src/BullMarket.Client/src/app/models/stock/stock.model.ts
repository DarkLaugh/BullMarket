import { CommentModel } from './comment.model';

export interface StockModel {
    id: string;
    className: string;
    exchange: string;
    symbol: string;
    status: string;
    tradable: boolean;
    marginable: boolean;
    shortable: boolean;
    easyToBorrow: boolean;
    price: number;
    priceState: PriceState;
    comments: CommentModel[];
}

export enum PriceState {
    Default = 'Default',
    Up = 'Up', 
    Down = 'Down'
}