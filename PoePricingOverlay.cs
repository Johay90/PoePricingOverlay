using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using ImGuiNET;
using System;

namespace PoePricingOverlay
{
    public class PoePricingOverlay : BaseSettingsPlugin<PoePricingOverlaySettings>
    {
        private string[] currencies = new string[] { "chaos", "divine" };
        private int selectedCurrencyIndex = 0;
        private int stackSize = 1;
        private float pricePerItem = 1.0f;
        private string pricePerItemInput = "1"; // Using string to handle fractional input
        private string output = "";
        private bool overlayVisible = false;

        public override bool Initialise()
        {
            base.Initialise();
            Input.RegisterKey(Settings.ToggleOverlayHotkey);
            Input.RegisterKey(Settings.UpdateStackSizeHotkey);
            return true;
        }

        public override void Render()
        {
            if (Settings.ToggleOverlayHotkey.PressedOnce())
            {
                overlayVisible = !overlayVisible;
            }

            if (Settings.UpdateStackSizeHotkey.PressedOnce())
            {
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

                if (ImGui.InputText("Price per Item", ref pricePerItemInput, 100))
                {
                    pricePerItem = ParsePricePerItem(pricePerItemInput);
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
            float totalValue = stackSize * pricePerItem;
            if (!IsWholeNumber(totalValue))
            {
                // If the total price is not a whole number, find the first threshold
                stackSize = CalculateFirstWholeNumberThreshold(pricePerItem);
            }
    
            int totalPrice = (int)(stackSize * pricePerItem);
            output = $"~price {totalPrice}/{stackSize} {currencies[selectedCurrencyIndex]}";
        }

        private int CalculateFirstWholeNumberThreshold(float pricePerItem)
        {
            int thresholdStackSize = 1;
            while (!IsWholeNumber(thresholdStackSize * pricePerItem))
            {
                thresholdStackSize++;
            }
            return thresholdStackSize;
        }

        private int CalculateMaxWholeNumberStackSize(float pricePerItem, int maxStackSize)
        {
            int stackSize = maxStackSize;
            while (stackSize > 1 && !IsWholeNumber(stackSize * pricePerItem))
            {
                stackSize--;
            }
            return stackSize;
        }

        private bool IsWholeNumber(float value)
        {
            return Math.Abs(value % 1) <= (double.Epsilon * 100);
        }

        private float ParsePricePerItem(string input)
        {
            if (input.Contains("/"))
            {
                var parts = input.Split('/');
                if (parts.Length == 2 && float.TryParse(parts[0], out float numerator) && float.TryParse(parts[1], out float denominator) && denominator != 0)
                {
                    return numerator / denominator;
                }
            }
            else if (float.TryParse(input, out float value))
            {
                return value;
            }

            return 1f; // Default value if parsing fails
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
                    stackSize = CalculateMaxWholeNumberStackSize(pricePerItem, stackSize);
                    UpdateOutput();
                }
            }
        }
    }
}
