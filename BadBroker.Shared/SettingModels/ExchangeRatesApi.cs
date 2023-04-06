namespace BadBroker.Shared.SettingModels
{
    public class ExchangeRatesApi
    {
        /// <summary>
        /// Api key to verify your identity on request to Exchange Rates Api.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Exchange Rates Api Url.
        /// </summary>
        public string ApiUrl { get; set; }
    }
}
