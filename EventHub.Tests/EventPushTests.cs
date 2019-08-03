using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Tests
{
    [TestFixture]
    public class EventPushTests
    {
        private D d;
        private A a;

        [SetUp]
        public void Initialize()
        {
            d = new D();
            EventHub.Instance.Register(d);
            a = new A();
            EventHub.Instance.Register(a);
        }
        [TearDown]
        public void TearDown()
        {
            EventHub.Instance.Deregister(d);
            EventHub.Instance.Deregister(a);
        }
        [Test, Description("When a sub class event is posted, a super class event subscriber must recieve it")]
        public void SuperClassEventSubscriberCanRecieveSubClassEvent()
        {
            int testData = 1;
            var obj = new ObjectClass { SubData = testData };
            EventHub.Instance.Post(obj);
            Task.Delay(100).Wait();
            Assert.That(d.MethodHit, Is.EqualTo(true), $"'{obj.GetType().FullName}' subscriber wasn't hit in '{d.GetType().FullName}'");
            Assert.That(d.obj, Is.EqualTo(obj), $"Data wasn't changed in '{d.GetType().FullName}'");
            Assert.DoesNotThrow(()=> { var g = (ObjectClass)d.obj; }, "SuperClass object can't be casted to SubClass");
        }
        [Test, Description("When a super class event is posted, a sub class event subscriber must not recieve it")]
        public void SubClassEventSubscriberCanNotRecieveSuperClassEvent()
        {
            int testData = 1;
            var obj = new SuperClass { SuperData = testData };
            EventHub.Instance.Post(obj);
            Task.Delay(100).Wait();
            Assert.That(d.MethodHit, Is.EqualTo(true), $"'{obj.GetType().FullName}' subscriber wasn't hit in '{d.GetType().FullName}'");
            Assert.That(d.obj, Is.EqualTo(obj), $"Data wasn't changed in '{d.GetType().FullName}'");

            Assert.That(a.SubMethodHit, Is.Not.EqualTo(true), $"'{obj.GetType().FullName}' subscriber was hit in '{a.GetType().FullName}'");
            Assert.That(a.obj, Is.Not.EqualTo(obj), $"Data was changed in '{a.GetType().FullName}'");
        }
    }
}
