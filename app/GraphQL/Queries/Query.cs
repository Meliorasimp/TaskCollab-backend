namespace app.GraphQL.Queries
{
    public class Query
    {
        // This is the root query type
        // This may look useless, but it's necessary for HotChocolate to recognize query extensions
        // Do not remove if you value your sanity
        public string Hello() => "Hello, World!";
    }
}
