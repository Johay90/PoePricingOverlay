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
        private string pricePerItemInput = "1";
        private string output = "";
        private bool overlayVisible = false;
        private float unroundedTotalPrice;
        private int roundingDecimalPlace = 2;

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

                if (ImGui.InputInt("Round to Decimal Place", ref roundingDecimalPlace))
                {
                    roundingDecimalPlace = Math.Max(0, roundingDecimalPlace); 
                    UpdateOutput();
                }

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

                ImGui.Text(output);
                
                ImGui.Text($"Unrounded Total Price: {unroundedTotalPrice}");

                if (ImGui.Button("Copy"))
                {
                    ImGui.SetClipboardText(output);
                }

                if (ImGui.Button("Reset"))
                {
                    stackSize = 1;
                    pricePerItemInput = "1"; 
                    pricePerItem = 1.0f;
                    UpdateOutput();
                }
                
                ImGui.End();
            }
        }

        private void UpdateOutput()
        {
            stackSize = AdjustStackSize(pricePerItem, stackSize);
            unroundedTotalPrice = stackSize * pricePerItem;
            int roundedTotalPrice = (int)Math.Round(unroundedTotalPrice);
            output = $"~price {roundedTotalPrice}/{stackSize} {currencies[selectedCurrencyIndex]}";
        }

        private int AdjustStackSize(float pricePerItem, int maxStackSize)
        {
            int bestStackSize = 1;
            float bestPriceDifference = float.MaxValue;

            if (pricePerItem == 1.0f)
            {
                return maxStackSize;
            }

            for (int i = 1; i <= maxStackSize; i++)
            {
                float totalValue = i * pricePerItem;
                float roundedTotalValue = (float)Math.Round(totalValue, roundingDecimalPlace);
                float priceDifference = Math.Abs(roundedTotalValue - totalValue);

                // Checking if the current stack size results in a closer match to a whole number
                if (priceDifference < bestPriceDifference || (priceDifference == bestPriceDifference && i > bestStackSize))
                {
                    bestStackSize = i;
                    bestPriceDifference = priceDifference;
                }
            }
            return bestStackSize;
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

            return 1f;
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
