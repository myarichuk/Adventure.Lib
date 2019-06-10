using System.Collections.Generic;
using Entitas;

namespace Adventure.EntityUtils
{
    public class HierarchyComponent : IComponent
    {
        public Entity Parent { get; set; }
        public List<Entity> Children { get; set; }
    }
}
