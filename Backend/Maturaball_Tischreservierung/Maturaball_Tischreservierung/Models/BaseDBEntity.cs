using System.ComponentModel.DataAnnotations;

namespace Maturaball_Tischreservierung.Models
{
    public class BaseDBEntity
    {
        [Key]
        public Guid Id { get; set; }

        public Guid Version { get; set; }
    }
}
