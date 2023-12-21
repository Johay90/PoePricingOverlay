using ExileCore;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using ImGuiNET;
using System;
using ExileCore.PoEMemory.Components;

namespace PoePricingOverlay
{
    public class PoePricingOverlay : BaseSettingsPlugin<PoePricingOverlaySettings>
    {
        private string[] currencies = new string[] { "Chaos", "Divine" };
        private int selectedCurrencyIndex = 0;
        private int stackSize = 1;
        private float pricePerItem = 1.0f;
        private string output = "";
        private bool overlayVisible = false;

        public override bool Initialise()
        {
            base.Initialise();
            Input.RegisterKey(Settings.ToggleOverlayHotkey);
            return true;
        }

        public override void Render()
        {
            if (Input.GetKeyState(Settings.ToggleOverlayHotkey))
            {
                overlayVisible = !overlayVisible;
                TryUpdateStackSizeFromHoveredItem();
            }

            if (overlayVisible)
            {
                ImGui.Begin("Pricing Overlay", ref overlayVisible, ImGuiWindowFlags.AlwaysAutoResize);

                if (ImGui.InputInt("Stack Size", ref stackSize))
                {
                    stackSize = Math.Max(stackSize, 1);
                    UpdateOutput();
                }

                if (ImGui.InputFloat("Price per Item", ref pricePerItem))
                {
                    pricePerItem = Math.Max(pricePerItem, 0f);
                    UpdateOutput();
                }

                if (ImGui.Combo("Currency", ref selectedCurrencyIndex, currencies, currencies.Length))
                {
                    UpdateOutput();
                }

                ImGui.Text("Output: ");
                ImGui.SameLine();
                if (ImGui.Selectable(output, true))
                {
                    ImGui.SetClipboardText(output);
                }

                if (ImGui.Button("Copy"))
                {
                    ImGui.SetClipboardText(output);
                }

                ImGui.End();
            }
        }

        private void UpdateOutput()
        {
            int totalPrice = (int)Math.Round(stackSize * pricePerItem);
            output = $"~price {totalPrice}/{stackSize} {currencies[selectedCurrencyIndex]}";
        }

        private void TryUpdateStackSizeFromHoveredItem()
        {
            var uiHover = GameController.Game.IngameState.UIHover;
            var hoveredEntity = uiHover?.Entity;

            if (hoveredEntity != null)
            {
                var stackComponent = hoveredEntity.GetComponent<Stack>();
                if (stackComponent != null)
                {
                    stackSize = stackComponent.Size;
                    UpdateOutput();
                }
            }
        }
    }
}
