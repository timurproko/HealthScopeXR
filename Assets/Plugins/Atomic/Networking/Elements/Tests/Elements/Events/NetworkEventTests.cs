using System;
using Atomic.Networking;
using Atomic.Networking.Elements;
using NUnit.Framework;

namespace Atomic.Networking.Elements
{
    internal sealed class NetworkEventTests
    {
        private MockNetworkObject _object;
        private MockNetworkFacade _facade;

        [SetUp]
        public void SetUp()
        {
            _facade = new MockNetworkFacade();
            _object = new MockNetworkObject();
            _object.Facade = _facade;
        }

        [TearDown]
        public void TearDown()
        {
            _object.Mock_Despawn();
        }
        
        [Test]
        public void Constructor_AgentIsNull_ThrowsException()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                NetworkEvent _ = new NetworkEvent(null);
            });
        }

        [Test]
        public void Invoke_BeforeSpawn_NothingHappened()
        {
            //Arrange:
            bool wasEvent1 = false;
            bool wasEvent2 = false;

            NetworkEvent networkEvent = new NetworkEvent(_object);
            networkEvent.OnEvent += () => wasEvent1 = true;
            networkEvent.Subscribe(() => wasEvent2 = true);

            //Act:
            networkEvent.Invoke();

            //Assert:
            Assert.IsFalse(wasEvent1);
            Assert.IsFalse(wasEvent2);
        }

        [Test]
        public void InvokeEvent_AfterSpawn()
        {
            //Arrange:
            bool wasEvent1 = false;
            bool wasEvent2 = false;

            NetworkEvent networkEvent = new NetworkEvent(_object);
            networkEvent.OnEvent += () => wasEvent1 = true;
            networkEvent.Subscribe(() => wasEvent2 = true);

            //Act:
            _object.Mock_Spawn();
            networkEvent.Invoke();
            _object.Render();

            //Assert:
            Assert.IsTrue(wasEvent1);
            Assert.IsTrue(wasEvent2);
        }

        [TestCase(5)]
        public void InvokeEvent_SeveralTimes_AfterSpawn(int times)
        {
            //Arrange:
            int wasEvent1 = 0;
            int wasEvent2 = 0;

            NetworkEvent networkEvent = new NetworkEvent(_object);
            networkEvent.OnEvent += () => wasEvent1++;
            networkEvent.Subscribe(() => wasEvent2++);

            //Act:
            _object.Mock_Spawn();

            for (int i = 0; i < times; i++) 
                networkEvent.Invoke();

            _object.Render();

            //Assert:
            Assert.AreEqual(times, wasEvent1);
            Assert.AreEqual(times, wasEvent2);
        }

        [Test]
        public void Subscribe_AfterSpawn_And_Invoke()
        {
            //Arrange:
            bool wasEvent1 = false;
            bool wasEvent2 = false;

            NetworkEvent networkEvent = new NetworkEvent(_object);

            //Act:
            _object.Mock_Spawn();

            networkEvent.OnEvent += () => wasEvent1 = true;
            networkEvent.Subscribe(() => wasEvent2 = true);

            networkEvent.Invoke();
            _object.Render();

            //Assert:
            Assert.IsTrue(wasEvent1);
            Assert.IsTrue(wasEvent2);
        }

        [Test]
        public void Unsubscribe()
        {
            bool wasEvent1 = false;
            bool wasEvent2 = false;

            void OnEventRaisen1() => wasEvent1 = true;
            void OnEventRaisen2() => wasEvent2 = true;

            //Arrange:
            NetworkEvent networkEvent = new NetworkEvent(_object);
            _object.Mock_Spawn();

            networkEvent.OnEvent += OnEventRaisen1;
            networkEvent.Subscribe(OnEventRaisen2);

            networkEvent.Invoke();

            _object.Render();

            //Pre-assert:
            Assert.IsTrue(wasEvent1);
            Assert.IsTrue(wasEvent2);

            //Act:
            wasEvent1 = false;
            wasEvent2 = false;

            networkEvent.OnEvent -= OnEventRaisen1;
            networkEvent.Unsubscribe(OnEventRaisen2);
            networkEvent.Invoke();

            //Assert:
            Assert.IsFalse(wasEvent1);
            Assert.IsFalse(wasEvent2);
        }


        [Test]
        public void Dispose()
        {
            bool wasEvent1 = false;
            bool wasEvent2 = false;

            void OnEventRaisen1() => wasEvent1 = true;
            void OnEventRaisen2() => wasEvent2 = true;

            //Arrange:
            NetworkEvent networkEvent = new NetworkEvent(_object);
            _object.Mock_Spawn();

            networkEvent.OnEvent += OnEventRaisen1;
            networkEvent.Subscribe(OnEventRaisen2);

            networkEvent.Invoke();

            _object.Render();

            //Pre-assert:
            Assert.IsTrue(wasEvent1);
            Assert.IsTrue(wasEvent2);

            //Act:
            wasEvent1 = false;
            wasEvent2 = false;

            networkEvent.Dispose();
            networkEvent.Invoke();

            //Assert:
            Assert.IsFalse(wasEvent1);
            Assert.IsFalse(wasEvent2);
        }
    }
}