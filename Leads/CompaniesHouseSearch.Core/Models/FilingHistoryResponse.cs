using System.Text.Json.Serialization;

namespace CompaniesHouseSearch.Models;

public class FilingHistoryResponse
{
    [JsonPropertyName("items")]
    public List<FilingHistoryItem> Items { get; set; } = [];

    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }
}

public class FilingHistoryItem
{
    [JsonPropertyName("category")]
    public string Category { get; set; } = "";

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    [JsonPropertyName("date")]
    public string Date { get; set; } = "";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("links")]
    public FilingLinks? Links { get; set; }

    public string? DocumentMetadataUrl => Links?.DocumentMetadata;

    // Extract the document ID from the metadata URL (last path segment)
    public string? DocumentId => DocumentMetadataUrl is not null
        ? DocumentMetadataUrl.TrimEnd('/').Split('/').LastOrDefault()
        : null;

    public string DescriptionFormatted =>
        Description.Replace("-", " ").Replace("_", " ");
}

public class FilingLinks
{
    [JsonPropertyName("document_metadata")]
    public string? DocumentMetadata { get; set; }
}
