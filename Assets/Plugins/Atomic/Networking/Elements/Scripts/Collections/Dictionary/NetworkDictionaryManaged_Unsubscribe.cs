using System;
using Atomic.Elements;

namespace Atomic.Networking.Elements
{
    public partial class NetworkDictionaryManaged<K, V, KRaw, VRaw>
    {
        public void UnsubscribeAll()
        {
            if (this.OnItemAdded != null)
            {
                Delegate[] delegates = this.OnItemAdded.GetInvocationList();
                for (int i = 0, count = delegates.Length; i < count; i++)
                    this.OnItemAdded -= (AddItemHandler<K, V>) delegates[i];

                this.OnItemAdded = null;
            }

            if (this.OnItemRemoved != null)
            {
                Delegate[] delegates = this.OnItemRemoved.GetInvocationList();
                for (int i = 0, count = delegates.Length; i < count; i++)
                    this.OnItemRemoved -= (RemoveItemHandler<K, V>) delegates[i];

                this.OnItemRemoved = null;
            }

            if (this.OnItemChanged != null)
            {
                Delegate[] delegates = this.OnItemChanged.GetInvocationList();
                for (int i = 0, count = delegates.Length; i < count; i++)
                    this.OnItemChanged -= (SetItemHandler<K, V>) delegates[i];

                this.OnItemChanged = null;
            }

            if (this.OnStateChanged != null)
            {
                Delegate[] delegates = this.OnStateChanged.GetInvocationList();
                for (int i = 0, count = delegates.Length; i < count; i++)
                    this.OnStateChanged -= (StateChangedHandler) delegates[i];

                this.OnStateChanged = null;
            }
        }
    }
}