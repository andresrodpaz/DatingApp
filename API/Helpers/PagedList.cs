using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
    /// <summary>
    /// Represents a paged list of items.
    /// </summary>
    /// <typeparam name="T">The type of the items.</typeparam>
    public class PagedList<T> : List<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}"/> class.
        /// </summary>
        /// <param name="items">The items in the current page.</param>
        /// <param name="count">The total count of items.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The size of the page.</param>
        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            // Set the current page number.
            CurrentPage = pageNumber;
            
            // Calculate the total number of pages.
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            
            // Set the page size.
            PageSize = pageSize;
            
            // Set the total count of items.
            TotalCount = count;
            
            // Add the items to the current page.
            AddRange(items);
        }

        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the total number of pages.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the total count of items.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Asynchronously creates a new instance of the <see cref="PagedList{T}"/> class.
        /// </summary>
        /// <param name="source">The source queryable collection.</param>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the paged list.</returns>
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            // Get the total count of items asynchronously.
            var count = await source.CountAsync();

            // Skip the items of previous pages and take the items for the current page asynchronously.
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            // Return a new instance of PagedList with the items, total count, page number, and page size.
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
