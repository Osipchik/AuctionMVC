using System.ComponentModel.DataAnnotations;
using Data;
using Microsoft.AspNetCore.Http;
using Web.Attributes;

namespace Web.DTO
{
    public class CreateLotModel : LotModel
    {
        [DataType(DataType.Upload)]
        [MaxFileSize(5 * 1024 * 1024)]
        [AllowedExtensions(new [] {".jpg", ".jpeg", ".png", ".webp", ".gif"})]
        public IFormFile Image { get; set; }

        public CreateLotModel() { }
        
        public CreateLotModel(Lot lot) : base(lot) { }
    }
}