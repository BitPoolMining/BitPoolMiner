using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPoolMiner
{
    class Notes
    {

        // BitPoolMiner API
        //
        //      GetBitPoolMinerVersion
        //          Version # of BitPoolMiner
        //
        //      GetMinerVersions
        //          MinerTypeID (BPM Enum)
        //          MinerName
        //          DownloadURL
        //          Algo's supported (relevant to our pool)
        //          MinerVersionNumber
        //          CardType (AMD/NVidia)
        //          64Bit (Boolean)
        //
        //      GetStratumConnection (enum CoinType, enum MinerTypeID, str BPMWorkerUniqueID, str BPMMinerAddress, str BPMWorker, enum CardType, bool 64Bit) 
        //          StratumConnection (str)       
        //
        // BitPoolMiner API



        // Pool API 
        //
        //      GetSupportedCoins (OUT List of Coins)
        //              CoinName
        //              CoinSymbol
        //              CoinType (BPM Enum Value)
        //              Enabled
        //              Algo
        //
        //      GetCoinStratum(IN CoinType)(Out List of Stratums)
        //              StratumURL
        //              Port
        //              Description
        //              BPMPort (Boolean to determine if this is the unique port to use for BPM)        
        //
        // Pool API



        // MinerSettingsAPI
        //
        //      PostMinerAccount (str BPMMinerUsername, str BPMMinerPassword)
        //              BPMMinerId (Internal GUID)
        //
        //      PostWorkerAccount (BPMMinerId, BPMWorkerName)
        //              BPMWorkerId (Internal GUID)
        //
        //      PostMinerWalletAddress (GUID BPMMinerId, enum CoinType, string Address, bool Enabled)
        //              BPMMinerWalletAddress (GUID)
        //
        //      PostWorkerWalletAddress (GUID BPMMinerId, enum CoinType, string Address, bool Enabled)
        //              BPMMinerWalletAddress (GUID)
        //
        //      PutEnableWalletAddress (GUID BPMMinerWalletAddress, enum CoinType, string Address, bool Enabled)
        //              Result (bool)
        //
        //      GetMinerWalletAddressList (GUID BPMMinerId)
        //              enum CoinType
        //              string Address
        //              bool Enabled
        //
        //      GetWorkerWalletAddressList (GUID BPMMinerId, GUID BPMWorkerId)
        //              enum CoinType
        //              string Address
        //              bool Enabled
        //
        //      GetMinerWorkerList (GUID BPMMinerId)
        //              BPMWorkerId (GUID)
        //              BPMWorkerName (string)
        //
        // MinerSettingsAPI


        // MinerMonitorAPI
        //
        //      PostMinerWorkerStats (GUID BPMMinerId, GUID BPMWorkerId,  
        //                            Status (Online, Offline)
        //                            StatusTime (DateTimeUTC)
        //                            LIST<
        //                                  GPU#,
        //                                  GPU TYPE,
        //                                  GPU NAME,
        //                                  CoinType,
        //                                  Algo,
        //                                  MinerType,
        //                                  HashRate,
        //                                  Temp,
        //                                  Fanspeed,
        //                                  Wattage,
        //                            >)
        //              MinerWorkerStatsId (GUID)
        //
        //      GetMinerWorkerStats(GUID BPMMinerId, GUID BPMWorkerId,
        //              Status(Online, Offline)
        //              StatusTime(DateTimeUTC)
        //              LIST<
        //                  GPU#,
        //                  GPU TYPE,
        //                  GPU NAME,
        //                  CoinType,
        //                  Algo,
        //                  MinerType,
        //                  HashRate,
        //                  Temp,
        //                  Fanspeed,
        //                  Wattage,
        //                >)
        //              MinerWorkerStatsId (GUID)
        //
        //
        // MinerMonitorAPI
    }
}
