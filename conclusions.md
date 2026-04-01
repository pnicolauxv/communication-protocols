# Communication Protocols Benchmark – Architecture & Results
## Objective

Evaluate and compare communication protocols between microservices:

- HTTP
- HTTP + mTLS (mutual TLS)
- gRPC

Evaluation criteria:

- Performance: latency, throughput
- Security: encryption and authentication
- Operational complexity

Two dotnet microservices were implemented:

- Service B: exposes the endpoints
- Service A: client calling Service B with a protocol selector (?protocol=http|grpc|mtls)

## Architecture
```
                     +------------------------+
                     | Service A (Client)     |
                     | 5000                   |
                     +-----------+------------+
                                 |
        +------------------------+------------------------+
        |                        |                        |
        v                        v                        v

+--------------------+  +--------------------+  +------------------------+
| HTTP               |  | gRPC               |  | HTTP + mTLS            |
| 5001               |  | 5002               |  | 5003                   |
|--------------------|  |--------------------|  |------------------------|
| - REST             |  | - HTTP/2           |  | - REST                 |
| - JSON             |  | - Protobuf         |  | - JSON                 |
| - No Auth          |  |   (binary)         |  | - TLS + Auth           |
+---------+----------+  +---------+----------+  +----------+-------------+
          |                       |                        |
          v                       v                        v

   +--------------+        +--------------+        +--------------+
   | Service B    |        | Service B    |        | Service B    |
   +--------------+        +--------------+        +--------------+
```
- Service A performs all tests through a single endpoint: /test?protocol=...
- mTLS adds mutual authentication and TLS encryption over HTTP.
- gRPC uses HTTP/2 with binary serialization (Protobuf).

## Benchmark Results (k6)
The tests have been done with k6 Grafana during 2 days, the first one the cluter was going really sloow, the second one was smooth.

### Cluster slow
#### 50 VUs Big payload
| Protocol | Avg Latency | P95 Latency | Throughput (req/s) | Failure Rate | Data Received |
| -------- | ----------- | ----------- | ------------------ | ------------ | ------------- |
| HTTP     | 544 ms      | 696 ms      | 73 req/s           | 1.24%        | 1.5 GB        |
| gRPC     | 509 ms      | 561 ms      | 78 req/s           | 1.16%        | 1.6 GB        |
| mTLS     | 642 ms      | 788 ms      | 62 req/s           | 1.46%        | 1.2 GB        |


#### 50 VUs Small payload

| Protocol | Avg Latency | P95 Latency | Throughput (req/s) | Failure Rate | Data Received |
| -------- | ----------- | ----------- | ------------------ | ------------ | ------------- |
| HTTP     | 259 ms      | 148 ms      | 153 req/s          | 0.59%        | 5.1 MB        |
| gRPC     | 262 ms      | 145 ms      | 151 req/s          | 0.61%        | 5.1 MB        |
| mTLS     | 273 ms      | 159 ms      | 147 req/s          | 0.65%        | 4.9 MB        |


#### 500 VUs Small payload

| Protocol | Avg Latency | P95 Latency | Throughput (req/s) | Failure Rate | Data Received |
| -------- | ----------- | ----------- | ------------------ | ------------ | ------------- |
| HTTP     | 272 ms      | 180 ms      | 1459 req/s         | 0.62%        | 49 MB         |
| gRPC     | 281 ms      | 196 ms      | 1409 req/s         | 0.65%        | 48 MB         |
| mTLS     | 297 ms      | 205 ms      | 1340 req/s         | 0.68%        | 45 MB         |

### Resource management - Custer smooth

HTTP
| CPU / Memory | Avg Latency | P95       | Throughput (req/s) | Fail % |
| ------------ | ----------- | --------- | ------------------ | ------ |
| 250m / 256Mi | 172.74 ms   | 226.91 ms | 2861.26            | 0.00%  |
| 250m / 64Mi  | 168.74 ms   | 209.97 ms | 2934.07            | 0.00%  |
| 125m / 128Mi | 299.85 ms   | 203.54 ms | 1323.93            | 0.00%  |
| 125m / 64Mi  | 223.34 ms   | 310.57 ms | 2215.34            | 0.00%  |
| 63m / 256Mi  | 349.77 ms   | 622.52 ms | 1413.75            | 0.00%  |
| 63m / 64Mi   | 335.46 ms   | 686.38 ms | 1476.35            | 0.00%  |

gRPC
| CPU / Memory | Avg Latency | P95       | Throughput (req/s) | Fail % |
| ------------ | ----------- | --------- | ------------------ | ------ |
| 250m / 256Mi | 166.01 ms   | 214.71 ms | 2981.93            | 0.00%  |
| 250m / 64Mi  | 162.99 ms   | 212.56 ms | 3035.88            | 0.00%  |
| 125m / 128Mi | 323.45 ms   | 573.29 ms | 1532.08            | 0.00%  |
| 63m / 256Mi  | 378.26 ms   | 629.72 ms | 1308.59            | 0.00%  |
| 63m / 64Mi   | 379.22 ms   | 662.17 ms | 1303.53            | 0.00%  |

mTLS
| CPU / Memory | Avg Latency | P95       | Throughput (req/s) | Fail % |
| ------------ | ----------- | --------- | ------------------ | ------ |
| 250m / 256Mi | 226.45 ms   | 301.29 ms | 2185.19            | 0.00%  |
| 250m / 64Mi  | 225.19 ms   | 295.86 ms | 2199.13            | 0.00%  |
| 125m / 128Mi | 435.07 ms   | 595.25 ms | 1137.59            | 0.00%  |
| 63m / 256Mi  | 434.23 ms   | 903.10 ms | 1140.17            | 0.00%  |
| 63m / 64Mi   | 1.97 s      | 15.96 s   | 250.92             | 0.00%  |

Final Conclusions (Across All Tests)
1) HTTP and gRPC are very similar
2) mTLS consistently adds overhead

## Security & Complexity Comparison

| Protocol    | Security                    | Operational Complexity |
| ----------- | --------------------------- | ---------------------- |
| HTTP        | None                        | Low                    |
| gRPC        | Optional TLS                | Medium                 |
| HTTP + mTLS | Encryption + authentication | High                   |


## Conclusions 
In most scenarios, gRPC requires transforming data into a binary format, which introduces additional processing time. It is also more complex to implement, so both factors should be carefully considered.

For internal microservice communication within the same network, gRPC or HTTP are recommended due to their low latency and high throughput.

In high-security environments that require mutual authentication, mTLS is a suitable choice, although it comes with increased latency.