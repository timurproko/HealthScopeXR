namespace Atomic.Networking.Elements
{
    internal sealed class MockEntitySerializer : INetworkSerializer<MockEntity, int>
    {
        public int Serialize(in MockEntity value)
        {
            return value?.id ?? 0;
        }

        public MockEntity Deserialize(in int raw)
        {
            MockEntity.TryGet(raw, out MockEntity entity);
            return entity;
        }
    }
}