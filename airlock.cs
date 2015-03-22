public static Dictionary<string, int> movingDoors;
public static Dictionary<string, int> nextMovingDoors = new Dictionary<string, int>();

public static Dictionary<string, Dictionary<string, IMyDoor>> doorsByGroup = new Dictionary<string, Dictionary<string, IMyDoor>>();
public static Dictionary<string, Dictionary<string, IMySensorBlock>> sensorsByGroup = new Dictionary<string, Dictionary<string, IMySensorBlock>>();

// "<prefix>Door <group> <index>"
// "<prefix>Sensor <group> <index>"
const string prefix = "DS-1 AL "; // regex!

public static IMyGridTerminalSystem GridTerminalSystem2;
public static IMyTerminalBlock debugBlock;
static string dbg;

public static void debugprint(String s) {
    dbg += s;
    dbg += " ";
    //dbg += "\n";
    //debugBlock.SetCustomName(dbg);
}


void Main() {
    dbg = "";
    List<IMyTerminalBlock> debugBlocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyProgrammableBlock>(debugBlocks);
    debugBlock = debugBlocks[0];
    debugprint("test");
    GridTerminalSystem2 = GridTerminalSystem;
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

        LinkedList<DoorWrapper> doorWrappers = new LinkedList<DoorWrapper>();
        var doorGroup2 = new List<KeyValuePair<string, IMyDoor>>(doorGroup);
        for (int j = 0; j < doorGroup2.Count; j++) {
            var indexEntry = doorGroup2[j];

            string index = indexEntry.Key;
            IMyDoor door = indexEntry.Value;

            if (!sensorGroup.ContainsKey(index)) {
                continue;
            }

            IMySensorBlock sensor = sensorGroup[index];

            doorWrappers.AddLast(new DoorWrapper(door, sensor));
        }


        var currentNode = doorWrappers.First;
        while (currentNode != null) {
            DoorWrapper doorWrapper = currentNode.Value;
            currentNode = currentNode.Next;
            doorWrapper.otherDoorWrappers = Filter(doorWrappers, doorWrapper);
            doorWrapper.refreshSensorState();
        }
    }
}

public LinkedList<T> Filter<T>(LinkedList<T> list, T except) where T : class {
    var ret = new LinkedList<T>();

    var currentNode = list.First;
    while (currentNode != null) {
        T element = currentNode.Value;
        currentNode = currentNode.Next;

        if (element == except) {
            continue;
        }

        ret.AddLast(element);
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
    public LinkedList<DoorWrapper> otherDoorWrappers;

    public DoorWrapper(IMyDoor door, IMySensorBlock sensor) {
        this.door = door;
        this.sensor = sensor;
    }

    public int state {
        get {
            if (nextMovingDoors.ContainsKey(door.CustomName)) {
                return debugstate(nextMovingDoors[door.CustomName]);
            }

            if (movingDoors.ContainsKey(door.CustomName)) {
                return debugstate(movingDoors[door.CustomName]);
            }

            if (door.Open) {
                return debugstate(DoorState.OPEN);
            }
            else {
                return debugstate(DoorState.CLOSED);
            }
        }
    }

    private int debugstate(int state) {
        Color color;
        switch (state) {
            case DoorState.OPEN:    color = new Color(  0, 255,   0); break;
            case DoorState.OPENING: color = new Color(  0,   0, 255); break;
            case DoorState.CLOSED:  color = new Color(255,   0,   0); break;
            case DoorState.CLOSING: color = new Color(255, 255,   0); break;
            default: return state;
        }

        var light = GridTerminalSystem2.GetBlockWithName(door.CustomName+" Light") as IMyInteriorLight;
        if (light != null) light.SetValue("Color", color);

        return state;
    }


    void open() {
        debugprint(door.CustomName+" looped1 "+(otherDoorWrappers != null));
        if (otherDoorWrappers != null) {
            debugprint(door.CustomName+" looped2 "+(otherDoorWrappers.First != null));
            var currentNode = otherDoorWrappers.First;
            while (currentNode != null) {
                DoorWrapper otherDoorWrapper = currentNode.Value;
                currentNode = currentNode.Next;
                if (otherDoorWrapper.state != DoorState.CLOSED) {
                    close();
                    return;
                }
            }
        }

        switch (state) {
            case DoorState.OPEN:
            case DoorState.OPENING:
                return;

            case DoorState.CLOSED:
            case DoorState.CLOSING:

                door.ApplyAction("Open_On");

                nextMovingDoors[door.CustomName] = DoorState.OPENING;
                var tmp = state;
                break;
        }
    }

    void close() {
        switch (state) {
            case DoorState.CLOSED:
            case DoorState.CLOSING:
                return;

            case DoorState.OPEN:
            case DoorState.OPENING:
                door.ApplyAction("Open_Off");

                //nextMovingDoors[door.CustomName] = DoorState.CLOSING;
                var tmp = state;
                break;
        }
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
