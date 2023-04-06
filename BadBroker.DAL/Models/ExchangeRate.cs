using System;

namespace BadBroker.DAL.Models
{
    public class ExchangeRate
    {
        public DateTime Date { get; set; }

        public string BaseCurrency { get; set; }

        public string FromCurrency { get; set; }

        public decimal Rate { get; set; }
    }
}
