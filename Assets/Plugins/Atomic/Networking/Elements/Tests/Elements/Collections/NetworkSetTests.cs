using System;
using NUnit.Framework;

namespace Atomic.Networking.Elements
{
    internal sealed class NetworkSetTests
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
                NetworkSet<int> _ = new NetworkSet<int>(null, capacity: 4, eventCapacity: 4);
            });
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Constructor_InvalidCapacity_ThrowsException(int capacity)
        {
            Assert.Catch<ArgumentException>(() =>
            {
                NetworkSet<int> _ = new NetworkSet<int>(_object, capacity, eventCapacity: 4);
            });
        }

        [TestCase(-2)]
        [TestCase(-1)]
        public void Constructor_InvalidEventCapacity_ThrowsException(int eventCapacity)
        {
            Assert.Catch<ArgumentException>(() =>
            {
                NetworkSet<int> _ = new NetworkSet<int>(_object, 4, eventCapacity);
            });
        }

        [Test]
        public void Constructor_BeforeSpawned()
        {
            //Arrange:
            var initial = new[] {-2, 3, -4};
            var set = new NetworkSet<int>(_object, capacity: 5, initial, eventCapacity: 1);

            //Assert:
            Assert.AreEqual(3, set.Count);
            Assert.IsTrue(set.Contains(-2));
            Assert.IsTrue(set.Contains(3));
            Assert.IsTrue(set.Contains(-4));

            //Assert:
            Assert.IsFalse(set.IsReadOnly);
            Assert.IsFalse(set.IsEmpty());
            Assert.IsTrue(set.IsNotEmpty());

            Assert.AreEqual(initial, set);
        }

        [Test]
        public void Constructor_AfterSpawned()
        {
            //Arrange:
            var initial = new[] {-2, 3, -4};
            var set = new NetworkSet<int>(_object, capacity: 5, initial, eventCapacity: 1);

            //Act:
            _object.Mock_Spawn();

            //Assert:
            Assert.AreEqual(3, set.Count);
            Assert.AreEqual(initial, set);

            Assert.IsTrue(set.Contains(-2));
            Assert.IsTrue(set.Contains(3));
            Assert.IsTrue(set.Contains(-4));
            Assert.IsFalse(set.Contains(5));

            Assert.IsFalse(set.IsReadOnly);
            Assert.IsFalse(set.IsEmpty());
            Assert.IsTrue(set.IsNotEmpty());
        }

        [Test]
        public void Add_BeforeSpawned()
        {
            //Arrange:
            bool stateChanged = false;
            int addedItem = -1;

            var initial = new[] {-2, 3, -4};
            var set = new NetworkSet<int>(_object, capacity: 4, initial, eventCapacity: 1);
            set.OnStateChanged += () => stateChanged = true;
            set.OnItemAdded += v => { addedItem = v; };

            //Act:
            bool success = set.Add(555);

            //Assert:
            Assert.IsTrue(success);
            Assert.AreEqual(4, set.Count);
            Assert.IsTrue(set.Contains(555));

            Assert.IsFalse(stateChanged);
            Assert.AreEqual(-1, addedItem);
        }

        [Test]
        public void Add_ItemThatAlreadyExists_BeforeSpawned_ReturnFalse()
        {
            //Arrange:
            bool stateChanged = false;
            int addedItem = -1;

            var initial = new[] {-2, 3, -4, 555};
            var set = new NetworkSet<int>(_object, capacity: 10, initial, eventCapacity: 1);
            set.OnStateChanged += () => stateChanged = true;
            set.OnItemAdded += v => { addedItem = v; };

            //Act:
            bool success = set.Add(555);

            //Assert:
            Assert.IsFalse(success);
            Assert.AreEqual(4, set.Count);
            Assert.IsTrue(set.Contains(555));

            Assert.IsFalse(stateChanged);
            Assert.AreEqual(-1, addedItem);
        }

        [Test]
        public void Add_SetIsFull_BeforeSpawned_ThrowsException()
        {
            //Arrange:
            var initial = new[] {-2, 3, -4, 7};
            var set = new NetworkSet<int>(_object, capacity: 4, initial, eventCapacity: 1);

            //Assert:
            Assert.Catch<Exception>(() =>
            {
                bool _ = set.Add(555);
            });
        }

        [Test]
        public void Add_AfterSpawned()
        {
            //Arrange:
            bool stateChanged = false;
            int addedItem = -1;

            var initial = new[] {-2, 3, -4};
            var set = new NetworkSet<int>(_object, capacity: 4, initial, eventCapacity: 1);
            set.OnStateChanged += () => stateChanged = true;
            set.OnItemAdded += v => { addedItem = v; };

            //Act:
            _object.Mock_Spawn();
            bool success = set.Add(555);
            _object.Render();

            //Assert: 
            Assert.IsTrue(success);
            Assert.AreEqual(4, set.Count);
            Assert.IsTrue(set.Contains(555));

            //Assert:
            Assert.IsTrue(stateChanged);
            Assert.AreEqual(555, addedItem);
        }

        [Test]
        public void Add_ItemThatAlreadyExists_AfterSpawned_ReturnsFalse()
        {
            //Arrange:
            bool stateChanged = false;
            int addedItem = -1;

            var initial = new[] {-2, 3, -4, 555};
            var set = new NetworkSet<int>(_object, capacity: 10, initial, eventCapacity: 1);
            set.OnStateChanged += () => stateChanged = true;
            set.OnItemAdded += v => { addedItem = v; };

            //Act:
            _object.Mock_Spawn();
            bool success = set.Add(555);
            _object.Render();

            //Assert:
            Assert.IsFalse(success);
            Assert.AreEqual(4, set.Count);
            Assert.IsTrue(set.Contains(555));

            Assert.IsFalse(stateChanged);
            Assert.AreEqual(-1, addedItem);
        }

        [Test]
        public void Add_SetIsFull_AfterSpawned_ThrowsException()
        {
            //Arrange:
            var initial = new[] {-2, 3, -4, 7};
            var set = new NetworkSet<int>(_object, capacity: 4, initial, eventCapacity: 1);

            _object.Mock_Spawn();

            //Assert:
            Assert.Catch<Exception>(() =>
            {
                bool _ = set.Add(555);
            });
        }

        [Test]
        public void Remove_BeforeSpawned()
        {
            //Arrange:
            bool stateChanged = false;
            int removedItem = -1;

            //Arrange:
            var initial = new[] {-2, 3, -4, 7};
            var set = new NetworkSet<int>(_object, capacity: 4, initial, eventCapacity: 1);

            set.OnItemRemoved += v => removedItem = v;
            set.OnStateChanged += () => stateChanged = true;

            Assert.AreEqual(4, set.Count);

            //Act:
            bool success = set.Remove(-4);

            //Assert:
            Assert.IsTrue(success);
            Assert.AreEqual(3, set.Count);

            Assert.IsFalse(set.Contains(-4));
            Assert.IsTrue(set.Contains(-2));
            Assert.IsTrue(set.Contains(7));
            Assert.IsTrue(set.Contains(3));

            Assert.IsFalse(stateChanged);
            Assert.AreEqual(-1, removedItem);
        }

        [Test]
        public void Remove_AfterSpawned()
        {
            //Arrange:
            bool stateChanged = false;
            int removedItem = -1;

            //Arrange:
            var initial = new[] {-2, 3, -4, 7};
            var set = new NetworkSet<int>(_object, capacity: 4, initial, eventCapacity: 1);

            set.OnItemRemoved += v => removedItem = v;
            set.OnStateChanged += () => stateChanged = true;

            Assert.AreEqual(4, set.Count);

            //Act:
            _object.Mock_Spawn();
            bool success = set.Remove(-4);
            _object.Render();

            //Assert:
            Assert.IsTrue(success);
            Assert.AreEqual(3, set.Count);

            Assert.IsFalse(set.Contains(-4));
            Assert.IsTrue(set.Contains(-2));
            Assert.IsTrue(set.Contains(7));
            Assert.IsTrue(set.Contains(3));

            Assert.IsTrue(stateChanged);
            Assert.AreEqual(-4, removedItem);

            //Assert internal:
            Assert.AreEqual(2, set.free_list());
        }

        [Test]
        public void Remove_AbsentItem_AfterSpawned_ReturnFalse()
        {
            //Arrange:
            bool stateChanged = false;
            int removedItem = -1;

            var initial = new[] {-2, 3, -4, 7};
            var set = new NetworkSet<int>(_object, capacity: 4, initial, eventCapacity: 1);
            set.OnItemRemoved += v => removedItem = v;
            set.OnStateChanged += () => stateChanged = true;

            //Act:
            _object.Mock_Spawn();
            bool success = set.Remove(100);
            _object.Render();

            //Assert:
            Assert.IsFalse(success);
            Assert.AreEqual(4, set.Count);

            Assert.IsFalse(set.Contains(100));

            Assert.IsTrue(set.Contains(-2));
            Assert.IsTrue(set.Contains(3));
            Assert.IsTrue(set.Contains(-4));
            Assert.IsTrue(set.Contains(7));

            Assert.IsFalse(stateChanged);
            Assert.AreEqual(-1, removedItem);
        }

        [Test]
        public void CheckInternalState_AfterSpawn()
        {
            //Arrange:
            var initial = new[] {-2, 100, 5};
            var set = new NetworkSet<int>(_object, capacity: 4, initial, eventCapacity: 1);
            _object.Mock_Spawn();

            const int undefined = NetworkSet<int>.UNDEFINED;

            //Pre-Assert:
            Assert.AreEqual(3, set.Count);
            Assert.AreEqual(-1, set.free_list());

            //Remove 1:
            Assert.IsTrue(set.Remove(100));

            NetworkSet<int>.Node node = set.item(1);
            Assert.AreEqual(100, node.value);
            Assert.AreEqual(undefined, node.next);
            Assert.AreEqual(undefined, set.bucket(0)); //Check bucket that keeps "100"

            Assert.AreEqual(2, set.Count);
            Assert.AreEqual(1, set.free_list());
            //////////////////////////////////////////

            //Remove 2:
            Assert.IsTrue(set.Remove(-2));

            node = set.item(0);
            Assert.AreEqual(-2, node.value);
            Assert.AreEqual(1, node.next);
            Assert.AreEqual(undefined, set.bucket(2));

            Assert.AreEqual(1, set.Count);
            Assert.AreEqual(0, set.free_list());
            //////////////////////////////////////////

            //Remove 3:
            Assert.IsTrue(set.Remove(5));

            node = set.item(2);
            Assert.AreEqual(5, node.value);
            Assert.AreEqual(0, node.next);
            Assert.AreEqual(undefined, set.bucket(1));

            Assert.AreEqual(0, set.Count);
            Assert.AreEqual(undefined, set.free_list()); //Reset free list if count is zero!

            //////////////////////////////////////////

            //Add 1:
            Assert.IsTrue(set.Add(1));

            node = set.item(0);
            Assert.AreEqual(1, node.value);
            Assert.AreEqual(undefined, node.next);

            Assert.AreEqual(1, set.Count);
            Assert.AreEqual(undefined, set.free_list());

            //////////////////////////////////////////

            //Add 2:
            Assert.IsTrue(set.Add(10));

            node = set.item(1);
            Assert.AreEqual(10, node.value);
            Assert.AreEqual(undefined, node.next);

            Assert.AreEqual(2, set.Count);
            Assert.AreEqual(undefined, set.free_list());

            //////////////////////////////////////////

            //Add 3:
            Assert.IsTrue(set.Add(100));

            node = set.item(2);
            Assert.AreEqual(100, node.value);
            Assert.AreEqual(undefined, node.next);

            Assert.AreEqual(3, set.Count);
            Assert.AreEqual(undefined, set.free_list());

            //////////////////////////////////////////

            //Add 4:
            Assert.IsTrue(set.Add(1000));

            node = set.item(3);
            Assert.AreEqual(1000, node.value);
            Assert.AreEqual(2, node.next);

            Assert.AreEqual(4, set.Count);
            Assert.AreEqual(undefined, set.free_list());

            //////////////////////////////////////////

            //Remove 5:
            Assert.IsTrue(set.Remove(100));
            Assert.AreEqual(3, set.Count);
            Assert.AreEqual(2, set.free_list());
        }

        [Test]
        public void ExceptWith_BeforeSpawned()
        {
            //Arrange:
            bool stateChanged = false;

            var initial = new[] {-2, 3, -4, 7};
            var set = new NetworkSet<int>(_object, capacity: 4, initial, eventCapacity: 1);

            set.OnStateChanged += () => stateChanged = true;

            //Act:
            set.ExceptWith(new[] {3, -4, 100});

            //Assert:
            Assert.IsFalse(stateChanged);
            Assert.AreEqual(2, set.Count);

            Assert.IsTrue(set.Contains(-2));
            Assert.IsTrue(set.Contains(7));

            Assert.IsFalse(set.Contains(3));
            Assert.IsFalse(set.Contains(-4));
        }

        [Test]
        public void ExceptWith_AfterSpawned()
        {
            //Arrange:
            bool stateChanged = false;

            var initial = new[] {-2, 3, -4, 7};
            var set = new NetworkSet<int>(_object, capacity: 4, initial, eventCapacity: 1);

            set.OnStateChanged += () => stateChanged = true;

            //Act:

            _object.Mock_Spawn();
            set.ExceptWith(new[] {3, -4, 100});
            _object.Render();

            //Assert:
            Assert.IsTrue(stateChanged);
            Assert.AreEqual(2, set.Count);

            Assert.IsTrue(set.Contains(-2));
            Assert.IsTrue(set.Contains(7));

            Assert.IsFalse(set.Contains(3));
            Assert.IsFalse(set.Contains(-4));
        }

        [Test]
        public void IntersectWith_BeforeSpawned()
        {
            //Arrange:
            bool stateChanged = false;

            var initial = new[] {-2, 3, -4, 7};
            var set = new NetworkSet<int>(_object, capacity: 4, initial, eventCapacity: 1);
            set.OnStateChanged += () => stateChanged = true;

            //Act:
            set.IntersectWith(new[] {3, -2, 10});

            //Assert:
            Assert.IsFalse(stateChanged);
            Assert.AreEqual(2, set.Count);

            Assert.IsFalse(set.Contains(-4));
            Assert.IsFalse(set.Contains(7));

            Assert.IsTrue(set.Contains(-2));
            Assert.IsTrue(set.Contains(3));
        }

        [Test]
        public void IntersectWith_AfterSpawned()
        {
            //Arrange:
            bool stateChanged = false;

            var initial = new[] {-2, 3, -4, 7};
            var set = new NetworkSet<int>(_object, capacity: 4, initial, eventCapacity: 1);
            set.OnStateChanged += () => stateChanged = true;

            //Act:
            _object.Mock_Spawn();
            set.IntersectWith(new[] {3, -2, 10});
            _object.Render();

            //Assert:
            Assert.IsTrue(stateChanged);
            Assert.AreEqual(2, set.Count);

            Assert.IsFalse(set.Contains(-4));
            Assert.IsFalse(set.Contains(7));

            Assert.IsTrue(set.Contains(-2));
            Assert.IsTrue(set.Contains(3));
        }

        [Test]
        public void SymmetricExceptWith_BeforeSpawned()
        {
            //Arrange:
            bool stateChanged = false;

            var set = new NetworkSet<char>(_object, capacity: 8, new[] {'v', 'p', 'm', 'i', 'w'}, eventCapacity: 1);
            set.OnStateChanged += () => stateChanged = true;

            //Act:
            set.SymmetricExceptWith(new[] {'p', 'i', 'j'});

            //Assert:
            Assert.IsFalse(stateChanged);
            Assert.AreEqual(4, set.Count);

            Assert.IsTrue(set.Contains('v'));
            Assert.IsTrue(set.Contains('m'));
            Assert.IsTrue(set.Contains('w'));
            Assert.IsTrue(set.Contains('j'));

            Assert.IsFalse(set.Contains('p'));
            Assert.IsFalse(set.Contains('i'));
        }

        [Test]
        public void SymmetricExceptWith_AfterSpawned()
        {
            //Arrange:
            bool stateChanged = false;

            var set = new NetworkSet<char>(_object, capacity: 8, new[] {'v', 'p', 'm', 'i', 'w'}, eventCapacity: 1);
            set.OnStateChanged += () => stateChanged = true;

            //Act:
            _object.Mock_Spawn();
            set.SymmetricExceptWith(new[] {'p', 'i', 'j'});
            _object.Render();

            //Assert:
            Assert.IsTrue(stateChanged);
            Assert.AreEqual(4, set.Count);

            Assert.IsTrue(set.Contains('v'));
            Assert.IsTrue(set.Contains('m'));
            Assert.IsTrue(set.Contains('w'));
            Assert.IsTrue(set.Contains('j'));

            Assert.IsFalse(set.Contains('p'));
            Assert.IsFalse(set.Contains('i'));
        }

        [Test]
        public void UnionWith_BeforeSpawned()
        {
            //Arrange:
            bool stateChanged = false;

            var set = new NetworkSet<char>(_object, capacity: 8, new[] {'v', 'p', 'm', 'i', 'w'}, eventCapacity: 1);
            set.OnStateChanged += () => stateChanged = true;

            //Act:
            set.UnionWith(new[] {'p', 'i', 'j'});

            //Assert:
            Assert.IsFalse(stateChanged);
            Assert.AreEqual(6, set.Count);

            Assert.IsTrue(set.Contains('v'));
            Assert.IsTrue(set.Contains('m'));
            Assert.IsTrue(set.Contains('w'));
            Assert.IsTrue(set.Contains('j'));
            Assert.IsTrue(set.Contains('p'));
            Assert.IsTrue(set.Contains('i'));
        }

        [Test]
        public void UnionWith_AfterSpawned()
        {
            //Arrange:
            bool stateChanged = false;

            var set = new NetworkSet<char>(_object, capacity: 8, new[] {'v', 'p', 'm', 'i', 'w'}, eventCapacity: 1);
            set.OnStateChanged += () => stateChanged = true;

            //Act:
            _object.Mock_Spawn();
            set.UnionWith(new[] {'p', 'i', 'j'});
            _object.Render();

            //Assert:
            Assert.IsTrue(stateChanged);
            Assert.AreEqual(6, set.Count);

            Assert.IsTrue(set.Contains('v'));
            Assert.IsTrue(set.Contains('m'));
            Assert.IsTrue(set.Contains('w'));
            Assert.IsTrue(set.Contains('j'));
            Assert.IsTrue(set.Contains('p'));
            Assert.IsTrue(set.Contains('i'));
        }

        [Test]
        public void CopyTo()
        {
            //Arrange:
            var set = new NetworkSet<char>(_object, capacity: 4, new[] {'v', 'p', 'm', 'i'}, eventCapacity: 0);
            var arr = new char[3];

            //Act:
            set.CopyTo(arr);

            //Assert:
            Assert.AreEqual(new[] {'v', 'p', 'm'}, arr);
        }

        [Test]
        public void Clear_BeforeSpawned()
        {
            //Arrange:
            var stateChanged = false;
            var set = new NetworkSet<char>(_object, capacity: 4, new[] {'v', 'p', 'm'}, eventCapacity: 0);

            //Act:
            set.OnStateChanged += () => stateChanged = true;
            set.Clear();

            Assert.IsFalse(stateChanged);
            Assert.AreEqual(0, set.Count);
            Assert.AreEqual(Array.Empty<char>(), set);
        }

        [Test]
        public void Clear_AfterSpawned()
        {
            //Arrange:
            var stateChanged = false;
            var set = new NetworkSet<char>(_object, capacity: 4, new[] {'v', 'p', 'm'}, eventCapacity: 0);
            set.OnStateChanged += () => stateChanged = true;

            //Act:
            _object.Mock_Spawn();
            set.Clear();
            _object.Render();

            Assert.IsTrue(stateChanged);
            Assert.AreEqual(0, set.Count);
            Assert.AreEqual(Array.Empty<char>(), set);
        }

        [Test]
        public void Clear_AndSetIsEmpty_AfterSpawned_NothingHappened()
        {
            //Arrange:
            var stateChanged = false;
            var set = new NetworkSet<char>(_object, capacity: 4, eventCapacity: 0);
            set.OnStateChanged += () => stateChanged = true;

            //Act:
            _object.Mock_Spawn();
            set.Clear();
            _object.Render();

            Assert.IsFalse(stateChanged);
        }

        [Test]
        public void ReplaceToCollection_BeforeSpawned()
        {
            //Arrange:
            bool stateChanged = false;

            var set = new NetworkSet<char>(_object, capacity: 4, new[] {'v', 'p', 'm', 'w'}, eventCapacity: 0);
            set.OnStateChanged += () => stateChanged = true;

            //Act:
            set.ReplaceTo('p', 'i', 'j');

            //Assert:
            Assert.IsFalse(stateChanged);
            Assert.AreEqual(3, set.Count);

            Assert.IsFalse(set.Contains('v'));
            Assert.IsFalse(set.Contains('m'));
            Assert.IsFalse(set.Contains('w'));

            Assert.IsTrue(set.Contains('j'));
            Assert.IsTrue(set.Contains('p'));
            Assert.IsTrue(set.Contains('i'));
        }

        [Test]
        public void ReplaceToCollection_AfterSpawned()
        {
            //Arrange:
            bool stateChanged = false;

            var set = new NetworkSet<char>(_object, capacity: 4, new[] {'v', 'p', 'm', 'w'}, eventCapacity: 0);
            set.OnStateChanged += () => stateChanged = true;

            //Act:
            _object.Mock_Spawn();
            set.ReplaceTo('p', 'i', 'j');
            _object.Render();

            //Assert:
            Assert.IsTrue(stateChanged);
            Assert.AreEqual(3, set.Count);

            Assert.IsFalse(set.Contains('v'));
            Assert.IsFalse(set.Contains('m'));
            Assert.IsFalse(set.Contains('w'));

            Assert.IsTrue(set.Contains('j'));
            Assert.IsTrue(set.Contains('p'));
            Assert.IsTrue(set.Contains('i'));
        }

        [Test]
        public void ReplaceToItem_BeforeSpawned()
        {
            //Arrange:
            bool stateChanged = false;

            var set = new NetworkSet<char>(_object, capacity: 4, new[] {'v', 'p', 'm', 'w'}, eventCapacity: 0);
            set.OnStateChanged += () => stateChanged = true;

            //Act:
            set.ReplaceTo('a');

            //Assert:
            Assert.IsFalse(stateChanged);
            Assert.AreEqual(1, set.Count);

            Assert.IsFalse(set.Contains('v'));
            Assert.IsFalse(set.Contains('m'));
            Assert.IsFalse(set.Contains('w'));
            Assert.IsFalse(set.Contains('p'));

            Assert.IsTrue(set.Contains('a'));
        }

        [Test]
        public void ReplaceToItem_AfterSpawned()
        {
            //Arrange:
            bool stateChanged = false;

            var set = new NetworkSet<char>(_object, capacity: 4, new[] {'v', 'p', 'm', 'w'}, eventCapacity: 0);
            set.OnStateChanged += () => stateChanged = true;

            //Act:
            _object.Mock_Spawn();
            set.ReplaceTo('a');
            _object.Render();

            //Assert:
            Assert.IsTrue(stateChanged);
            Assert.AreEqual(1, set.Count);

            Assert.IsFalse(set.Contains('v'));
            Assert.IsFalse(set.Contains('m'));
            Assert.IsFalse(set.Contains('w'));
            Assert.IsFalse(set.Contains('p'));

            Assert.IsTrue(set.Contains('a'));
        }
    }
}