setx GPU_FORCE_64BIT_PTR 0
setx GPU_MAX_HEAP_SIZE 100
setx GPU_USE_SYNC_OBJECTS 1
setx GPU_MAX_ALLOC_PERCENT 100
setx GPU_SINGLE_ALLOC_PERCENT 100

EthDcrMiner64.exe -epool stratum+tcp://us-east.exp.bitpoolmining.com:3030 -ewal YOUR_ETH_ADDRESS.YOUR_WORKER_NAME -epsw x -mport 127.0.0.1:-2882 -allpools 0 -nofee 1 -allcoins 1 -esm 3 -di 0 -mode 1=0
