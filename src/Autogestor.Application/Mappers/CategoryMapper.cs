using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Categories;
using Autogestor.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Autogestor.Application.Mappers;

[Mapper]
public static partial class CategoryMapper
{
    public static partial CategoryResponse ToResponse(Category category);

    public static partial IReadOnlyList<CategoryResponse> ToResponseList(IReadOnlyList<Category> categories);

    public static PagedResponse<CategoryResponse> ToPagedResponse(
        IReadOnlyList<Category>? categories,
        bool hasNextPage)
    {
        IReadOnlyList<CategoryResponse> items = ToResponseList(categories: categories ?? []);

        return new PagedResponse<CategoryResponse>
        {
            Data = items,
            HasNextPage = hasNextPage,
            NextCursor = hasNextPage && items.Count > 0 ? items[^1].Id : null
        };
    }
}
