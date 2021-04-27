namespace Airslip.Security.Jwt
{
    public record JwtSettings
    {
        public string Key { get; init; } = string.Empty;
        public string Issuer { get; init; } = string.Empty;
        public string Audience { get; init; } = string.Empty;
        public int ExpiresTime { get; init; } = 300;
    }
}