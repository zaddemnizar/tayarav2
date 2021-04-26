using System.Collections.Generic;

namespace tayarav2
{
    public class Metadata
    {
        public string key { get; set; }
        public string value { get; set; }
        public int? numericValue { get; set; }
    }

    public class Input
    {
        public string title { get; set; }
        public string description { get; set; }
        public string price { get; set; }
        public string currency { get; set; } = "TND";
        public List<string> images { get; set; }
        public string sector { get; set; }
        public string category { get; set; }
        public string subdivisionId { get; set; }
        public List<Metadata> metadata { get; set; }
    }
    public class Variables
    {
        public Input input { get; set; }
    }

    public class AnnonceImmobilier
    {
        public string operationName { get; set; }
        public string query { get; set; } = "mutation PostListing($input: ClassifiedAdInput!) {\n  postListing(input: $input) {\n    uuid\n    title\n    categories {\n      id\n      name\n      engName\n      __typename\n    }\n    __typename\n  }\n}\n";
        public Variables variables { get; set; }
    }


}
