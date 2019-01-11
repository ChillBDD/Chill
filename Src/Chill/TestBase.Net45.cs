using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chill.Autofac;

namespace Chill
{
    public partial class TestBase
    {
        partial void GetBuiltInContainer(ref object attribute)
        {
            if (attribute == null)
            {
                attribute = new ChillContainerAttribute(typeof (AutofacChillContainer));
            }
        }
    }
}
