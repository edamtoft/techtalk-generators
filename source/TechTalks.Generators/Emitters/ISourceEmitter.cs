using Microsoft.CodeAnalysis.Text;

namespace TechTalks.Generators.Emitters
{
  internal interface ISourceEmitter
  {
    SourceText GetSource();
  }
}