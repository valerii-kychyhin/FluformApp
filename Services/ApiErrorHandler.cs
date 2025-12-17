using System.Text.Json;
using Microsoft.Maui.Controls;
using Refit;
using System.Linq;
using System.Collections.Generic;

public static class ApiErrorHandler
{
    public static async Task HandleApiExceptionAsync(ApiException ex)
    {
        // 422 — Validation errors
        if (ex.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
        {
            try
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(ex.Content);

                if (error?.errors != null)
                {
                    var details = string.Join("\n", error.errors.SelectMany(e => e.Value));
                    await Application.Current.MainPage.DisplayAlert("Validation Error", details, "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Validation Error", error?.message ?? "Validation failed", "OK");
                }
            }
            catch
            {
                await Application.Current.MainPage.DisplayAlert("Validation Error", "An error occurred while processing validation.", "OK");
            }
            return;
        }

        // 401 — Unauthorized
        if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Unauthorized",
                "Your session has expired or you are not authorized.",
                "OK"
            );
            return;
        }

        // 500 — Internal Server Error
        if (ex.StatusCode == System.Net.HttpStatusCode.InternalServerError)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Server Error",
                "An internal server error occurred. Please try again later.",
                "OK"
            );
            return;
        }

        // Other errors
        await Application.Current.MainPage.DisplayAlert("Error", ex.Content ?? ex.Message, "OK");
    }
}
