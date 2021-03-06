﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BonVoyage.Models;
using Newtonsoft.Json.Linq;

namespace BonVoyage.Clients
{
    public class VenuesClient : BaseClient
    {
        public VenuesClient(HttpClient httpClient) : base(httpClient)
        {
        }

        /// <param name="placeName">
        /// The name of a place in the world (i.e. San Fransisco, CA). Used to pass as the value for 'near' parameter.
        /// </param>
        /// <param name="categoryId">
        /// The id of the category to limit results to. If specifying a top-level category, all sub-categories will also match the query.
        /// </param>
        /// <seealso href="https://developer.foursquare.com/docs/venues/search" />
        public Task<IReadOnlyCollection<CompactVenue>> Search(string placeName, string categoryId)
        {
            return Search(placeName, categoryId, 50);
        }

        /// <param name="placeName">
        /// The name of a place in the world (i.e. San Fransisco, CA). Used to pass as the value for 'near' parameter.
        /// </param>
        /// <param name="categoryId">
        /// The id of the category to limit results to. If specifying a top-level category, all sub-categories will also match the query.
        /// </param>
        /// <param name="limit">The number of search results. Min: 1, Max: 50</param>
        /// <seealso href="https://developer.foursquare.com/docs/venues/search" />
        public async Task<IReadOnlyCollection<CompactVenue>> Search(string placeName, string categoryId, int limit)
        {
            if (placeName == null) throw new ArgumentNullException(nameof(placeName));
            if (categoryId == null) throw new ArgumentNullException(nameof(categoryId));
            if (limit < 1) throw new ArgumentOutOfRangeException(nameof(limit), limit, "Cannot be lower than 1");
            if (limit > 50) throw new ArgumentOutOfRangeException(nameof(limit), limit, "Cannot be greater than 50");
            
            using (var response = await HttpClient.GetAsync($"v2/venues/search?near={placeName}&categoryId={categoryId}&limit={limit.ToString(CultureInfo.InvariantCulture)}").ConfigureAwait(false))
            {
                var resultAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var jObject = DeserializeObject<JObject>(resultAsString);
                var venues = DeserializeObject<IEnumerable<CompactVenue>>(jObject["response"]["venues"].ToString());

                return new ReadOnlyCollection<CompactVenue>(venues.ToList());
            }
        }
    }
}