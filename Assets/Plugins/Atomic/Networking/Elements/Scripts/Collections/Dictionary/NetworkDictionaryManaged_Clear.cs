// ReSharper disable RedundantAssignment
namespace Atomic.Networking.Elements
{
    public partial class NetworkDictionaryManaged<K, V, KRaw, VRaw>
    {
        public void Clear()
        {
            if (!m_agent.IsActive)
            {
                this.m_localDictionary.Clear();
                return;
            }

            ref int count = ref this.entry_count();
            if (count == 0)
                return;

            this.ClearInternal(ref count);
        }

        private void ClearInternal(ref int count)
        {
            count = 0;
            this.FreeBuckets();
            this.FreeEntries();
            this.free_list() = UNDEFINED;
            this.version()++;
        }
    }
}