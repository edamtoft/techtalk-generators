using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Diagnostics;
using System.Threading;
using TechTalks.Generators.Mediator.Emitters;
using TechTalks.Generators.Mediator.Models;

namespace TechTalks.Generators.Mediator
{
  [Generator]
  public sealed class MediatorGenerator : IIncrementalGenerator
  {
    /// <summary>
    /// Called by roslyn at startup. Wires up the three-step pipeline
    /// </summary>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
      //if (!Debugger.IsAttached) Debugger.Launch();

      var provider = context.SyntaxProvider
        .CreateSyntaxProvider(IsHandlerMethod, ResolveHandleMethod);

      context.RegisterSourceOutput(provider, GenerateSource);
    }

    /// <summary>
    /// First tier: Select if the syntax is potentially of interest.
    /// </summary>
    private static bool IsHandlerMethod(SyntaxNode node, CancellationToken cancellationToken)
    {
      return node is MethodDeclarationSyntax method 
        && (method.Identifier.ValueText?.StartsWith("Handle") ?? false);
    }

    /// <summary>
    /// Second tier: Use the semantic model to get additional information about the method, 
    /// or rule out false positives from tier-1
    /// </summary>
    private static MediatorGroup? ResolveHandleMethod(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
      if (cancellationToken.IsCancellationRequested)
      {
        return null;
      }
      try
      {
        return new MediatorGroup((IMethodSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken));
      }
      catch
      {
        return null;
      }
    }

    /// <summary>
    /// Third tier: Generate the source
    /// </summary>
    private static void GenerateSource(SourceProductionContext context, MediatorGroup? possibleGroup)
    {
      if (!possibleGroup.HasValue)
      {
        return;
      }

      var group = possibleGroup.Value;

      context.AddSource($"{group.Class.Name}.{group.RequestName}.cs", new RequestEmitter().GetSource(group));
      context.AddSource($"{group.Class.Name}.{group.ResponseName}.cs", new ResponseEmitter().GetSource(group));
      context.AddSource($"{group.Class.Name}.{group.HandlerName}.cs", new HandlerEmitter().GetSource(group));
    }
  }
}