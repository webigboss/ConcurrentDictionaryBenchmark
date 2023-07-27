``` ini

BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22621.1992/22H2/2022Update/SunValley2)
Intel Core i9-10900K CPU 3.70GHz, 1 CPU, 20 logical and 10 physical cores
.NET SDK=7.0.400-preview.23322.28
  [Host]     : .NET 6.0.20 (6.0.2023.32017), X64 RyuJIT AVX2
  DefaultJob : .NET 6.0.20 (6.0.2023.32017), X64 RyuJIT AVX2


```
|                              Method | UserSize | CacheSize | GetOrAddOperations | ThreadCount | EnableStatistics |     Mean |    Error |   StdDev |       Gen0 |      Gen1 | Allocated |
|------------------------------------ |--------- |---------- |------------------- |------------ |----------------- |---------:|---------:|---------:|-----------:|----------:|----------:|
|                       **ConcurrentLru** |   **200000** |     **10000** |            **1000000** |          **10** |            **False** | **653.0 ms** | **13.06 ms** | **31.79 ms** | **23000.0000** | **3000.0000** | **256.37 MB** |
| ConcurrentLruWithAbsoluteExpiration |   200000 |     10000 |            1000000 |          10 |            False | 646.8 ms |  9.90 ms |  8.27 ms | 24000.0000 | 4000.0000 | 265.51 MB |
|                       **ConcurrentLru** |   **200000** |     **50000** |            **1000000** |          **10** |            **False** | **689.0 ms** |  **5.52 ms** |  **4.89 ms** | **14000.0000** | **4000.0000** |  **171.4 MB** |
| ConcurrentLruWithAbsoluteExpiration |   200000 |     50000 |            1000000 |          10 |            False | 761.7 ms |  9.65 ms |  9.48 ms | 17000.0000 | 4000.0000 |  205.8 MB |
