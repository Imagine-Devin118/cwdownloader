using System.Collections.Generic;
using Newtonsoft.Json;

public class OutputInteractiveMaterialAndBookDto
{
    public int Id { get; set; }

    public string MaterialName { get; set; }

    public string CoverImageUrl { get; set; }

    public List<InteractiveTextBook> booksummary { get; set; }
}

public class InteractiveTextBook
{
    public int Id { get; set; }

    public string BookName { get; set; }

    public int TeachMaterialId { get; set; }

    public string CoverImageUrl { get; set; }

    public int VersionId { get; set; }

    public int PocketVersionid { get; set; }

    [JsonIgnore]
    public bool downloading;
}