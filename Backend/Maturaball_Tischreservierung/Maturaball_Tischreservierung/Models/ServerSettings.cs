namespace Maturaball_Tischreservierung.Models
{
    public class ServerSettings : BaseDBEntity
    {
        public bool IsRegistrationOpen { get; set; } = false;
        public bool IsReservationOpen { get; set; } = false;
        public bool IsPageOpen { get; set; } = false;
        public bool IsInEarlyAccess { get; set; } = false;

        public string GreetMessage { get; set; } = "Welcome";
    }
}
