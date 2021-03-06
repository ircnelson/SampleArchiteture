using System;
using System.Linq;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SampleArchitecture.Application.Infrastructure.Pagination;

namespace SampleArchitecture.WebApi.Infrastructure
{
    [ApiController]
    public abstract class BaseController : Controller
    {
        private const string PageLink = "page";

        protected IMediator Dispatcher => HttpContext.RequestServices.GetService<IMediator>();
        
        protected IActionResult Single<T>(T model, Func<T,bool> criteria = null)
        {
            if (model == null)
            {
                return NotFound();
            }
            var isValid = criteria == null || criteria(model);
            if (isValid)
            {
                return Ok(model);
            }

            return NotFound();
        }
        
        protected IActionResult Collection<T>(IPagedResult<T> pagedResult, Func<IPagedResult<T>,bool> criteria = null)
        {
            if (pagedResult == null)
            {
                return NotFound();
            }
            var isValid = criteria == null || criteria(pagedResult);
            if (!isValid)
            {
                return NotFound();
            }
            if (pagedResult.IsEmpty)
            {
                return Ok(Enumerable.Empty<T>());
            }
            Response.Headers.Add("link", GetLinkHeader(pagedResult));
            Response.Headers.Add("x-total-count", pagedResult.TotalResults.ToString());

            return Ok(pagedResult.Items);
        }

        private string GetLinkHeader(IPagedResult result)
        {
            var first = GetPageLink(result.CurrentPage, 1);
            var last = GetPageLink(result.CurrentPage, result.TotalPages);
            var prev = string.Empty;
            var next = string.Empty;
            if (result.CurrentPage > 1 && result.CurrentPage <= result.TotalPages)
            {
                prev = GetPageLink(result.CurrentPage, result.CurrentPage - 1);
            }
            if (result.CurrentPage < result.TotalPages)
            {
                next = GetPageLink(result.CurrentPage, result.CurrentPage + 1);
            }

            return $"{FormatLink(next, "next")}{FormatLink(last, "last")}" +
                   $"{FormatLink(first, "first")}{FormatLink(prev, "prev")}";
        }

        private string GetPageLink(int currentPage, int page)
        {
            var path = Request.Path.HasValue ? Request.Path.ToString() : string.Empty;
            var queryString = Request.QueryString.HasValue ?  Request.QueryString.ToString() : string.Empty;
            var conjunction = string.IsNullOrWhiteSpace(queryString) ? "?" : "&";
            var fullPath = $"{path}{queryString}";
            var pageArg = $"{PageLink}={page}";
            var link = fullPath.Contains($"{PageLink}=")
                ? fullPath.Replace($"{PageLink}={currentPage}", pageArg)
                : fullPath += $"{conjunction}{pageArg}";

            return link;
        }

        private static string FormatLink(string path, string rel)
            => string.IsNullOrWhiteSpace(path) ? string.Empty : $"<{path}>; rel=\"{rel}\",";
    }
}