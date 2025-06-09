using System;
using NUnit.Framework;

namespace Atomic.Networking.Elements
{
    public sealed class NetworkTimestampTests
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
        public void Constructor_InitializesCorrectly()
        {
            Assert.NotNull(new NetworkTimestamp(_object));
        }

        [Test]
        public void StartFromSeconds_AfterSpawn_SetsEndTickCorrectly()
        {
            //Arrange:
            _facade.IsActive = true;
            _facade.Tick = 100;
            _facade.DeltaTime = 0.1f;

            var timestamp = new NetworkTimestamp(_object);
            _object.Mock_Spawn();

            //Act:
            timestamp.StartFromSeconds(0.5f);

            //Assert:
            Assert.AreEqual(105, timestamp.EndTick);
        }

        [Test]
        public void StartFromTicks_AfterSpawn_SetsEndTickCorrectly()
        {
            //Arrange:
            _facade.IsActive = true;
            _facade.Tick = 50;

            var timestamp = new NetworkTimestamp(_object);
            _object.Mock_Spawn();

            //Act:
            timestamp.StartFromTicks(10);

            //Assert:
            Assert.AreEqual(60, timestamp.EndTick);
        }

        [Test]
        public void Stop_SetsEndTickToMinusOne()
        {
            //Arrange:
            _facade.IsActive = true;
            _facade.Tick = 50;

            var timestamp = new NetworkTimestamp(_object);
            _object.Mock_Spawn();

            timestamp.StartFromTicks(100);

            //Act:
            timestamp.Stop();

            //Assert:
            Assert.That(timestamp.EndTick, Is.EqualTo(-1));
        }
        
        [Test]
        public void StartFromSeconds_ThrowsOnNegative()
        {
            var timestamp = new NetworkTimestamp(_object);
            Assert.Throws<ArgumentOutOfRangeException>(() => timestamp.StartFromSeconds(-1f));
        }

        [Test]
        public void StartFromTicks_ThrowsOnNegative()
        {
            var timestamp = new NetworkTimestamp(_object);
            Assert.Throws<ArgumentOutOfRangeException>(() => timestamp.StartFromTicks(-5));
        }

        [Test]
        public void IsIdle_ReturnsTrueIfInactive()
        {
            var timestamp = new NetworkTimestamp(_object);
            Assert.IsTrue(timestamp.IsIdle());
        }

        [Test]
        public void IsPlaying_ReturnsTrueWhenTickIsInFuture()
        {
            //Arrange:
            _facade.IsActive = true;
            _facade.Tick = 10;

            var timestamp = new NetworkTimestamp(_object);
            _object.Mock_Spawn();

            //Act:
            timestamp.StartFromTicks(20);

            //Assert:
            Assert.IsTrue(timestamp.IsPlaying());
        }

        [Test]
        public void IsExpired_ReturnsTrueWhenTickIsInPast()
        {
            //Arrange:
            _facade.IsActive = true;
            _facade.Tick = 20;

            var timestamp = new NetworkTimestamp(_object);
            _object.Mock_Spawn();

            //Act:
            timestamp.StartFromTicks(10);
            _facade.Tick = 100;

            //Assert:
            Assert.IsTrue(timestamp.IsExpired());
        }

        [Test]
        public void GetProgress_ReturnsCorrectRatio()
        {
            //Arrange:
            _facade.IsActive = true;
            _facade.Tick = 0;
            _facade.DeltaTime = 1f;

            var timestamp = new NetworkTimestamp(_object);
            _object.Mock_Spawn();

            //Act:
            timestamp.StartFromSeconds(8);

            //Assert:
            float progress = timestamp.GetProgress(duration: 20f);
            Assert.That(progress, Is.EqualTo(0.6f).Within(0.001f));
        }

        [Test]
        public void RemainingTicks_ReturnsCorrectValue()
        {
            //Arrange:
            _facade.IsActive = true;
            _facade.Tick = 5;

            var timestamp = new NetworkTimestamp(_object);
            
            _object.Mock_Spawn();

            //Act:
            timestamp.StartFromTicks(4);
            _facade.Tick = 8;

            int remaining = timestamp.RemainingTicks;
            Assert.That(remaining, Is.EqualTo(1));
        }

        [Test]
        public void RemainingTime_ReturnsCorrectValue()
        {
            _facade.IsActive = true;
            _facade.Tick = 5;
            _facade.DeltaTime = 0.2f;

            var timestamp = new NetworkTimestamp(_object);
            _object.Mock_Spawn();

            //Act:
            timestamp.StartFromTicks(10);

            //Assert:
            float remainingTime = timestamp.RemainingTime;
            Assert.That(remainingTime, Is.EqualTo(2.0f).Within(0.001f));
        }
    }
}