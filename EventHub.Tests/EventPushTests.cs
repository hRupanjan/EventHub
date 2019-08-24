using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHubProject.Tests
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
            Assert.DoesNotThrow(() => { var g = (ObjectClass)d.obj; }, "SuperClass object can't be casted to SubClass");
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
        [Test, Description("A partial class distributed events should be called")]
        public void PartialClassEventSubscriberShouldRecieveEvent()
        {
            var p = new P();
            EventHub.Instance.Register(p);
            int testData = 1;
            var obj = new ObjectClass { SubData = testData };
            EventHub.Instance.Post(obj);
            Task.Delay(100).Wait();
            Assert.That(p.MethodHit, Is.EqualTo(true), $"'{obj.GetType().FullName}' subscriber wasn't hit in '{p.GetType().FullName}'");
            Assert.That(p.obj, Is.EqualTo(obj), $"Data wasn't changed in '{p.GetType().FullName}'");
        }
        [Test, Description("Load Test"), Retry(10)]
        public void EventPushLoadTest()
        {
            var m = new M();
            EventHub.Instance.Register(m);
            int i;
            int length = 1000;
            for (i = 1; i <= length; i++)
            {
                EventHub.Instance.Post(i);
            }
            int delay = 1;
            Task.Delay(delay).Wait();
            double eff = ((float)m.Number / (length)) * 100;
            Assert.That(m.Number, Is.EqualTo(length), $"Value should be {length} in '{m.GetType().FullName}'. Eff: {eff}% in {delay} milli-second");
        }
        [Test, Description("Efficiency Test"), Retry(5)]
        public void EventPushAverageEfficiencyTest()
        {
            var m = new M();
            EventHub.Instance.Register(m);
            int i;
            int length = 1000;
            var lst = new List<double>();
            for (int j = 0; j < 200; j++)
            {
                for (i = 1; i <= length; i++)
                {
                    EventHub.Instance.Post(i);
                }
                int delay = 1;
                Task.Delay(delay).Wait();
                double eff = ((float)m.Number / (length)) * 100;
                lst.Add(eff);
            }
            var avg = lst.Average();
            var maxEff = 60.0;
            Assert.That(avg, Is.GreaterThan(maxEff), $"Average Efficiency should be greater than {maxEff}%");
        }
    }
}
