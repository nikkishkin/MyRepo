using System.Collections.Generic;

namespace ArtGallery.Models.Forum
{
    public class ForumModel
    {
        public IEnumerable<CommentModel> Comments { get; set; }

        public int PictureIndex { get; set; }
    }
}