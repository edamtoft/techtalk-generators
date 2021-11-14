using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalks.Generators.Mediator.Emitters;
using TechTalks.Generators.Mediator.Models;

namespace TechTalks.Generators.Mediator.Emitters
{
  internal sealed class HandlerEmitter : ISourceEmitter
  {
    public HandlerEmitter(MediatorGroup group) => Group = group;

    private MediatorGroup Group { get; }

    public SourceText GetSource()
    {
      var src = new StringBuilder(1024);

      WriteUsings(src);

      WriteStartNamespace(src);
      
      WriteStartClass(src);

      WriteFields(src);

      WriteConstructor(src);

      WriteHandleMethod(src);

      WriteEndClass(src);

      WriteEndNamespace(src);

      return SourceText.From(src.ToString(), Encoding.UTF8);
    }

    private void WriteHandleMethod(StringBuilder src)
    {
      if (Group.IsAwaitable)
      {
        WriteHandleMethodAsync(src);
      }
      else
      {
        WriteHandleMethodSync(src);
      }
    }

    private void WriteHandleMethodSync(StringBuilder src)
    {
      src.AppendLine($"    public Task<{Group.ResponseName}> Handle({Group.RequestName} request, CancellationToken ct)");
      src.AppendLine("    {");
      src.Append($"      var rawResponse = _handlers.{Group.SourceMethod.Name}(");
      WriteHandlerParams(src, "request");
      src.AppendLine(");");
      src.Append("      var responseObject = ");
      WriteCreateResponse(src, "rawResponse");
      src.AppendLine(";");
      src.AppendLine("      return Task.FromResult(responseObject);");
      src.AppendLine("    }");
    }

    private void WriteHandleMethodAsync(StringBuilder src)
    {
      src.AppendLine($"    public async Task<{Group.ResponseName}> Handle({Group.RequestName} request, CancellationToken ct)");
      src.AppendLine("    {");
      src.Append($"      var rawResponse = await _handlers.{Group.SourceMethod.Name}(");
      WriteHandlerParams(src, "request");
      src.Append("      var responseObject = ");
      WriteCreateResponse(src, "rawResponse");
      src.AppendLine(";");
      src.Append("      return responseObject;");
      src.AppendLine("    }");
    }

    private void WriteCreateResponse(StringBuilder src, string rawResponseVar)
    {
      src.Append($"new {Group.ResponseName}(");
      if (Group.ResponseParams.Length == 1)
      {
        src.Append(rawResponseVar);
      }
      else
      {
        // multiple response params means tuple
        src.Append(string.Join(",", Group.ResponseParams.Select(p => $"{rawResponseVar}.{p.PropertyName}")));
      }
      src.Append(")");
    }

    private void WriteHandlerParams(StringBuilder src, string requestVar)
    {
      src.Append(string.Join(",", Group.RequestParams.Select(p => $"{requestVar}.{p.PropertyName}")));
    }

    private void WriteConstructor(StringBuilder src)
    {
      src.AppendLine($"    public {Group.HandlerName}({Group.Class.Name} handlers)");
      src.AppendLine("    {");
      src.AppendLine($"      _handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));");
      src.AppendLine("    }");
      src.AppendLine();
    }

    private void WriteFields(StringBuilder src)
    {
      src.AppendLine($"    private readonly {Group.Class.Name} _handlers;");
      src.AppendLine();
    }

    private void WriteStartClass(StringBuilder src)
    {
      src.AppendLine("  /// <summary>");
      src.AppendLine($"  /// Auto-generated handler based on handler defined in <see cref=\"{Group.Class.Name}\"/>.");
      src.AppendLine("  /// </summary>");
      src.AppendLine($"  public sealed class {Group.HandlerName} : IRequestHandler<{Group.RequestName},{Group.ResponseName}>");
      src.AppendLine("  {");
    }

    private void WriteEndClass(StringBuilder src)
    {
      src.AppendLine("  }");
    }

    private static void WriteUsings(StringBuilder src)
    {
      src.AppendLine("using System.Threading;");
      src.AppendLine("using System.Threading.Tasks;");
      src.AppendLine("using MediatR;");
    }

    private void WriteStartNamespace(StringBuilder src)
    {
      src.AppendLine($"namespace {Group.Namespace}");
      src.AppendLine("{");
    }

    private static void WriteEndNamespace(StringBuilder src)
    {
      src.AppendLine("}");
    }
  }
}