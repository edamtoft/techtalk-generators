using Microsoft.CodeAnalysis;
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace TechTalks.Generators.Mediator.Models
{
  internal readonly struct MediatorParameter
  {
    public string Name { get; }
    public ITypeSymbol Type { get; }

    public MediatorParameter(IParameterSymbol source)
    {
      Name = PascalCase(source.Name ?? "");
      Type = source?.Type ?? throw new ArgumentNullException(nameof(source));
    }

    public MediatorParameter(string name, ITypeSymbol type)
    {
      Name = name ?? throw new ArgumentNullException(nameof(name));
      Type = type ?? throw new ArgumentNullException(nameof(type));
    }

    private static string PascalCase(string name)
    {
      var sb = new StringBuilder(name);
      if (!char.IsUpper(sb[0]))
      {
        sb[0] = char.ToUpper(sb[0]);
      }
      return sb.ToString();
    }
  }
}