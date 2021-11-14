using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace TechTalks.Generators.Benchmarks.Json
{
  public class JsonBenchmarks
  {
    [Benchmark]
    public void WithReflection()
    {
      // New options to prevent caching between runs

      var options = new JsonSerializerOptions();

      // Round-trip serialize

      var json = JsonSerializer.Serialize(new Sample
      {
        Property1 = "Hello World",
        Property2 = false,
        Property3 = Guid.NewGuid(),
        Property4 = 123
      }, options);

      _ = JsonSerializer.Deserialize<Sample>(json, options);
    }

    [Benchmark]
    public void WithSourceGen()
    {
      // New context to prevent caching between runs

      var context = new SampleContext(new JsonSerializerOptions());

      // Round-trip serialize

      var json = JsonSerializer.Serialize(new Sample
      {
        Property1 = "Hello World",
        Property2 = false,
        Property3 = Guid.NewGuid(),
        Property4 = 123
      }, context.Sample);

      _ = JsonSerializer.Deserialize(json, context.Sample);
    }
  }
}
