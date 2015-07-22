﻿using System.Collections.Generic;

namespace ArtGallery.Models.Home
{
    public class ExhibitListModel
    {
        public IEnumerable<ExhibitModel> Exhibits { get; set; }

        public int PageNumber { get; set; }

        public int PagesCount { get; set; }

        public bool NextPageExists { get; set; }
    }
}