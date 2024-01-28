using System.ComponentModel.DataAnnotations.Schema;

namespace Maturaball_Tischreservierung.Models
{
    public class ClientAccessCode : BaseDBEntity
    {
        public string AccessCode { get; set; } = "";

        public Guid ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        public Client? Client { get; set; }

        public DateTime ExpiresAt { get; set; }
        public DateTime? UsedAt { get; set; } = null;
    }
}
