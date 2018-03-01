using System.Web;

namespace DGame.Web.Models
{
    public class CreateAdvertiseViewModel
    {
        public HttpPostedFileBase File { get; set; }

        public string TransactionHash { get; set; }
    }
}