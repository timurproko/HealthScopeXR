/**
* Code generation. Don't modify! 
**/

using Atomic.Entities;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using Atomic.Entities;
using Atomic.Elements;
using Shapes;
using Oculus.Interaction;

namespace Game
{
	public static class EntityAPI
	{
		///Tags
		public const int Player = -1615495341;


		///Values
		public const int Transform = -180157682; // Transform
		public const int TriggerReceiver = 1006843418; // TriggerEventReceiver
		public const int FinishedEvent = 875656263; // Atomic.Elements.IEvent
		public const int StartedEvent = -1073248219; // Atomic.Elements.IEvent
		public const int AudioSource = 907064781; // AudioSource


		///Tag Extensions

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasPlayerTag(this IEntity obj) => obj.HasTag(Player);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool AddPlayerTag(this IEntity obj) => obj.AddTag(Player);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool DelPlayerTag(this IEntity obj) => obj.DelTag(Player);


		///Value Extensions

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Transform GetTransform(this IEntity obj) => obj.GetValueUnsafe<Transform>(Transform);

		public static ref Transform RefTransform(this IEntity obj) => ref obj.GetValueUnsafe<Transform>(Transform);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetTransform(this IEntity obj, out Transform value) => obj.TryGetValueUnsafe(Transform, out value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddTransform(this IEntity obj, Transform value) => obj.AddValue(Transform, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasTransform(this IEntity obj) => obj.HasValue(Transform);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool DelTransform(this IEntity obj) => obj.DelValue(Transform);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetTransform(this IEntity obj, Transform value) => obj.SetValue(Transform, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TriggerEventReceiver GetTriggerReceiver(this IEntity obj) => obj.GetValueUnsafe<TriggerEventReceiver>(TriggerReceiver);

		public static ref TriggerEventReceiver RefTriggerReceiver(this IEntity obj) => ref obj.GetValueUnsafe<TriggerEventReceiver>(TriggerReceiver);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetTriggerReceiver(this IEntity obj, out TriggerEventReceiver value) => obj.TryGetValueUnsafe(TriggerReceiver, out value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddTriggerReceiver(this IEntity obj, TriggerEventReceiver value) => obj.AddValue(TriggerReceiver, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasTriggerReceiver(this IEntity obj) => obj.HasValue(TriggerReceiver);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool DelTriggerReceiver(this IEntity obj) => obj.DelValue(TriggerReceiver);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetTriggerReceiver(this IEntity obj, TriggerEventReceiver value) => obj.SetValue(TriggerReceiver, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Atomic.Elements.IEvent GetFinishedEvent(this IEntity obj) => obj.GetValueUnsafe<Atomic.Elements.IEvent>(FinishedEvent);

		public static ref Atomic.Elements.IEvent RefFinishedEvent(this IEntity obj) => ref obj.GetValueUnsafe<Atomic.Elements.IEvent>(FinishedEvent);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetFinishedEvent(this IEntity obj, out Atomic.Elements.IEvent value) => obj.TryGetValueUnsafe(FinishedEvent, out value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddFinishedEvent(this IEntity obj, Atomic.Elements.IEvent value) => obj.AddValue(FinishedEvent, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasFinishedEvent(this IEntity obj) => obj.HasValue(FinishedEvent);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool DelFinishedEvent(this IEntity obj) => obj.DelValue(FinishedEvent);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetFinishedEvent(this IEntity obj, Atomic.Elements.IEvent value) => obj.SetValue(FinishedEvent, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Atomic.Elements.IEvent GetStartedEvent(this IEntity obj) => obj.GetValueUnsafe<Atomic.Elements.IEvent>(StartedEvent);

		public static ref Atomic.Elements.IEvent RefStartedEvent(this IEntity obj) => ref obj.GetValueUnsafe<Atomic.Elements.IEvent>(StartedEvent);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetStartedEvent(this IEntity obj, out Atomic.Elements.IEvent value) => obj.TryGetValueUnsafe(StartedEvent, out value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddStartedEvent(this IEntity obj, Atomic.Elements.IEvent value) => obj.AddValue(StartedEvent, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasStartedEvent(this IEntity obj) => obj.HasValue(StartedEvent);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool DelStartedEvent(this IEntity obj) => obj.DelValue(StartedEvent);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetStartedEvent(this IEntity obj, Atomic.Elements.IEvent value) => obj.SetValue(StartedEvent, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static AudioSource GetAudioSource(this IEntity obj) => obj.GetValueUnsafe<AudioSource>(AudioSource);

		public static ref AudioSource RefAudioSource(this IEntity obj) => ref obj.GetValueUnsafe<AudioSource>(AudioSource);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetAudioSource(this IEntity obj, out AudioSource value) => obj.TryGetValueUnsafe(AudioSource, out value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddAudioSource(this IEntity obj, AudioSource value) => obj.AddValue(AudioSource, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasAudioSource(this IEntity obj) => obj.HasValue(AudioSource);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool DelAudioSource(this IEntity obj) => obj.DelValue(AudioSource);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetAudioSource(this IEntity obj, AudioSource value) => obj.SetValue(AudioSource, value);
    }
}
