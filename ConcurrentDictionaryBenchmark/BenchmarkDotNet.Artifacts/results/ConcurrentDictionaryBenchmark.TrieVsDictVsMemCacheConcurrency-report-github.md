``` ini

BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22621.1992/22H2/2022Update/SunValley2)
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.400-preview.23330.10
  [Host]     : .NET 6.0.20 (6.0.2023.32017), X64 RyuJIT AVX2
  DefaultJob : .NET 6.0.20 (6.0.2023.32017), X64 RyuJIT AVX2


```
|                                                   Method | UserSize | GetOperations | TenantSize | ThreadCount | SlidingExpirationInMs |     Mean |    Error |   StdDev | Ratio | RatioSD |      Gen0 |      Gen1 |     Gen2 | Allocated | Alloc Ratio |
|--------------------------------------------------------- |--------- |-------------- |----------- |------------ |---------------------- |---------:|---------:|---------:|------:|--------:|----------:|----------:|---------:|----------:|------------:|
|                               ConcurrentDictWithComparer |    50000 |        100000 |         20 |          10 |                     5 | 40.33 ms | 0.794 ms | 1.809 ms |  1.00 |    0.00 | 3153.8462 | 1307.6923 | 384.6154 |  18.49 MB |        1.00 |
|                           ConcurrentDictWithUserCacheKey |    50000 |        100000 |         20 |          10 |                     5 | 39.36 ms | 0.775 ms | 1.617 ms |  0.98 |    0.07 | 3153.8462 | 1384.6154 | 384.6154 |  18.49 MB |        1.00 |
|                         MemoryCacheWithSlidingExpiration |    50000 |        100000 |         20 |          10 |                     5 | 38.97 ms | 0.766 ms | 1.301 ms |  0.98 |    0.06 | 4214.2857 | 1785.7143 | 357.1429 |  23.28 MB |        1.26 |
| MemoryCacheNotPerTenantWithSizeLimitAndSlidingExpiration |    50000 |        100000 |         20 |          10 |                     5 | 74.94 ms | 1.803 ms | 5.259 ms |  1.81 |    0.13 | 5142.8571 | 2142.8571 | 571.4286 |  33.01 MB |        1.79 |
|                                  MemoryCacheNotPerTenant |    50000 |        100000 |         20 |          10 |                     5 | 79.83 ms | 2.174 ms | 6.343 ms |  2.00 |    0.20 | 5428.5714 | 2142.8571 | 571.4286 |  31.09 MB |        1.68 |
|                                            ConcurrentLru |    50000 |        100000 |         20 |          10 |                     5 | 37.25 ms | 0.740 ms | 2.111 ms |  0.94 |    0.06 | 2933.3333 | 1333.3333 | 466.6667 |   15.8 MB |        0.85 |
