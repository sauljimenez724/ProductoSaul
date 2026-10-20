using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using ProductoSaul.Data;

namespace ProductoSaul.Components.Account
{
    /*
     * Proveedor de estado de autenticación para aplicaciones Blazor Server.
     * Este proveedor verifica el "security stamp" del usuario conectado cada 30 minutos mientras el circuito interactivo esté activo.
     * El "security stamp" se utiliza para detectar cambios en el estado de seguridad del usuario, como un cambio de contraseña
     * o la revocación de un inicio de sesión.
     */
    internal sealed class IdentityRevalidatingAuthenticationStateProvider(
            ILoggerFactory loggerFactory,
            IServiceScopeFactory scopeFactory,
            IOptions<IdentityOptions> options)
        : RevalidatingServerAuthenticationStateProvider(loggerFactory)
    {
        // Intervalo de revalidación: cada 30 minutos
        protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

        // Método para validar el estado de autenticación
        protected override async Task<bool> ValidateAuthenticationStateAsync(
            AuthenticationState authenticationState, CancellationToken cancellationToken)
        {
            // Crea un nuevo ámbito para obtener un UserManager fresco, garantizando datos actualizados
            await using var scope = scopeFactory.CreateAsyncScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            return await ValidateSecurityStampAsync(userManager, authenticationState.User);
        }

        // Método auxiliar para validar el "security stamp" del usuario
        private async Task<bool> ValidateSecurityStampAsync(UserManager<ApplicationUser> userManager, ClaimsPrincipal principal)
        {
            var user = await userManager.GetUserAsync(principal);
            if (user is null)
            {
                // El usuario ya no existe
                return false;
            }
            else if (!userManager.SupportsUserSecurityStamp)
            {
                // Si el UserManager no admite "security stamp", la validación pasa automáticamente
                return true;
            }
            else
            {
                // Compara el "security stamp" del ClaimsPrincipal con el del usuario en la base de datos
                var principalStamp = principal.FindFirstValue(options.Value.ClaimsIdentity.SecurityStampClaimType);
                var userStamp = await userManager.GetSecurityStampAsync(user);
                return principalStamp == userStamp;
            }
        }
    }
}
