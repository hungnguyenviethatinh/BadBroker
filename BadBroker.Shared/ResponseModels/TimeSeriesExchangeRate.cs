using Newtonsoft.Json;
using System.Collections.Generic;

namespace BadBroker.Shared.ResponseModels
{
    /// <summary>
    /// Exchange rate data between two dates
    /// </summary>
    /// <example>
    /// {
    ///   "success": true,
    ///   "timeseries": true,
    ///   "start_date": "2012-05-01",
    ///   "end_date": "2012-05-03",
    ///   "base": "EUR",
    ///   "rates": {
    ///     "2012-05-01": {
    ///       "USD": 1.322891,
    ///       "AUD": 1.278047,
    ///       "CAD": 1.302303
    ///     },
    ///     "2012-05-02": {
    ///       "USD": 1.315066,
    ///       "AUD": 1.274202,
    ///       "CAD": 1.299083
    ///     },
    ///     "2012-05-03": {
    ///       "USD": 1.314491,
    ///       "AUD": 1.280135,
    ///       "CAD": 1.296868
    ///     },
    ///   }
    /// }
    /// </example>
    public class TimeSeriesExchangeRate
    {
        /// <summary>
        /// Returns true or false depending on whether or not your API request has succeeded.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Returns true if a request to the timeseries endpoint is made.
        /// </summary>
        public bool TimeSeries { get; set; }

        /// <summary>
        /// The start date of your time frame.
        /// </summary>
        [JsonProperty(PropertyName = "start_date")]
        public string StartDate { get; set; }

        /// <summary>
        /// The end date of your time frame.
        /// </summary>
        [JsonProperty(PropertyName = "end_date")]
        public string EndDate { get; set; }

        /// <summary>
        /// Returns the three-letter currency code of the base currency used for this request.
        /// </summary>
        public string Base { get; set; }

        /// <summary>
        /// Returns exchange rate data for the currencies you have requested.
        /// </summary>
        public Dictionary<string, Dictionary<string, double>> Rates { get; set; }
    }
}
