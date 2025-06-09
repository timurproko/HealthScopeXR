using System;
using NUnit.Framework;

namespace Atomic.Networking.Elements
{
    internal sealed class NetworkVariableTests
    {
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
            NetworkVariable<int> instance = new NetworkVariable<int>(_object, 5);

            //Assert:
            Assert.AreEqual(5, instance.Value);
        }

        [Test]
        public void Constructor_AgentIsNull_ThrowsException()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                NetworkVariable<int> _ = new NetworkVariable<int>(null, 5);
            });
        }

        [Test]
        public void Constructor_DefaultValue()
        {
            //Arrange:
            NetworkVariable<int> instance = new NetworkVariable<int>(_object);

            //Assert:
            Assert.AreEqual(0, instance.Value);   
        }

        [Test]
        public void Constructor_And_Spawn()
        {
            //Arrange:
            NetworkVariable<int> instance = new NetworkVariable<int>(_object, 5);

            _object.Mock_Spawn();

            //Assert:
            Assert.AreEqual(5, instance.Value);
        }

        [Test]
        public void SetValue_BeforeSpawn_Success()
        {   
            //Arrange:
            NetworkVariable<int> instance = new NetworkVariable<int>(_object, 5);
            
            //Act:
            instance.Value = 3;
            
            //Assert:
            Assert.AreEqual(3, instance.Value);
        }

        [Test]
        public void SetValue_AfterSpawn_Success()
        {
            //Arrange:
            int newValue = -1;
            NetworkVariable<int> instance = new NetworkVariable<int>(_object, 5);
            instance.OnValueChanged += v => newValue = v; 
            
            //Act:
            _object.Mock_Spawn();
            instance.Value = 3;
            _object.Render();
            
            //Assert:
            Assert.AreEqual(3, instance.Value);
            Assert.AreEqual(3, newValue);
        }

        [Test]
        public void UnsubscribeAll_And_CheckCorrectValue()
        {
            //Arrange:
            int newValue = -1;
            NetworkVariable<int> instance = new NetworkVariable<int>(_object, 5);
            instance.OnValueChanged += v => newValue = v; 
            
            //Act:
            _object.Mock_Spawn();
            instance.UnsubscribeAll();
            instance.Value = 3;
            _object.Render();
            
            //Assert:
            Assert.AreEqual(3, instance.Value);
            Assert.AreEqual(-1, newValue);
        }

        [Test]
        public void SetValue_OnSame_AfterSpawned_OnValueChangedNotInvoked()
        {
            //Arrange:
            int newValue = -1;
            NetworkVariable<int> instance = new NetworkVariable<int>(_object, 5);
            instance.OnValueChanged += v => newValue = v; 
            
            //Act:
            _object.Mock_Spawn();
            instance.Value = 5;
            _object.Render();
            
            //Assert:
            Assert.AreEqual(5, instance.Value);
            Assert.AreEqual(-1, newValue);
        }
    }
}