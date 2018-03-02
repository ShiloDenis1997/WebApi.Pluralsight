using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace CountingKs.Services
{
    public class CountingKsIdentityService : ICountingKsIdentityService
    {
#if DEBUG
        public string CurrentUser => "shawnwildermuth";
#else
        public string CurrentUser => Thread.CurrentPrincipal.Identity.Name;
#endif
}
}