using System;
using Dragonfly.BaseModule;
using Dragonfly.Graphics.Math;


namespace Dragonfly.Engine.Test.GraphicTests
{
    public class UiTest : GraphicsTest
    {
        public UiTest()
        {
            Name = "Component Tests: Ui System";
            EngineUsage = BaseMod.Usage.Generic3D;
            TestDurationSeconds = 10.0f;
        }

        public override void CreateScene()
        {
            BaseMod baseMod = Context.GetModule<BaseMod>();
            baseMod.MainPass.ClearValue = new Float4("#84c4ef");

            // add a test window with controls
            CompUiWindow testWnd = new CompUiWindow(baseMod.UiContainer, "500px 600px", "50px 50px", PositionOrigin.TopLeft);
            {
                testWnd.Title = Name;

                // add controls
                CompUiCtrlLabel lblTest1 = new CompUiCtrlLabel(testWnd, "Open new Window", UiPositioning.Inside(testWnd, "0em 1em"));
                CompUiCtrlButton btnTest = new CompUiCtrlButton(testWnd, UiPositioning.RightOf(lblTest1), "Albedo calculator");
                new CompActionOnEvent(btnTest.Clicked, NewWindowBtn_Click);
                UiPositioning.AlignCenterVertically(lblTest1, btnTest);

                CompUiCtrlLabel lblTest2 = new CompUiCtrlLabel(testWnd, "Test slider: ", UiPositioning.Inside(testWnd, "0em 4em"));
                CompUiCtrlSlider sdrTest = new CompUiCtrlSlider(testWnd, UiPositioning.RightOf(lblTest2), "10em 2em", 0, 1);
                UiPositioning.AlignCenterVertically(lblTest2, sdrTest);

                CompUiCtrlLabel lblTest3 = new CompUiCtrlLabel(testWnd, "Test color picker: ", UiPositioning.Inside(testWnd, "0em 7em"));
                CompUiCtrlColorPicker pickerTest = new CompUiCtrlColorPicker(testWnd, UiPositioning.RightOf(lblTest3));

                CompUiCtrlLabel lblTest4 = new CompUiCtrlLabel(testWnd, "Test color swatch: ", UiPositioning.Inside(testWnd, "0em 18em"));
                CompUiCtrlColorSwatch colorSwatchTest = new CompUiCtrlColorSwatch(testWnd, UiPositioning.RightOf(lblTest4));
                UiPositioning.AlignCenterVertically(lblTest4, colorSwatchTest);
                
                CompUiCtrlLabel lblTest5 = new CompUiCtrlLabel(testWnd, "Test checkbox: ", UiPositioning.Inside(testWnd, "0em 21em"));
                CompUiCtrlCheckbox chkTest1 = new CompUiCtrlCheckbox(testWnd, UiPositioning.RightOf(lblTest5));
                UiPositioning.AlignCenterVertically(lblTest5, chkTest1);

                CompUiCtrlLabel lblTest6 = new CompUiCtrlLabel(testWnd, "Test tex input: ", UiPositioning.Inside(testWnd, "0em 24em"));
                CompUiCtrlTextInput txtTest1 = new CompUiCtrlTextInput(testWnd, "Write text here!", UiPositioning.RightOf(lblTest6));
                UiPositioning.AlignCenterVertically(lblTest6, txtTest1);

                CompUiCtrlLabel lblTest7 = new CompUiCtrlLabel(testWnd, "Test graph: ", UiPositioning.Inside(testWnd, "0em 27em"));
                CompUiCtrlGraph testGraph = new CompUiCtrlGraph(testWnd, UiPositioning.RightOf(lblTest7), "20em 8em");
                testGraph.TracesAlpha = (Float3)1.0f;
                testGraph.FillAlpha = (Float3)0.4f;
                for (int i = 0; i < CompUiCtrlGraph.MaxDataPoints; i++)
                {
                    float x = i / 5.0f;
                    testGraph.AddDataPoint(x, FMath.Sin(x), FMath.Sin(x + FMath.TWO_PI / 3.0f), FMath.Sin(x + 2.0f * FMath.TWO_PI / 3.0f));
                }
            }
            testWnd.Show();

            // add another window to test focus
            CompUiWindow testFocusWnd = new CompUiWindow(baseMod.UiContainer, "500px 400px", "400px 300px", PositionOrigin.Center);
            testFocusWnd.Title = "Test focus and Z-Index";
            testFocusWnd.Show();
        }


        private void NewWindowBtn_Click()
        {
            FRandom rnd = new FRandom();
            CompUiWindow newWin = new CompUiWindow(Context.GetModule<BaseMod>().UiContainer, "500px 400px", new UiCoords(rnd.NextFloat2(Float2.Zero, Float2.One), UiUnit.Percent));
            newWin.Title = "Albedo calculator " + newWin.ID;

            CompUiCtrlLabel lblHexColor = new CompUiCtrlLabel(newWin, "Color HEX: ", UiPositioning.Inside(newWin, "0em 1em"));
            CompUiCtrlTextInput txtHexColor = new CompUiCtrlTextInput(newWin, "#00FF00", UiPositioning.RightOf(lblHexColor));
            CompUiCtrlButton btnSetColor = new CompUiCtrlButton(newWin, UiPositioning.RightOf(txtHexColor, "1em"), "SetColor");
            UiPositioning.AlignCenterVertically(lblHexColor, txtHexColor, btnSetColor);

            CompUiCtrlColorSwatch colorSwatch = new CompUiCtrlColorSwatch(newWin, UiPositioning.Inside(newWin, "0em 4em"));
            CompUiCtrlLabel lblAlbedo = new CompUiCtrlLabel(newWin, "Albedo: N/A", UiPositioning.Inside(newWin, "0em 7em"));

            new CompActionOnEvent(btnSetColor.Clicked, () => colorSwatch.SelectedColor.Set(new Float3(txtHexColor.Text)));
            CompActionOnChange.MonitorValue<Float3>(colorSwatch.SelectedColor, (color) => lblAlbedo.Text.Set("Albedo: " + GetAlbedo(color)));

            newWin.Show();
        }

        private float GetAlbedo(Float3 color)
        {
            return Color.GetLuminanceFromRGB(SRGB.Decode(color));
        }
    }
}
