using System.Web;

namespace DGame.Web.Models
{
    public class CreateGameViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public HttpPostedFileBase File { get; set; }
    }
}