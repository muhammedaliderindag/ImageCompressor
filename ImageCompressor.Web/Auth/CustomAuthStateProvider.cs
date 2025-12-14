using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace ImageCompressor.Web.Auth;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationState _anonymous;

    public CustomAuthStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
        _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
                return _anonymous;

            // Token'ı çözümle ve kimliği oluştur
            var principal = GetPrincipalFromToken(token);
            return new AuthenticationState(principal);
        }
        catch
        {
            return _anonymous;
        }
    }

    public void NotifyUserAuthentication(string token)
    {
        var principal = GetPrincipalFromToken(token);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    public void NotifyUserLogout()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
    }

    // --- ÖNEMLİ DÜZELTME BURADA ---
    private ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var claims = jwtToken.Claims.ToList();

        // Sorunu Çözen Kısım: "role" claim'ini bulup .NET'in anladığı dile çeviriyoruz
        var roleClaim = claims.FirstOrDefault(c => c.Type == "role");
        if (roleClaim != null)
        {
            // Eğer "role" varsa, bunu "ClaimTypes.Role" olarak da ekle
            claims.Add(new Claim(ClaimTypes.Role, roleClaim.Value));
        }

        // Kimliği oluştururken Rol ve İsim claim'lerini açıkça belirtiyoruz
        var identity = new ClaimsIdentity(claims, "JwtAuth",
            nameType: ClaimTypes.Name,
            roleType: ClaimTypes.Role); // <-- Bu parametre çok kritik!

        return new ClaimsPrincipal(identity);
    }
}