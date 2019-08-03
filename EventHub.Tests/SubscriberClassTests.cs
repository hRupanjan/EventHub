// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation
using NUnit.Framework;
using System.Threading.Tasks;

namespace EventHub.Tests
{
    [TestFixture]
    public class SubscriberClassTests
    {
        private A a;
        private B b;

        [SetUp]
        public void Initialize()
        {
            a = new A();
            EventHub.Instance.Register(a);
            b = new B();
            EventHub.Instance.Register(b);
        }
        [TearDown]
        public void TearDown()
        {
            EventHub.Instance.Deregister(a);
            EventHub.Instance.Deregister(b);
        }
        [Test, Description("When both sub-class and super-class is registered to the same event, make sure the subclass subscriber is called, not the super")]
        public void SubClassSubscriberIsCalledOnEventPost()
        {
            int testData = 1;
            var obj = new ObjectClass { SubData = testData };
            EventHub.Instance.Post(obj);
            Task.Delay(100).Wait();
            Assert.That(a.SubMethodHit, Is.EqualTo(true), $"'{obj.GetType().FullName}' subscriber wasn't hit in '{a.GetType().FullName}'");
            Assert.That(a.obj, Is.EqualTo(obj), $"Data wasn't changed in '{a.GetType().FullName}'");
            Assert.That(a.obj.SubData, Is.EqualTo(testData), $"Data property wasn't changed in '{a.GetType().FullName}'");

            Assert.That(a.SupMethodHit, Is.Not.EqualTo(true), $"'{obj.GetType().FullName}' subscriber in super class was hit in '{a.GetType().FullName}'");
            Assert.That(a.supObj, Is.Not.EqualTo(obj), $"Data in super class was changed in '{a.GetType().FullName}'");
            Assert.That(a.supObj.SubData, Is.Not.EqualTo(testData), $"Data property in super class was changed in '{a.GetType().FullName}'");
        }
        [Test, Description("When a single uninherited class is registered to an event, make sure the subscriber is called")]
        public void SuperClassSubscriberIsCalledOnEventPost()
        {
            int testData = 1;
            var obj = new ObjectClass { SubData = testData };
            EventHub.Instance.Post(obj);
            Task.Delay(100).Wait();
            Assert.That(b.SupMethodHit, Is.EqualTo(true), $"'{obj.GetType().FullName}' subscriber wasn't hit in '{b.GetType().FullName}'");
            Assert.That(b.supObj, Is.EqualTo(obj), $"Data wasn't changed in '{b.GetType().FullName}'");
            Assert.That(b.supObj.SubData, Is.EqualTo(testData), $"Data property wasn't changed in '{b.GetType().FullName}'");
        }
        [Test, Description("When multiple classes are registered to an event, make sure all are called")]
        public void MultipleClassSubscribersAreCalledOnEventPost()
        {
            int testData = 1;
            var obj = new ObjectClass { SubData = testData };
            EventHub.Instance.Post(obj);
            Task.Delay(100).Wait();
            Assert.That(b.SupMethodHit, Is.EqualTo(true), $"'{obj.GetType().FullName}' subscriber wasn't hit in '{b.GetType().FullName}'");
            Assert.That(b.supObj, Is.EqualTo(obj), $"Data wasn't changed in '{b.GetType().FullName}'");
            Assert.That(b.supObj.SubData, Is.EqualTo(testData), $"Data property wasn't changed in '{b.GetType().FullName}'");

            Assert.That(a.SubMethodHit, Is.EqualTo(true), $"'{obj.GetType().FullName}' subscriber wasn't hit in '{a.GetType().FullName}'");
            Assert.That(a.obj, Is.EqualTo(obj), $"Data wasn't changed in '{a.GetType().FullName}'");
            Assert.That(a.obj.SubData, Is.EqualTo(testData), $"Data property wasn't changed in '{a.GetType().FullName}'");
        }
        [Test, Description("When multiple classes are registered to an event, any deregistered class subscriber should not be called")]
        public void DeregisteredClassSubscribersShouldNotBeCalledOnEventPost()
        {
            EventHub.Instance.Deregister(b);
            int testData = 1;
            var obj = new ObjectClass { SubData = testData };
            EventHub.Instance.Post(obj);
            Task.Delay(100).Wait();
            Assert.That(a.SubMethodHit, Is.EqualTo(true), $"'{obj.GetType().FullName}' subscriber wasn't hit in '{a.GetType().FullName}'");

            Assert.That(b.SupMethodHit, Is.Not.EqualTo(true), $"'{obj.GetType().FullName}' subscriber was hit in '{b.GetType().FullName}'");
        }

