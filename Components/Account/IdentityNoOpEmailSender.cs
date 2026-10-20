using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using ProductoSaul.Data;

namespace ProductoSaul.Components.Account
{
    /*
             * Clase para manejar el envío de correos electrónicos relacionados con la autenticación.
             * Esta implementación es una versión "no operativa" (NoOp), útil para entornos de prueba
             * donde no se ha configurado un servicio real de envío de correos electrónicos.
             * Nota: Al configurar un servicio real, elimina el bloque "else if (EmailSender is IdentityNoOpEmailSender)"
             * de RegisterConfirmation.razor.
             */
    internal sealed class IdentityNoOpEmailSender : IEmailSender<ApplicationUser>
    {
        private readonly NoOpEmailSender emailSender = new();

        // Envía un enlace de confirmación de correo electrónico al usuario
        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
            emailSender.SendEmailAsync(
                email,
                "Confirma tu correo electrónico",
                $"Por favor, confirma tu cuenta haciendo <a href='{confirmationLink}'>clic aquí</a>.");

        // Envía un enlace para restablecer la contraseña
        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
            emailSender.SendEmailAsync(
                email,
                "Restablece tu contraseña",
                $"Por favor, restablece tu contraseña haciendo <a href='{resetLink}'>clic aquí</a>.");

        // Envía un código para restablecer la contraseña
        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
            emailSender.SendEmailAsync(
                email,  
                "Restablece tu contraseña",
                $"Por favor, restablece tu contraseña usando el siguiente código: {resetCode}");
    }
}
