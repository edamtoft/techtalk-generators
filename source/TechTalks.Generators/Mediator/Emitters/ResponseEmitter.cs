using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalks.Generators.Mediator.Models;

namespace TechTalks.Generators.Mediator.Emitters
{
  internal sealed class ResponseEmitter : ISourceEmitter
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    public SourceText GetSource(MediatorGroup group)
    {
      var src = new StringBuilder();

      src.AppendLine($"namespace {group.Namespace}");
      src.AppendLine("{");
      src.AppendLine("  /// <summary>");
      src.AppendLine($"  /// Auto-generated response based on handler defined in <see cref=\"{group.Class.Name}\"/>.");
      src.AppendLine("  /// </summary>");
      src.AppendLine($"  public record {group.ResponseName}({group.ResponseType} Value);");
      src.AppendLine("}");

      return SourceText.From(src.ToString(), Encoding.UTF8);
    }
  }
}
