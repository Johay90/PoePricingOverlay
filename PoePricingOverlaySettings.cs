using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;

namespace PoePricingOverlay;

public class PoePricingOverlaySettings : ISettings
{
    public PoePricingOverlaySettings()
    {
        ToggleOverlayHotkey = new HotkeyNode(System.Windows.Forms.Keys.F8);
    }

    public ToggleNode Enable { get; set; } = new ToggleNode(false);
    [Menu("Toggle Overlay Hotkey")]
    public HotkeyNode ToggleOverlayHotkey { get; set; }
}

