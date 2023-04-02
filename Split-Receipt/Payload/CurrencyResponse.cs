namespace Split_Receipt.Payload
{
    /// <summary>
    ///  Class <c>CurrencyResponse</c> is a class that consume data from internal 
    ///  currency api.
    /// </summary>
    public class CurrencyResponse
    {
        public string Base { get; set; }
        public DateTime Date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
