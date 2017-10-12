using System.Web.Http;

namespace QvaDev.CTraderAccess.Controllers
{
    [AllowAnonymous]
    public class RedirectController : ApiController
    {
        public IHttpActionResult Get(string id, [FromUri]string code = null)
        {
            //var redirectUri = $"{p.AccessBaseUrl}auth?grant_type=authorization_code&" +
            //                  $"client_id={p.ClientId}&" +
            //                  $"client_secret={p.Secret}&" +
            //                  $"redirect_uri={Uri.EscapeDataString(p.Playground)}";

            return Ok("Sikerült!!!");
        }
    }
}
