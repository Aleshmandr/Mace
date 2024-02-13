# Mace
Mace is a MVVM framework for Unity
This framework is used in the [Uice framework](https://github.com/Aleshmandr/Uice) but can be used separately.

## Installation
You can add this Git package via Unity package manager:
https://github.com/Aleshmandr/Mace.git

## Framework's Philosophy
The framework encourages a [Model-View-ViewModel](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93viewmodel) approach, splitting the main concerns of any UI piece into a couple of classes that interoperate together.

![MVVM diagram](https://user-images.githubusercontent.com/3226755/94297948-9f713e00-ff65-11ea-9dce-f44bbef708a6.png)

* The **model** is any raw data that your game should display to the user. It could be player status, enemies status, systems information, configuration, etc.

* The **view** is a passive interface that displays data (the model) and routes user commands (events) to the viewmodel to act upon that data. It is typically a GameObject built of uGui components with a main class to handle them.

* The **viewmodel** is an intermediary class that retrieves data from the model and formats it for display in the view. It also reacts to user events in the view and updates the model.

Mace contains some base classes and systems that implement this pattern to let you focus on your game's concrete needs.

## Observables
The observable family is the bread and butter of Mace. They act as the glue that keeps everything interconnected.

The View and the ViewModel communicate with each other through observable objects. These classes have mechanisms to automatically notify observers about changes in their internal state.

There are four members in the observable family, each of them intended for a particular need in the system. 

### ObservableVariable
This is the battle horse of the whole system; one of the simpler and most useful of all the members of the family. It just wraps a variable and notifies when its value changes. It's worth noting that it won't raise a change event when its `.Value` is set with the same value that is already stored.

### ObservableCollection
It represents a collection or a list of elements of the same type. You'll find all operations that you'd expect for a regular `List<T>`, and it'll notify a particular event for each of them.

The `ObservableCollection` grants you a lot of control when dealing with collections of data and provides an extensive pool of relevant information about the events that take place for those entities listening for changes in its contents.

### ObservableCommand
The `ObservableCommand` is the channel through which the view can communicate with the ViewModel when the user performs an action with the intention to change the underlying model.

In addition to an `.Execute()` method to request an action by the ViewModel, it also exposes an `ObservableVariable<bool>` that tells the requester whether said action can be performed or not in this particular moment. This is really useful to give the user feedback about the available actions they have at their disposal, by greying out buttons when `.CanExecute.Value` is `false`, for example.

There are two versions of `ObservableCommand`, one of them with a generic parameter `T`, so you can add some information about the requested action.

### ObservableEvent
The last member of the family and the most obvious of them all. An `ObservableEvent` is just... and event; something that takes place in a particular moment and which value (if any) is not supposed to be stored to be consulted in the future. 

Like the `ObservableCommand`, the event has a generic version that can be used to supply additional information about the observed happening.

## ViewModel
The `ViewModel` is some sort of _translator_ between your business model and the view. It holds all the data that the view requires in a simple and easy to consume format. 

For that matter, it exposes Observable objects and keeps them up to date according to changes in the business model. Included in those observable objects, there could be `ObservableCommand`s, which are the way the View is able to communicate user driven events to the ViewModel so it can process them and update the model accordingly.

The ViewModel is **not** a `MonoBehaviour`, meaning that it can be passed away between classes in the Mace system and it doesn't necessarily live on a concrete `GameObject`. 

Creating a new ViewModel is pretty straightforward. You can extend the `ViewModel` class, which already handles a couple of mechanism for you and allows your code to be easily attached to the game's update loop or react to some events during the ViewModel's lifecycle. 

```csharp
using Mace;

public class MyViewModel : ViewModel
{
    public IReadonlyObservableVariable<float> MyVariable => myVariable;
    public IObservableCommand MyCommand => myCommand;
    public IObservableEvent MyEvent => myEvent;

    private ObservableVariable<float> myVariable;
    private ObservableCommand myCommand;
    private ObservableEvent myEvent;

    private MyModel model;

    // You can freely use constructors
    public MyViewModel(MyModel model)
    {
        myVariable = new ObservableVariable<float>(myModel.myVariable);

        myCommand = new ObservableCommand();
        myCommand.ExecuteRequested += OnMyCommandExecuteRequested;

        myEvent = new ObservableEvent();

        this.model = model;
    }

    // Called when the ViewModel starts being used
    protected override void OnEnable()
    {
        model.NotifiedSomething += OnMyModelNotifiedSomething;
    }

    // Called when the ViewModel is put on hold and not used anymore
    protected override void OnDisable()
    {
        model.NotifiedSomething -= OnMyModelNotifiedSomething;
    }

    // Update works as MonoBehaviour's Update method
    protected override void Update()
    {
        myVariable.Value = model.myVariable;
    }

    private void OnMyCommandExecuteRequested()
    {
        model.DoSomething();
    }

    private void OnMyModelNotifiedSomething()
    {
        myEvent.Raise();
    }
}
```

You are free to create your ViewModel from scratch instead of extending `ViewModel`. The only restriction is that you need to implement the `IViewModel` interface for it to be used by the framework.

It's worth mentioning that the ViewModel is the entry point for everything that's gonna happen in the View so it is a good idea to keep it as simple and as clean as possible. Of course, you should always keep in mind that the ViewModel is not responsible for anything related to the way the View is displaying its data. It should only provide data, not styles or behaviours. 

## Bindings
Bindings are the last link the framework chain. They are responsible of providing updated information about changes in the ViewModel to the Unity Components that use them.

![image](https://github.com/Aleshmandr/Uice/assets/11294931/e2144732-e6aa-4be0-95b5-aa29a6f0b6e8)

There are matching binders for every member of the observable family and some other elements to ease development and displaying the information in the Editor.

### Binders

Binders are the actual components (`MonoBehaviour`s) that operate over a Unity `Component` so the View can reflect the internal state of the ViewModel.

Mace includes many binders for uGui's components as well as some other collections that let your UI objects react to changes on the ViewModel.

### OperatorBinders
These are a special kind of binders. Binders that can process one or more values from the ViewModel and expose a derived value based on its input. 

This mechanism grants Mace an important expressive power, allowing the designers and UI artists to mix and match properties to create their own to fit the requirements of the View. Remember that the View can (and will) constantly change as the project grows and evolves, and having a clear and strict separation between the ViewModel (the data to be displayed) and the View (how is that data displayed) is a guarantee for a more agile development process.

OperatorBinders are heavily inspired by [ReactiveX](http://reactivex.io/documentation/operators.html) operators, so you can expect to find many of the most valuable operations from that library. 
