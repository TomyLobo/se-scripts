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
    for (int i = 0; i < doorsByGroup2.Count; i++) {
        var groupEntry = doorsByGroup2[i];
        string groupName = groupEntry.Key;

        Dictionary<string, IMyDoor> doorGroup = groupEntry.Value;
        if (!sensorsByGroup.ContainsKey(groupName)) {
            continue;
        }

        Dictionary<string, IMySensorBlock> sensorGroup = sensorsByGroup[groupName];

        List<DoorWrapper> doorWrappers = new List<DoorWrapper>();
        var doorGroup2 = new List<KeyValuePair<string, IMyDoor>>(doorGroup);
        for (int j = 0; j < doorGroup2.Count; j++) {
            var indexEntry = doorGroup2[j];

            string index = indexEntry.Key;
            IMyDoor door = indexEntry.Value;

            if (!sensorGroup.ContainsKey(index)) {
                continue;
            }

            IMySensorBlock sensor = sensorGroup[index];

            doorWrappers.Add(new DoorWrapper(door, sensor));
        }

        for (int k = 0; k < doorWrappers.Count; k++) {
            var doorWrapper = doorWrappers[k];
            doorWrapper.otherDoorWrappers = Filter(doorWrappers, doorWrapper);
            doorWrapper.refreshSensorState();
        }
    }
}

public List<T> Filter<T>(List<T> list, T except) where T : class {
    var ret = new List<T>();
    for (int index = 0; index < list.Count; index++) {
        T element = list[index] as T;
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
    Discover<IMySensorBlock>("Sensor", sensorsByGroup);
}

public void Discover<T>(string type, Dictionary<string, Dictionary<string, T>> blocksByGroup) where T : class, IMyTerminalBlock {
    blocksByGroup.Clear();
    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<T>(blocks);

    for (int i = 0; i < blocks.Count; i++) {
        T block = blocks[i] as T;

        var match = System.Text.RegularExpressions.Regex.Match(block.CustomName, "^" + prefix + type + " ([^ ]+) ([^ ]+)$");
        if (!match.Success) {
            continue;
        }

        var groupName = match.Groups[1].Value;
        var index = match.Groups[2].Value;

        if (!blocksByGroup.ContainsKey(groupName)) {
            blocksByGroup[groupName] = new Dictionary<string, T>();
        }
        var group = blocksByGroup[groupName];

        group[index] = block;
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

    public int state {
        get {
            if (nextMovingDoors.ContainsKey(door.CustomName)) {
                return nextMovingDoors[door.CustomName];
            }

            if (movingDoors.ContainsKey(door.CustomName)) {
                return movingDoors[door.CustomName];
            }

            if (door.Open) {
                return DoorState.OPEN;
            }
            else {
                return DoorState.CLOSED;
            }
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
                    for (int i = 0; i < otherDoorWrappers.Count; i++) {
                        DoorWrapper otherDoorWrapper = otherDoorWrappers[i];
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

    public void refreshSensorState() {
        bool state = sensor.IsActive;
        if (state) {
            open();
        }
        else {
            close();
        }
    }
}
