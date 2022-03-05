using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.Identity.Initialize
{
    public interface IDbInitializer
    {
        public void Initialize();
    }
}