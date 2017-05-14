using Mot.Shared.Model.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace motMachineInterface
{
    public class FormsAuthManager : AuthorizationManager
    {
        private readonly ClaimsPrincipal principal;
        public FormsAuthManager(IEnumerable<IAccessPolicy> policies) : base(policies)
        {
            principal = base.CurrentPrincipal;
        }

        protected override ClaimsPrincipal CurrentPrincipal => principal;
    }
}
