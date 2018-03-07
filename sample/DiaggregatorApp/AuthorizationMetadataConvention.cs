using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DiaggregatorApp
{
    internal class AuthorizationMetadataConvention : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            foreach (var filter in action.Filters.OfType<AuthorizeFilter>().ToArray())
            {
                action.Filters.Remove(filter);
            }

            foreach (var metadata in action.Attributes.OfType<IAuthorizeData>())
            {
                action.Filters.Add(new FilterAuthorizeData(metadata));
            }
        }

        private class FilterAuthorizeData : IFilterMetadata, IAuthorizeData
        {
            private readonly IAuthorizeData _data;

            public FilterAuthorizeData(IAuthorizeData data)
            {
                _data = data;
            }

            public string Policy { get => _data.Policy; set => _data.Policy = value; }
            public string Roles { get => _data.Roles; set => _data.Roles = value; }
            public string AuthenticationSchemes { get => _data.AuthenticationSchemes; set => _data.AuthenticationSchemes = value; }
        }
    }
}