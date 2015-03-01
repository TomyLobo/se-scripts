void Main() {
    List<IMyTerminalBlock> assemblers = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> ingotContainers = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> resultContainers = new List<IMyTerminalBlock>();

    GridTerminalSystem.GetBlocksOfType<IMyAssembler>(assemblers);
    ingotContainers.Add(GridTerminalSystem.GetBlockWithName("DS-1 IngotContainer"));
    resultContainers.Add(GridTerminalSystem.GetBlockWithName("DS-1 ResultContainer"));

    for(int i = 0; i < assemblers.Count; i++) {
        cleanAssemblerInput(assemblers[i] as IMyAssembler, ingotContainers);
        cleanAssemblerOutput(assemblers[i] as IMyAssembler, resultContainers);
    }
}


void cleanAssemblerOutput(IMyAssembler assembler, List<IMyTerminalBlock> containers) {
    IMyInventory assemblerInv = assembler.GetInventory(1);

    // Solange der Assember noch nicht leer ist...
    while (assemblerInv.CurrentVolume != 0) {
        // ...suche den ersten leeren Container...
        IMyInventory containerDestination = getFirstEmpty(containers);

        if (containerDestination == null)
            return;

        // ...und transferiere alle Items dahin.
        List<IMyInventoryItem> assemblerItems = assemblerInv.GetItems();
        for (int i = assemblerItems.Count - 1; i >= 0; i--) {
            assemblerInv.TransferItemTo(containerDestination, i, null, true, null);
        }
    }
}


void cleanAssemblerInput(IMyAssembler assembler, List<IMyTerminalBlock> containers) {
    if (assembler.IsProducing)
        return;

    IMyInventory containerDestination = getFirstEmpty(containers);

    if (containerDestination == null)
        return;

    IMyInventory assemblerInv = assembler.GetInventory(0);
    List<IMyInventoryItem> assemblerItems = assemblerInv.GetItems();

    for (int i = assemblerItems.Count -1; i >= 0; i--) {
        assemblerInv.TransferItemTo(containerDestination, i, null, true, null);
    }
}

float getPercent(IMyInventory inv) {
    return ((float)inv.CurrentVolume / (float)inv.MaxVolume) * 100f;
}


bool IsFull(IMyInventory inv) {
    if (getPercent(inv) >= 95)
        return true;
    else
        return false;
}

IMyInventory getFirstEmpty(List<IMyTerminalBlock> containers) {
    // search our containers until we find an empty one
    for (int n = 0; n < containers.Count; n++) {
        IMyInventory containerInv = (containers[n] as IMyInventoryOwner).GetInventory(0);
        if (!IsFull(containerInv)) {
            return containerInv;
        }
    }

    return null;
}
