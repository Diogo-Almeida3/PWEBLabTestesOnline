using System.Collections.Generic;

namespace PWEBLabTestesOnline.Models
{
    public class UsersViewModel
    {
        public ApplicationUser ApplicationUser { get; set; }
        public string Role { get; set; }
        public ICollection<Laboratories> Laboratories { get; set; }
    }
}
