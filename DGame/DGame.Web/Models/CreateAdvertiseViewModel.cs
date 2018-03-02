using System.Web;

namespace DGame.Web.Models
{
    public class CreateAdvertiseViewModel
    {
        public string Link { get; set; }

        public HttpPostedFileBase File { get; set; }

        public string TransactionHash { get; set; }
    }
}