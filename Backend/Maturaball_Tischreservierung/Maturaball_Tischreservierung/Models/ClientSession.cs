using System.ComponentModel.DataAnnotations.Schema;

namespace Maturaball_Tischreservierung.Models
{
    public class ClientSession : BaseDBEntity
    {
        public Guid ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        public Client? Client { get; set; }
    }
}
