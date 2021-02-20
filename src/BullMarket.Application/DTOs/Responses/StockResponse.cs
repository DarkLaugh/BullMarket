namespace BullMarket.Application.DTOs.Responses
{
    public class StockResponse
    {
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public float CurrentPrice { get; set; }
        public float PreviousPrice { get; set; }
        public int QuantityAvailable { get; set; }
    }
}
