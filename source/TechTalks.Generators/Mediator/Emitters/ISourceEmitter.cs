using Microsoft.CodeAnalysis.Text;
using TechTalks.Generators.Mediator.Models;

namespace TechTalks.Generators.Mediator.Emitters
{
  internal interface ISourceEmitter
  {
    SourceText GetSource(MediatorGroup group);
  }
}