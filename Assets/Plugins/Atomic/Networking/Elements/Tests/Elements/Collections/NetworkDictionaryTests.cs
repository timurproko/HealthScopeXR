using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Atomic.Networking.Elements
{
    internal sealed class NetworkDictionaryTests
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

        #region Instantiate

        [Test]
        public void Constructor_AgentIsNull_ThrowsException()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                NetworkDictionary<char, int> _ = new NetworkDictionary<char, int>(null, capacity: 4);
            });
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Constructor_InvalidCapacity_ThrowsException(int capacity)
        {
            Assert.Catch<ArgumentException>(() =>
            {
                NetworkDictionary<char, int> _ = new NetworkDictionary<char, int>(_object, capacity);
            });
        }

        [Test]
        public void Constructor_BeforeSpawned()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('c', 2), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);

            Assert.AreEqual(initial.Length, dictionary.Count);
            Assert.IsTrue(dictionary.Contains(initial[0]));
            Assert.IsTrue(dictionary.Contains(initial[1]));
            Assert.IsTrue(dictionary.Contains(initial[2]));
            Assert.IsTrue(dictionary.Contains(initial[3]));

            Assert.IsTrue(dictionary.ContainsKey(initial[0].Key));
            Assert.IsTrue(dictionary.ContainsKey(initial[1].Key));
            Assert.IsTrue(dictionary.ContainsKey(initial[2].Key));
            Assert.IsTrue(dictionary.ContainsKey(initial[3].Key));

            Assert.IsFalse(dictionary.Contains(new KeyValuePair<char, int>('µ', 0)));
            Assert.IsFalse(dictionary.ContainsKey('µ'));
        }

        [Test]
        public void Constructor_BeforeSpawned_WithHashCollision()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('µ', 6)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 4, eventCapacity: 1, initial);

            Assert.AreEqual(initial.Length, dictionary.Count);
            Assert.IsTrue(dictionary.Contains(initial[0]));
            Assert.IsTrue(dictionary.Contains(initial[1]));
            Assert.IsTrue(dictionary.Contains(initial[2]));

            Assert.IsTrue(dictionary.ContainsKey(initial[0].Key));
            Assert.IsTrue(dictionary.ContainsKey(initial[1].Key));
            Assert.IsTrue(dictionary.ContainsKey(initial[2].Key));
        }

        [Test]
        public void Constructor_AfterSpawned()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('c', 2), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);

            _object.Mock_Spawn();

            Assert.AreEqual(initial.Length, dictionary.Count);
            Assert.IsTrue(dictionary.Contains(initial[0]));
            Assert.IsTrue(dictionary.Contains(initial[1]));
            Assert.IsTrue(dictionary.Contains(initial[2]));
            Assert.IsTrue(dictionary.Contains(initial[3]));

            Assert.IsTrue(dictionary.ContainsKey(initial[0].Key));
            Assert.IsTrue(dictionary.ContainsKey(initial[1].Key));
            Assert.IsTrue(dictionary.ContainsKey(initial[2].Key));
            Assert.IsTrue(dictionary.ContainsKey(initial[3].Key));

            Assert.IsFalse(dictionary.Contains(new KeyValuePair<char, int>('µ', 0)));
            Assert.IsFalse(dictionary.ContainsKey('µ'));
        }

        [Test]
        public void Constructor_AfterSpawned_WithHashCollision()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('µ', 6)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 4, eventCapacity: 1, initial);
            _object.Mock_Spawn();
            Assert.AreEqual(initial.Length, dictionary.Count);
            Assert.IsTrue(dictionary.Contains(initial[0]));
            Assert.IsTrue(dictionary.Contains(initial[1]));
            Assert.IsTrue(dictionary.Contains(initial[2]));

            Assert.IsTrue(dictionary.ContainsKey(initial[0].Key));
            Assert.IsTrue(dictionary.ContainsKey(initial[1].Key));
            Assert.IsTrue(dictionary.ContainsKey(initial[2].Key));
        }

        #endregion

        #region Add()

        [Test]
        public void Add_BeforeSpawned()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);

            var pair = new KeyValuePair<char, int>('c', 7);
            dictionary.Add(pair);

            Assert.AreEqual(initial.Length + 1, dictionary.Count);
            Assert.IsTrue(dictionary.Contains(pair));
        }

        [Test]
        public void Add_KeyThatAlreadyExists_BeforeSpawned_ThrowsException()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);

            var pair = initial[1];
            Assert.Catch<ArgumentException>(() => { dictionary.Add(pair); });
            Assert.Catch<ArgumentException>(() => { dictionary.Add(pair.Key, pair.Value); });
            pair = new KeyValuePair<char, int>(initial[1].Key, 6);
            Assert.Catch<ArgumentException>(() => { dictionary.Add(pair); });
            Assert.Catch<ArgumentException>(() => { dictionary.Add(pair.Key, pair.Value); });
            Assert.AreEqual(initial.Length, dictionary.Count);
        }

        [Test]
        public void WhenAddPairAndDictionaryIsFullBeforeSpawnedThenThrowsException()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 4, eventCapacity: 1, initial);

            var pair = new KeyValuePair<char, int>('µ', 6);
            Assert.Catch<Exception>(() => { dictionary.Add(pair); });
            Assert.Catch<Exception>(() => { dictionary.Add(pair.Key, pair.Value); });
            Assert.AreEqual(initial.Length, dictionary.Count);
        }

        [Test]
        public void AddAfterSpawned()
        {
            bool stateChanged = false;
            char addedKey = default;
            int addedValue = -1;

            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);
            dictionary.OnStateChanged += () => stateChanged = true;
            dictionary.OnItemAdded += (k, v) =>
            {
                addedKey = k;
                addedValue = v;
            };

            var pair = new KeyValuePair<char, int>('f', 6);
            _object.Mock_Spawn();
            dictionary.Add(pair);
            _object.Render();

            Assert.IsTrue(dictionary.Contains(pair));
            Assert.AreEqual(pair.Value, dictionary[pair.Key]);
            Assert.AreEqual(initial.Length + 1, dictionary.Count);

            Assert.IsTrue(stateChanged);
            Assert.AreEqual(pair.Key, addedKey);
            Assert.AreEqual(pair.Value, addedValue);
        }

        [TestCase(123, -1)]
        [TestCase(99, 99)]
        public void AddOfSameTypeAfterSpawned(int key, int value)
        {
            bool stateChanged = false;
            int addedKey = int.MinValue;
            int addedValue = int.MinValue;

            var initial = new[]
            {
                new KeyValuePair<int, int>(9, 1), new KeyValuePair<int, int>(8, -1), new KeyValuePair<int, int>(7, 9),
                new KeyValuePair<int, int>(6, 99)
            };
            var dictionary = new NetworkDictionary<int, int>(_object, capacity: 16, eventCapacity: 1, initial);
            dictionary.OnStateChanged += () => stateChanged = true;
            dictionary.OnItemAdded += (k, v) =>
            {
                addedKey = k;
                addedValue = v;
            };

            var pair = new KeyValuePair<int, int>(key, value);
            _object.Mock_Spawn();
            dictionary.Add(pair);
            _object.Render();

            Assert.IsTrue(dictionary.Contains(pair));
            Assert.AreEqual(pair.Value, dictionary[pair.Key]);
            Assert.AreEqual(initial.Length + 1, dictionary.Count);

            Assert.IsTrue(stateChanged);
            Assert.AreEqual(pair.Key, addedKey);
            Assert.AreEqual(pair.Value, addedValue);
        }

        [Test]
        public void WhenAddKeyThatAlreadyExistsAfterSpawnedThenException()
        {
            bool stateChanged = false;
            char addedKey = default;
            int addedValue = -1;

            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);
            dictionary.OnStateChanged += () => stateChanged = true;
            dictionary.OnItemAdded += (k, v) =>
            {
                addedKey = k;
                addedValue = v;
            };

            _object.Mock_Spawn();
            var pair = new KeyValuePair<char, int>(initial[1].Key, 6);
            _object.Render();
            Assert.Catch<ArgumentException>(() => { dictionary.Add(pair); });
            Assert.Catch<ArgumentException>(() => { dictionary.Add(pair.Key, pair.Value); });
            pair = new KeyValuePair<char, int>(initial[1].Key, 6);
            Assert.Catch<ArgumentException>(() => { dictionary.Add(pair); });
            Assert.Catch<ArgumentException>(() => { dictionary.Add(pair.Key, pair.Value); });
            Assert.AreEqual(initial.Length, dictionary.Count);

            Assert.IsFalse(stateChanged);
            Assert.AreNotEqual(pair.Key, addedKey);
            Assert.AreEqual(-1, addedValue);
        }

        [Test]
        public void WhenAddPairAndDictionaryFullAfterSpawnedThenThrowsException()
        {
            bool stateChanged = false;
            char addedKey = default;
            int addedValue = -1;

            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 4, eventCapacity: 1, initial);
            dictionary.OnStateChanged += () => stateChanged = true;
            dictionary.OnItemAdded += (k, v) =>
            {
                addedKey = k;
                addedValue = v;
            };

            _object.Mock_Spawn();
            var pair = new KeyValuePair<char, int>('µ', 6);
            _object.Render();
            Assert.Catch<Exception>(() => { dictionary.Add(pair); });
            Assert.Catch<Exception>(() => { dictionary.Add(pair.Key, pair.Value); });
            Assert.AreEqual(initial.Length, dictionary.Count);

            Assert.IsFalse(stateChanged);
            Assert.AreNotEqual(pair.Key, addedKey);
            Assert.AreEqual(-1, addedValue);
        }

        #endregion

        #region Get

        [Test]
        public void GetValueBeforeSpawned()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);

            var key = initial[1].Key;
            bool success = dictionary.TryGetValue(key, out var value);
            Assert.IsTrue(success);
            Assert.AreEqual(initial[1].Value, value);
            Assert.AreEqual(initial[1].Value, dictionary[key]);
            Assert.AreEqual(initial.Length, dictionary.Count);
        }

        [Test]
        public void WhenGetByKeyThatNotExistsBeforeSpawnedThenThrowsException()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);

            var key = 'µ';
            bool success = dictionary.TryGetValue(key, out var value);

            Assert.IsFalse(success);
            Assert.Catch<KeyNotFoundException>(() =>
            {
                var value = dictionary[key];
            });
            Assert.AreEqual(initial.Length, dictionary.Count);
        }

        [Test]
        public void GetValueAfterSpawned()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);
            _object.Mock_Spawn();

            var key = initial[1].Key;
            bool success = dictionary.TryGetValue(key, out var value);
            Assert.IsTrue(success);
            Assert.AreEqual(initial[1].Value, value);
            Assert.AreEqual(initial[1].Value, dictionary[key]);
            Assert.AreEqual(initial.Length, dictionary.Count);
        }

        [Test]
        public void WhenGetByKeyThatNotExistsAfterSpawnedThenThrowsException()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);
            _object.Mock_Spawn();

            var key = 'µ';
            bool success = dictionary.TryGetValue(key, out var value);

            Assert.IsFalse(success);
            Assert.Catch<KeyNotFoundException>(() =>
            {
                var value = dictionary[key];
            });
            Assert.AreEqual(initial.Length, dictionary.Count);
        }

        #endregion

        #region Set

        [Test]
        public void SetValueBeforeSpawned()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);

            var value = 5;
            dictionary[initial[1].Key] = value;
            Assert.AreEqual(value, dictionary[initial[1].Key]);
            ;
            Assert.AreNotEqual(initial[1].Value, dictionary[initial[1].Key]);
            Assert.AreEqual(initial.Length, dictionary.Count);

            var pair = new KeyValuePair<char, int>('µ', 66);
            dictionary[pair.Key] = pair.Value;
            Assert.IsTrue(dictionary.ContainsKey(pair.Key));
            Assert.AreEqual(dictionary[pair.Key], pair.Value);
            Assert.AreEqual(initial.Length + 1, dictionary.Count);
        }

        [Test]
        public void WhenSetValueAndDictionaryFullBeforeSpawnedThenThrowsException()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary =
                new NetworkDictionary<char, int>(_object, capacity: initial.Length, eventCapacity: 1, initial);

            var value = 5;
            dictionary[initial[1].Key] = value;
            Assert.AreEqual(value, dictionary[initial[1].Key]);
            Assert.AreNotEqual(initial[1].Value, dictionary[initial[1].Key]);
            Assert.AreEqual(initial.Length, dictionary.Count);

            var pair = new KeyValuePair<char, int>('µ', 66);
            Assert.Catch<Exception>(() => { dictionary[pair.Key] = pair.Value; });
            Assert.AreEqual(initial.Length, dictionary.Count);
        }

        [Test]
        public void SetValueAfterSpawned()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1),
                new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9),
                new KeyValuePair<char, int>('®', 99)
            };

            var dictionary = new NetworkDictionary<char, int>(
                _object,
                capacity: 16,
                eventCapacity: 1,
                initial
            );

            _object.Mock_Spawn();

            const int value = 5;
            dictionary[initial[1].Key] = value;
            Assert.AreEqual(value, dictionary[initial[1].Key]);
            Assert.AreNotEqual(initial[1].Value, dictionary[initial[1].Key]);
            Assert.AreEqual(initial.Length, dictionary.Count);

            var pair = new KeyValuePair<char, int>('µ', 66);
            dictionary[pair.Key] = pair.Value;
            Assert.IsTrue(dictionary.ContainsKey(pair.Key));
            Assert.AreEqual(dictionary[pair.Key], pair.Value);
            Assert.AreEqual(initial.Length + 1, dictionary.Count);
        }

        [Test]
        public void SetValue_WithSameKey_DictionaryFull_AfterSpawned_ThrowsException()
        {
            //Arrange:
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1),
                new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9),
                new KeyValuePair<char, int>('®', 99)
            };

            var dictionary = new NetworkDictionary<char, int>(
                _object,
                capacity: initial.Length,
                eventCapacity: 1,
                initial
            );
            
            _object.Mock_Spawn();

            //Act:
            const int value = 5;
            char key1 = initial[1].Key;
            dictionary[key1] = value;

            //Assert:
            Assert.AreEqual(value, dictionary[key1]);
            Assert.AreNotEqual(initial[1].Value, dictionary[key1]);
            Assert.AreEqual(initial.Length, dictionary.Count);
        }
        
        [Test]
        public void SetValue_WithAnotherKey_DictionaryFull_AfterSpawned_ThrowsException()
        {
            //Arrange:
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1),
                new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9),
                new KeyValuePair<char, int>('®', 99)
            };

            var dictionary = new NetworkDictionary<char, int>(
                _object,
                capacity: initial.Length,
                eventCapacity: 1,
                initial
            );
            
            _object.Mock_Spawn();

            //Act:
            const int value = 5;
            char key1 = initial[1].Key;
            dictionary[key1] = value;

            //Assert:
            Assert.Throws<Exception>(() => dictionary['µ'] = 66);
            Assert.AreEqual(initial.Length, dictionary.Count);
        }

        [Test]
        public void ChangeValueAfterSpawned()
        {
            bool stateChanged = false;
            char changedKey = default;
            int changedValue = -1;

            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);
            dictionary.OnStateChanged += () => stateChanged = true;
            dictionary.OnItemChanged += (k, v) =>
            {
                changedKey = k;
                changedValue = v;
            };
            _object.Mock_Spawn();
            var value = 77;
            dictionary[initial[2].Key] = value;
            _object.Render();

            Assert.AreEqual(value, dictionary[initial[2].Key]);
            Assert.AreNotEqual(initial[2].Value, dictionary[initial[2].Key]);
            Assert.AreEqual(initial.Length, dictionary.Count);

            Assert.IsTrue(stateChanged);
            Assert.AreEqual(initial[2].Key, changedKey);
            Assert.AreEqual(value, changedValue);
        }

        #endregion

        #region Contain()

        [Test]
        public void ContainsBeforeSpawned()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);

            Assert.IsTrue(dictionary.Contains(initial[1]));
            Assert.IsFalse(dictionary.Contains(new KeyValuePair<char, int>('c', 7)));
            Assert.AreEqual(initial.Length, dictionary.Count);
        }

        [Test]
        public void ContainsAfterSpawned()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);
            _object.Mock_Spawn();

            Assert.IsTrue(dictionary.Contains(initial[1]));
            Assert.IsFalse(dictionary.Contains(new KeyValuePair<char, int>('c', 7)));
            Assert.AreEqual(initial.Length, dictionary.Count);
        }

        [Test]
        public void ContainsKeyBeforeSpawned()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);

            Assert.IsTrue(dictionary.ContainsKey(initial[1].Key));
            Assert.IsFalse(dictionary.ContainsKey('c'));
            Assert.AreEqual(initial.Length, dictionary.Count);
        }

        [Test]
        public void ContainsKeyAfterSpawned()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);
            _object.Mock_Spawn();

            Assert.IsTrue(dictionary.ContainsKey(initial[1].Key));
            Assert.IsFalse(dictionary.ContainsKey('c'));
            Assert.AreEqual(initial.Length, dictionary.Count);
        }

        [Test]
        public void ContainsValueBeforeSpawned()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);

            Assert.IsTrue(dictionary.ContainsValue(initial[1].Value));
            Assert.IsFalse(dictionary.ContainsValue(7));
            Assert.AreEqual(initial.Length, dictionary.Count);
        }

        [Test]
        public void ContainsValueAfterSpawned()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);
            _object.Mock_Spawn();

            Assert.IsTrue(dictionary.ContainsValue(initial[1].Value));
            Assert.IsFalse(dictionary.ContainsValue(7));
            Assert.AreEqual(initial.Length, dictionary.Count);
        }

        #endregion

        #region Remove()

        [Test]
        public void RemoveBeforeSpawned()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);

            var pair = initial[1];
            bool success = dictionary.Remove(pair);

            Assert.IsTrue(success);
            Assert.AreEqual(initial.Length - 1, dictionary.Count);
            Assert.IsFalse(dictionary.Contains(pair));
        }

        [Test]
        public void WhenRemoveAbsentItemBeforeSpawnedThenReturnFalse()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);

            var pair = new KeyValuePair<char, int>('c', 7);
            bool success = dictionary.Remove(pair);

            Assert.IsFalse(success);
            Assert.AreEqual(initial.Length, dictionary.Count);
            Assert.IsFalse(dictionary.Contains(pair));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(3)]
        public void RemoveAfterSpawned(int index)
        {
            bool stateChanged = false;
            char removedKey = default;
            int removedValue = -1;

            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);

            dictionary.OnItemRemoved += (k, v) =>
            {
                removedKey = k;
                removedValue = v;
            };
            dictionary.OnStateChanged += () => stateChanged = true;

            _object.Mock_Spawn();
            var pair = initial[index];
            bool success = dictionary.Remove(pair);
            _object.Render();

            Assert.IsTrue(success);
            Assert.AreEqual(initial.Length - 1, dictionary.Count);
            Assert.IsFalse(dictionary.Contains(pair));

            Assert.IsTrue(stateChanged);
            Assert.AreEqual(pair.Key, removedKey);
            Assert.AreEqual(pair.Value, removedValue);
        }

        [Test]
        public void WhenRemoveAbsentItemAfterSpawnedThenReturnFalse()
        {
            bool stateChanged = false;
            char removedKey = default;
            int removedValue = -1;

            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);

            dictionary.OnItemRemoved += (k, v) =>
            {
                removedKey = k;
                removedValue = v;
            };
            dictionary.OnStateChanged += () => stateChanged = true;

            _object.Mock_Spawn();
            var pair = new KeyValuePair<char, int>('c', 7);
            bool success = dictionary.Remove(pair);
            _object.Render();

            Assert.IsFalse(success);
            Assert.AreEqual(initial.Length, dictionary.Count);
            Assert.IsFalse(dictionary.Contains(pair));

            Assert.IsFalse(stateChanged);
            Assert.AreNotEqual(pair.Key, removedKey);
            Assert.AreEqual(-1, removedValue);
        }

        #endregion

        #region Clear()

        [Test]
        public void ClearBeforeSpawn()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);

            dictionary.Clear();
            Assert.AreEqual(0, dictionary.Count);
            Assert.IsFalse(dictionary.Contains(initial[0]));
            Assert.IsFalse(dictionary.Contains(initial[2]));
        }

        [Test]
        public void ClearAfterSpawn()
        {
            var stateChanged = false;
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);
            dictionary.OnStateChanged += () => stateChanged = true;

            _object.Mock_Spawn();
            dictionary.Clear();
            _object.Render();

            Assert.AreEqual(0, dictionary.Count);
            Assert.IsFalse(dictionary.Contains(initial[0]));
            Assert.IsFalse(dictionary.Contains(initial[2]));

            Assert.IsTrue(stateChanged);
        }

        #endregion

        #region Key Collection

        [Test]
        public void ForEachKeyBeforeSpawn()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);

            Assert.AreEqual(initial.Length, dictionary.Count);
            int index = 0;
            foreach (var key in dictionary.Keys)
            {
                Assert.AreEqual(initial[index].Key, key);
                index++;
            }
        }

        [Test]
        public void ForEachKeyAfterSpawn()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);
            _object.Mock_Spawn();

            Assert.AreEqual(initial.Length, dictionary.Count);
            int index = 0;
            foreach (var key in dictionary.Keys)
            {
                Assert.AreEqual(initial[index].Key, key);
                index++;
            }
        }

        [Test]
        public void ForEachKeyReferenceBetweenSpawn()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);

            Assert.AreEqual(initial.Length, dictionary.Count);
            var keys = dictionary.Keys;
            int index = 0;
            foreach (var key in keys)
            {
                Assert.AreEqual(initial[index].Key, key);
                index++;
            }

            _object.Mock_Spawn();

            index = 0;
            foreach (var key in keys)
            {
                Assert.AreEqual(initial[index].Key, key);
                index++;
            }
        }

        #endregion

        #region Value Collection

        [Test]
        public void ForEachValuesBeforeSpawn()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);

            Assert.AreEqual(initial.Length, dictionary.Count);
            int index = 0;
            foreach (var value in dictionary.Values)
            {
                Assert.AreEqual(initial[index].Value, value);
                index++;
            }
        }

        [Test]
        public void ForEachValueAfterSpawn()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);
            _object.Mock_Spawn();

            Assert.AreEqual(initial.Length, dictionary.Count);
            int index = 0;
            foreach (var value in dictionary.Values)
            {
                Assert.AreEqual(initial[index].Value, value);
                index++;
            }
        }

        [Test]
        public void ForEachValueReferenceBetweenSpawn()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 16, eventCapacity: 1, initial);

            Assert.AreEqual(initial.Length, dictionary.Count);
            var values = dictionary.Values;
            int index = 0;
            foreach (var value in values)
            {
                Assert.AreEqual(initial[index].Value, value);
                index++;
            }

            _object.Mock_Spawn();

            index = 0;
            foreach (var value in values)
            {
                Assert.AreEqual(initial[index].Value, value);
                index++;
            }
        }

        #endregion

        #region Enumerator

        [Test]
        public void CheckPairEnumeratorBeforeSpawn()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 12, eventCapacity: 1, initial);

            var index = 0;
            var enumerator = dictionary.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Assert.AreEqual(initial[index], enumerator.Current);
                index++;
            }

            Assert.AreEqual(index, dictionary.Count);
            enumerator.Dispose();
        }

        [Test]
        public void CheckPairEnumeratorAfterSpawn()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 12, eventCapacity: 1, initial);
            _object.Mock_Spawn();

            var index = 0;
            var enumerator = dictionary.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Assert.AreEqual(initial[index], enumerator.Current);
                index++;
            }

            Assert.AreEqual(index, dictionary.Count);
            enumerator.Dispose();
        }

        [Test]
        public void CheckKeysEnumeratorBeforeSpawn()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 12, eventCapacity: 1, initial);

            var index = 0;
            var enumerator = dictionary.Keys.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Assert.AreEqual(initial[index].Key, enumerator.Current);
                index++;
            }

            Assert.AreEqual(index, dictionary.Count);
            enumerator.Dispose();
        }

        [Test]
        public void CheckKeysEnumeratorAfterSpawn()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 12, eventCapacity: 1, initial);
            _object.Mock_Spawn();

            var index = 0;
            var enumerator = dictionary.Keys.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Assert.AreEqual(initial[index].Key, enumerator.Current);
                index++;
            }

            Assert.AreEqual(index, dictionary.Count);
            enumerator.Dispose();
        }

        [Test]
        public void CheckValuesEnumeratorBeforeSpawn()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 12, eventCapacity: 1, initial);

            var index = 0;
            var enumerator = dictionary.Values.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Assert.AreEqual(initial[index].Value, enumerator.Current);
                index++;
            }

            Assert.AreEqual(index, dictionary.Count);
            enumerator.Dispose();
        }

        [Test]
        public void CheckValuesEnumeratorAfterSpawn()
        {
            var initial = new[]
            {
                new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1),
                new KeyValuePair<char, int>('e', 9), new KeyValuePair<char, int>('®', 99)
            };
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 12, eventCapacity: 1, initial);
            _object.Mock_Spawn();

            var index = 0;
            var enumerator = dictionary.Values.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Assert.AreEqual(initial[index].Value, enumerator.Current);
                index++;
            }

            Assert.AreEqual(index, dictionary.Count);
            enumerator.Dispose();
        }

        #endregion

        #region CopyTo

        [Test]
        public void CheckCopyToBeforeSpawned()
        {
            var initial = new[] {new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1)};
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 2, eventCapacity: 1, initial);

            Assert.AreEqual(initial.Length, dictionary.Count);
            KeyValuePair<char, int>[] copy = new KeyValuePair<char, int>[initial.Length + 1];
            dictionary.CopyTo(copy, 1);
            Assert.AreEqual(initial[0], copy[1]);
            Assert.AreEqual(initial[1], copy[2]);
        }

        [Test]
        public void WhenCopyToNullBeforeSpawnedThenThrowsException()
        {
            var initial = new[] {new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1)};
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 2, eventCapacity: 1, initial);

            Assert.AreEqual(initial.Length, dictionary.Count);
            Assert.Catch<NullReferenceException>(() => { dictionary.CopyTo(null, 1); });
        }

        [Test]
        public void WhenCopyToWrongIndexBeforeSpawnedThenThrowsException()
        {
            var initial = new[] {new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1)};
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 2, eventCapacity: 1, initial);

            Assert.AreEqual(initial.Length, dictionary.Count);
            KeyValuePair<char, int>[] copy = new KeyValuePair<char, int>[initial.Length + 1];
            Assert.Catch<IndexOutOfRangeException>(() => { dictionary.CopyTo(copy, 100); });
        }

        [Test]
        public void WhenCopyToInsufficientSizeBeforeSpawnedThenThrowsException()
        {
            var initial = new[] {new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1)};
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 2, eventCapacity: 1, initial);

            Assert.AreEqual(initial.Length, dictionary.Count);
            KeyValuePair<char, int>[] copy = new KeyValuePair<char, int>[initial.Length + 1];
            Assert.Catch<IndexOutOfRangeException>(() => { dictionary.CopyTo(copy, 100); });
        }

        [Test]
        public void CheckCopyToAfterSpawned()
        {
            var initial = new[] {new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1)};
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 2, eventCapacity: 1, initial);
            _object.Mock_Spawn();

            Assert.AreEqual(initial.Length, dictionary.Count);
            KeyValuePair<char, int>[] copy = new KeyValuePair<char, int>[initial.Length + 1];
            dictionary.CopyTo(copy, 1);
            Assert.AreEqual(initial[0], copy[1]);
            Assert.AreEqual(initial[1], copy[2]);
        }

        [Test]
        public void WhenCopyToNullAfterSpawnedThenThrowsException()
        {
            var initial = new[] {new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1)};
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 2, eventCapacity: 1, initial);
            _object.Mock_Spawn();
            Assert.AreEqual(initial.Length, dictionary.Count);
            Assert.Catch<NullReferenceException>(() => { dictionary.CopyTo(null, 1); });
        }

        [Test]
        public void WhenCopyToWrongIndexAfterSpawnedThenThrowsException()
        {
            var initial = new[] {new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1)};
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 2, eventCapacity: 1, initial);
            _object.Mock_Spawn();
            Assert.AreEqual(initial.Length, dictionary.Count);
            KeyValuePair<char, int>[] copy = new KeyValuePair<char, int>[initial.Length + 1];
            Assert.Catch<IndexOutOfRangeException>(() => { dictionary.CopyTo(copy, 100); });
        }

        [Test]
        public void WhenCopyToInsufficientSizeAfterSpawnedThenThrowsException()
        {
            var initial = new[] {new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1)};
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 2, eventCapacity: 1, initial);
            _object.Mock_Spawn();
            Assert.AreEqual(initial.Length, dictionary.Count);
            KeyValuePair<char, int>[] copy = new KeyValuePair<char, int>[1];
            Assert.Catch<ArgumentException>(() => { dictionary.CopyTo(copy, 0); });
        }

        [Test]
        public void CheckCopyKeysToBeforeSpawn()
        {
            var initial = new[] {new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1)};
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 2, eventCapacity: 1, initial);

            Assert.AreEqual(initial.Length, dictionary.Count);
            char[] copy = new char[initial.Length + 1];
            dictionary.Keys.CopyTo(copy, 1);
            Assert.AreEqual(initial[0].Key, copy[1]);
            Assert.AreEqual(initial[1].Key, copy[2]);
        }

        [Test]
        public void WhenCopyKeysToNullBeforeSpawnedThenThrowsException()
        {
            var initial = new[] {new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1)};
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 2, eventCapacity: 1, initial);

            Assert.AreEqual(initial.Length, dictionary.Count);
            Assert.Catch<ArgumentNullException>(() => { dictionary.Keys.CopyTo(null, 1); });
        }

        [Test]
        public void WhenCopyKeysToWrongIndexBeforeSpawnedThenThrowsException()
        {
            var initial = new[] {new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1)};
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 2, eventCapacity: 1, initial);

            Assert.AreEqual(initial.Length, dictionary.Count);
            char[] copy = new char[initial.Length + 1];
            Assert.Catch<ArgumentOutOfRangeException>(() => { dictionary.Keys.CopyTo(copy, 100); });
        }

        [Test]
        public void WhenCopyKeysToInsufficientSizeBeforeSpawnedThenThrowsException()
        {
            var initial = new[] {new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1)};
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 2, eventCapacity: 1, initial);

            Assert.AreEqual(initial.Length, dictionary.Count);
            char[] copy = new char[1];
            Assert.Catch<ArgumentException>(() => { dictionary.Keys.CopyTo(copy, 0); });
        }

        [Test]
        public void CheckCopyKeysToAfterSpawn()
        {
            var initial = new[] {new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1)};
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 2, eventCapacity: 1, initial);
            _object.Mock_Spawn();

            Assert.AreEqual(initial.Length, dictionary.Count);
            char[] copy = new char[initial.Length + 1];
            dictionary.Keys.CopyTo(copy, 1);
            Assert.AreEqual(initial[0].Key, copy[1]);
            Assert.AreEqual(initial[1].Key, copy[2]);
        }

        [Test]
        public void WhenCopyKeysToNullAfterSpawnedThenThrowsException()
        {
            var initial = new[] {new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1)};
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 2, eventCapacity: 1, initial);
            _object.Mock_Spawn();

            Assert.AreEqual(initial.Length, dictionary.Count);
            Assert.Catch<ArgumentNullException>(() => { dictionary.Keys.CopyTo(null, 1); });
        }

        [Test]
        public void WhenCopyKeysToWrongIndexAfterSpawnedThenThrowsException()
        {
            var initial = new[] {new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1)};
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 2, eventCapacity: 1, initial);
            _object.Mock_Spawn();

            Assert.AreEqual(initial.Length, dictionary.Count);
            char[] copy = new char[initial.Length + 1];
            Assert.Catch<ArgumentOutOfRangeException>(() => { dictionary.Keys.CopyTo(copy, 100); });
        }

        [Test]
        public void WhenCopyKeysToInsufficientSizeAfterSpawnedThenThrowsException()
        {
            var initial = new[] {new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1)};
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 2, eventCapacity: 1, initial);
            _object.Mock_Spawn();

            Assert.AreEqual(initial.Length, dictionary.Count);
            char[] copy = new char[1];
            Assert.Catch<ArgumentException>(() => { dictionary.Keys.CopyTo(copy, 0); });
        }

        [Test]
        public void CheckCopyValuesToBeforeSpawn()
        {
            var initial = new[] {new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1)};
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 2, eventCapacity: 1, initial);

            Assert.AreEqual(initial.Length, dictionary.Count);
            int[] copy = new int[initial.Length + 1];
            dictionary.Values.CopyTo(copy, 1);
            Assert.AreEqual(initial[0].Value, copy[1]);
            Assert.AreEqual(initial[1].Value, copy[2]);
        }

        [Test]
        public void WhenCopyValuesToNullBeforeSpawnedThenThrowsException()
        {
            var initial = new[] {new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1)};
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 2, eventCapacity: 1, initial);

            Assert.AreEqual(initial.Length, dictionary.Count);
            Assert.Catch<ArgumentNullException>(() => { dictionary.Values.CopyTo(null, 1); });
        }

        [Test]
        public void WhenCopyValuesToWrongIndexBeforeSpawnedThenThrowsException()
        {
            var initial = new[] {new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1)};
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 2, eventCapacity: 1, initial);

            Assert.AreEqual(initial.Length, dictionary.Count);
            int[] copy = new int[initial.Length + 1];
            Assert.Catch<ArgumentOutOfRangeException>(() => { dictionary.Values.CopyTo(copy, 100); });
        }

        [Test]
        public void WhenCopyValuesToInsufficientSizeBeforeSpawnedThenThrowsException()
        {
            var initial = new[] {new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1)};
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 2, eventCapacity: 1, initial);

            Assert.AreEqual(initial.Length, dictionary.Count);
            int[] copy = new int[1];
            Assert.Catch<ArgumentException>(() => { dictionary.Values.CopyTo(copy, 0); });
        }

        [Test]
        public void CheckCopyValuesToAfterSpawn()
        {
            var initial = new[] {new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1)};
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 2, eventCapacity: 1, initial);
            _object.Mock_Spawn();

            Assert.AreEqual(initial.Length, dictionary.Count);
            int[] copy = new int[initial.Length + 1];
            dictionary.Values.CopyTo(copy, 1);
            Assert.AreEqual(initial[0].Value, copy[1]);
            Assert.AreEqual(initial[1].Value, copy[2]);
        }

        [Test]
        public void WhenCopyValuesToNullAfterSpawnedThenThrowsException()
        {
            var initial = new[] {new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1)};
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 2, eventCapacity: 1, initial);
            _object.Mock_Spawn();

            Assert.AreEqual(initial.Length, dictionary.Count);
            Assert.Catch<ArgumentNullException>(() => { dictionary.Values.CopyTo(null, 1); });
        }

        [Test]
        public void WhenCopyValuesToWrongIndexAfterSpawnedThenThrowsException()
        {
            var initial = new[] {new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1)};
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 2, eventCapacity: 1, initial);
            _object.Mock_Spawn();

            Assert.AreEqual(initial.Length, dictionary.Count);
            int[] copy = new int[initial.Length + 1];
            Assert.Catch<ArgumentOutOfRangeException>(() => { dictionary.Values.CopyTo(copy, 100); });
        }

        [Test]
        public void WhenCopyValuesToInsufficientSizeAfterSpawnedThenThrowsException()
        {
            var initial = new[] {new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1)};
            var dictionary = new NetworkDictionary<char, int>(_object, capacity: 2, eventCapacity: 1, initial);
            _object.Mock_Spawn();

            Assert.AreEqual(initial.Length, dictionary.Count);
            int[] copy = new int[1];
            Assert.Catch<ArgumentException>(() => { dictionary.Values.CopyTo(copy, 0); });
        }

        //[Test]
        //public void CheckCopyToArray()
        //{
        //    var initial = new[] { new KeyValuePair<char, int>('a', 1), new KeyValuePair<char, int>('b', -1) };
        //    var dictionary = new NetworkDictionary<char, int>(_agent, capacity: 2, eventCapacity: 1, initial);
        //    _agent.Mock_Spawn();
        //    
        //    Assert.AreEqual(initial.Length, dictionary.Count);
        //    object[] copy = new object[initial.Length+1];
        //    dictionary.CopyTo(copy, 1);
        //    Assert.AreEqual(initial[0], copy[1]);
        //    Assert.AreEqual(initial[1], copy[2]);
        //}

        #endregion
    }
}