# Data Binding

Data binding provides a simple way for applications and games to 
transport data from your model classes into the UI widgets. Models
are plain C# objects that implement the INotifyPropertyChanged 
mechanism (that is fire an event whenever a property is changed).
Data binding establishes a connection between your model and game
logic classes and the UI and simplifies the task of keeping both
areas of concern synchronised with each other.

The Steropes.UI library supports two forms of data binding:

* ***Object bindings*** are used when whole objects or values change.
  All ``INotifyPropertyChanged.PropertyChanged`` events are atomic
  changes - the system signals that the value for a property has 
  changed, but does not provide any futher details.

* ***Collection bindings*** are used for lists of values. This 
  event can provide details on what element in a list has changed
  and how it has changed. This makes it more efficient to keep 
  collections of values synchronised with each others changes.

  Collection change events are generated via the 
  ``INotifyCollectionChanged.CollectionChanged`` interface. In the
  standard library this interface is implemented by the 
  ``ObservableCollection<T>`` and ``ReadOnlyObservableCollection<T>``
  objects.

The Microsoft WPF data binding documentation contains a great introduction
to the theory and mechanics of databinding. (Please note that Steropes.UI)
does not use WPF data binding classes, as they are strongly tied to
the WPF framework.

Data binding in Steropes.UI works by wrapping plain C# objects and properties
into ``I(ReadOnly)ObservableObject`` instances that listen to 
PropertyChanged events. ``(ReadOnly)ObservableCollection`` are likewise 
wrapped into ``I(ReadOnly)ObservableListBinding`` instances.

