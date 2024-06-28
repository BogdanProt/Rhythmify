using System;
using SpotifyAPI.Web;
namespace Rhythmify.Services
{
    public class SpotifyService
    {
        private readonly SpotifyClient _spotifyClient;
        public SpotifyService(SpotifyClientConfig spotifyClientConfig)
        {
            var request = new ClientCredentialsRequest("CLIENT_ID", "CLIENT_SECRET");
            var response = new OAuthClient().RequestToken(request).Result;
            _spotifyClient = new SpotifyClient(response.AccessToken);
        }

        public async Task<SearchResponse> SearchTracksAsync(string query)
        {
            var searchRequest = new SearchRequest(SearchRequest.Types.Track, query);
            var searchResponse = await _spotifyClient.Search.Item(searchRequest);
            return searchResponse;
        }
    }
}
