using System;
using Atomic.Elements;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Atomic.Networking.Elements
{
    public sealed class LocalTimestamp : ITimestamp
    {
#if ODIN_INSPECTOR
        [ShowInInspector, HideInEditorMode]
#endif
        public int EndTick => _endTick;

#if ODIN_INSPECTOR
        [ShowInInspector, HideInEditorMode]
#endif
        public int RemainingTicks => this.GetRemainingTicks();

#if ODIN_INSPECTOR
        [ShowInInspector, HideInEditorMode]
#endif
        public float RemainingTime => this.GetRemainingTime();

        private readonly INetworkFacade _facade;
        private int _endTick;

        public LocalTimestamp(INetworkFacade facade)
        {
            _facade = facade;
        }

#if ODIN_INSPECTOR
        [Button]
#endif
        public void StartFromSeconds(float seconds)
        {
            if (seconds < 0)
                throw new ArgumentOutOfRangeException(nameof(seconds));

            if (_facade is not {IsActive: true})
                return;

            _endTick = _facade.Tick + (int) Math.Ceiling((double) seconds / _facade.DeltaTime);
        }

#if ODIN_INSPECTOR
        [Button]
#endif
        public void StartFromTicks(int ticks)
        {
            if (ticks < 0)
                throw new ArgumentOutOfRangeException(nameof(ticks));

            if (_facade is not {IsActive: true})
                return;

            _endTick = _facade.Tick + ticks;
        }

#if ODIN_INSPECTOR
        [Button]
#endif
        public void Stop() => _endTick = -1;

        public float GetProgress(float duration) =>
            _facade.IsActive ? 1 - this.GetRemainingTime() / duration : -1;

        public bool IsIdle() => !_facade.IsActive || _endTick == -1;

        public bool IsPlaying() => _facade is {IsActive: true} && _endTick > 0 && _facade.Tick < _endTick;

        public bool IsExpired() => _facade is not {IsActive: true} || _endTick > 0 && _endTick <= _facade.Tick;

        private int GetRemainingTicks() => _facade is not {IsActive: true}
            ? -1
            : _endTick > 0
                ? Math.Max(0, _endTick - _facade.Tick)
                : 0;

        private float GetRemainingTime()
        {
            int ticks = this.GetRemainingTicks();
            return ticks != -1 ? ticks * _facade.DeltaTime : default;
        }
    }
}