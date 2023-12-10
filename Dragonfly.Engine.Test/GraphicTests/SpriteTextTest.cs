using Dragonfly.BaseModule;
using Dragonfly.Graphics.Math;
using System;

namespace Dragonfly.Engine.Test.GraphicTests
{
    public class SpriteTextTest : GraphicsTest
    {
        CompUiCtrlLabel renderModeDescr;

        public SpriteTextTest()
        {
            EngineUsage = BaseMod.Usage.Generic3D;
            Name = "Component Tests: Text Rendering";
        }

        public override void CreateScene()
        {
            BaseMod baseMod = Context.GetModule<BaseMod>();
            baseMod.MainPass.ClearValue = new Float4("#ffffff");

            // add test labels           
            FRandom rand = new FRandom();
            UiHeight fontSize = "10%";
            UiCoords fontPos = "0% 10%";
            CompUiCtrlLabel curLbl = null;

            CoordContext.Push(baseMod.UiContainer.Coords);
            for (int i = 0; i < 15; i++)
            {
                curLbl = new CompUiCtrlLabel(baseMod.UiContainer, "The quick brown fox jumps over the lazy dog", fontPos);
                curLbl.Font.Color = i < 8 ? rand.NextSatColor() : Float3.Zero;
                curLbl.Font.Size = fontSize;
                fontPos = fontPos + (UiSize)"5% 10%";
                fontSize.Value = fontSize.Value * 0.9f;
            }
            CoordContext.Pop();

            curLbl = new CompUiCtrlLabel(baseMod.UiContainer, "Absolute positioned and sized text", new UiCoords(5, 100));
            curLbl.Font.Size = "32px";
            curLbl.Font.Color = Float3.Zero;
            curLbl = new CompUiCtrlLabel(baseMod.UiContainer, "Title Text is 3em", "5 200");
            curLbl.Font.Size = "3em";
            curLbl.Font.Color = Float3.Zero;
            curLbl = new CompUiCtrlLabel(baseMod.UiContainer, "Body text is 1em", "5 300");
            curLbl.Font.Size = "1em";
            curLbl.Font.Color = Float3.Zero;

            renderModeDescr = new CompUiCtrlLabel(baseMod.UiContainer, "", new UiCoords(5, 20));
            renderModeDescr.Font.Size = "32px";
            renderModeDescr.Font.Color = Float3.Zero;

            UpdateRendermodeLabel();

            // add key toggle component
            new CompActionOnEvent(new CompEventKeyPressed(Context.Scene.Root,  Utils.VKey.K_M).Event, ToggleMode);
        }

        private void UpdateRendermodeLabel()
        {
            BaseMod baseMod = Context.GetModule<BaseMod>();
            renderModeDescr.Text.Set(string.Format("Render mode: {0}, press M to change mode.", baseMod.UiContainer.TextRenderMode.ToString()));
        }

        private void ToggleMode()
        {
            BaseMod baseMod = Context.GetModule<BaseMod>();

            int textModeCount = Enum.GetValues(typeof(TextRenderMode)).Length;
            baseMod.UiContainer.TextRenderMode = (TextRenderMode)(((int)baseMod.UiContainer.TextRenderMode + 1) % textModeCount);

            UpdateRendermodeLabel();
        }
    }
}
