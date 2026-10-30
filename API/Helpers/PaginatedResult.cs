using System;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers;

public class PaginatedResult<T>
{
    public PaginationMetaData MetaData { get; set; } = default!; // use default for the type, in this case a class type which is null, add the '!' to ignore the null warning
    public List<T> Items { get; set; } = [];

};

public class PaginationMetaData
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
};

public class PaginationHelper
{
    public static async Task<PaginatedResult<T>> CreateAsync<T>(IQueryable<T> query, int pageNumber, int pageSize)
    {
        var count = await query.CountAsync(); // get items in query before pagination to know total number of items avaible before paginating
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize) // deferred query 
                .ToListAsync(); // query is made to the database 
        return new PaginatedResult<T>
        {
            MetaData = new PaginationMetaData
            {
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                PageSize = pageSize,
                TotalCount = count
            },
            Items = items
        };
    }
}