        [Test, Description("When multiple instances of the same class are registered to an event, all the instances must be called")]
        public void SeperateInstancesOfTheRegisteredClassShouldBeCalledOnEventPost()
        {
            A newA = new A();
            EventHub.Instance.Register(newA);
            int testData = 1;
            var obj = new ObjectClass { SubData = testData };
            EventHub.Instance.Post(obj);
            Task.Delay(100).Wait();
            Assert.That(a.SubMethodHit, Is.EqualTo(true), $"'{obj.GetType().FullName}' subscriber wasn't hit in 'First instance'");

            Assert.That(newA.SubMethodHit, Is.EqualTo(true), $"'{obj.GetType().FullName}' subscriber wasn't hit in 'Second instance'");
            EventHub.Instance.Deregister(newA);
        }
        [Test, Description("A cancelled event must not be called")]
        public void CancelledEventsMustNotBeCalled()
        {
            C c = new C();
            EventHub.Instance.Deregister(a);
            EventHub.Instance.Deregister(b);
            EventHub.Instance.Register(c);
            int testData = 1;
            var obj = new ObjectClass { SubData = testData };
            EventHub.Instance.Post(obj);
            Task.Delay(100).Wait();
            EventHub.Instance.Cancel(obj);
            Assert.That(c.SubMethodHit, Is.Not.EqualTo(true), $"'{obj.GetType().FullName}' subscriber was hit in '{c.GetType().FullName}'");
            EventHub.Instance.Deregister(c);
        }
        [Test, Description("Method inheritence should work properly with registered classes (new / override)")]
        public void SubscriberInheritenceTest()
        {
            B v = new V();
            int testData = 1;
            var obj = new ObjectClass { SubData = testData };
            var supObj = new SuperClass { SuperData = testData };
            EventHub.Instance.Deregister(a);
            EventHub.Instance.Deregister(b);
            EventHub.Instance.Register(v);
            EventHub.Instance.Post(obj);
            EventHub.Instance.Post(supObj);
            Task.Delay(100).Wait();
            Assert.That(((V)v).NewMethodHit, Is.EqualTo(true), $"'{obj.GetType().FullName}' subscriber wasn't hit in '{v.GetType().FullName}'");
            Assert.That(((V)v).VirtualMethodHit, Is.EqualTo(true), $"'{supObj.GetType().FullName}' subscriber wasn't hit in '{v.GetType().FullName}'");

            Assert.That(((V)v).SupMethodHit, Is.Not.EqualTo(true), $"'{obj.GetType().FullName}' subscriber was hit in '{typeof(B).FullName}'");
            Assert.That(((V)v).UlMethodHit, Is.Not.EqualTo(true), $"'{supObj.GetType().FullName}' subscriber was hit in '{typeof(B).FullName}'");
            EventHub.Instance.Deregister(v);
        }
        [Test, Description("A subscriber with multpile arguments throws error")]
        public void MultipleParameterSubscriberRegistrationThrowsError()
        {
            T2 t = new T2();
            Assert.Throws(typeof(EventHubException), () => { EventHub.Instance.Register(t); }, $"{t.GetType().FullName} didn't throw the expected Exception");
        }
        [Test, Description("A subscriber with no arguments throws error")]
        public void NoParameterSubscriberRegistrationThrowsError()
        {
            T3 t = new T3();
            Assert.Throws(typeof(EventHubException), () => { EventHub.Instance.Register(t); }, $"{t.GetType().FullName} didn't throw the expected Exception");
        }
        [Test, Description("A subscriber class with multiple subscribers with same event type throws error")]
        public void SameClassSameEventRegistrationThrowsError()
        {
            T1 t = new T1();
            Assert.Throws(typeof(EventHubException), () => { EventHub.Instance.Register(t); }, $"{t.GetType().FullName} didn't throw the expected Exception");
        }
    }
}
