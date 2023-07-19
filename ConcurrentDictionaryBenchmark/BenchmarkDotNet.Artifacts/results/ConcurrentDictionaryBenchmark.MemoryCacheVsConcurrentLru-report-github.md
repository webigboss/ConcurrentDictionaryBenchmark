``` ini

BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22621.1992/22H2/2022Update/SunValley2)
Intel Core i9-10900K CPU 3.70GHz, 1 CPU, 20 logical and 10 physical cores
.NET SDK=7.0.400-preview.23322.28
  [Host] : .NET 6.0.20 (6.0.2023.32017), X64 RyuJIT AVX2


```
|      Method | UserSize | CacheSize | GetOrAddOperations | ThreadCount | EnableStatistics | Mean | Error |
|------------ |--------- |---------- |------------------- |------------ |----------------- |-----:|------:|
| MemoryCache |   200000 |     50000 |            1000000 |           4 |            False |   NA |    NA |

Benchmarks with issues:
  MemoryCacheVsConcurrentLru.MemoryCache: DefaultJob [UserSize=200000, CacheSize=50000, GetOrAddOperations=1000000, ThreadCount=4, EnableStatistics=False]
