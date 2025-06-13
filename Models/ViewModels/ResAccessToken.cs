namespace TodoApi.Models.ViewModels;

public class ResAccessToken
{
    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    [System.Text.Json.Serialization.JsonPropertyName("access_token")]
    public required string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the expiration time of the access token.
    /// </summary>
    [System.Text.Json.Serialization.JsonPropertyName("expires_at")]
    public DateTime ExpiresAt { get; set; }

}