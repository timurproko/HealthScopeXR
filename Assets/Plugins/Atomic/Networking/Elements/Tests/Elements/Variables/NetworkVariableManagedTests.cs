using System;
using NUnit.Framework;

namespace Atomic.Networking.Elements
{
    internal sealed class NetworkVariableManagedTests
    {
        private readonly MockEntitySerializer _serializer = new();
        private MockNetworkObject _object;

        [SetUp]
        public void SetUp()
        {
            _object = new MockNetworkObject();
        }

        [TearDown]
        public void TearDown()
        {
            _object.Mock_Despawn();
        }

        [Test]
        public void Constructor()
        {
            //Arrange:
            MockEntity obj = new MockEntity();
            var instance = new NetworkVariableManaged<MockEntity, int>(_object, _serializer, obj);

            //Assert:
            Assert.AreEqual(obj, instance.Value);
        }

        [Test]
        public void Constructor_AgentIsNull_ThrowsException()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                MockEntity obj = new MockEntity();
                var _ = new NetworkVariableManaged<MockEntity, int>(null, _serializer, obj);
            });
        }

        [Test]
        public void Constructor_SerializerIsNull_ThrowsException()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                MockEntity obj = new MockEntity();
                var _ = new NetworkVariableManaged<MockEntity, int>(_object, null, obj);
            });
        }


        [Test]
        public void SetValue_BeforeSpawned_Success()
        {
            //Arrange:
            MockEntity obj1 = new MockEntity();
            MockEntity obj2 = new MockEntity();
            var instance = new NetworkVariableManaged<MockEntity, int>(_object, _serializer, obj1);

            //Act:
            instance.Value = obj2;

            //Assert:
            Assert.AreEqual(obj2, instance.Value);
        }

        [Test]
        public void SetValue_AfterSpawned_Success()
        {
            //Arrange:
            MockEntity obj1 = new MockEntity();
            MockEntity obj2 = new MockEntity();
            var instance = new NetworkVariableManaged<MockEntity, int>(_object, _serializer, obj1);

            MockEntity newValue = null;
            instance.OnValueChanged += v => newValue = v;

            //Act:
            _object.Mock_Spawn();
            instance.Value = obj2;
            _object.Render();

            //Assert:
            Assert.AreNotEqual(obj1, instance.Value);
            Assert.AreEqual(obj2, instance.Value);
            Assert.AreEqual(obj2, newValue);
        }

        [Test]
        public void SetValue_OnSame_AfterSpawned_OnValueChangedNotInvoked()
        {
            //Arrange:
            MockEntity obj1 = new MockEntity();
            var instance = new NetworkVariableManaged<MockEntity, int>(_object, _serializer, obj1);

            MockEntity newValue = null;
            instance.OnValueChanged += v => newValue = v;

            //Act:
            _object.Mock_Spawn();
            instance.Value = obj1;
            _object.Render();

            //Assert:
            Assert.AreEqual(obj1, instance.Value);
            Assert.IsNull(newValue);
        }

        [Test]
        public void UnsubscribeAll_And_CheckCorrectValue()
        {
            //Arrange:
            MockEntity obj1 = new MockEntity();
            MockEntity obj2 = new MockEntity();
            var instance = new NetworkVariableManaged<MockEntity, int>(_object, _serializer, obj1);

            MockEntity newValue = null;
            instance.OnValueChanged += v => newValue = v;

            //Act:
            _object.Mock_Spawn();
            instance.UnsubscribeAll();
            instance.Value = obj2;
            _object.Render();

            //Assert:
            Assert.AreEqual(obj2, instance.Value);
            Assert.IsNull(newValue);
        }
    }
}