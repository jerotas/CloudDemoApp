﻿using System.Collections.Generic;
using DotNet5CloudWeb.Models.DocumentDbModels;

namespace DotNet5CloudWeb.Models {
    public class QueryAlbumsModel
    {
        public QueryAlbumsModel()
        {
            AlbumNames = new List<string>();
        }

        public string AlbumNameFilter { get; set; }
        public List<string> AlbumNames { get; set; } 
        public List<Album> AlbumDetails { get; set; }
    }
}