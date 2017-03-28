using System.Security.Principal;
using System.Threading;

namespace LibraryService.Tests
{
    public static class TestHelpers
    {
        public static void SetCurrentPrincipal(string role)
        {
            var identity = new GenericIdentity("TestUser");
            var roles = new[] { role };
            var principal = new GenericPrincipal(identity, roles);
            Thread.CurrentPrincipal = principal;
        }
    }
}
