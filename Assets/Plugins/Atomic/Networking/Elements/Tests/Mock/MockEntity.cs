using System.Collections.Generic;

namespace Atomic.Networking.Elements
{
    internal sealed class MockEntity
    {
        private static readonly Dictionary<int, MockEntity> s_objects = new();
        private static int ID_GEN;

        public readonly int id;

        public MockEntity()
        {
            this.id = ++ID_GEN;
            s_objects.Add(this.id, this);
        }

        public static bool TryGet(int id, out MockEntity entity)
        {
            return s_objects.TryGetValue(id, out entity);
        }
    }
}