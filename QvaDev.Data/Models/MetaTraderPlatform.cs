﻿using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
    public class MetaTraderPlatform : BaseDescriptionEntity
    {
        [Required]
        public string SrvFilePath { get; set; }
    }
}
