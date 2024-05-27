using System;
using SpotifyAPI.Web;
namespace Rhythmify.Services
{
	public class SpotifyService
	{
		private readonly SpotifyClient _spotifyClient;
		public SpotifyService(SpotifyClientConfig spotifyClientConfig)
		{
			var request = new ClientCredentialsRequest("d2d70f2bfbfb4f2594339fc8916b16a1", "3894a81b65754b039d84817bb52ea144");
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

