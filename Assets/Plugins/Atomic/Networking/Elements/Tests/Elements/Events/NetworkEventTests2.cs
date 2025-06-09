using System;
using System.Collections.Generic;
using Atomic.Networking;
using NUnit.Framework;
using UnityEngine;

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace Atomic.Networking.Elements
{
    [TestFixture]
    internal sealed class NetworkEventTests2
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
        public void Constructor_AgentIsNull_ThenException()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                NetworkEvent<int, float> _ = new NetworkEvent<int, float>(null);
            });
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Constructor_InvalidCapacity_ThrowsException(int capacity)
        {
            Assert.Catch<ArgumentException>(() =>
            {
                NetworkEvent<int, int> _ = new NetworkEvent<int, int>(_object, capacity);
            });
        }

        [Test]
        public void Invoke_BeforeSpawn_NothingHappened()
        {
            //Arrange:
            (int, int) wasEvent1 = (-1, -1);
            (int, int) wasEvent2 = (-1, -1);

            NetworkEvent<int, int> networkEvent = new NetworkEvent<int, int>(_object);
            networkEvent.OnEvent += (v1, v2) => wasEvent1 = new(v1, v2);
            networkEvent.Subscribe((v1, v2) => wasEvent2 = new(v1, v2));

            //Act:
            networkEvent.Invoke(5, -2);

            //Assert:
            Assert.AreEqual((-1, -1), wasEvent1);
            Assert.AreEqual((-1, -1), wasEvent2);
        }

        [Test]
        public void InvokeEvent()
        {
            //Arrange:
            (int, int) wasEvent1 = (-1, -1);
            (int, int) wasEvent2 = (-1, -1);

            NetworkEvent<int, int> networkEvent = new NetworkEvent<int, int>(_object);
            networkEvent.OnEvent += (v1, v2) => wasEvent1 = new(v1, v2);
            networkEvent.Subscribe((v1, v2) => wasEvent2 = new(v1, v2));

            //Act:
            _object.Mock_Spawn();
            networkEvent.Invoke(5, -2);
            _object.Render();

            //Assert:
            Assert.AreEqual((5, -2), wasEvent1);
            Assert.AreEqual((5, -2), wasEvent2);
        }

        [Test]
        public void Invoke_SeveralTimes()
        {
            //Arrange:
            List<(int, int)> wasEvent1 = new List<(int, int)>();
            List<(int, int)> wasEvent2 = new List<(int, int)>();

            NetworkEvent<int, int> networkEvent = new NetworkEvent<int, int>(_object);
            networkEvent.OnEvent += (v1, v2) => wasEvent1.Add(new(v1, v2));
            networkEvent.Subscribe((v1, v2) => wasEvent2.Add(new(v1, v2)));

            //Act:
            _object.Mock_Spawn();

            (int, int)[] queue = {(5, 3), (8, 2), (-2, 1), (0, 0)};

            foreach ((int item1, int item2) in queue) 
                networkEvent.Invoke(item1, item2);

            _object.Render();
            
            //Assert:
            for (int i = 0; i < queue.Length; i++)
            {
                Assert.AreEqual(queue[i], wasEvent1[i]);
                Assert.AreEqual(queue[i], wasEvent2[i]);
            }
        }

        [Test]
        public void Invoke_MoreTimes_ThenQueueCapacity()
        {
            //Arrange:
            List<(int, int)> wasEvent1 = new List<(int, int)>();
            List<(int, int)> wasEvent2 = new List<(int, int)>();

            NetworkEvent<int, int> networkEvent = new NetworkEvent<int, int>(_object);
            networkEvent.OnEvent += (v1, v2) => wasEvent1.Add(new(v1, v2));
            networkEvent.Subscribe((v1, v2) => wasEvent2.Add(new(v1, v2)));

            //Act:
            _object.Mock_Spawn();

            (int, int)[] queue = {(5, 1), (8, 8), (-2, -2), (0, 0), (-7, -6), (9, 10)};

            foreach ((int item1, int item2) in queue) 
                networkEvent.Invoke(item1, item2);

            _object.Render();

            //Assert:
            Assert.AreEqual(4, wasEvent1.Count);
            Assert.AreEqual(4, wasEvent2.Count);

            Assert.AreEqual((-2, -2), wasEvent1[0]);
            Assert.AreEqual((-2, -2), wasEvent2[0]);

            Assert.AreEqual((0, 0), wasEvent1[1]);
            Assert.AreEqual((0, 0), wasEvent2[1]);

            Assert.AreEqual((-7, -6), wasEvent1[2]);
            Assert.AreEqual((-7, -6), wasEvent2[2]);

            Assert.AreEqual((9, 10), wasEvent1[3]);
            Assert.AreEqual((9, 10), wasEvent2[3]);
        }

        [Test]
        public void Subscribe_AfterSpawn()
        {
            //Arrange:
            (int, int) wasEvent1 = (-1, -1);
            (int, int) wasEvent2 = (-1, -1);

            NetworkEvent<int, int> networkEvent = new NetworkEvent<int, int>(_object);

            //Act:
            _object.Mock_Spawn();
            networkEvent.OnEvent += (v1, v2) => wasEvent1 = new(v1, v2);
            networkEvent.Subscribe((v1, v2) => wasEvent2 = new(v1, v2));

            networkEvent.Invoke(5, 3);
            _object.Render();

            //Assert:
            Assert.AreEqual((5, 3), wasEvent1);
            Assert.AreEqual((5, 3), wasEvent2);
        }

        [Test]
        public void Unsubscribe()
        {
            (int, int) wasEvent1 = (-1, -1);
            (int, int) wasEvent2 = (-1, -1);

            void OnEventRaisen1(int value1, int value2) => wasEvent1 = new(value1, value2);
            void OnEventRaisen2(int value1, int value2) => wasEvent2 = new(value1, value2);

            //Arrange:
            NetworkEvent<int, int> networkEvent = new NetworkEvent<int, int>(_object);
            _object.Mock_Spawn();

            networkEvent.OnEvent += OnEventRaisen1;
            networkEvent.Subscribe(OnEventRaisen2);

            networkEvent.Invoke(7, 4);

            _object.Render();

            //Pre-assert:
            Assert.AreEqual((7, 4), wasEvent1);
            Assert.AreEqual((7, 4), wasEvent2);

            //Act:
            wasEvent1 = (-1, -1);
            wasEvent2 = (-1, -1);

            networkEvent.OnEvent -= OnEventRaisen1;
            networkEvent.Unsubscribe(OnEventRaisen2);
            networkEvent.Invoke(-10, -10);

            //Assert:
            Assert.AreEqual((-1, -1), wasEvent1);
            Assert.AreEqual((-1, -1), wasEvent2);
        }

        [Test]
        public void Dispose()
        {
            (int, int) wasEvent1 = (-1, -1);
            (int, int) wasEvent2 = (-1, -1);

            void OnEventRaisen1(int value1, int value2) => wasEvent1 = new(value1, value2);
            void OnEventRaisen2(int value1, int value2) => wasEvent2 = new(value1, value2);

            //Arrange:
            NetworkEvent<int, int> networkEvent = new NetworkEvent<int, int>(_object);
            _object.Mock_Spawn();

            networkEvent.OnEvent += OnEventRaisen1;
            networkEvent.Subscribe(OnEventRaisen2);

            networkEvent.Invoke(10, 10);

            _object.Render();

            //Pre-assert:
            Assert.AreEqual((10, 10), wasEvent1);
            Assert.AreEqual((10, 10), wasEvent2);

            //Act:
            wasEvent1 = (-1, -1);
            wasEvent2 = (-1, -1);

            networkEvent.Dispose();
            networkEvent.Invoke(-3, -3);

            //Assert:
            Assert.AreEqual((-1, -1), wasEvent1);
            Assert.AreEqual((-1, -1), wasEvent2);
        }
    }
}