ZM is an Equihash miner for Nvidia GPUs
Supports currencies which use Equihash as their POW e.g. ZCash.
Official site: Official site: https://bitcointalk.org/index.php?topic=2021765.0

- Compatible with devices having Compute Capability 5.0 and greater (Maxwell/Pascal).
- Supports stratum/pool based mining.
- Multi-GPU support.
- Supports remote monitoring
- Supports setup of failover pools
- Supports configuration using configuration files
- contains 2% development fee



Dependencies
============

Linux:
 openssl 1.0 (for versions <= 0.5.7)

Windows:
 Visual C++ Redistributable for visual studio 2015 (VCRUNTIME140.dll)



Usage
=====

To get a description about available options - launch zm without parameters.

Minimal example:
 zm --server servername.com --port 1234 --user username

Packages for windows include a 'start.bat' for simplicity.
Don't forget to change your pool and login information.

$ zm --help
ZM 0.6, dstm's ZCASH/Equihash Cuda Miner

Usage:
 zm --server hostname --port port_nr --user user_name
    [--pass password] [options]...

 zm --cfg-file[=path]

 Stratum:
    --server         Stratum server hostname
                     prefix hostname with 'ssl://' for encrypted
                     connections - e.g. ssl://mypool.com
    --port           Stratum server port number
    --user           Username
    --pass           Worker password

 Options:
    --help           Print this help
    --list-devices   List available cuda devices to use
    --dev            Space separated list of cuda devices
    --time           Enable output of timestamps
    --color          colorize the output
    --logfile        [=path] Append logs to the file named by 'path'
                     If 'path' is not given append to 'zm.log' in
                     current working directory.
    --noreconnect    Disable automatic reconnection on network errors.

    --temp-target    In C - If set, enables temperature controller.
                     The workload of each GPU will be continuously
                     adjusted such that the temperature stays around
                     this value. It is recommended to set your fan speed
                     to 100% when using this setting.

    --telemetry      [=ip:port]. Starts telemetry server. Telemetry data
                     can be accessed using a web browser(http) or by json-rpc.
                     If no arguments are given the server listens on
                     127.0.0.1:2222 - Example: --telemetry=0.0.0.0:2222
                     Valid port range [1025-65535]

    --cfg-file       [=path] Use configuration file. All additional command
                     line options are ignored - configuration is done only
                     through configuration file. If 'path' is not given
                     use 'zm.cfg' in current working directory.

 Example:
    zm --server servername.com --port 1234 --user username




User interface
==============

ZM's output on system with 7 GPUs:

>  GPU0 42°C  Sol/s: 739.0  Sol/W: 2.85  Avg: 739.0  I/s: 390.1  Sh: 54.00  1.00  29  ++++++++++++++++++
>  GPU1 44°C  Sol/s: 459.0  Sol/W: 2.91  Avg: 459.0  I/s: 243.5  Sh: 27.00  1.00  29  +++++++++
>  GPU2 46°C  Sol/s: 451.3  Sol/W: 2.77  Avg: 451.3  I/s: 243.5  Sh: 15.00  1.00  28  +++++
>  GPU3 43°C  Sol/s: 449.7  Sol/W: 2.83  Avg: 449.7  I/s: 242.8  Sh: 24.00  1.00  29  ++++++++*
>  GPU4 42°C  Sol/s: 449.0  Sol/W: 2.76  Avg: 449.0  I/s: 243.5  Sh: 36.00  1.00  28  ++++++++++++
   GPU5 43°C  Sol/s: 446.4  Sol/W: 2.79  Avg: 446.4  I/s: 243.6  Sh: 30.00  1.00  28  ++++++++++
   GPU6 42°C  Sol/s: 461.4  Sol/W: 2.84  Avg: 461.4  I/s: 243.8  Sh: 18.00  1.00  28  ++++++
   ========== Sol/s: 3455.8 Sol/W: 2.82  Avg: 3455.8 I/s: 1850.9 Sh: 204.00 1.00  28


Sol/s: solutions per second
Sol/W: efficiency - average Sol/s per Watt
Avg  : average solutions per second
I/s  : iterations per second done by the GPU
Sh   : <shares per minute> <accepted shares ratio> <network latency in ms>


> : indicates that a new job was received
+ : indicates one submitted share
* : indicates one submitted dev fee share
= : sum/average of the stats if mining on multiple GPUs



Overclocking
============
ZM runs stable on stock settings.
On some GPUs overclocking might need readjustment in comparison with other mining software.
