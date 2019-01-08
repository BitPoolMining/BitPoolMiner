# BitPoolMiner - Open Source GPU Miner and GPU Monitor
GPU Mining and Monitoring App that is free to use and open source.  Written to work specifically with www.bitpoolmining.com.  You can get full details of BPM here www.bitpoolmining.com/bpm

Lots of features:
* Allows remote monitoring of workers and all gpus, including hashrates, temps, fan speed, power, etc.
* Forecasting based on data from www.whattomine.com using your actual hashrates.
* Actual payout data converted to your local currency using historical rates from www.cryptocompare.com.
* Coin information from www.coinmarketcap.com
* Easy mining with built in miners with Nvidia and AMD GPU support.  Automatic bat file generation for mining to reduce steps needed for mining
* Auto restart mining, in case miner crashes.
* Auto mine on start.
* Detailed profitability charting and data from last 30 days of mining, converted to local currency.
* Detailed worker/gpu statistics show 24 hour graphs for power and hashrates.

### Installing from binary

Find the latest release of BPM and run setup.exe

```
Run setup.exe from the latest release here https://github.com/BitPoolMining/BitPoolMiner/releases
```

If necessary, add an exclusion rule to the BPM folder in Windows Defender or similar software.

```
Add an exclusion to the folder C:\Users\username\AppData\Local\Apps\2.0
```

### Supported Coins and Miners

| Coins | NVidia Miner | AMD Miner |
| ------------- | ------------- | ------------- |
| ETH | N/A | Claymore |
| ETC | N/A | Claymore |
| EXP | N/A | Claymore |
| RVN | TRex, Ravencoin Miner | WildRig |
| SUQA | CryptoDredge | N/A |
| BTG | EWBF | N/A |
| VTC | CCMiner, CCMinerNanashi | N/A |
| MONA | CCMiner, CCMinerNanashi | N/A |
| ZCL | DSTM, EWBF | N/A |
| BTCP | DSTM, EWBF | N/A |
| HUSH | DSTM, EWBF | N/A |
| KMD | DSTM, EWBF | N/A |


