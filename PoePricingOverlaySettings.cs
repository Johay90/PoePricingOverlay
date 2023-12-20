using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;

namespace PoePricingOverlay;

public class PoePricingOverlaySettings : ISettings
{
    //Mandatory setting to allow enabling/disabling your plugin
    

    public PoePricingOverlaySettings()
    {
        // Default values for settings can be set here
        ToggleOverlayHotkey = new HotkeyNode(System.Windows.Forms.Keys.F8); // Default hotkey
    }

    public ToggleNode Enable { get; set; } = new ToggleNode(false);
    [Menu("Toggle Overlay Hotkey")]
    public HotkeyNode ToggleOverlayHotkey { get; set; }
}

