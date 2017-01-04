﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Library.Models
{
    [Serializable]
    public class Branch: Identifiable
    {
        public int Id { get; set; }

        [Required, StringLength(20)]
        public string Name { get; set; }
    }
}