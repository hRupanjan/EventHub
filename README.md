# ![icon](./icon/EventHub-48x48.png) EventHub
This project is inspired by [EventBus](http://greenrobot.org/eventbus/) for Android & Java. you can checkout their repository at this [link](https://github.com/greenrobot/EventBus).

This is a project that implements the Pub/Sub model in a decoupled architecture. It can be especially useful in situations where entities have there own life-cycle.

This architecture can be used to develop applications for Xamarin.Forms, Xamarin.Android, Xamarin.iOS.
It helps in seamless communication between various parts of the respective application models.

[![Build Status](https://dev.azure.com/hrupanjan/EventHub/_apis/build/status/hRupanjan.EventHub?branchName=master)](https://dev.azure.com/hrupanjan/EventHub/_build/latest?branchName=master)
## Features
- **Attribute based API**: Put the *[EventSubscriber]* attribute to your subscriber methods to mark it as a handler.
- **Good performance**: It is optimized to dispatch events as soon as they posted.
- **Asynchronous event Delivery & Execution**: All the event executions and delivery are asynchronous.
- **Event & Subscriber inheritance**: In EventHub, the object oriented paradigm apply to event and subscriber classes. Let’s say event class A is the superclass of B. Posted events of type B will also be posted to subscribers interested in A. Similarly the inheritance of subscriber classes are considered.
- **Main thread delivery**: Using platform specific main thread invocation methods, you can also deliver UI changes to the application.
- **No 3rd party dependency**: This is no Xamarin binding, thus no 3rd party dependency or binding difficulties.

### Future Scope
1. Build time subscriber method template cache, so that run time performance stays unaffected.
2. Assign batches for every event posted.
3. Cancellation will depend on execution state of the batch.
4. Priority based event posting.

### License
EventHub binaries and source code can be used according to the [Apache License 2.0](https://github.com/hRupanjan/EventHub/blob/master/LICENSE).