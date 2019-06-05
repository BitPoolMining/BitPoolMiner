setx GPU_FORCE_64BIT_PTR 0
setx GPU_MAX_HEAP_SIZE 100
setx GPU_USE_SYNC_OBJECTS 1
setx GPU_MAX_ALLOC_PERCENT 100
setx GPU_SINGLE_ALLOC_PERCENT 100

EthDcrMiner64.exe -epool us-east.etc.bitpoolmining.com:1011 -ewal 0xa89f1bfaa2cc83fb90da5c0beca9fb95e8d8b711.test -epsw PasswordTest -mode 1 -allpools 1 -dbg 1 -esm 3