using System.Threading.Tasks;

namespace EventHubProject.Tests
{
    internal class A : B
    {
        public ObjectClass obj = new ObjectClass { };
        public bool SubMethodHit = false;
        [EventSubscriber]
        public override void Subscriber(ObjectClass objectClass)
        {
            obj = objectClass;
            SubMethodHit = true;
        }
    }

    internal class B
    {
        public ObjectClass supObj = new ObjectClass { };
        public bool SupMethodHit = false;

        public SuperClass ulObj = new SuperClass { };
        public bool UlMethodHit = false;
        [EventSubscriber]
        public virtual void Subscriber(ObjectClass objectClass)
        {
            supObj = objectClass;
            SupMethodHit = true;
        }
        [EventSubscriber]
        public virtual void Subscriber(SuperClass objectClass)
        {
            ulObj = objectClass;
            UlMethodHit = true;
        }
    }

    internal class C : B
    {
        public ObjectClass obj = new ObjectClass { };
        public bool SubMethodHit = false;
        [EventSubscriber]
        public new void Subscriber(ObjectClass objectClass)
        {
            Task.Delay(1500).Wait();
            obj = objectClass;
            SubMethodHit = true;
        }
    }

    internal class D
    {
        public SuperClass obj = new SuperClass { };
        public bool MethodHit = false;
        [EventSubscriber]
        public void Subscriber(SuperClass objectClass)
        {
            obj = objectClass;
            MethodHit = true;
        }
    }

    internal class V : B
    {
        public ObjectClass objNew = new ObjectClass { };
        public bool NewMethodHit = false;

        public SuperClass objVirtual = new SuperClass { };
        public bool VirtualMethodHit = false;
        [EventSubscriber]
        public new void Subscriber(ObjectClass objectClass)
        {
            objNew = objectClass;
            NewMethodHit = true;
        }
        [EventSubscriber]
        public override void Subscriber(SuperClass objectClass)
        {
            objVirtual = objectClass;
            VirtualMethodHit = true;
        }
    }

    internal class T1
    {
        
        [EventSubscriber]
        public void Subscriber(ObjectClass objectClass)
        {
            
        }
        [EventSubscriber]
        public void Subscriber1(ObjectClass objectClass)
        {
            
        }
    }

    internal class T2
    {

        [EventSubscriber]
        public void Subscriber(ObjectClass objectClass, string str)
        {

        }
    }

    internal class T3
    {

        [EventSubscriber]
        public void Subscriber()
        {

        }
    }
}
