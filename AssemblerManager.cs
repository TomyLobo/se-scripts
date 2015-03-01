void Main() {
    List<IMyTerminalBlock> assemblers = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> ingotContainers = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> resultContainers = new List<IMyTerminalBlock>();

    GridTerminalSystem.GetBlocksOfType<IMyAssembler>(assemblers);
    ingotContainers.Add(GridTerminalSystem.GetBlockWithName("DS-1 IngotContainer"));
    resultContainers.Add(GridTerminalSystem.GetBlockWithName("DS-1 ResultContainer"));

    for (int i = 0; i < assemblers.Count; i++) {
        IMyAssembler assembler = assemblers[i] as IMyAssembler;
        cleanAssemblerInput(assembler, ingotContainers);
        cleanAssemblerOutput(assembler, resultContainers);
    }

    compactInventories(ingotContainers);
    compactInventories(resultContainers);
}

void compactInventories(List<IMyTerminalBlock> containers) {
    for(int i = 0; i < containers.Count; i++) {
        IMyInventoryOwner container = containers[i] as IMyInventoryOwner;
        compactInventory(container);
    }
}


void compactInventory(IMyInventoryOwner container) {
    Dictionary<string, int> itemIds = new Dictionary<string, int>();

    // Alles durchiterieren
    IMyInventory containerInv = container.GetInventory(0);
    List<IMyInventoryItem> items = containerInv.GetItems();
    for (int targetSlot = items.Count - 1; targetSlot >= 0; targetSlot--) {
        var targetContent = items[targetSlot].Content;

        // Alle nachfolgenden Items durchiterieren
        for (int sourceSlot = items.Count - 1; sourceSlot >= targetSlot; sourceSlot--) {
            var sourceContent = items[sourceSlot].Content;

            // Passt nicht => weiter
            if (!targetContent.CanStack(sourceContent)) {
                continue;
            }

            // Transfer von sourceSlot nach targetSlot
            containerInv.TransferItemTo(containerInv, sourceSlot, targetSlot, false, null);
            items = containerInv.GetItems();
        }
    }
}


void cleanAssemblerOutput(IMyAssembler assembler, List<IMyTerminalBlock> containers) {
    IMyInventory assemblerInv = assembler.GetInventory(1);

    // Solange der Assember noch nicht leer ist...
    while (assemblerInv.CurrentVolume != 0) {
        // ...suche den ersten leeren Container...
        IMyInventory containerInv = getFirstEmpty(containers);

        if (containerInv == null)
            return;

        // ...und transferiere alle Items dahin.
        List<IMyInventoryItem> assemblerItems = assemblerInv.GetItems();
        for (int i = assemblerItems.Count - 1; i >= 0; i--) {
            assemblerInv.TransferItemTo(containerInv, i, null, true, null);
        }
    }
}


void cleanAssemblerInput(IMyAssembler assembler, List<IMyTerminalBlock> containers) {
    if (assembler.IsProducing)
        return;

    IMyInventory containerInv = getFirstEmpty(containers);

    if (containerInv == null)
        return;

    IMyInventory assemblerInv = assembler.GetInventory(0);
    List<IMyInventoryItem> assemblerItems = assemblerInv.GetItems();

    for (int i = assemblerItems.Count -1; i >= 0; i--) {
        assemblerInv.TransferItemTo(containerInv, i, null, true, null);
    }
}

float getPercent(IMyInventory inventory) {
    return ((float)inventory.CurrentVolume / (float)inventory.MaxVolume) * 100f;
}


bool IsFull(IMyInventory inventory) {
    if (getPercent(inventory) >= 95)
        return true;
    else
        return false;
}

IMyInventory getFirstEmpty(List<IMyTerminalBlock> containers) {
    // search our containers until we find an empty one
    for (int n = 0; n < containers.Count; n++) {
        IMyInventoryOwner container = containers[n] as IMyInventoryOwner;
        IMyInventory containerInventory = container.GetInventory(0);
        if (!IsFull(containerInventory)) {
            return containerInventory;
        }
    }

    return null;
}
