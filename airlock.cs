static Dictionary<string, int> movingDoors;
static Dictionary<string, int> nextMovingDoors = new Dictionary<string, int>();

static Dictionary<string, Dictionary<string, IMyDoor>> doorsByGroup = new Dictionary<string, Dictionary<string, IMyDoor>>();
static Dictionary<string, Dictionary<string, IMySensorBlock>> sensorsByGroup = new Dictionary<string, Dictionary<string, IMySensorBlock>>();

// "<prefix>Door <group> <index>"
// "<prefix>Sensor <group> <index>"
string prefix = "DS-1 AL "; // regex!

void Main() {
    movingDoors = nextMovingDoors;
    nextMovingDoors = new Dictionary<string, int>();

    DiscoverDoors();
    DiscoverSensors();

    var doorsByGroup2 = new List<KeyValuePair<string, Dictionary<string, IMyDoor>>>(doorsByGroup);
    foreach (var groupEntry in doorsByGroup2) {
        string groupName = groupEntry.Key;
        Dictionary<string, IMyDoor> doorGroup = groupEntry.Value;
        if (!sensorsByGroup.ContainsKey(groupName)) {
            continue;
        }

        Dictionary<string, IMySensorBlock> sensorGroup = sensorsByGroup[groupName];

        var doorWrappers = new List<DoorWrapper>();
        var doorGroup2 = new List<KeyValuePair<string, IMyDoor>>(doorGroup);
        foreach (var indexEntry in doorGroup2) {
            string index = indexEntry.Key;
            IMyDoor door = indexEntry.Value;
            if (!sensorGroup.ContainsKey(index)) {
                continue;
            }
            IMySensorBlock sensor = sensorGroup[index];

            doorWrappers.Add(new DoorWrapper(door, sensor));
        }

        for (int index = 0; index < doorWrappers.Count; index++) {
            var doorWrapper = doorWrappers[index];
            doorWrapper.otherDoorWrappers = Filter(doorWrappers, doorWrapper);
        }
    }
}

List<T> Filter<T>(List<T> list, T except) where T : class {
    var ret = new List<T>();
    for (int index = 0; index < list.Count; index++) {
        T element = list[index];
        if (element == except) {
            continue;
        }
    
        ret.Add(element);
    }
    return ret;
}


void DiscoverDoors() {
    Discover<IMyDoor>("Door", doorsByGroup);
}

void DiscoverSensors() {
    Discover<IMySensorBlock>("Door", sensorsByGroup);
}

public void Discover<T>(string type, Dictionary<string, Dictionary<string, T>> blocksByGroup) where T : class, IMyTerminalBlock {
    List<IMyTerminalBlock> doors = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<T>(doors);

    for (int i = 0; i < doors.Count; i++) {
        T door = doors[i] as T;

        var match = System.Text.RegularExpressions.Regex.Match(door.CustomName, "^" + prefix + type + " ([^ ]+) ([^ ]+)$");
        if (!match.Success) {
            continue;
        }

        var groupName = match.Groups[0].Value;
        var index = match.Groups[1].Value;

        if (!blocksByGroup.ContainsKey(groupName)) {
            blocksByGroup[groupName] = new Dictionary<string, T>();
        }
        var group = blocksByGroup[groupName];

        group[index] = door;
    }
}

public class DoorState {
    public const int CLOSED = 0;
    public const int OPENING = 1;
    public const int OPEN = 2;
    public const int CLOSING = 3;
}

public class DoorWrapper {
    private IMyDoor door;
    private IMySensorBlock sensor;
    public List<DoorWrapper> otherDoorWrappers;

    public DoorWrapper(IMyDoor door, IMySensorBlock sensor) {
        this.door = door;
        this.sensor = sensor;
    }

    /*
    public DoorWrapper(IMyDoor door, DoorWrapper otherDoorWrapper) : this(door) {
        this.otherDoorWrapper = otherDoorWrapper;
        otherDoorWrapper.otherDoorWrapper = this;
    }
    */

    public int state {
        get {
            if (nextMovingDoors.ContainsKey(door.CustomName)) {
                return nextMovingDoors[door.CustomName];
            }

            if (movingDoors.ContainsKey(door.CustomName)) {
                return movingDoors[door.CustomName];
            }

            return door.Open ? DoorState.OPEN : DoorState.CLOSED;
        }
    }


    void open() {
        switch (state) {
            case DoorState.OPEN:
            case DoorState.OPENING:
                return;

            case DoorState.CLOSED:
            case DoorState.CLOSING:
                if (otherDoorWrappers != null) {
                    for (int index = 0; index < otherDoorWrappers.Count; index++) {
                        DoorWrapper otherDoorWrapper = otherDoorWrappers[index];
                        if (otherDoorWrapper.state != DoorState.CLOSED) {
                            return;
                        }
                    }
                }

                door.ApplyAction("Open_On");

                nextMovingDoors[door.CustomName] = DoorState.OPENING;
                break;
        }
    }

    void close() {
        door.ApplyAction("Open_Off");

        nextMovingDoors[door.CustomName] = DoorState.CLOSING;
    }

    void setDoorSensorState(bool state) {
        if (state) {
            open();
        }
        else {
            close();
        }
    }
}