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
		public const int IsDebug = -1441853691; // bool
		public const int IsCaptured = -1670798162; // bool
		public const int IsAIWorking = 999067129; // bool
		public const int Text = -1682372359; // Text
		public const int FinishedEvent = 875656263; // Atomic.Elements.IEvent
		public const int StartedEvent = -1073248219; // Atomic.Elements.IEvent
		public const int Description = -344403983; // Text
		public const int Counter = 245345764; // Disc
		public const int ButtonCapture = 1082207470; // InteractableUnityEventWrapper
		public const int ButtonFilter = 1644026029; // InteractableUnityEventWrapper
		public const int ButtonAI = -347772254; // InteractableUnityEventWrapper
		public const int AudioSource = 907064781; // AudioSource
		public const int Snapshot = -717523101; // Texture2D


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
		public static bool GetIsDebug(this IEntity obj) => obj.GetValueUnsafe<bool>(IsDebug);

		public static ref bool RefIsDebug(this IEntity obj) => ref obj.GetValueUnsafe<bool>(IsDebug);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetIsDebug(this IEntity obj, out bool value) => obj.TryGetValueUnsafe(IsDebug, out value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddIsDebug(this IEntity obj, bool value) => obj.AddValue(IsDebug, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasIsDebug(this IEntity obj) => obj.HasValue(IsDebug);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool DelIsDebug(this IEntity obj) => obj.DelValue(IsDebug);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetIsDebug(this IEntity obj, bool value) => obj.SetValue(IsDebug, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool GetIsCaptured(this IEntity obj) => obj.GetValueUnsafe<bool>(IsCaptured);

		public static ref bool RefIsCaptured(this IEntity obj) => ref obj.GetValueUnsafe<bool>(IsCaptured);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetIsCaptured(this IEntity obj, out bool value) => obj.TryGetValueUnsafe(IsCaptured, out value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddIsCaptured(this IEntity obj, bool value) => obj.AddValue(IsCaptured, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasIsCaptured(this IEntity obj) => obj.HasValue(IsCaptured);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool DelIsCaptured(this IEntity obj) => obj.DelValue(IsCaptured);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetIsCaptured(this IEntity obj, bool value) => obj.SetValue(IsCaptured, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool GetIsAIWorking(this IEntity obj) => obj.GetValueUnsafe<bool>(IsAIWorking);

		public static ref bool RefIsAIWorking(this IEntity obj) => ref obj.GetValueUnsafe<bool>(IsAIWorking);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetIsAIWorking(this IEntity obj, out bool value) => obj.TryGetValueUnsafe(IsAIWorking, out value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddIsAIWorking(this IEntity obj, bool value) => obj.AddValue(IsAIWorking, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasIsAIWorking(this IEntity obj) => obj.HasValue(IsAIWorking);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool DelIsAIWorking(this IEntity obj) => obj.DelValue(IsAIWorking);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetIsAIWorking(this IEntity obj, bool value) => obj.SetValue(IsAIWorking, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Text GetText(this IEntity obj) => obj.GetValueUnsafe<Text>(Text);

		public static ref Text RefText(this IEntity obj) => ref obj.GetValueUnsafe<Text>(Text);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetText(this IEntity obj, out Text value) => obj.TryGetValueUnsafe(Text, out value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddText(this IEntity obj, Text value) => obj.AddValue(Text, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasText(this IEntity obj) => obj.HasValue(Text);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool DelText(this IEntity obj) => obj.DelValue(Text);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetText(this IEntity obj, Text value) => obj.SetValue(Text, value);

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
		public static Text GetDescription(this IEntity obj) => obj.GetValueUnsafe<Text>(Description);

		public static ref Text RefDescription(this IEntity obj) => ref obj.GetValueUnsafe<Text>(Description);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetDescription(this IEntity obj, out Text value) => obj.TryGetValueUnsafe(Description, out value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddDescription(this IEntity obj, Text value) => obj.AddValue(Description, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasDescription(this IEntity obj) => obj.HasValue(Description);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool DelDescription(this IEntity obj) => obj.DelValue(Description);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetDescription(this IEntity obj, Text value) => obj.SetValue(Description, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Disc GetCounter(this IEntity obj) => obj.GetValueUnsafe<Disc>(Counter);

		public static ref Disc RefCounter(this IEntity obj) => ref obj.GetValueUnsafe<Disc>(Counter);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetCounter(this IEntity obj, out Disc value) => obj.TryGetValueUnsafe(Counter, out value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddCounter(this IEntity obj, Disc value) => obj.AddValue(Counter, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasCounter(this IEntity obj) => obj.HasValue(Counter);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool DelCounter(this IEntity obj) => obj.DelValue(Counter);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetCounter(this IEntity obj, Disc value) => obj.SetValue(Counter, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static InteractableUnityEventWrapper GetButtonCapture(this IEntity obj) => obj.GetValueUnsafe<InteractableUnityEventWrapper>(ButtonCapture);

		public static ref InteractableUnityEventWrapper RefButtonCapture(this IEntity obj) => ref obj.GetValueUnsafe<InteractableUnityEventWrapper>(ButtonCapture);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetButtonCapture(this IEntity obj, out InteractableUnityEventWrapper value) => obj.TryGetValueUnsafe(ButtonCapture, out value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddButtonCapture(this IEntity obj, InteractableUnityEventWrapper value) => obj.AddValue(ButtonCapture, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasButtonCapture(this IEntity obj) => obj.HasValue(ButtonCapture);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool DelButtonCapture(this IEntity obj) => obj.DelValue(ButtonCapture);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetButtonCapture(this IEntity obj, InteractableUnityEventWrapper value) => obj.SetValue(ButtonCapture, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static InteractableUnityEventWrapper GetButtonFilter(this IEntity obj) => obj.GetValueUnsafe<InteractableUnityEventWrapper>(ButtonFilter);

		public static ref InteractableUnityEventWrapper RefButtonFilter(this IEntity obj) => ref obj.GetValueUnsafe<InteractableUnityEventWrapper>(ButtonFilter);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetButtonFilter(this IEntity obj, out InteractableUnityEventWrapper value) => obj.TryGetValueUnsafe(ButtonFilter, out value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddButtonFilter(this IEntity obj, InteractableUnityEventWrapper value) => obj.AddValue(ButtonFilter, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasButtonFilter(this IEntity obj) => obj.HasValue(ButtonFilter);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool DelButtonFilter(this IEntity obj) => obj.DelValue(ButtonFilter);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetButtonFilter(this IEntity obj, InteractableUnityEventWrapper value) => obj.SetValue(ButtonFilter, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static InteractableUnityEventWrapper GetButtonAI(this IEntity obj) => obj.GetValueUnsafe<InteractableUnityEventWrapper>(ButtonAI);

		public static ref InteractableUnityEventWrapper RefButtonAI(this IEntity obj) => ref obj.GetValueUnsafe<InteractableUnityEventWrapper>(ButtonAI);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetButtonAI(this IEntity obj, out InteractableUnityEventWrapper value) => obj.TryGetValueUnsafe(ButtonAI, out value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddButtonAI(this IEntity obj, InteractableUnityEventWrapper value) => obj.AddValue(ButtonAI, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasButtonAI(this IEntity obj) => obj.HasValue(ButtonAI);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool DelButtonAI(this IEntity obj) => obj.DelValue(ButtonAI);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetButtonAI(this IEntity obj, InteractableUnityEventWrapper value) => obj.SetValue(ButtonAI, value);

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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Texture2D GetSnapshot(this IEntity obj) => obj.GetValueUnsafe<Texture2D>(Snapshot);

		public static ref Texture2D RefSnapshot(this IEntity obj) => ref obj.GetValueUnsafe<Texture2D>(Snapshot);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetSnapshot(this IEntity obj, out Texture2D value) => obj.TryGetValueUnsafe(Snapshot, out value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddSnapshot(this IEntity obj, Texture2D value) => obj.AddValue(Snapshot, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasSnapshot(this IEntity obj) => obj.HasValue(Snapshot);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool DelSnapshot(this IEntity obj) => obj.DelValue(Snapshot);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetSnapshot(this IEntity obj, Texture2D value) => obj.SetValue(Snapshot, value);
    }
}
