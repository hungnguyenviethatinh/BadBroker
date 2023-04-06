using System;
using System.Collections.Generic;

namespace BadBroker.Shared.ResponseModels
{
    public class BestRevenue
    {
        public DateTime BuyDate { get; set; }

        public DateTime SellDate { get; set; }

        public string Tool { get; set; }

        public double Revenue { get; set; }

        public IEnumerable<Rate> Rates { get; set; }
    }

    public class Rate
    {
        public DateTime Date { get; set; }

        public double RUB { get; set; }

        public double EUR { get; set; }

        public double GDP { get; set; }

        public double JPY { get; set; }
    }
}