In case the data format the UI needs is different from the values the model
provides, you can create derived values from the ``I(ReadOnly)ObservableObject`` 
and  `I(ReadOnly)ObservableListBinding``. This allows you to:

* transform/map and filter values
* combine values from multiple bindings

and for list bindings

* transform elements of the list
* apply bulk operations over all elements (sort, group, remove duplicates).

## Using Bindings

A game is - in most parts - very similar to ordinary applications. 
Game and business logic is contained in the game's data model, the
rendered graphics and all UI components rendered with it make up the
user interface and mediation between the two systems is left to the
view models. View models are special purpose data structures that 
pre-process the game model data so that it is presentable in the UI.

The game data model is usually optimized for space and speed and
in many cases these structures do not map very well to the information
that needs to be presented to the player. A view model extracts the
necessary data from the game model and make this information readily
available for the UI. While the data model may only contain a numeric
ID or boolean flag representing an item in an inventory, the UI must
display graphics, descriptions and any other associated information
for this inventory item. 

## Example

Lets move this from a theoretical discussion to a more concrete example. 
This is some basic sample code for a simple game. Here a player has a 
name, a certain amount of hit points and power ups that are stored for
later use by the player (healing potions, strength potions etc.)

      interface IPowerUp 
      {
         string Name {get; set;}
      }

      class Player: INotifyPropertyChanged
      {
        int hitPoints;
        string name;

        public Player()
        {
          Inventory = new ObservableCollection<IPowerUp>();
        }

        public int HitPoints
        {
          get { return hitPoints; }
          set
          {
            if (value == hitPoints) return;
            hitPoints = value;
            OnPropertyChanged();
          }
        }

        public string Name
        {
          get { return name; }
          set
          {
            if (value == name) return;
            name = value;
            OnPropertyChanged();
          }
        }

        public ObservableCollection<IPowerUp> Inventory { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
      }

The player object implements the ``INotifyPropertyChanged`` interface
so that changes to the properties generate a corresponding notification.
The ``ObservableCollection`` is a read-only property, as there is not need
to change the collection itself, only its contents. ``ObservableCollection``
has an ``CollectionChanged`` event that fires whenever the collection's contents
change.

## Object Bindings

Object bindings allow you to connect object properties of your model and
UI classes. Each binding set up follows the same schema:

1. Lift the property into a binding

       Player player = new Player();
       IReadOnlyObservableValue<int> hitpointsBinding = 
             player.BindingFor(p => p.HitPoints);

2. Optional: Transform the raw data into a UI friendly data format

       IReadOnlyObservableValue<string> hitpointTextBinding = 
             hitpointsBinding.Map(v => string.Format("{0:D}", v));

3. Connect the bindingo to the target property.

       Label label = new Label(uiStyle);
       hitpointTextBinding.BindTo(textValue => label.Text = textValue);

### Creating Binding Sources - Lifting data out of source object

This library implements most binding related methods as extension
methods. You can find object binding related methods in the 
``Steropes.UI.Bindings.Binding`` class. 

Bindings monitor the source of the binding for ``INotifyPropertyChanged``
events, will reflect the new state after the event and will notify 
downstream bindings of any change happening in this binding. 

Steropes.UI offers two way primary ways of creating bindings.

* The extension method ``object.BindingFor(source, expression)`` 
  is the most direct method to create a binding. The resulting binding 
  will monitor the source object for propertychanged events and will 
  reevaluate the given expression on each such event.

  This method will attempt to parse an expression tree to extract
  the property name for the property that is accessed in the 
  expression. If you need more control over the name of the monitored 
  property,   use the alternative method 
  ``object.BindingFor(source, propertyName, expression)`` instead.

  If the expression chains multiple member or property accessors 
  together, it will *not* create a chained expression and the 
  resulting binding will only change if and only if the source 
  object signals a property change.

* ``Binding.Create(expression)`` can be used to create bindings
  that attempt to extract all relevant information from an expression
  tree. 

      var binding = Binding.Create(() => player.HitPoints)

  This method expects a simple expression and will use the 
  first constant expression as source of a chain of bindings.
  It is possible to chain multiple property access calls in
  a single expression. 

      var binding = Binding.Create(() => sample.Property.PropA.PropB)

  The resulting binding will monitor all intermediary objects 
  for changes and will update the binding accordingly.

  Warning: This method can result in unwanted captured member
  variables as the expression given can form a closure over 
  local parameter and variables. In that case you can switch 
  to the safer ``Binding.ChainFor`` method.

* The extension method ``object.ChainFor(source, expression)`` 
  is a more explicit version of the ``Binding.Create(..)`` method 
  above which allows you explicitly specify the starting point
  of the chained binding expression.

All bindings implement the ``IBindingSubscription`` interface that
allows you to unbind bindings via the ``Dispose()`` or
``Unbind`` method. You can recursively disolve a whole chain of
bindings via the ``UnbindAll`` extension method. Eventual upstream 
bindings can be retrieved via the ``Sources`` property.

### Transforming Bound Data Values 

Bound values can be transformed into derived values via ``Map``
and ``Filter`` bindings.

*   A ``Filter`` binding will evaulate a predicate function for each
    value change in the binding and will return the bound value if
    the predicate returns true, otherwise it will return a default
    value.

* A ``AsInstanceOf`` binding is a special purpose filter binding
  that safely casts the bound values into the given type.

* ``Map`` bindings invoke a given transformation function on the
  bound value. Use this to convert, format or otherwise transform
  values.

  The library provides a series of special purpose binding extension
  methods for 
  
  * string operations (class ``StringBinding``)
  * boolean combinators (class ``BooleanBindings``) 
  * simple arithmetics (class ``NumericBindings``) 
  * and common operations on objects (like null checks, 
    also see ``BooleanBindings``).

Bindings can be chained using mapping functions:

    IReadOnlyObservableValue<string> nameBinding          = player.BindingFor(p => p.Name);
    IReadOnlyObservableValue<string> upperCaseNameBinding = nameBinding.Map(name => name.ToUpper());
    IReadOnlyObservableValue<int>    lengthBinding        = nameBinding.Length();

If a binding returns a complex object, the library provides convenience
methods that create continuation bindings that follow a property chain.

    IReadOnlyObservableValue<Player> playerBinding = gameModel.BindingFor(g => g.CurrentPlayer);
    IReadOnlyObservableValue<string> nameBinding = 
        playerBinding.Bind(p => p.Name).OrElse("No player active");
    

### Binding Sinks - Pushing data into target objects

Once you have extracted or derived a value suitable for displaying in
the UI, you can apply the bound value to the UI using the ``BindTo``
extension method.

    Label nameLabel = new Label(uiStyle);
    IReadOnlyObservableValue<string> nameBinding = ...

    nameBinding.BindTo(nameValue => nameLabel.Text = nameValue);

### Two-Way Binding

It is possible to bind two writeable properties with each other.
The system will try its best to keep both properties in sync with
each other. 

There is no specific method for this, a two way binding is simply
the binding of two binding chains between two objects with an opposite
data flow direction.

    TextField nameField = new TextField(uiStyle);
    Player player = new Player();

    // forward binding 
    nameField.BindingFor(f => f.Text).BindTo(text => player.Name = text);
    // reverse binding
    player.BindingFor(p => p.Name).BindTo(name => nameField.Text = name);

The binding implementations provided by Steropes.UI contain a circuit
breaker to avoid infinite loops caused by invalid binding setups.

## Collection Bindings

A collection binding monitors the contents of an observable collection,
transforms the elements contained in a collection and binds the values
of the collection binding to a target collection.

Unlike the object bindings above, collection bindings assume that
the monitored collection itself remains associated with the source object 
and that operations on the source object do not replace the collection
itself but change the contents instead.

The Steropes.UI binding system offers two styles of list bindings. 

* ``IReadOnlyObservableListBinding<T>`` for one way bindings. The binding
  represents a read-only view over the source collection and will reflect 
  all changes made to that source collection, but cannot be modified itself.

* ``IObservableListBinding<T>`` represents a two way binding. Each
  binding is a ``IList<T>`` implementation and any change made to either 
  the binding or the source collection will be replicated in the other
  party.

Note: In case your collection object itself changes, use a object binding
as first step in the binding chain.

### Creating Binding Sources - Lifting data out of source collections

The library offers a few extension methods to create a list binding.

    var player = new Player();
    IObservableListBinding<IPowerUp> binding = player.Inventory.ToBinding();

If you create a binding from an ReadOnlyObservableCollection, the 
subsequent binding will be a ``IReadOnlyObservableListBinding<T>`` 
instead.

    var readOnlyCollection = new ReadOnlyObservableCollection<IPowerUp>(player.Inventory);
    IReadOnlyObservableListBinding<IPowerUp> binding = readOnlyCollection.ToBinding();

### Transforming Bound Data Values 

The values stored in an observable list frequently needs to be transformed
into derived values more suitable for displaying in a user interface.

The library contains three types of derived list bindings:

* List operations
  * ``IReadOnlyObservableValue<int> ListBinding.CountBinding()`` binds the
    number of elements in the list to an observable value.
  * ``ListBinding.ItemsBinding`` binds the contents of the list binding into
    an observable value containing the binding contents as readonly-list.
  * ``ListBinding.ItemAt(index)`` produces a single element binding for the
    value stored at the given index position (or null if there is no value).
  * ``ListBinding.RangeBinding(index, count)`` produces a subset list binding for the
    value stored at the given range positions (or null if there is no value).

* Transformations

  Note: All transformations are considered one way transformations (that means
  we can assert that there is a function f(x) => y, but we cannot equally assert
  that this operation is reversible, ie that a function f(y) => y exists). Therefore
  all transformations result in ReadOnlyObservableListBindings.

  * ``ListBinding.Map<TSource,TTarget>(Func<TSource,TTarget> fn)`` executes
    the given transformation function on each changed value whenever the data 
    contained in the list changes.
  * ``ListBinding.MapAll<TSource,TTarget>(Func<TSource,TTarget> fn)``
    executes the given transformation function on the whole list data. Use
    this to perform global operations that can change the composition or
    number of elements of the list.
  * ``ListBinding.OrderBy<TSource,TTarget>(Func<TSource,TTarget> fn)``
    sorts the contents of the list whenever the contents change. 

### Binding Sinks - Pushing data into target collections

To synchronize two list bindings or observable lists you need to apply the
changes made to the source binding to a target binding. Steropes.UI offers
both one way bindings (where a target list reflects all changes made to the
source list) and two way bindigns (where changes made to either end of the
binding chain will be reflected in both sides).

The target of a list binding sink operation must always be a modifiable
list, that is either an ``IObservableListBinding`` or an ``ObservableCollection``
instance.

A one way binding created using the binding code assumes that the target 
list or list binding is only modified by a single source. If there are more 
than one sources that modifies the target list the result of such an 
operation is undefined. Expect random errors in that case.

    ObservableList<string> target = new ObservableList<string>();
    ObservableList<string> source = new ObservableList<string>();
    
    source.BindTo(target);

You can create two way bindings between an two list bindings via
the ``BindTwoWay`` binding.

    source.BindTwoWay(target);

The system contains a circuit breaker mechanism to prevent circular list 
event bindings to create infinite loops and may result in errors during
subsequent list updates.
