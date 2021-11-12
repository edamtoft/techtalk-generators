using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalks.Generators.Mediator.Models;

namespace TechTalks.Generators.Mediator.Emitters
{
  internal sealed class RequestEmitter : ISourceEmitter
  {
    public SourceText GetSource(MediatorGroup group)
    {
      var requestParams = string.Join(",", group.RequestParams.Select(p => $"{p.Type} {p.Name}"));

      var src = new StringBuilder();

      src.AppendLine("using MediatR;");
      src.AppendLine();
      src.AppendLine($"namespace {group.Namespace}");
      src.AppendLine("{");
      src.AppendLine("  /// <summary>");
      src.AppendLine($"  /// Auto-generated request based on handler defined in <see cref=\"{group.Class.Name}\"/>.");
      src.AppendLine("  /// </summary>");
      src.AppendLine($"  public record {group.RequestName}({requestParams}) : IRequest<{group.ResponseName}>;");
      src.AppendLine("}");

      return SourceText.From(src.ToString(), Encoding.UTF8);
    }
  }
}
