using Elastic.Clients.Elasticsearch;
using UserPermissions.Domain.Entities;

namespace UserPermissions.Infrastructure.Elasticsearch
{
    public class ElasticsearchService : IElasticsearchService
    {
        private readonly ElasticsearchClient _client;

        public ElasticsearchService(string uri)
        {
            var settings = new ElasticsearchClientSettings(new Uri(uri))
                .DefaultIndex("permissions");
            _client = new ElasticsearchClient(settings);
        }

        public async Task IndexPermissionAsync(Permission permission)
        {
            var response = await _client.IndexAsync(permission);
            if (!response.IsValidResponse)
            {
                Console.WriteLine($"Failed to index permission: {response.ElasticsearchServerError}");
            }
        }
    }
}