Dictionary<string, DoorState> movingDoors = new Dictionary<string, DoorState>();
Dictionary<string, DoorState> nextMovingDoors;

Dictionary<string, Dictionary<string, IMyDoor>> doorsByGroup = new Dictionary<string, Dictionary<string, IMyDoor>>();
Dictionary<string, Dictionary<string, IMySensorBlock>> sensorsByGroup = new Dictionary<string, Dictionary<string, IMySensorBlock>>();

// "<prefix>Door <group> <index>"
// "<prefix>Sensor <group> <index>"
string prefix = "DS-1 AL "; // regex!

void Main() {
    movingDoors = nextMovingDoors;
    nextMovingDoors = new Dictionary<string, DoorState>();

    discoverDoors();
    discoverSensors();
}


void discoverDoors() {
    discover<IMyDoor>("Door", doorsByGroup);
}

void discoverSensors() {
    discover<IMySensorBlock>("Door", sensorsByGroup);
}

void discover<T>(string type, Dictionary<string, Dictionary<string, T>> blocksByGroup) where T : IMyTerminalBlock {
    List<IMyTerminalBlock> doors = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<T>(doors);

    for (int i = 0; i < doors.Count; i++) {
        T door = doors[i] as T;

        var match = System.Text.RegularExpressions.Regex.Match(door.CustomName, "^" + prefix + type + " ([^ ]+) ([^ ]+)$");
        if (!match.Success) {
            continue;
        }

        string groupName = match.Groups[0];
        string index = match.Groups[1];

        var group = blocksByGroup[groupName];
        if (group == null) {
            group = blocksByGroup[groupName] = new List<T>();
        }

        group[index] = door;
    }
}

public enum DoorState {
    CLOSED, OPENING, OPEN, CLOSING
}

public class DoorWrapper {
    private IMyDoor door;
    private DoorWrapper otherDoorWrapper;

    public DoorWrapper(IMyDoor door) {
        this.door = door;
    }

    public DoorWrapper(IMyDoor door, DoorWrapper otherDoorWrapper) : this(door) {
        this.otherDoorWrapper = otherDoorWrapper;
        otherDoorWrapper.otherDoorWrapper = this;
    }

    public DoorState state {
        get {
            if (nextMovingDoors.contains(door.CustomName)) {
                return nextMovingDoors[door.CustomName];
            }

            if (movingDoors.contains(door.CustomName)) {
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
            case OPEN:
            case OPENING:
                return;

            case CLOSED:
            case CLOSING:
                if (otherDoorWrapper != null && otherDoorWrapper.state != DoorState.CLOSED) {
                    return;
                }

                door.ApplyAction("Open_On");

                nextMovingDoors[door.CustomName] = DoorState.OPENING;
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