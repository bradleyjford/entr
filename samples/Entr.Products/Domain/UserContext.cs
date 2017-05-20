using System;
using Entr.Domain;

namespace Entr.Products.Domain
{
    public class UserContext : UserContext<Guid>
    {
        public override Guid UserId => Guid.Empty;
    }
}
