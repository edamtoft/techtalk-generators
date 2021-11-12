using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechTalks.Generators.Mediator.Models
{
  internal readonly struct MediatorGroup
  {
    public MediatorGroup(IMethodSymbol sourceMethod) : this()
    {
      SourceMethod = sourceMethod ?? throw new ArgumentNullException(nameof(sourceMethod));
      var root = sourceMethod.Name.Substring("Handle".Length);
      RequestName = root + "Request";
      ResponseName = root + "Response";
      HandlerName = root + "Handler";
      RequestParams = sourceMethod.Parameters.Select(p => new MediatorParameter(p)).ToImmutableArray();
      (ResponseType, IsAsync) = ResolveResponseType((INamedTypeSymbol)sourceMethod.ReturnType);
    }

    public IMethodSymbol SourceMethod { get; }
    public string RequestName { get; }
    public string ResponseName { get; }
    public string HandlerName { get; }
    public ImmutableArray<MediatorParameter> RequestParams { get; }
    public ITypeSymbol ResponseType { get; }
    public bool IsAsync { get; }
    public ISymbol Namespace => SourceMethod.ContainingNamespace;
    public ITypeSymbol Class => SourceMethod.ContainingType;

    private static (ITypeSymbol responseType, bool isAsync) ResolveResponseType(INamedTypeSymbol returnType)
    {
      switch (returnType.Name)
      {
        case nameof(Task):
        case nameof(ValueTask):
          return (returnType.TypeArguments[0], true);
        default:
          return (returnType, false);
      };
    }
  }
}
