using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalks.Generators.Models;

namespace TechTalks.Generators.Emitters
{
  internal sealed class ResponseEmitter : ISourceEmitter
  {
    public ResponseEmitter(MediatorGroup group) => Group = group;

    private MediatorGroup Group { get; }


    public SourceText GetSource()
    {
      var src = new StringBuilder();

      src.AppendLine($"namespace {Group.Namespace}.Responses");
      src.AppendLine("{");
      src.AppendLine("  /// <summary>");
      src.AppendLine($"  /// Auto-generated response based on handler defined in <see cref=\"{Group.Class}\"/>.");
      src.AppendLine("  /// </summary>");
      src.Append($"  public record {Group.ResponseName}(");
      src.Append(string.Join(",", Group.ResponseParams.Select(p => $"{p.Type} {p.PropertyName}")));
      src.AppendLine(");");
      src.AppendLine("}");

      return SourceText.From(src.ToString(), Encoding.UTF8);
    }
  }
}
