### HTTP vs gRPC

| Payload | Protocol | med (ms) | min (ms) | max (ms) | p95 (ms) | req/s |
| ------- | -------- | -------- | -------- | -------- | -------- | ----- |
| 1 KB    | HTTP     | 176.66   | 125.25   | 540.38   | 212.71   | 2783  |
| 1 KB    | gRPC     | 160.16   | 125.63   | 320.53   | 219.35   | 2896  |
| 10 KB   | HTTP     | 193.35   | 131.58   | 1830     | 260.58   | 2409  |
| 10 KB   | gRPC     | 182.19   | 118.88   | 1960     | 230.68   | 2494  |
| 20 KB   | HTTP     | 231.82   | 133.86   | 1750     | 387.61   | 1936  |
| 20 KB   | gRPC     | 205.85   | 134.22   | 1770     | 344.82   | 2132  |
| 30 KB   | HTTP     | 258.76   | 133.45   | 2410     | 504.5    | 1617  |
| 30 KB   | gRPC     | 226.87   | 132.9    | 2480     | 482.83   | 1681  |
| 40 KB   | HTTP     | 305.11   | 134.36   | 3670     | 679.79   | 1256  |
| 40 KB   | gRPC     | 326.69   | 134.9    | 2390     | 658.12   | 1192  |
| 50 KB   | HTTP     | 536.3    | 136.02   | 3110     | 897.92   | 831   |
| 50 KB   | gRPC     | 556.77   | 136.42   | 3040     | 1210     | 751   |
| 60 KB   | HTTP     | 613.55   | 139.62   | 4160     | 949.22   | 739   |
| 60 KB   | gRPC     | 692.72   | 151.62   | 4170     | 1390     | 642   |
| 70 KB   | HTTP     | 837.95   | 136.91   | 3870     | 1260     | 536   |
| 70 KB   | gRPC     | 948.54   | 138.75   | 4150     | 1880     | 486   |

We’ve observed that gRPC performance degrades progressively with large payloads, primarily due to serialization overhead. We attempted to improve this by sending data in chunks, but this approach did not yield better results, each chunk carries its own headers, which introduces additional overhead and negates the potential benefits.

Additionally, timing measurements can fluctuate because of internal CPU and memory behavior within pods.

Overall, gRPC performs comparably—or slightly better than HTTP—for small payloads. However, for larger payloads, HTTP consistently outperforms gRPC.