using Microsoft.AspNetCore.Identity;
using ProductoSaul.Data;

namespace ProductoSaul.Components.Account
{
    /*
         * Clase auxiliar para acceder al usuario autenticado en el contexto actual.
         * Si el usuario no está disponible o no puede cargarse, redirige a una página de error.
         */
    internal sealed class IdentityUserAccessor(UserManager<ApplicationUser> userManager, IdentityRedirectManager redirectManager)
    {
        private readonly UserManager<ApplicationUser> userManager = userManager; // Maneja las operaciones relacionadas con los usuarios
        private readonly IdentityRedirectManager redirectManager = redirectManager; // Gestiona las redirecciones en caso de errores

        /*
         * Obtiene el usuario autenticado asociado al contexto actual.
         * Si no puede cargar al usuario, redirige a una página de error con un mensaje apropiado.
         */
        public async Task<ApplicationUser> GetRequiredUserAsync(HttpContext context)
        {
            var user = await userManager.GetUserAsync(context.User); // Obtiene el usuario autenticado

            if (user is null)
            {
                // Redirige a la página de error si no se encuentra al usuario
                redirectManager.RedirectToWithStatus(
                    "Account/InvalidUser",
                    $"Error: No se pudo cargar el usuario con ID '{userManager.GetUserId(context.User)}'.",
                    context);
            }

            return user;
        }
    }
}
