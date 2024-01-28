using System.ComponentModel.DataAnnotations;

namespace Maturaball_Tischreservierung.Models
{
    public class Client : BaseDBEntity
    {
        public string Name { get; set; } = "";
        public string Surname { get; set; } = "";
        [EmailAddress]
        public string Email { get; set; } = "";

        public string Class { get; set; } = "";

        public bool EmailVerified { get; set; } = false;
    }
}
