namespace BitPoolMiner.Persistence.API.Base
{
    /// <summary>
    /// URL for API
    /// </summary>
    public static class APIConstants
    {
        public const string APIURL = "https://bitpoolminerapi.bitpoolmining.com/api/";
        public const string CoinMarketCapAPIURL = "https://api.coinmarketcap.com/v1/ticker/{0}/?convert={1}";
        public const string CryptoCompareHistoDayAPIURL = "https://min-api.cryptocompare.com/data/histoday?fsym={0}&tsym={1}&limit=31&aggregate=1&e=CCCAGG";
        public const string WhatToMineAPIURL = "http://www.whattomine.com/coins/{0}.json";
    }

    /// <summary>
    /// List of API endpoint names
    /// </summary>
    public static class APIEndpoints
    {
        public const string GetAccountGuid = "AccountGUID";
        public const string GetGPUSettings = "GPUSettings";
        public const string GetAccountWorkers = "AccountMiners";
        public const string GetAccountWallet = "AccountWallet";
        public const string GetMinerConfigString = "MinerConfig";
        public const string GetMinerMonitorStats = "MiningMonitorStatistics";
        public const string GetMiningMonitorStatistics24Hour = "MiningMonitorStatistics24Hour";
        public const string GetMinerPayments = "MinerPayments";

        public const string PostAccountWorkers = "AccountMiners";
        public const string PostMinerMonitorStats = "MiningMonitorStatistics";
        public const string PostGPUSettings = "GPUSettings";
        public const string PostAccountWallet = "AccountWallet";

        public const string DeletetAccountWorkers = "AccountMiners";
        public const string DeleteAccountWallet = "AccountWallet";
    }
}
