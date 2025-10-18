using System;
using System.Collections.Generic;

namespace Archery.Models.DTO
{
    public class EliteArcherDTO : ArcherDTO
    {
        public int Rank { get; set; }
        public int TotalScore { get; set; }
        public string MonthYear { get; set; } = DateTime.Now.ToString("yyyy-MM");
        public List<GiftDTO> Gifts { get; set; } = new List<GiftDTO>();
        public string? SelectedGift { get; set; } // nếu đã chọn (từ in-memory)
    }

    public class GiftDTO
    {
        public int Id { get; set; } // local id (0..9)
        public string GiftName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}