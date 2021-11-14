using Microsoft.CodeAnalysis;
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace TechTalks.Generators.Models
{
  internal readonly struct MediatorParameter
  {
    public string PropertyName { get; }
    public ITypeSymbol Type { get; }

    public MediatorParameter(IParameterSymbol source)
    {
      if (source is null)
      {
        throw new ArgumentNullException(nameof(source));
      }

      PropertyName = PascalCase(source.Name ?? "");
      Type = source.Type;
    }

    public MediatorParameter(string propertyName, ITypeSymbol type)
    {
      PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
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