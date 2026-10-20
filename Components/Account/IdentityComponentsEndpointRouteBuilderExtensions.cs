using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using ProductoSaul.Components.Account.Pages;
using ProductoSaul.Components.Account.Pages.Manage;
using ProductoSaul.Data;

namespace ProductoSaul.Components.Account
{
    internal static class IdentityComponentsEndpointRouteBuilderExtensions
    {
        /*
         * Extensión para configurar los endpoints adicionales necesarios para las páginas de Identidad,
         * basadas en Razor Components dentro del directorio `/Components/Account/Pages`.
         */
        public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
        {
            ArgumentNullException.ThrowIfNull(endpoints);

            // Grupo de rutas para las páginas de la cuenta
            var accountGroup = endpoints.MapGroup("/Account");

            // Endpoint para realizar un inicio de sesión externo
            accountGroup.MapPost("/PerformExternalLogin", (
                HttpContext context,
                [FromServices] SignInManager<ApplicationUser> signInManager,
                [FromForm] string provider,
                [FromForm] string returnUrl) =>
            {
                // Configura el redireccionamiento al proveedor externo
                IEnumerable<KeyValuePair<string, StringValues>> query = [
                    new("ReturnUrl", returnUrl),
                    new("Action", ExternalLogin.LoginCallbackAction)];

                var redirectUrl = UriHelper.BuildRelative(
                    context.Request.PathBase,
                    "/Account/ExternalLogin",
                    QueryString.Create(query));

                var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
                return TypedResults.Challenge(properties, [provider]);
            });

            // Endpoint para cerrar sesión
            accountGroup.MapPost("/Logout", async (
                ClaimsPrincipal user,
                SignInManager<ApplicationUser> signInManager,
                [FromForm] string returnUrl) =>
            {
                await signInManager.SignOutAsync();
                return TypedResults.LocalRedirect($"~/{returnUrl}");
            });

            // Grupo de rutas protegidas bajo la sección de "Administrar cuenta"
            var manageGroup = accountGroup.MapGroup("/Manage").RequireAuthorization();

            // Endpoint para vincular un inicio de sesión externo
            manageGroup.MapPost("/LinkExternalLogin", async (
                HttpContext context,
                [FromServices] SignInManager<ApplicationUser> signInManager,
                [FromForm] string provider) =>
            {
                // Limpia cualquier cookie de inicio de sesión externo para garantizar un proceso limpio
                await context.SignOutAsync(IdentityConstants.ExternalScheme);

                var redirectUrl = UriHelper.BuildRelative(
                    context.Request.PathBase,
                    "/Account/Manage/ExternalLogins",
                    QueryString.Create("Action", ExternalLogins.LinkLoginCallbackAction));

                var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, signInManager.UserManager.GetUserId(context.User));
                return TypedResults.Challenge(properties, [provider]);
            });

            // Logger para la descarga de datos personales
            var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var downloadLogger = loggerFactory.CreateLogger("DownloadPersonalData");

            // Endpoint para descargar los datos personales del usuario
            manageGroup.MapPost("/DownloadPersonalData", async (
                HttpContext context,
                [FromServices] UserManager<ApplicationUser> userManager,
                [FromServices] AuthenticationStateProvider authenticationStateProvider) =>
            {
                var user = await userManager.GetUserAsync(context.User);
                if (user is null)
                {
                    return Results.NotFound($"No se pudo cargar el usuario con ID '{userManager.GetUserId(context.User)}'.");
                }

                var userId = await userManager.GetUserIdAsync(user);
                downloadLogger.LogInformation("El usuario con ID '{UserId}' solicitó sus datos personales.", userId);

                // Incluye solo los datos personales para la descarga
                var personalData = new Dictionary<string, string>();
                var personalDataProps = typeof(ApplicationUser).GetProperties().Where(
                    prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
                foreach (var p in personalDataProps)
                {
                    personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
                }

                var logins = await userManager.GetLoginsAsync(user);
                foreach (var l in logins)
                {
                    personalData.Add($"{l.LoginProvider} clave del proveedor externo", l.ProviderKey);
                }

                personalData.Add("Clave del autenticador", (await userManager.GetAuthenticatorKeyAsync(user))!);
                var fileBytes = JsonSerializer.SerializeToUtf8Bytes(personalData);

                context.Response.Headers.TryAdd("Content-Disposition", "attachment; filename=DatosPersonales.json");
                return TypedResults.File(fileBytes, contentType: "application/json", fileDownloadName: "DatosPersonales.json");
            });

            return accountGroup;
        }
    }
}
