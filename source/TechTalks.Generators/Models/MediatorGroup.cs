using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using System.Diagnostics;
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
      (ResponseParams, IsAwaitable) = ResolveResponseType(sourceMethod, (INamedTypeSymbol)sourceMethod.ReturnType);
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

    private static (ImmutableArray<MediatorParameter> responseParams, bool isAwaitable) ResolveResponseType(IMethodSymbol method, INamedTypeSymbol returnType)
    {
      switch (returnType.Name)
      {
        case nameof(Task):
        case nameof(ValueTask):
          return (ReturnResponseParams(method, (INamedTypeSymbol)returnType.TypeArguments[0]), true);
        default:
          return (ReturnResponseParams(method, returnType), false);
      };
    }

    private static ImmutableArray<MediatorParameter> ReturnResponseParams(IMethodSymbol method, INamedTypeSymbol returnType)
    {
      if (returnType.IsTupleType)
      {
        return returnType.TupleElements
          .Select(element => new MediatorParameter(element.Name, element.Type))
          .ToImmutableArray();
      }

      var responseValue = method
        .GetAttributes()
        .FirstOrDefault(attr => attr.AttributeClass.Name == "HandlerAttribute")?.NamedArguments
        .FirstOrDefault(arg => arg.Key == "ResponseValue").Value.Value;

      if (responseValue is string value)
      {
        return ImmutableArray.Create(new MediatorParameter(value, returnType));
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
