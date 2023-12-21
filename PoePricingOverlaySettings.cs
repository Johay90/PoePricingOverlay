using System.Windows.Forms;
using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;

namespace PoePricingOverlay;

public class PoePricingOverlaySettings : ISettings
{
    public ToggleNode Enable { get; set; } = new ToggleNode(false);
    [Menu("Toggle Overlay Hotkey")]
    public HotkeyNode ToggleOverlayHotkey { get; set; } = new HotkeyNode(Keys.F9);
    [Menu("Update Stack Size Hotkey")]
    public HotkeyNode UpdateStackSizeHotkey { get; set; } = new HotkeyNode(Keys.F10);
}

