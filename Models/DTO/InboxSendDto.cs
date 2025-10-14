namespace Archery.Models.DTO
{
    public class InboxSendDto
    {
        public int Receiver { get; set; }
        public string Message { get; set; } = null!;
    }

}