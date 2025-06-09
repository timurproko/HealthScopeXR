using System;
using NUnit.Framework;

namespace Atomic.Networking.Elements
{
    internal sealed class NetworkArrayTests
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
                NetworkArray<int> _ = new NetworkArray<int>(null, 5);
            });
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-5)]
        public void Constructor_InvalidLength_ThrowsException(int length)
        {
            Assert.Catch<ArgumentException>(() =>
            {
                NetworkArray<int> _ = new NetworkArray<int>(_object, length);
            });
        }

        [Test]
        public void Constructor()
        {
            //Arrange:
            NetworkArray<int> array = new NetworkArray<int>(_object, capacity: 5);

            //Assert:
            Assert.AreEqual(5, array.Length);

            for (int i = 0; i < 5; i++)
                Assert.AreEqual(0, array[i]);
        }

        [Test]
        public void Constructor_InitialItems()
        {
            //Arrange:
            NetworkArray<int> array = new NetworkArray<int>(_object, capacity: 5, new[] {-1, 2, -3, 4, -5});

            //Assert:
            Assert.AreEqual(5, array.Length);

            Assert.AreEqual(-1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(-3, array[2]);
            Assert.AreEqual(4, array[3]);
            Assert.AreEqual(-5, array[4]);
        }

        [Test]
        public void Constructor_InitialItems_MoreThanLength_TrimLastElements()
        {
            //Arrange:
            NetworkArray<int> array = new NetworkArray<int>(_object, capacity: 3, new[] {-1, 2, -3, 4, -5});

            //Assert:
            Assert.AreEqual(3, array.Length);

            Assert.AreEqual(-1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(-3, array[2]);

            Assert.Catch<IndexOutOfRangeException>(() =>
            {
                var _ = array[3];
            });
            Assert.Catch<IndexOutOfRangeException>(() =>
            {
                var _ = array[4];
            });
        }

        [Test]
        public void Get_BeforeSpawned()
        {
            //Arrange:
            var array = new NetworkArray<int>(_object, 5, new[] {1, 2, 3, 4, 10});

            //Assert:
            Assert.AreEqual(5, array.Length);

            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(4, array[3]);
            Assert.AreEqual(10, array[4]);
        }

        [Test]
        public void Enumerate_BeforeSpawned()
        {
            var initial = new[] {1, 2, 3, 4, 10};
            var array = new NetworkArray<int>(_object, 5, initial);

            int index = 0;
            foreach (int element in array)
                Assert.AreEqual(initial[index++], element);
        }

        [Test]
        public void Enumerate_AfterSpawned()
        {
            var initial = new[] {1, 2, 3, 4, 10};
            var array = new NetworkArray<int>(_object, 5, initial);

            _object.Mock_Spawn();

            int index = 0;
            foreach (int element in array)
                Assert.AreEqual(initial[index++], element);
        }


        [Test]
        public void Get_AfterSpawned()
        {
            //Arrange:
            var array = new NetworkArray<int>(_object, capacity: 5, new[] {1, 2, 3, 4, 10});

            //Act:
            _object.Mock_Spawn();

            //Assert:
            Assert.AreEqual(5, array.Length);

            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(4, array[3]);
            Assert.AreEqual(10, array[4]);
        }

        [TestCase(20)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(-1)]
        [TestCase(-10)]
        public void Get_IndexOutOfRange_BeforeSpawned_ThrowsException(int index)
        {
            //Arrange:
            var array = new NetworkArray<int>(_object, capacity: 5, new[] {1, 2, 3, 4, 10});

            //Act:
            Assert.Catch<IndexOutOfRangeException>(() =>
            {
                int _ = array[index];
            });
        }

        [TestCase(20)]
        [TestCase(5)]
        [TestCase(-1)]
        [TestCase(-10)]
        public void Get_IndexOutOfRange_AfterSpawned_ThrowsException(int index)
        {
            //Arrange:
            var array = new NetworkArray<int>(_object, 5, new[] {1, 2, 3, 4, 10});

            //Act:
            Assert.Catch<IndexOutOfRangeException>(() =>
            {
                int _ = array[index];
            });
        }

        [TestCase(20)]
        [TestCase(3)]
        [TestCase(-1)]
        [TestCase(-10)]
        public void Set_IndexOutOfRange_BeforeSpawned_ThrowsException(int index)
        {
            //Arrange:
            var array = new NetworkArray<int>(_object, capacity: 3);

            //Act:
            Assert.Catch<IndexOutOfRangeException>(() => array[index] = 40);
        }

        [Test]
        public void Set_AfterSpawned()
        {
            //Arrange:
            var array = new NetworkArray<int>(_object, capacity: 5);

            int changedItem = -1;
            int changedIndex = -1;
            bool wasEvent = false;

            array.OnItemChanged += (index, value) =>
            {
                changedIndex = index;
                changedItem = value;
            };

            array.OnStateChanged += () => wasEvent = true;

            //Act:
            _object.Mock_Spawn();
            array[3] = 25;
            _object.Render();

            //Assert:
            Assert.IsTrue(wasEvent);

            Assert.AreEqual(25, array[3]);
            Assert.AreEqual(25, changedItem);
            Assert.AreEqual(3, changedIndex);
        }

        [Test]
        public void Set_BeforeSpawned()
        {
            //Arrange:
            var array = new NetworkArray<int>(_object, capacity: 5);

            int changedItem = -1;
            int changedIndex = -1;
            bool wasEvent = false;

            array.OnItemChanged += (index, value) =>
            {
                changedIndex = index;
                changedItem = value;
            };

            array.OnStateChanged += () => wasEvent = true;

            //Act:
            array[3] = 25;

            //Assert:
            Assert.IsFalse(wasEvent);

            Assert.AreEqual(25, array[3]);
            Assert.AreNotEqual(25, changedItem);
            Assert.AreNotEqual(3, changedIndex);
        }
    }
}