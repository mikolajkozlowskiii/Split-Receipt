namespace Split_Receipt.Payload
{
    /// <summary>
    ///  The <c>CurrencyResponse</c> class is a representation of the data
    ///  received from an internal currency API. It contains information about the base currency,
    ///  the date of the exchange rates, and a dictionary of currency rates with their respective values.
    /// </summary>
    public class CurrencyResponse
    {
        public string Base { get; set; }
        public DateTime Date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
