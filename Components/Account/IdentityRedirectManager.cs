using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace ProductoSaul.Components.Account
{
    /*
     * Clase para gestionar redirecciones dentro de componentes de identidad.
     * Permite manejar redirecciones con soporte para agregar mensajes de estado utilizando cookies
     * y asegura que las rutas sean válidas para evitar redirecciones abiertas.
     */
    internal sealed class IdentityRedirectManager(NavigationManager navigationManager)
    {
        // Nombre de la cookie donde se almacena el mensaje de estado
        public const string StatusCookieName = "Identity.StatusMessage";

        // Configuración de la cookie para mensajes de estado
        private static readonly CookieBuilder StatusCookieBuilder = new()
        {
            SameSite = SameSiteMode.Strict, // Política de SameSite para mayor seguridad
            HttpOnly = true, // Previene acceso a la cookie desde JavaScript
            IsEssential = true, // Esencial para funcionamiento, incluso si el consentimiento de cookies está habilitado
            MaxAge = TimeSpan.FromSeconds(5), // Duración de la cookie
        };

        // Redirige a una URI específica
        [DoesNotReturn]
        public void RedirectTo(string? uri)
        {
            uri ??= "";

            // Previene redirecciones abiertas verificando si la URI es válida y relativa
            if (!Uri.IsWellFormedUriString(uri, UriKind.Relative))
            {
                uri = navigationManager.ToBaseRelativePath(uri);
            }

            // Maneja la navegación y asegura que se llame desde un componente de renderizado estático
            navigationManager.NavigateTo(uri);
            throw new InvalidOperationException($"{nameof(IdentityRedirectManager)} solo puede usarse durante el renderizado estático.");
        }

        // Redirige a una URI específica con parámetros de consulta
        [DoesNotReturn]
        public void RedirectTo(string uri, Dictionary<string, object?> queryParameters)
        {
            var uriWithoutQuery = navigationManager.ToAbsoluteUri(uri).GetLeftPart(UriPartial.Path); // Obtiene la URI base sin parámetros de consulta
            var newUri = navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters); // Construye la nueva URI con los parámetros
            RedirectTo(newUri);
        }

        // Redirige a una URI específica agregando un mensaje de estado mediante cookies
        [DoesNotReturn]
        public void RedirectToWithStatus(string uri, string message, HttpContext context)
        {
            context.Response.Cookies.Append(StatusCookieName, message, StatusCookieBuilder.Build(context)); // Agrega el mensaje en una cookie
            RedirectTo(uri);
        }

        // Obtiene la ruta actual sin parámetros de consulta
        private string CurrentPath => navigationManager.ToAbsoluteUri(navigationManager.Uri).GetLeftPart(UriPartial.Path);

        // Redirige a la página actual
        [DoesNotReturn]
        public void RedirectToCurrentPage() => RedirectTo(CurrentPath);

        // Redirige a la página actual con un mensaje de estado
        [DoesNotReturn]
        public void RedirectToCurrentPageWithStatus(string message, HttpContext context)
            => RedirectToWithStatus(CurrentPath, message, context);
    }
}
