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
    public SourceText GetSource(MediatorGroup group)
    {
      var src = new StringBuilder();

      src.AppendLine("using System.Threading;");
      src.AppendLine("using System.Threading.Tasks;");
      src.AppendLine("using MediatR;");
      src.AppendLine();
      src.AppendLine($"namespace {group.Namespace}");
      src.AppendLine("{");
      src.AppendLine("  /// <summary>");
      src.AppendLine($"  /// Auto-generated handler based on handler defined in <see cref=\"{group.Class.Name}\"/>.");
      src.AppendLine("  /// </summary>");
      src.AppendLine($"  public sealed class {group.HandlerName} : IRequestHandler<{group.RequestName},{group.ResponseName}>");
      src.AppendLine("  {");
      src.AppendLine($"    private readonly {group.Class.Name} _handlers;");
      src.AppendLine();
      src.AppendLine($"    public {group.HandlerName}({group.Class.Name} handlers)");
      src.AppendLine("    {");
      src.AppendLine($"      _handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));");
      src.AppendLine("    }");
      src.AppendLine();

      var requestParams = string.Join(",", group.RequestParams.Select(p => $"request.{p.Name}"));

      if (group.IsAsync)
      {
        src.AppendLine($"    public async Task<{group.ResponseName}> Handle({group.RequestName} request, CancellationToken ct)");
        src.AppendLine("    {");
        src.AppendLine($"      var value = await _handlers.{group.SourceMethod.Name}({requestParams});");
        src.AppendLine($"      return new {group.ResponseName}(value);");
        src.AppendLine("    }");
      }
      else
      {
        src.AppendLine($"    public Task<{group.ResponseName}> Handle({group.RequestName} request, CancellationToken ct)");
        src.AppendLine("    {");
        src.AppendLine($"      var value = _handlers.{group.SourceMethod.Name}({requestParams});");
        src.AppendLine($"      return Task.FromResult(new {group.ResponseName}(value));");
        src.AppendLine("    }");
      }

      src.AppendLine("  }");
      src.AppendLine("}");

      return SourceText.From(src.ToString(), Encoding.UTF8);
    }
  }
}