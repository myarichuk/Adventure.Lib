using System.Collections.Generic;
// ReSharper disable ClassNeverInstantiated.Global

namespace Adventure.EntityUtils
{
    public class EntityTemplate
    {
        public string Name { get; set; } //human readable name is always good :)

        public string InheritsFrom { get; set; }

        public Dictionary<string, Dictionary<string, object>> Components { get; set; }

        public string Parent { get; set; }
        public List<string> Children { get; set; }
    }
}
