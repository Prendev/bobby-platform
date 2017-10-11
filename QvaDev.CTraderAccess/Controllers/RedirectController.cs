using System.Web.Http;

namespace QvaDev.CTraderAccess.Controllers
{
    public class RedirectController : ApiController
    {
        public IHttpActionResult Get([FromUri]string id, string code)
        {
            return Ok();
        }
    }
}
