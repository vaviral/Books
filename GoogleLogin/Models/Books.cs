using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoogleLogin.Models
{
    public class GoogleBooksApiResult
    {
        public string Kind { get; set; }
        public int TotalItems { get; set; }
        public BookDetails[] Items { get; set; }
    }
    public class BookDetails
    {
        public string Kind { get; set; }
        public string Id { get; set; }
        public string Etag { get; set; }
        public string SelfLink { get; set; }
        public VolumeInfo VolumeInfo { get; set; }
        public SaleInfo SaleInfo { get; set; }
        public SearchInfo SearchInfo { get; set; }
    }
    public class VolumeInfo
    {
        public string Title { get; set; }
        public string[] Authors { get; set; }
        public string Publisher { get; set; }
        public string PublishedDate { get; set; }
        public string Description { get; set; }
        public IndustryIdentifiers[] IndustryIdentifiers { get; set; }
        public ReadingModes ReadingModes { get; set; }
        public string PageCount { get; set; }
        public string PrintType { get; set; }
        public string[] Categories { get; set; }
        public string MaturityRating { get; set; }
        public string AllowAnonLogging { get; set; }
        public string ContentVersion { get; set; }
        public ImageLinks ImageLinks { get; set; }
        public string Language { get; set; }
        public string PreviewLink { get; set; }
        public string InfoLink { get; set; }
        public string CanonicalVolumeLink { get; set; }
    }
    public class IndustryIdentifiers
    {
        public string Type { get; set; }
        public string Identifier { get; set; }
    }
    public class ReadingModes
    {
        public bool Text { get; set; }
        public string Image { get; set; }
    }
    public class ImageLinks
    {
        public string SmallThumbnail { get; set; }
        public string Thumbnail { get; set; }
    }
    public class SaleInfo
    {
        public string Country { get; set; }
        public string Saleability { get; set; }
        public bool IsEbook { get; set; }
        public LastPrice LastPrice { get; set; }
        public LastPrice RetailPrice { get; set; }
        public string BuyLink { get; set; }
        public Offers[] Offers { get; set; }
    }
    public class AccessInfo
    {
        public string Country { get; set; }
        public string Viewability { get; set; }
        public bool Embeddable { get; set; }
        public bool PublicDomain { get; set; }
        public string TextToSpeechPermission { get; set; }
        public Epub Epub { get; set; }
        public Pdf Pdf { get; set; }
        public string WebReaderLink { get; set; }
        public string AccessViewStatus { get; set; }
        public bool QuoteSharingAllowed { get; set; }
    }
    public class Epub
    {
        public bool IsAvailable { get; set; }
    }
    public class Pdf
    {
        public bool IsAvailable { get; set; }
    }
    public class SearchInfo
    {
        public string TextSnippet { get; set; }
    }
    public class LastPrice
    {
        public double Amount { get; set; }
        public string CurrencyCode { get; set; }
    }
    public class RetailPrice
    {
        public double Amount { get; set; }
        public string CurrencyCode { get; set; }
    }
    public class Offers
    {
        public int FinskyOfferType { get; set; }
        public ListPrice ListPrice { get; set; }
        public ListPrice RetailPrice { get; set; }
    }
    public class ListPrice
    {
        public double AmountInMicros { get; set; }
        public string CurrencyCode { get; set; }
    }
    public class Result
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class Favourites
    {
        public string UserId { get; set; }
        public List<string> LinkForThumbnail { get; set; }
        public List<string> Publisher { get; set; }
        public List<string> NameOfBook { get; set; }
        public List<string> IsbnOfBook { get; set; }
        public List<List<string>> Authors { get; set; }
        public List<List<string>> Category { get; set; }
    }
}