public static class Events
{
    public static PickupEvent onPickup = new PickupEvent();
    public static InventoryChangedEvent onInventoryChanged = new();
    public static SettingsChangedEvent onSettingsChanged = new();
    public static AudioSettingsChangedEvent onAudioSettingsChanged = new();
}


public class PickupEvent : GameEvent
{ 
    public ItemPickup item;
}

public class InventoryChangedEvent : GameEvent
{
    public int itemId, itemCount;
}

public class SettingsChangedEvent : GameEvent
{
    public Settings changedSettings;
}

public class AudioSettingsChangedEvent : GameEvent
{
    public SettingsAudio changedSettings;
}