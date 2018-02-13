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

### Creating Binding Sources - Lifting data out of source object

### Transforming Bound Data Values 

### Binding Sinks - Pushing data into target objects

## Collection Bindings

### Creating Binding Sources - Lifting data out of source collections

### Transforming Bound Data Values 

### Binding Sinks - Pushing data into target collections

