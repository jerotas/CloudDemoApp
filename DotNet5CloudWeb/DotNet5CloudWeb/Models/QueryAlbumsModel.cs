using System.Collections.Generic;
using DotNet5CloudWeb.Controllers;
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
        public List<AlbumCollection> AlbumDetails { get; set; }

        public bool HasFilter => !string.IsNullOrWhiteSpace(AlbumNameFilter) && AlbumNameFilter != AlbumController.NoFilterWord;
    }
}