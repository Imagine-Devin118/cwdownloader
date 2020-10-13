using System.Collections.Generic;

public class Book
{
    /// <summary>
    ///
    /// </summary>
    public int VersionId { get; set; }
    /// <summary>
    ///
    /// </summary>
    public int PocketVersionId { get; set; }
    /// <summary>
    ///
    /// </summary>
    public List<string> Resources { get; set; }
}

public class Body<T>
{
    /// <summary>
    ///
    /// </summary>
    public List<T> data { get; set; }
    /// <summary>T
    ///
    /// </summary>
    public int totalCount { get; set; }
}

public class RequestBody<T>
{
    /// <summary>
    ///
    /// </summary>
    public string message { get; set; }
    /// <summary>
    ///
    /// </summary>
    public int statusCode { get; set; }
    /// <summary>
    ///
    /// </summary>
    public Body<T> body { get; set; }
}