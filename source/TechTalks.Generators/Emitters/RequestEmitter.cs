using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalks.Generators.Models;

namespace TechTalks.Generators.Emitters
{
  internal sealed class RequestEmitter : ISourceEmitter
  {
    public RequestEmitter(MediatorGroup group) => Group = group;

    private MediatorGroup Group { get; }

    public SourceText GetSource()
    {
      var requestParams = string.Join(",", Group.RequestParams.Select(p => $"{p.Type} {p.PropertyName}"));

      var src = new StringBuilder();

      src.AppendLine("using MediatR;");
      src.AppendLine($"using {Group.Namespace}.Responses;");
      src.AppendLine();
      src.AppendLine($"namespace {Group.Namespace}.Requests");
      src.AppendLine("{");
      src.AppendLine("  /// <summary>");
      src.AppendLine($"  /// Auto-generated request based on handler defined in <see cref=\"{Group.Class}\"/>.");
      src.AppendLine("  /// </summary>");
      src.AppendLine($"  public record {Group.RequestName}({requestParams}) : IRequest<{Group.ResponseName}>;");
      src.AppendLine("}");

      return SourceText.From(src.ToString(), Encoding.UTF8);
    }
  }
}
