namespace Estudaki.Modules.Identity.Application.DTOs;

/// <summary>
/// DTO de resposta para o login com sucesso - inclui roles do usuário.
/// </summary>
public record LoginResponseDto(
    string UserId,
    string Email,
    string UserName,
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiry,
    DateTime RefreshTokenExpiry,
    int AccessTokenExpiresInSeconds,
    List<string> Roles
)
{
    /// <summary>
    /// Factory method para criar um DTO a partir de um LoginCommandResult.
    /// </summary>
    public static LoginResponseDto FromLoginCommandResult(
        string userId,
        string email,
        string userName,
        string accessToken,
        string refreshToken,
        DateTime accessTokenExpiry,
        DateTime refreshTokenExpiry,
        List<string> roles)
    {
        var expiresInSeconds = (int)(accessTokenExpiry - DateTime.UtcNow).TotalSeconds;
        return new LoginResponseDto(
            userId,
            email,
            userName,
            accessToken,
            refreshToken,
            accessTokenExpiry,
            refreshTokenExpiry,
            expiresInSeconds > 0 ? expiresInSeconds : 0,
            roles
        );
    }
}

/// <summary>
/// DTO de resposta para a renovação de token com sucesso - inclui roles do usuário.
/// </summary>
public record RefreshTokenResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiry,
    DateTime RefreshTokenExpiry,
    int AccessTokenExpiresInSeconds,
    List<string> Roles
)
{
    /// <summary>
    /// Factory method para criar um DTO a partir de um RefreshTokenCommandResult.
    /// </summary>
    public static RefreshTokenResponseDto FromRefreshTokenCommandResult(
        string accessToken,
        string refreshToken,
        DateTime accessTokenExpiry,
        DateTime refreshTokenExpiry,
        List<string> roles)
    {
        var expiresInSeconds = (int)(accessTokenExpiry - DateTime.UtcNow).TotalSeconds;
        return new RefreshTokenResponseDto(
            accessToken,
            refreshToken,
            accessTokenExpiry,
            refreshTokenExpiry,
            expiresInSeconds > 0 ? expiresInSeconds : 0,
            roles
        );
    }
}
