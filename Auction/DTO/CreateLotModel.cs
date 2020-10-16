﻿using System;
using System.ComponentModel.DataAnnotations;
using Auction.Models;
using Microsoft.AspNetCore.Http;

namespace Auction.DTO
{
    public class CreateLotModel
    {
        [MaxLength(35, ErrorMessage = "Title must be less then 35 characters")]
        public string Title { get; set; }
        
        [MaxLength(135, ErrorMessage = "Description must be less then 35 characters")]
        public string Description { get; set; }
        
        public IFormFile Image { get; set; }
        public string ImageUrl { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime LunchAt { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime EndAt { get; set; }
        
        public decimal Goal { get; set; }
        
        [DataType(DataType.MultilineText)]
        public string Story { get; set; }

        public CreateLotModel(Lot lot)
        {
            Title = lot.Title;
            Description = lot.Description;
            ImageUrl = lot.ImageUrl;
            LunchAt = lot.LunchAt;
            EndAt = lot.EndAt;
            Goal = lot.Goal;
            Story = lot.Story;
        }
    }
}