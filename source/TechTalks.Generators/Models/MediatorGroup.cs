using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace TechTalks.Generators.Models
{
  internal readonly struct MediatorGroup
  {
    public MediatorGroup(IMethodSymbol sourceMethod) : this()
    {

      SourceMethod = sourceMethod ?? throw new ArgumentNullException(nameof(sourceMethod));
      RequestParams = sourceMethod.Parameters.Select(p => new MediatorParameter(p)).ToImmutableArray();
      (ResponseParams, IsAwaitable) = ResolveResponseType((INamedTypeSymbol)sourceMethod.ReturnType);
    }

    public IMethodSymbol SourceMethod { get; }
    public ImmutableArray<MediatorParameter> RequestParams { get; }
    public ImmutableArray<MediatorParameter> ResponseParams { get; }
    public bool IsAwaitable { get; }
    public INamespaceSymbol Namespace => SourceMethod.ContainingNamespace;
    public ITypeSymbol Class => SourceMethod.ContainingType;
    public string RequestName => $"{SourceMethod.Name}Request";
    public string ResponseName => $"{SourceMethod.Name}Response";
    public string HandlerName => $"{SourceMethod.Name}Handler";

    private static (ImmutableArray<MediatorParameter> responseParams, bool isAwaitable) ResolveResponseType(INamedTypeSymbol returnType)
    {
      switch (returnType.Name)
      {
        case nameof(Task):
        case nameof(ValueTask):
          return (ReturnResponseParams((INamedTypeSymbol)returnType.TypeArguments[0]), true);
        default:
          return (ReturnResponseParams(returnType), false);
      };
    }

    private static ImmutableArray<MediatorParameter> ReturnResponseParams(INamedTypeSymbol returnType)
    {
      if (returnType.IsTupleType)
      {
        return returnType.TupleElements
          .Select(element => new MediatorParameter(element.Name, element.Type))
          .ToImmutableArray();
      }

      switch (returnType.SpecialType)
      {
        case SpecialType.System_Boolean:
          return ImmutableArray.Create(new MediatorParameter("Successful", returnType));
        default:
          return ImmutableArray.Create(new MediatorParameter("Value", returnType));
      }
    }
  }
}
