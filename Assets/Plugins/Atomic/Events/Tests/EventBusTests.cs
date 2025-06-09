using System;
using NUnit.Framework;
using UnityEngine;

namespace Atomic.Events
{
    public sealed class EventBusTests
    {
        [Test]
        public void Constructor()
        {
            Assert.DoesNotThrow(() =>
            {
                var unused = new EventBus();
            });
        }

        [Test]
        public void DeclareEvent()
        {
            //Arrange:
            var eventBus = new EventBus();
            
            //Act:
            eventBus.Def(1);
            
            //Assert:
            Assert.IsTrue(eventBus.IsDefined(1));
        }

        [Test]
        public void Subscribe()
        {
            //Arrange:
            var eventBus = new EventBus();
            eventBus.Def(1);
            
            //Act:
            eventBus.Subscribe(1, () => {});
        }

        [Test]
        public void WhenSubscribeOnDeclaredEventThenException()
        {
            //Arrange:
            var eventBus = new EventBus();
            
            //Act:
            Assert.Throws<Exception>(() => eventBus.Subscribe(1, () => {}));
        }
        
        [Test]
        public void InvokeEvent()
        {
            //Arrange:
            bool wasEvent = false;
            
            var eventBus = new EventBus();
            eventBus.Def(1);
            eventBus.Subscribe(1, () => wasEvent = true);
            
            //Act:
            eventBus.Invoke(1);
            
            //Assert:
            Assert.IsTrue(wasEvent);
        }
        
        // eventBus.Undeclare("Hello");

        // public void InvokeEventWithArgs2()
        // {
        //     var eventBus = new EventBus();
        //     eventBus.Def<GameObject>("Attack");
        //     
        //     
        //     var args = new object();
        //     var wasEvent = false;
        //     object wasTarget = null;
        //     
        //     eventBus.Def<object>("Hello");
        //     eventBus.Subscribe<object>("Hello", t =>
        //     {
        //         wasTarget = t;
        //         wasEvent = true;
        //     });
        //     eventBus.Invoke("Hello", args);
        //     
        //     Assert.IsTrue(wasEvent);
        //     Assert.AreEqual(args, wasTarget)
        //
        // }
        
        [Test]
        public void InvokeEventWithArgs()
        {
            var args = new object();
            var wasEvent = false;
            object wasTarget = null;
            
            var eventBus = new EventBus();
            eventBus.Def<object>("Hello");
            eventBus.Subscribe<object>("Hello", t =>
            {
                wasTarget = t;
                wasEvent = true;
            });
            eventBus.Invoke("Hello", args);
            
            Assert.IsTrue(wasEvent);
            Assert.AreEqual(args, wasTarget);
        }
    }
}