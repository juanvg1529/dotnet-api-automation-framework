// Each feature runs as an NUnit fixture; features run in parallel.
// Scenarios are isolated because every one creates its own test data.
[assembly: Parallelizable(ParallelScope.Fixtures)]
[assembly: LevelOfParallelism(4)]
