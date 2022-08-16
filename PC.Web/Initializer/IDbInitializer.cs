using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PC.Web.Initializer
{
    public interface IDbInitializer
    {
        public void Initialize();
    }
}
