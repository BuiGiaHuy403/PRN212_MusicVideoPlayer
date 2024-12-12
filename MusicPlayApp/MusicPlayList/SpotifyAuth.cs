using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MusicPlayList
{
    public class SpotifyAuth
    {
        private const string clientId = "644e888f1048471ca3933fa2fd46e75d";
        private const string clientSecret = "1b573d4dc72b4fb1aa5704d1b9bd0882";
        private const string redirectUri = "http://localhost:5000/callback";

        public async Task<string> GetAccessTokenAsync()
        {
            var server = new EmbedIOAuthServer(new Uri(redirectUri), 5000);
            await server.Start();

            var tcs = new TaskCompletionSource<string>();
            server.AuthorizationCodeReceived += async (sender, response) =>
            {
                try
                {
                    // Stop the server after receiving the code
                    await server.Stop();

                    // Request access token using the code
                    var tokenResponse = await new OAuthClient().RequestToken(
                        new AuthorizationCodeTokenRequest(clientId, clientSecret, response.Code, new Uri(redirectUri))
                    );

                    // Return the access token
                    tcs.SetResult(tokenResponse.AccessToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during token exchange: {ex.Message}");
                    tcs.SetException(ex);
                }
            };

            var request = new LoginRequest(server.BaseUri, clientId, LoginRequest.ResponseType.Code)
            {
                // Include additional scopes for accessing current playback
                Scope = new[] {
                    Scopes.Streaming,
                    Scopes.UserModifyPlaybackState,
                    Scopes.UserReadCurrentlyPlaying,
                    Scopes.UserReadPlaybackState
                }
            };

            // Open the authorization page
            Process.Start(new ProcessStartInfo(request.ToUri().ToString()) { UseShellExecute = true });

            // Wait for the task to complete and return the access token
            return await tcs.Task;
        }
    }
}
