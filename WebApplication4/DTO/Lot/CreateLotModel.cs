﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using WebApplication4.Attributes;

namespace WebApplication4.DTO.Lot
{
    public class CreateLotModel : LotModel
    {
        [DataType(DataType.Upload)]
        [MaxFileSize(5 * 1024 * 1024)]
        [AllowedExtensions(new [] {".jpg", ".jpeg", ".png", ".webp", ".gif"})]
        public IFormFile Image { get; set; }

        public CreateLotModel() { }
        
        public CreateLotModel(Domain.Core.Lot lot) : base(lot) { }
    }
}