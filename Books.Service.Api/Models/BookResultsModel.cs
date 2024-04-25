using System;
namespace Books.Service.Internal.Api.Models
{
    public class BookResultsModel
    {
        public string Kind { get; set; }
        public int TotalItems { get; set; }
        public IList<BookInfoModel> Items { get; set; }
    }

    public class BookInfoModel
    {
        public string Kind { get; set; }
        public string Id { get; set; }
        public string Etag { get; set; }
        public string SelfLink { get; set; }
        public VolumeInfoModel VolumeInfo { get; set; }
    }

    public class VolumeInfoModel
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public IList<string> Authors { get; set; }
        public string Publisher { get; set; }
        public string PublishedDate { get; set; }
        public string Description { get; set; }
        public IList<IndustryIdentifier> IndustryIdentifiers { get; set; }
        public int pageCount { get; set; }
        public string printType { get; set; }
        public IList<string> Categories { get; set; }
        public int? averageRating { get; set; }
        public int? ratingsCount { get; set; }
        public ImageLink ImageLinks { get; set; }

        public string Language { get; set; }
        
    }

    public class IndustryIdentifier
    {
        public string Type { get; set; }
        public string Identifier { get; set; }
    }

    public class ImageLink
    {
        public string SmallThumbnail { get; set; }
        public string Thumbnail { get; set; }
    }
}
