using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace EventHubProject
{
    class RealHub : EventHub
    {
        private static ConcurrentDictionary<string, Dictionary<string, CalleeMethod>> subscriberCallDictionary;
        private static ConcurrentDictionary<string, List<WeakReference<object>>> registeredClasses;
        private static ConcurrentDictionary<Guid, TaskTuple> asyncTasks;

        public RealHub()
        {
            subscriberCallDictionary = new ConcurrentDictionary<string, Dictionary<string, CalleeMethod>>();
            registeredClasses = new ConcurrentDictionary<string, List<WeakReference<object>>>();
            asyncTasks = new ConcurrentDictionary<Guid, TaskTuple>();
        }

        public override void Register(object subscribingClass)
        {
            lock (this)
            {
                if (subscribingClass == null)
                {
                    throw new NullReferenceException();
                }

                if (FetchSubscribers(subscribingClass))
                {
                    RegisterClass(subscribingClass);
                }
            }
        }
        public override void Deregister(object subscribingClass)
        {
            lock (this)
            {
                if (subscribingClass == null)
                {
                    throw new NullReferenceException();
                }

                DeregisterClass(subscribingClass);
            }
        }

        private List<string> GetTypeChain(Type eventType)
        {
            List<string> chain = new List<string>();
            Type type = eventType;
            while (type != null)
            {
                chain.Add(type.FullName);
                type = type?.BaseType;
            }
            return chain;
        }

        private bool TypeComparer(Type compareFromType, Type compareToType, Func<Type, Type, bool> comparer)
        {
            bool flag = false;
            while (compareFromType != null)
            {
                if (comparer(compareFromType, compareToType))
                {
                    flag = true;
                    break;
                }
                compareFromType = compareFromType?.BaseType;
            }
            return flag;
        }

        private string GetBestFittingType(List<string> subscribers, Type eventType)
        {
            string bestFit = "";
            Type type = eventType;
            while (type != null)
            {
                if (subscribers.Contains(type.FullName))
                {
                    bestFit = type.FullName;
                    break;
                }
                type = type?.BaseType;
            }
            return bestFit;
        }

        public override void Post(object eventType)
        {
            if (eventType == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                Task.Factory.StartNew(() =>
                {
                    List<CalleeMethod> calleeList = new List<CalleeMethod>();
                    foreach (var dict in subscriberCallDictionary.Values)
                    {
                        string bestFit = GetBestFittingType(dict.Keys.ToList(), eventType.GetType());
                        if (!string.IsNullOrEmpty(bestFit) && !string.IsNullOrWhiteSpace(bestFit))
                        {
                            var callee = dict[bestFit];
                            calleeList.Add(callee);
                        }
                    }
                    foreach (var callee in calleeList)
                    {
                        if (registeredClasses.ContainsKey(callee.Method.ReflectedType.FullName))
                        {
                            List<WeakReference<object>> list = new List<WeakReference<object>>();
                            foreach (var registeredClassWR in registeredClasses[callee.Method.ReflectedType.FullName])
                            {
                                object registeredClass = null;
                                registeredClassWR.TryGetTarget(out registeredClass);
                                if (registeredClass != null)
                                {
                                    switch (callee.Mode)
                                    {
                                        case ThreadMode.Async:
                                            Guid id = Guid.NewGuid();
                                            var canToken = new CancellationTokenSource();
                                            Task t = new Task(() =>
                                            {
                                                Thread.CurrentThread.Name = "Executor";
                                                try
                                                {
                                                    callee.Method.Invoke(registeredClass, new[] { eventType });
                                                }
                                                catch (Exception e)
                                                {
                                                    throw new EventHubException(e.Message, e);
                                                }
                                                finally
                                                {
                                                    TaskTuple temp;
                                                    asyncTasks.TryRemove(id, out temp);
                                                }
                                            }, canToken.Token);
                                            asyncTasks.TryAdd(id, new TaskTuple { RunnableTask = t, CancellationToken = canToken, Event = eventType }); // TODO: Multiple tasks for different registerars on the same event triggers error
                                            t.Start();
                                            break;
                                    }
                                    list.Add(new WeakReference<object>(registeredClass, false));
                                }
                            }
                            registeredClasses[callee.Method.DeclaringType.FullName] = list;
                        }
                    }
                });
            }
        }

        public override void Cancel(object eventType)
        {
            if (eventType == null)
            {
                throw new NullReferenceException();
            }
            var tasks = asyncTasks.Where(r => r.Value.Event == eventType);
            if (tasks.Count() > 0)
            {
                foreach (var task in tasks)
                {
                    TaskTuple t = asyncTasks[task.Key];
                    switch (t.RunnableTask.Status)
                    {

                        case TaskStatus.Created:
                        case TaskStatus.Running:
                        case TaskStatus.WaitingForActivation:
                        case TaskStatus.WaitingForChildrenToComplete:
                        case TaskStatus.WaitingToRun:
                            t.CancellationToken.Cancel();
                            break;
                        case TaskStatus.Faulted:
                        case TaskStatus.RanToCompletion:
                        case TaskStatus.Canceled:
                            throw new EventHubException($"The task for EventType '{eventType.GetType().FullName}' has '{t.RunnableTask.Status}' ");
                    }
                    TaskTuple temp;
                    asyncTasks.TryRemove(task.Key, out temp);
                }
            }
            else
            {
                throw new EventHubException($"The EventType '{eventType.GetType().FullName}' hasn't been posted yet.");
            }
        }

        private void RegisterClass(object subscribingClass)
        {
            Type subscriberClassType = subscribingClass.GetType();
            var classes = registeredClasses.Keys.Where(r => r.Equals(subscriberClassType.FullName));
            int classCount = classes.Count();
            if (classCount == 0)
            {
                registeredClasses.TryAdd(subscriberClassType.FullName, new List<WeakReference<object>>());
                var list = registeredClasses[subscriberClassType.FullName];
                list.Add(new WeakReference<object>(subscribingClass, false));
                registeredClasses[subscriberClassType.FullName] = list;
            }
            else
            {
                var lst = registeredClasses[subscriberClassType.FullName];
                if (lst.Where(r =>
                {
                    object o;
                    r.TryGetTarget(out o);
                    return o == subscribingClass;
                }).Count() == 0)
                {
                    var list = registeredClasses[subscriberClassType.FullName];
                    list.Add(new WeakReference<object>(subscribingClass, false));
                    registeredClasses[subscriberClassType.FullName] = list;
                }
            }

        }

        private void DeregisterClass(object subscribingClass)
        {
            Type subscriberClassType = subscribingClass.GetType();
            int classCount = registeredClasses.Keys.Where(r => r.Equals(subscriberClassType.FullName)).Count();
            if (classCount == 1)
            {
                var list = registeredClasses[subscriberClassType.FullName];
                if (list.Count() == 1)
                {
                    object o;
                    list[0].TryGetTarget(out o);
                    if (o == null || o == subscribingClass)
                    {
                        List<WeakReference<object>> references;
                        registeredClasses.TryRemove(subscriberClassType.FullName, out references);
                    }
                }
                else if (list.Count() > 1)
                {
                    List<WeakReference<object>> newList = new List<WeakReference<object>>();
                    list?.ForEach((r) =>
                    {
                        object o;
                        r.TryGetTarget(out o);
                        if (o != null && o != subscribingClass)
                        {
                            newList.Add(r);
                        }
                    });
                    registeredClasses[subscriberClassType.FullName] = newList;
                }
            }
        }

        private bool FetchSubscribers(object subscribingClass)
        {
            bool flag = false;
            Type subscriberClassType = subscribingClass.GetType();
            MethodInfo[] publicMethods = subscriberClassType.GetMethods();
            if (subscriberCallDictionary.ContainsKey(subscriberClassType.FullName))
            {
                flag = true;
            }
            else
            {
                foreach (var method in publicMethods)
                {
                    var attribs = method.GetCustomAttributes(typeof(EventSubscriberAttribute), true);
                    if (attribs != null && attribs.Length == 1)
                    {
                        var attrib = ((EventSubscriberAttribute)attribs[0]);
                        var paramCount = method.GetParameters().Count();
                        if (paramCount == 1)
                        {
                            var callee = new CalleeMethod { Method = method, Mode = attrib.Mode };
                            bool hasSubscriber = subscriberCallDictionary.ContainsKey(subscriberClassType.FullName);
                            if (!hasSubscriber)
                            {
                                subscriberCallDictionary.TryAdd(subscriberClassType.FullName, new Dictionary<string, CalleeMethod>());
                            }

                            var parameters = method.GetParameters();
                            var dict = subscriberCallDictionary[subscriberClassType.FullName];
                            var hasParam = dict.ContainsKey(parameters[0].ParameterType.FullName);
                            if (!hasParam)
                            {
                                flag = true;
                                dict.Add(parameters[0].ParameterType.FullName, callee);
                                subscriberCallDictionary[subscriberClassType.FullName] = dict;
                            }
                            else
                            {
                                var oldCallee = dict[parameters[0].ParameterType.FullName];
                                bool checker = subscriberClassType.FullName.Equals(method.DeclaringType.FullName) && !subscriberClassType.FullName.Equals(oldCallee.Method.DeclaringType.FullName);
                                //if (!checker) checker = checker || (TypeComparer(subscriberClassType, method.DeclaringType, (a, b) => { return a.FullName.Equals(b.FullName); }) && method.Attributes.HasFlag(MethodAttributes.NewSlot));
                                if (checker)
                                {
                                    flag = true;
                                    dict.Remove(parameters[0].ParameterType.FullName);
                                    dict.Add(parameters[0].ParameterType.FullName, callee);
                                    subscriberCallDictionary[subscriberClassType.FullName] = dict;
                                }
                                else if (subscriberClassType.FullName.Equals(method.DeclaringType.FullName) && subscriberClassType.FullName.Equals(oldCallee.Method.DeclaringType.FullName))
                                {
                                    throw new EventHubException($"There are multiple subscribed methods with the parameter of type '{parameters[0].ParameterType.FullName}'");
                                }
                            }
                        }
                        else
                        {
                            if (paramCount > 1)
                            {
                                throw new EventHubException($"The method '{method.DeclaringType.FullName}_{method.Name}' has multiple parameters, Currently only one is accepted");
                            }
                            else
                            {
                                throw new EventHubException($"The method '{method.DeclaringType.FullName}_{method.Name}' has zero parameters, One is needed to call the method");
                            }
                        }
                    }
                }
            }
            return flag;
        }
    }
}
