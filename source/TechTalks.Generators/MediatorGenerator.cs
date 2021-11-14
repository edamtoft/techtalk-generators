using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using TechTalks.Generators.Emitters;
using TechTalks.Generators.Models;

namespace TechTalks.Generators
{
  [Generator]
  public sealed class MediatorGenerator : IIncrementalGenerator
  {
    /// <summary>
    /// Called by roslyn at startup. Wires up the three-step pipeline
    /// </summary>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
      var provider = context.SyntaxProvider
        .CreateSyntaxProvider(IsMethod, ResolveHandleMethod);

      context.RegisterSourceOutput(provider, GenerateSource);
    }

    /// <summary>
    /// First tier: Select if the syntax is potentially of interest.
    /// </summary>
    private static bool IsMethod(SyntaxNode node, CancellationToken cancellationToken)
    {
      return node.IsKind(SyntaxKind.MethodDeclaration);
    }

    /// <summary>
    /// Second tier: Use the semantic model to get additional information about the method, 
    /// or rule out false positives from tier-1
    /// </summary>
    private static MediatorGroup? ResolveHandleMethod(GeneratorSyntaxContext context, CancellationToken ct)
    {
      if (ct.IsCancellationRequested)
      {
        return null;
      }
      try
      {
        if (context.SemanticModel.GetDeclaredSymbol(context.Node, ct) is IMethodSymbol method 
          && method.GetAttributes().Any(attr => attr.AttributeClass.Name == "HandlerAttribute"))
        {
          return new MediatorGroup(method);
        }
      }
      catch (Exception err)
      {
#if DEBUG
        if (!Debugger.IsAttached) Debugger.Launch();
#endif
      }
      return null;
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

      try
      {
        context.AddSource($"{group.RequestName}.cs", new RequestEmitter(group).GetSource());
        context.AddSource($"{group.ResponseName}.cs", new ResponseEmitter(group).GetSource());
        context.AddSource($"{group.HandlerName}.cs", new HandlerEmitter(group).GetSource());
      }
      catch (Exception err)
      {
#if DEBUG
        if (!Debugger.IsAttached) Debugger.Launch();
#endif
      }
    }
  }
}