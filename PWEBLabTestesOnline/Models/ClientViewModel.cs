using System.Collections.Generic;

namespace PWEBLabTestesOnline.Models
{
    public class ClientViewModel
    {
        public string Id { get; set; }

        public IEnumerable<ApplicationUser> Users { get; set; }
    }
}
