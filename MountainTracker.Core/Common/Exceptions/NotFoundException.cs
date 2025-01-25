using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainTracker.Core.Common.Exceptions
{
    public class NotFoundException : DomainException
    {
        public NotFoundException(string entityName, object key)
            : base($"{entityName} (key={key}) not found.") { }
    }
}
