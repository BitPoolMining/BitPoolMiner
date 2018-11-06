@echo off

:loop
wildrig.exe --algo=x16r --opencl-threads 3 --opencl-launch 20x128 --url=europe.rvn.bitpoolmining.com:3172 --user=RQAJU3nWpA3tE2uomV54UnkDhvXQrME3LK.RIG5 --pass=x --api-port=2883
if ERRORLEVEL 1000 goto custom
timeout /t 5
goto loop

:custom
echo Custom command here
timeout /t 5
goto loop