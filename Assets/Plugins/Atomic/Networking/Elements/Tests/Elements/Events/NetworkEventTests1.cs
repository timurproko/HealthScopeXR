using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Atomic.Networking.Elements
{
    internal sealed class NetworkEventTests1
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
                NetworkEvent<int> _ = new NetworkEvent<int>(null);
            });
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Constructor_InvalidQueueCapacity_ThrowsException(int capacity)
        {
            Assert.Catch<ArgumentException>(() =>
            {
                NetworkEvent<int> _ = new NetworkEvent<int>(_object, capacity);
            });
        }
        
        [Test]
        public void Invoke_BeforeSpawn_NothingHappened()
        {
            //Arrange:
            int wasEvent1 = -1;
            int wasEvent2 = -1;
        
            NetworkEvent<int> networkEvent = new NetworkEvent<int>(_object);
            networkEvent.OnEvent += v => wasEvent1 = v;
            networkEvent.Subscribe(v => wasEvent2 = v);
        
            //Act:
            networkEvent.Invoke(5);
        
            //Assert:
            Assert.AreNotEqual(5, wasEvent1);
            Assert.AreNotEqual(5, wasEvent2);
            Assert.AreEqual(-1, wasEvent1);
            Assert.AreEqual(-1, wasEvent2);
        }
       
        [Test]
        public void InvokeEvent()
        {
            //Arrange:
            int wasEvent1 = -1;
            int wasEvent2 = -1;
        
            NetworkEvent<int> networkEvent = new NetworkEvent<int>(_object);
            networkEvent.OnEvent += v => wasEvent1 = v;
            networkEvent.Subscribe(v => wasEvent2 = v);
        
            //Act:
            _object.Mock_Spawn();
            networkEvent.Invoke(5);
            _object.Render();

            //Assert:
            Assert.AreEqual(5, wasEvent1);
            Assert.AreEqual(5, wasEvent2);
        }

        [Test]
        public void InvokeEvent_SeveralTimes()
        {
            //Arrange:
            List<int> wasEvent1 = new List<int>();
            List<int> wasEvent2 = new List<int>();
        
            NetworkEvent<int> networkEvent = new NetworkEvent<int>(_object);
            networkEvent.OnEvent += v => wasEvent1.Add(v);
            networkEvent.Subscribe(v => wasEvent2.Add(v));
        
            //Act:
            _object.Mock_Spawn();

            int[] queue = {5, 8, -2, 0};

            foreach (int arg in queue) 
                networkEvent.Invoke(arg);

            _object.Render();

            //Assert:
            for (int i = 0; i < queue.Length; i++)
            {
                Assert.AreEqual(queue[i], wasEvent1[i]);
                Assert.AreEqual(queue[i], wasEvent2[i]);
            }
        }
        
        [TestCase(4)]
        public void InvokeEvent_MoreTimes_ThanQueueCapacity(int capacity)
        {
            //Arrange:
            List<int> wasEvent1 = new List<int>();
            List<int> wasEvent2 = new List<int>();
        
            NetworkEvent<int> networkEvent = new NetworkEvent<int>(_object, capacity);
            networkEvent.OnEvent += v => wasEvent1.Add(v);
            networkEvent.Subscribe(v => wasEvent2.Add(v));
        
            //Act:
            _object.Mock_Spawn();

            int[] queue = {5, 8, -2, 0, -7, 9};

            foreach (int arg in queue) 
                networkEvent.Invoke(arg);

            _object.Render();

            //Assert:
            Assert.AreEqual(4, wasEvent1.Count);
            Assert.AreEqual(4, wasEvent2.Count);
            
            Assert.AreEqual(-2, wasEvent1[0]);
            Assert.AreEqual(-2, wasEvent2[0]);

            Assert.AreEqual(0, wasEvent1[1]);
            Assert.AreEqual(0, wasEvent2[1]);
            
            Assert.AreEqual(-7, wasEvent1[2]);
            Assert.AreEqual(-7, wasEvent2[2]);
            
            Assert.AreEqual(9, wasEvent1[3]);
            Assert.AreEqual(9, wasEvent2[3]);
        }
     
        
        [Test]
        public void Subscribe_AfterSpawn()
        {
            //Arrange:
            int wasEvent1 = -1;
            int wasEvent2 = -1;
        
            NetworkEvent<int> networkEvent = new NetworkEvent<int>(_object);

            //Act:
            _object.Mock_Spawn();
            networkEvent.OnEvent += v => wasEvent1 = v;
            networkEvent.Subscribe(v => wasEvent2 = v);
            
            networkEvent.Invoke(5);
            _object.Render();

            //Assert:
            Assert.AreEqual(5, wasEvent1);
            Assert.AreEqual(5, wasEvent2);
        }

        [Test]
        public void Unsubscribe()
        {
            int wasEvent1 = -1;
            int wasEvent2 = 1;
        
            void OnEventRaisen1(int value) => wasEvent1 = value;
            void OnEventRaisen2(int value) => wasEvent2 = value;
        
            //Arrange:
            NetworkEvent<int> networkEvent = new NetworkEvent<int>(_object);
            _object.Mock_Spawn();
            
            networkEvent.OnEvent += OnEventRaisen1;
            networkEvent.Subscribe(OnEventRaisen2);
        
            networkEvent.Invoke(7);
            
            _object.Render();
        
            //Pre-assert:
            Assert.AreEqual(7, wasEvent1);
            Assert.AreEqual(7, wasEvent2);
        
            //Act:
            wasEvent1 = -1;
            wasEvent2 = -1;
        
            networkEvent.OnEvent -= OnEventRaisen1;
            networkEvent.Unsubscribe(OnEventRaisen2);
            networkEvent.Invoke(-10);
        
            //Assert:
            Assert.AreEqual(-1, wasEvent1);
            Assert.AreEqual(-1, wasEvent2);
        }
        
        [Test]
        public void Dispose()
        {
            int wasEvent1 = -1;
            int wasEvent2 = -1;
        
            void OnEventRaisen1(int value) => wasEvent1 = value;
            void OnEventRaisen2(int value) => wasEvent2 = value;
        
            //Arrange:
            NetworkEvent<int> networkEvent = new NetworkEvent<int>(_object);
            _object.Mock_Spawn();
            
            networkEvent.OnEvent += OnEventRaisen1;
            networkEvent.Subscribe(OnEventRaisen2);
        
            networkEvent.Invoke(10);
            
            _object.Render();
        
            //Pre-assert:
            Assert.AreEqual(10, wasEvent1);
            Assert.AreEqual(10, wasEvent2);
        
            //Act:
            wasEvent1 = -1;
            wasEvent2 = -1;
        
            networkEvent.Dispose();
            networkEvent.Invoke(-3);
        
            //Assert:
            Assert.AreEqual(-1, wasEvent1);
            Assert.AreEqual(-1, wasEvent2);
        }
    }
}