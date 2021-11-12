using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechTalks.Generators.Demo.Mediator
{
  public sealed class SampleHandlers
  {
    public async ValueTask<bool> HandleCreateUser(string userName, string email)
    {
      await Task.Delay(1000);

      return true;
    }
  }
}
