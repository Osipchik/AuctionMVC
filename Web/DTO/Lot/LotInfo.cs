﻿using System;

namespace Web.DTO.Lot
{
    public class LotInfo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsAvailable { get; set; }
    }
}