using POS.API.MiddleWares;

namespace POS.API.UNITTEST.ApisTests.UserApis
{
    public class MockAuthBearerMiddleware : AuthBearerMiddleware
    {
        public MockAuthBearerMiddleware() : base("mockJwtKey", "mockJwtIssuer", "mockJwtAudience")
        {
        }
    }
}
