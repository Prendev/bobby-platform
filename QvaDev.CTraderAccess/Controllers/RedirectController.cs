using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using System.Web;
using QvaDev.Common.Services;
using QvaDev.Data.Models;

namespace QvaDev.CTraderAccess.Controllers
{
    [AllowAnonymous]
    public class RedirectController : ApiController
    {
        public IHttpActionResult Get(string id, [FromUri]string code = null)
        {
            if (string.IsNullOrWhiteSpace(code)) return BadRequest("Missing code");

            var p = GetCTraderPlatforms()?.FirstOrDefault(e => e.ClientId == id);

            if (p == null) return BadRequest("Missing cTrader platform");

            var accessUri = $"{p.AccessBaseUrl}/auth?grant_type=authorization_code&" +
                            $"client_id={p.ClientId}&" +
                            $"client_secret={p.Secret}&" +
                            $"redirect_uri={HttpUtility.UrlEncode(p.Playground)}&" +
                            $"code={code}";

            return Redirect(accessUri);
        }

        private List<CTraderPlatform> GetCTraderPlatforms()
        {
            var platforms = new List<CTraderPlatform>();
            try
            {
                var fullPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/cTraderPlatforms.xml");
                var xmlService = new XmlService(null);
                platforms = xmlService.DeserializeXmlFile<List<CTraderPlatform>>(fullPath);
            }
            catch { }
            return platforms;
        }
    }
}
