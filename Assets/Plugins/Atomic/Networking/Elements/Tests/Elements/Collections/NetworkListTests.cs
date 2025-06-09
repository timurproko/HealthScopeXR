using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

// ReSharper disable CollectionNeverQueried.Local

namespace Atomic.Networking.Elements
{
    internal sealed class NetworkListTests
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
            Assert.Throws<ArgumentNullException>(() =>
            {
                NetworkList<int> _ = new NetworkList<int>(
                    agent: null,
                    capacity: 5,
                    eventCapacity: 4
                );
            });
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Constructor_InvalidCapacity_ThrowsException(int capacity)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                NetworkList<int> _ = new NetworkList<int>(_object, capacity);
            });
        }

        [TestCase(-2)]
        [TestCase(-1)]
        public void Constructor_InvalidEventCapacity_ThrowssException(int eventCapacity)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                NetworkList<int> _ = new NetworkList<int>(_object, 4, eventCapacity);
            });
        }

        [Test]
        public void Constructor_InitialItems()
        {
            //Arrange:
            var initialItems = new[] {-2, 3, -4};
            var list = new NetworkList<int>(_object, capacity: 5, initialItems, eventCapacity: 1);

            //Assert:
            Assert.AreEqual(3, list.Count);
            Assert.IsFalse(list.IsReadOnly);
            Assert.AreEqual(initialItems, list);
        }

        [Test]
        public void CopyTo_BeforeSpawned()
        {
            //Arrange:
            var initial = new[] {-2, 3, -4};
            var list = new NetworkList<int>(_object, capacity: 5, initial, eventCapacity: 1);
            var arr = new int[3];

            //Act:
            list.CopyTo(arr);

            //Assert:
            Assert.AreEqual(initial, arr);
        }

        [Test]
        public void CopyTo_AfterSpawned()
        {
            //Arrange:
            var initial = new[] {-2, 3, -4};
            var list = new NetworkList<int>(_object, capacity: 5, initial, eventCapacity: 1);
            var arr = new int[3];

            //Act:
            _object.Mock_Spawn();
            list.CopyTo(arr);

            //Assert:
            Assert.AreEqual(initial, arr);
        }

        [Test]
        public void IndexOf_BeforeSpawned()
        {
            //Arrange:
            var initial = new[] {-2, 3, -4};
            var list = new NetworkList<int>(_object, capacity: 5, initial, eventCapacity: 1);

            //Assert:
            Assert.AreEqual(0, list.IndexOf(-2));
            Assert.AreEqual(1, list.IndexOf(3));
            Assert.AreEqual(2, list.IndexOf(-4));
            Assert.AreEqual(-1, list.IndexOf(1000));
        }

        [Test]
        public void IndexOf_AfterSpawned()
        {
            //Arrange:
            var initial = new[] {-2, 3, -4};
            var list = new NetworkList<int>(_object, capacity: 5, initial, eventCapacity: 1);

            //Act:
            _object.Mock_Spawn();

            //Assert:
            Assert.AreEqual(0, list.IndexOf(-2));
            Assert.AreEqual(1, list.IndexOf(3));
            Assert.AreEqual(2, list.IndexOf(-4));
            Assert.AreEqual(-1, list.IndexOf(1000));
        }

        [Test]
        public void Contains_BeforeSpawned()
        {
            //Arrange:
            var initial = new[] {-2, 3, -4};
            var list = new NetworkList<int>(_object, capacity: 2, initial, eventCapacity: 1);

            //Assert:
            Assert.AreEqual(2, list.Count);
            Assert.IsTrue(list.Contains(-2));
            Assert.IsTrue(list.Contains(3));
            Assert.IsFalse(list.Contains(-4));
        }

        [Test]
        public void Contains_AfterSpawned()
        {
            //Arrange:
            var initial = new[] {-2, 3, -4};
            var list = new NetworkList<int>(_object, capacity: 3, initial, eventCapacity: 1);

            //Act:
            _object.Mock_Spawn();

            //Assert:
            Assert.AreEqual(3, list.Count);
            Assert.IsTrue(list.Contains(-2));
            Assert.IsTrue(list.Contains(3));
            Assert.IsTrue(list.Contains(-4));
        }

        [Test]
        public void Get_BeforeSpawned()
        {
            //Arrange:
            var initial = new[] {-2, 3, -4};
            var list = new NetworkList<int>(_object, capacity: 3, initial, eventCapacity: 1);

            //Assert:
            Assert.AreEqual(-2, list[0]);
            Assert.AreEqual(3, list[1]);
            Assert.AreEqual(-4, list[2]);

            Assert.Catch<IndexOutOfRangeException>(() =>
            {
                var _ = list[-1];
            });
        }

        [Test]
        public void Get_AfterSpawned()
        {
            //Arrange:
            var initial = new[] {-2, 3, -4};
            var list = new NetworkList<int>(_object, capacity: 3, initial, eventCapacity: 1);

            //Act:
            _object.Mock_Spawn();

            //Assert:
            Assert.AreEqual(-2, list[0]);
            Assert.AreEqual(3, list[1]);
            Assert.AreEqual(-4, list[2]);

            Assert.Catch<IndexOutOfRangeException>(() =>
            {
                var _ = list[-1];
            });
        }

        [Test]
        public void Set_BeforeSpawned()
        {
            //Arrange:
            var initial = new[] {-2, 3, -4};
            var list = new NetworkList<int>(_object, capacity: 3, initial, eventCapacity: 1);

            //Act:
            list[1] = 100;

            //Assert:
            Assert.AreEqual(-2, list[0]);
            Assert.AreEqual(100, list[1]);
            Assert.AreEqual(-4, list[2]);
        }

        [Test]
        public void Set_AfterSpawned()
        {
            //Arrange:
            var initial = new[] {-2, 3, -4};
            var list = new NetworkList<int>(_object, capacity: 3, initial, eventCapacity: 1);

            //Act:
            _object.Mock_Spawn();
            list[1] = 100;

            //Assert:
            Assert.AreEqual(-2, list[0]);
            Assert.AreEqual(100, list[1]);
            Assert.AreEqual(-4, list[2]);
        }

        [TestCase(-1)]
        [TestCase(3)]
        [TestCase(4)]
        public void Set_InvalidIndex_BeforeSpawned_ThrowsException(int index)
        {
            //Arrange:
            var initial = new[] {-2, 3, -4};
            var list = new NetworkList<int>(_object, capacity: 3, initial, eventCapacity: 1);

            //Assert:
            Assert.Catch<IndexOutOfRangeException>(() => list[index] = 100);
        }

        [TestCase(-1)]
        [TestCase(3)]
        [TestCase(4)]
        public void Set_InvalidIndex_AfterSpawned_Throws_Exception(int index)
        {
            //Arrange:
            var initial = new[] {-2, 3, -4};
            var list = new NetworkList<int>(_object, capacity: 3, initial, eventCapacity: 1);

            //Act:
            _object.Mock_Spawn();

            //Assert:
            Assert.Catch<IndexOutOfRangeException>(() => list[index] = 100);
        }

        [Test]
        public void Update_AfterSpawned()
        {
            //Arrange:
            bool stateChanged = false;
            int updatedItem = -1;
            int updatedIndex = -1;

            var initial = new[] {-2, 3, -4};
            var list = new NetworkList<int>(_object, capacity: 3, initial, eventCapacity: 1);

            list.OnItemChanged += (i, v) =>
            {
                updatedIndex = i;
                updatedItem = v;
            };
            list.OnStateChanged += () => stateChanged = true;

            //Act:
            _object.Mock_Spawn();
            list.Update(1, 100);
            _object.Render();

            //Assert:
            Assert.AreEqual(3, list.Count);

            Assert.AreEqual(-2, list[0]);
            Assert.AreEqual(100, list[1]);
            Assert.AreEqual(-4, list[2]);

            Assert.IsTrue(stateChanged);
            Assert.AreEqual(100, updatedItem);
            Assert.AreEqual(1, updatedIndex);
        }

        [Test]
        public void Update_SameItem_AfterSpawn_NothingHappened()
        {
            //Arrange:
            bool stateChanged = false;
            int updatedItem = -1;
            int updatedIndex = -1;
        
            var initial = new[] {-2, 3, -4};
            var list = new NetworkList<int>(_object, capacity: 3, initial, eventCapacity: 1);
        
            list.OnItemChanged += (i, v) =>
            {
                updatedIndex = i;
                updatedItem = v;
            };
            list.OnStateChanged += () => stateChanged = true;
        
            //Act:
            _object.Mock_Spawn();
            list.Update(1, 3);
            _object.Render();
        
            //Assert:
            Assert.AreEqual(3, list.Count);
        
            Assert.AreEqual(-2, list[0]);
            Assert.AreEqual(3, list[1]);
            Assert.AreEqual(-4, list[2]);
        
            Assert.IsFalse(stateChanged);
            Assert.AreNotEqual(3, updatedItem);
            Assert.AreNotEqual(1, updatedIndex);
        }
        
        [TestCase(-1)]
        [TestCase(3)]
        [TestCase(4)]
        public void Update_InvalidIndex_AfterSpawn_ThrowsException(int index)
        {
            //Arrange:
            var initial = new[] {-2, 3, -4};
            var list = new NetworkList<int>(_object, capacity: 3, initial, eventCapacity: 1);
        
            //Act:
            _object.Mock_Spawn();
        
            //Assert:
            Assert.Catch<IndexOutOfRangeException>(() => list.Update(index, 1000));
        }
        
        [Test]
        public void Clear_BeforeSpawned()
        {
            //Arrange:
            bool stateChanged = false;
            var initial = new[] {-2, 3, -4};
            var list = new NetworkList<int>(_object, capacity: 3, initial, eventCapacity: 1);
        
            //Act:
            list.OnStateChanged += () => stateChanged = true;
            list.Clear();
        
            //Assert:
            Assert.IsFalse(stateChanged);
            Assert.AreEqual(0, list.Count);
            Assert.AreEqual(Array.Empty<string>(), list);
        }
        
        [Test]
        public void Clear_AfterSpawned()
        {
            //Arrange:
            bool stateChanged = false;
            var initial = new[] {-2, 3, -4};
            var list = new NetworkList<int>(_object, capacity: 3, initial, eventCapacity: 1);
        
            //Act:
            _object.Mock_Spawn();
        
            list.OnStateChanged += () => stateChanged = true;
            list.Clear();
        
            _object.Render();
        
            //Assert:
            Assert.IsTrue(stateChanged);
            Assert.AreEqual(0, list.Count);
            Assert.AreEqual(Array.Empty<string>(), list);
        }
        
        [Test]
        public void Clear_ListIsEmpty_AfterSpawned_NothingHappened()
        {
            //Arrange:
            bool stateChanged = false;
            var list = new NetworkList<int>(_object, capacity: 3, eventCapacity: 1);
        
            //Act:
            _object.Mock_Spawn();
        
            list.OnStateChanged += () => stateChanged = true;
            list.Clear();
        
            _object.Render();
        
            Assert.IsFalse(stateChanged);
        }

        [Test]
        public void Add_And_ListIsEmpty_BeforeSpawned()
        {
            //Arrange:
            bool stateChanged = false;
            int addedItem = -1;
            int addedIndex = -1;
        
            var list = new NetworkList<int>(_object, capacity: 3, eventCapacity: 1);
            
            list.OnStateChanged += () => stateChanged = true;
            list.OnItemInserted += (i, v) =>
            {
                addedIndex = i;
                addedItem = v;
            };
        
            //Act:
            list.Add(555);
        
            //Assert:
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(555, list[0]);
            Assert.IsTrue(list.Contains(555));
        
            Assert.IsFalse(stateChanged);
            Assert.AreEqual(-1, addedItem);
            Assert.AreEqual(-1, addedIndex);
        }
     
        [Test]
        public void Add_And_ListIsNotEmpty_BeforeSpawned()
        {
            //Arrange:
            bool stateChanged = false;
            int addedItem = -1;
            int addedIndex = -1;
        
            var initial = new[] {-2, 3, -4};
            var list = new NetworkList<int>(_object, capacity: 5, initial, eventCapacity: 1);
        
            list.OnStateChanged += () => stateChanged = true;
            list.OnItemInserted += (i, v) =>
            {
                addedIndex = i;
                addedItem = v;
            };
        
            //Act:
            list.Add(555);
        
            //Assert:
            Assert.AreEqual(4, list.Count);
        
            Assert.AreEqual(-2, list[0]);
            Assert.AreEqual(3, list[1]);
            Assert.AreEqual(-4, list[2]);
            Assert.AreEqual(555, list[3]);
        
            Assert.IsFalse(stateChanged);
            Assert.AreEqual(-1, addedItem);
            Assert.AreEqual(-1, addedIndex);
        }

        [Test]
        public void Add_BeforeSpawned_And_ListIsFull_ThrowsException()
        {
            //Arrange:
            var initial = new[] {-2, 3, -4};
            var list = new NetworkList<int>(_object, capacity: 3, initial, eventCapacity: 1);
        
            //Act:
            Assert.Catch<Exception>(() => list.Add(555));
        }

        [Test]
        public void Add_ListIsEmpty_AfterSpawned()
        {
            //Arrange:
            bool stateChanged = false;
            int addedItem = -1;
            int addedIndex = -1;
        
            var list = new NetworkList<int>(_object, capacity: 3, eventCapacity: 1);
            list.OnStateChanged += () => stateChanged = true;
            list.OnItemInserted += (i, v) =>
            {
                addedIndex = i;
                addedItem = v;
            };
        
            //Act:
            _object.Mock_Spawn();
            list.Add(555);
            _object.Render();
        
            //Assert:
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(555, list[0]);
            Assert.IsTrue(list.Contains(555));
        
            Assert.IsTrue(stateChanged);
            Assert.AreEqual(555, addedItem);
            Assert.AreEqual(0, addedIndex);
        }
      
        [Test]
        public void Add_ListIsNotEmpty_AfterSpawned()
        {
            //Arrange:
            bool stateChanged = false;
            int addedItem = -1;
            int addedIndex = -1;
        
            var initial = new[] {-2, 3, -4};
            var list = new NetworkList<int>(_object, capacity: 5, initial, eventCapacity: 1);
        
            list.OnStateChanged += () => stateChanged = true;
            list.OnItemInserted += (i, v) =>
            {
                addedIndex = i;
                addedItem = v;
            };
        
            //Act:
            _object.Mock_Spawn();
            list.Add(555);
            _object.Render();
        
            //Assert:
            Assert.AreEqual(4, list.Count);
        
            Assert.AreEqual(-2, list[0]);
            Assert.AreEqual(3, list[1]);
            Assert.AreEqual(-4, list[2]);
            Assert.AreEqual(555, list[3]);
        
            Assert.IsTrue(stateChanged);
            Assert.AreEqual(555, addedItem);
            Assert.AreEqual(3, addedIndex);
        }
  
        [Test]
        public void Add_AfterSpawned_And_ListIsFull_ThrowsException()
        {
            //Arrange:
            var initial = new[] {-2, 3, -4};
            var list = new NetworkList<int>(_object, capacity: 3, initial, eventCapacity: 1);
        
            //Act:
            _object.Mock_Spawn();
            Assert.Catch<Exception>(() => list.Add(555));
        }

        [TestCase(3)]
        [TestCase(2)]
        [TestCase(1)]
        [TestCase(0)]
        public void Insert_BeforeSpawned(int index)
        {
            //Arrange:
            bool stateChanged = false;
            int addedItem = -1;
            int addedIndex = -1;
        
            var initial = new[] {-2, 3, -4};
            var list = new NetworkList<int>(_object, capacity: 4, initial, eventCapacity: 1);
        
            list.OnStateChanged += () => stateChanged = true;
            list.OnItemInserted += (i, v) =>
            {
                addedIndex = i;
                addedItem = v;
            };
        
            //Act:
            list.Insert(index, 555);
        
            //Assert:
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(555, list[index]);
        
            Assert.IsFalse(stateChanged);
            Assert.AreEqual(-1, addedItem);
            Assert.AreEqual(-1, addedIndex);
        }
        
        [TestCase(2)]
        [TestCase(1)]
        [TestCase(0)]
        public void Insert_ListIsFull_ThrowsException(int index)
        {
            //Arrange:
            var initial = new[] {-2, 3, -4};
            var list = new NetworkList<int>(_object, capacity: 3, initial, eventCapacity: 1);
        
            //Act:
            _object.Mock_Spawn();
            Assert.Catch<Exception>(() => list.Insert(index, 555));
        }

        [TestCase(-1)]
        [TestCase(-10)]
        [TestCase(4)]
        [TestCase(5)]
        public void Insert_InvalidIndex_ThrowsException(int index)
        {
            //Arrange:
            var initial = new[] {-2, 3, -4};
            var list = new NetworkList<int>(_object, capacity: 4, initial, eventCapacity: 1);
        
            //Act:
            _object.Mock_Spawn();
            Assert.Catch<ArgumentOutOfRangeException>(() => list.Insert(index, 555));
        }
 
        [TestCaseSource(nameof(Insert_AfterSpawned_TestCases))]
        public void Insert_AfterSpawned(int value, int index, int[] expertedResult)
        {
            //Arrange:
            bool stateChanged = false;
            int addedItem = -1;
            int addedIndex = -1;
        
            var initial = new[] {-2, 3, -4};
            var list = new NetworkList<int>(_object, capacity: 4, initial, eventCapacity: 1);
        
            list.OnStateChanged += () => stateChanged = true;
            list.OnItemInserted += (i, v) =>
            {
                addedIndex = i;
                addedItem = v;
            };
        
            //Act:
            _object.Mock_Spawn();
            list.Insert(index, value);
            _object.Render();
        
            //Assert:
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(value, list[index]);
            Assert.AreEqual(expertedResult, list.ToArray());
        
            Assert.IsTrue(stateChanged);
            Assert.AreEqual(value, addedItem);
            Assert.AreEqual(index, addedIndex);
        }
      
        public static IEnumerable<TestCaseData> Insert_AfterSpawned_TestCases()
        {
            yield return new TestCaseData(777, 0, new[] {777, -2, 3, -4});
            yield return new TestCaseData(555, 1, new[] {-2, 555, 3, -4});
            yield return new TestCaseData(333, 2, new[] {-2, 3, 333, -4});
            yield return new TestCaseData(111, 3, new[] {-2, 3, -4, 111});
        }
 
        [TestCase(-2, 0)]
        [TestCase(3, 1)]
        [TestCase(-4, 2)]
        [TestCase(5, 3)]
        public void Remove_BeforeSpawned(int targetValue, int expectedIndex)
        {
            //Arrange:
            bool stateChanged = false;
            int removedItem = -1;
            int removedIndex = -1;
        
            var initial = new[] {-2, 3, -4, 5};
            var list = new NetworkList<int>(_object, capacity: 4, initial, eventCapacity: 1);
        
            list.OnStateChanged += () => stateChanged = true;
            list.OnItemDeleted += (i, v) =>
            {
                removedIndex = i;
                removedItem = v;
            };
        
            //Act:
            bool success = list.Remove(targetValue);
        
            //Assert:
            Assert.IsTrue(success);
            Assert.AreEqual(3, list.Count);
            Assert.IsFalse(list.Contains(targetValue));
        
            Assert.IsFalse(stateChanged);
            Assert.AreEqual(-1, removedItem);
            Assert.AreEqual(-1, removedIndex);
        }
 
        [TestCaseSource(nameof(Remove_AfterSpawned_TestCases))]
        public void Remove_AfterSpawned(int targetValue, int expectedIndex, int[] exprectedArray)
        {
            //Arrange:
            bool stateChanged = false;
            int removedItem = -1;
            int removedIndex = -1;
        
            var initial = new[] {-2, 3, -4, 5};
            var list = new NetworkList<int>(_object, capacity: 4, initial, eventCapacity: 1);
        
            list.OnStateChanged += () => stateChanged = true;
            list.OnItemDeleted += (i, v) =>
            {
                removedIndex = i;
                removedItem = v;
            };
        
            //Act:
            _object.Mock_Spawn();
            bool success = list.Remove(targetValue);
            _object.Render();
        
            //Assert:
            Assert.IsTrue(success);
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(exprectedArray, list.ToArray());
        
            Assert.IsTrue(stateChanged);
            Assert.AreEqual(targetValue, removedItem);
            Assert.AreEqual(expectedIndex, removedIndex);
        }
        
        private static IEnumerable<TestCaseData> Remove_AfterSpawned_TestCases()
        {
            yield return new TestCaseData(-2, 0, new[] {3, -4, 5});
            yield return new TestCaseData(3, 1, new[] {-2, -4, 5});
            yield return new TestCaseData(-4, 2, new[] {-2, 3, 5});
            yield return new TestCaseData(5, 3, new[] {-2, 3, -4});
        }
      
        [Test]
        public void Remove_AbsentItem_AfterSpawned_ReturnFalse()
        {
            //Arrange:
            bool stateChanged = false;
            int removedItem = -1;
            int removedIndex = -1;
        
            var initial = new[] {-2, 3, -4, 5};
            var list = new NetworkList<int>(_object, capacity: 4, initial, eventCapacity: 1);
        
            list.OnStateChanged += () => stateChanged = true;
            list.OnItemDeleted += (i, v) =>
            {
                removedIndex = i;
                removedItem = v;
            };
        
            //Act:
            _object.Mock_Spawn();
            bool success = list.Remove(777);
            _object.Render();
        
            //Assert:
            Assert.IsFalse(success);
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(initial, list.ToArray());
        
            Assert.IsFalse(stateChanged);
            Assert.AreEqual(-1, removedItem);
            Assert.AreEqual(-1, removedIndex);
        }

        [TestCase(-2, 0)]
        [TestCase(3, 1)]
        [TestCase(-4, 2)]
        [TestCase(5, 3)]
        public void RemoveAt_BeforeSpawned(int expectedValue, int index)
        {
            //Arrange:
            bool stateChanged = false;
            int removedItem = -1;
            int removedIndex = -1;
        
            var initial = new[] {-2, 3, -4, 5};
            var list = new NetworkList<int>(_object, capacity: 4, initial, eventCapacity: 1);
        
            list.OnStateChanged += () => stateChanged = true;
            list.OnItemDeleted += (i, v) =>
            {
                removedIndex = i;
                removedItem = v;
            };
        
            //Act:
            list.RemoveAt(index);
        
            //Assert:
            Assert.AreEqual(3, list.Count);
            Assert.IsFalse(list.Contains(expectedValue));
        
            Assert.IsFalse(stateChanged);
            Assert.AreEqual(-1, removedItem);
            Assert.AreEqual(-1, removedIndex);
        }
 
        [TestCaseSource(nameof(RemoveAt_AfterSpawned_TestCases))]
        public void RemoveAt_AfterSpawned(int expectedItem, int index, int[] expectedResult)
        {
            //Arrange:
            bool stateChanged = false;
            int removedItem = -1;
            int removedIndex = -1;
        
            var initial = new[] {-2, 3, -4, 5};
            var list = new NetworkList<int>(_object, capacity: 4, initial, eventCapacity: 1);
        
            list.OnStateChanged += () => stateChanged = true;
            list.OnItemDeleted += (i, v) =>
            {
                removedIndex = i;
                removedItem = v;
            };
        
            //Act:
            _object.Mock_Spawn();
            list.RemoveAt(index);
            _object.Render();
        
            //Assert:
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(expectedResult, list.ToArray());
        
            Assert.IsTrue(stateChanged);
            Assert.AreEqual(expectedItem, removedItem);
            Assert.AreEqual(index, removedIndex);
        }
        
        private static IEnumerable<TestCaseData> RemoveAt_AfterSpawned_TestCases()
        {
            yield return new TestCaseData(-2, 0, new[] {3, -4, 5});
            yield return new TestCaseData(3, 1, new[] {-2, -4, 5});
            yield return new TestCaseData(-4, 2, new[] {-2, 3, 5});
            yield return new TestCaseData(5, 3, new[] {-2, 3, -4});
        }
        
        [TestCase(-1)]
        [TestCase(-2)]
        [TestCase(4)]
        [TestCase(5)]
        public void RemoveAt_InvalidIndex_NothingHappened(int index)
        {
            //Arrange:
            bool stateChanged = false;
            int removedItem = -1;
            int removedIndex = -1;
        
            var initial = new[] {-2, 3, -4, 5};
            var list = new NetworkList<int>(_object, capacity: 4, initial, eventCapacity: 1);
        
            list.OnStateChanged += () => stateChanged = true;
            list.OnItemDeleted += (i, v) =>
            {
                removedIndex = i;
                removedItem = v;
            };
        
            //Act:
            _object.Mock_Spawn();
            list.RemoveAt(index);
            _object.Render();
        
            //Assert:
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(initial, list.ToArray());
        
            Assert.IsFalse(stateChanged);
            Assert.AreEqual(-1, removedItem);
            Assert.AreEqual(-1, removedIndex);
        }
    }
}