namespace GraphQLDemo.Schema.Queries
{
    [UnionType("SearchResult")]
    public interface ISearchResultType
    {
        Guid Id { get; }
    }
}
