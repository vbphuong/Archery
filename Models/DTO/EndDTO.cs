namespace Archery.Models.DTO
{
    public class EndDTO
    {
        public int EndID { get; set; }
        public string? EndNumber { get; set; }
        public List<ArrowDTO> Arrows { get; set; } = new();
    }
}