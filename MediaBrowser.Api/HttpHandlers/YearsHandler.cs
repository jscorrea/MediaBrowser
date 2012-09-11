﻿using MediaBrowser.Common.Net.Handlers;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.DTO;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MediaBrowser.Api.HttpHandlers
{
    [Export(typeof(BaseHandler))]
    public class YearsHandler : BaseSerializationHandler<IBNItem[]>
    {
        public override bool HandlesRequest(HttpListenerRequest request)
        {
            return ApiService.IsApiUrlMatch("years", request);
        }
        
        protected override Task<IBNItem[]> GetObjectToSerialize()
        {
            Folder parent = ApiService.GetItemById(QueryString["id"]) as Folder;
            User user = ApiService.GetUserById(QueryString["userid"], true);

            return GetAllYears(parent, user);
        }

        /// <summary>
        /// Gets all years from all recursive children of a folder
        /// The CategoryInfo class is used to keep track of the number of times each year appears
        /// </summary>
        private async Task<IBNItem[]> GetAllYears(Folder parent, User user)
        {
            Dictionary<int, int> data = new Dictionary<int, int>();

            // Get all the allowed recursive children
            IEnumerable<BaseItem> allItems = parent.GetParentalAllowedRecursiveChildren(user);

            foreach (var item in allItems)
            {
                // Add the year from the item to the data dictionary
                // If the year already exists, increment the count
                if (item.ProductionYear == null)
                {
                    continue;
                }

                if (!data.ContainsKey(item.ProductionYear.Value))
                {
                    data.Add(item.ProductionYear.Value, 1);
                }
                else
                {
                    data[item.ProductionYear.Value]++;
                }
            }

            // Get the Year objects
            Year[] entities = await Task.WhenAll<Year>(data.Keys.Select(key => { return Kernel.Instance.ItemController.GetYear(key); })).ConfigureAwait(false);

            // Convert to an array of IBNItem
            IBNItem[] items = new IBNItem[entities.Length];

            for (int i = 0; i < entities.Length; i++)
            {
                Year e = entities[i];

                items[i] = ApiService.GetIBNItem(e, data[int.Parse(e.Name)]);
            }

            return items;
        }
    }
}
