using Microsoft.CodeAnalysis.Text;
using TechTalks.Generators.Models;

namespace TechTalks.Generators.Emitters
{
  internal interface ISourceEmitter
  {
    SourceText GetSource();
  }
}