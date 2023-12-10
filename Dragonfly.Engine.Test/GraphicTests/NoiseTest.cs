using Dragonfly.BaseModule;
using Dragonfly.Engine.Core;
using Dragonfly.Graphics.Math;
using System.Collections.Generic;

namespace Dragonfly.Engine.Test.GraphicTests
{
    class NoiseTest : GraphicsTest
    {
        CompMtlNoiseTest m;

        public NoiseTest()
        {
            Name = "Shader Tests: Noise Test";
            EngineUsage = BaseMod.Usage.Generic3D;
        }

        public override void CreateScene()
        {
            BaseMod baseMod = Context.GetModule<BaseMod>();
            baseMod.PostProcess.Tonemapping.Type = TonemappingType.Linear;
            CompRenderPass mainView = baseMod.MainPass;
            mainView.ClearValue = new Float4("#84c4ef");

            // create screen quad with a material to display the noise
            CompMesh screenQuad = BaseMod.CreateScreenMesh(Context.Scene.Root);
            m = new CompMtlNoiseTest(screenQuad);
            m.Distribution = new GpuNoiseDistribution() { StartAmplitude = 1.0f, AmplitudeMul = 0.5f, StartOctave = -6, EndOctave = -6 };
            screenQuad.Materials.Add(m.DisplayIn(baseMod.ToScreenPass));

            // create an ui to select the distribution
            CompUiWindow noiseEditWnd = new CompUiWindow(baseMod.UiContainer, "20em 32em", "10px 10px") { Title = "Noise Settings" };
            UiGridLayout wndLayout = new UiGridLayout(noiseEditWnd, 14, 2, UiPositioning.Inside(noiseEditWnd, "0 1em"));
            wndLayout.SetRowHeight("1.8em");
            wndLayout[0, 0] = new CompUiCtrlLabel(noiseEditWnd, "Start Octave:");
            CompUiCtrlSlider startOctaveSlider = new CompUiCtrlSlider(noiseEditWnd, UiCoords.Zero, -10, 0);
            startOctaveSlider.Percent = startOctaveSlider.GetPercentFromValue(m.Distribution.StartOctave);
            CompActionOnChange.MonitorValue(startOctaveSlider.Value, startOctave => m.Distribution.StartOctave = (int)System.Math.Min(startOctave, m.Distribution.EndOctave));
            wndLayout[0, 1] = startOctaveSlider;
            wndLayout[1, 0] = new CompUiCtrlLabel(noiseEditWnd, "End Octave:");
            CompUiCtrlSlider endOctaveSlider = new CompUiCtrlSlider(noiseEditWnd, UiCoords.Zero, -10, 0);
            endOctaveSlider.Percent = startOctaveSlider.GetPercentFromValue(m.Distribution.EndOctave);
            CompActionOnChange.MonitorValue(endOctaveSlider.Value, endOctave => m.Distribution.EndOctave = (int)System.Math.Max(endOctave, m.Distribution.StartOctave));
            wndLayout[1, 1] = endOctaveSlider;
            CompUiCtrlCheckbox valueNoiseCheck = new CompUiCtrlCheckbox(noiseEditWnd, UiCoords.Zero, true) { Text = "Value Noise" };
            CompActionOnChange.MonitorValue(valueNoiseCheck.Value, (value) => m.NoiseType = value ? 0 : m.NoiseType);
            wndLayout[2, 0] = valueNoiseCheck;
            CompUiCtrlCheckbox perlinNoiseCheck = new CompUiCtrlCheckbox(noiseEditWnd, UiCoords.Zero, false) { Text = "Perlin Noise" };
            CompActionOnChange.MonitorValue(perlinNoiseCheck.Value, value => m.NoiseType = value ? 1 : m.NoiseType);
            wndLayout[3, 0] = perlinNoiseCheck;
            CompUiCtrlCheckbox simplexNoiseCheck = new CompUiCtrlCheckbox(noiseEditWnd, UiCoords.Zero, false) { Text = "Simplex Noise" };
            CompActionOnChange.MonitorValue(simplexNoiseCheck.Value, value => m.NoiseType = value ? 2 : m.NoiseType);
            wndLayout[4, 0] = simplexNoiseCheck;
            CompUiCtrlCheckbox terraNoiseCheck = new CompUiCtrlCheckbox(noiseEditWnd, UiCoords.Zero, false) { Text = "Terra Noise" };
            CompActionOnChange.MonitorValue(terraNoiseCheck.Value, value => m.NoiseType = value ? 3 : m.NoiseType);
            wndLayout[5, 0] = terraNoiseCheck;
            CompUiCtrlCheckbox marbleNoiseCheck = new CompUiCtrlCheckbox(noiseEditWnd, UiCoords.Zero, false) { Text = "Marble Noise" };
            CompActionOnChange.MonitorValue(marbleNoiseCheck.Value, value => m.NoiseType = value ? 4 : m.NoiseType);
            wndLayout[6, 0] = marbleNoiseCheck;
            CompUiCtrlCheckbox mridgeNoiseCheck = new CompUiCtrlCheckbox(noiseEditWnd, UiCoords.Zero, false) { Text = "Peak Noise" };
            CompActionOnChange.MonitorValue(mridgeNoiseCheck.Value, value => m.NoiseType = value ? 5 : m.NoiseType);
            wndLayout[7, 0] = mridgeNoiseCheck;
            CompUiCtrlCheckbox sparseNoiseCheck = new CompUiCtrlCheckbox(noiseEditWnd, UiCoords.Zero, false) { Text = "Sparse Noise" };
            CompActionOnChange.MonitorValue(sparseNoiseCheck.Value, value => m.NoiseType = value ? 6 : m.NoiseType);
            wndLayout[8, 0] = sparseNoiseCheck;
            CompUiCtrlCheckbox.Group(valueNoiseCheck, perlinNoiseCheck, simplexNoiseCheck, terraNoiseCheck, marbleNoiseCheck, mridgeNoiseCheck, sparseNoiseCheck);


            CompUiCtrlCheckbox valueOutputCheck = new CompUiCtrlCheckbox(noiseEditWnd, UiCoords.Zero, true) { Text = "Show Value" };
            CompActionOnChange.MonitorValue(valueOutputCheck.Value, value => { if (value) m.Output = DFXVariants.NoiseTestOutputType.ValueOutput; });
            wndLayout[10, 0] = valueOutputCheck;

            CompUiCtrlCheckbox normalsOutputCheck = new CompUiCtrlCheckbox(noiseEditWnd, UiCoords.Zero, false) { Text = "Show Normals" };
            CompActionOnChange.MonitorValue(normalsOutputCheck.Value, value => { if (value) m.Output = DFXVariants.NoiseTestOutputType.NormalsOutput; });
            wndLayout[11, 0] = normalsOutputCheck;

            CompUiCtrlCheckbox slopeOutputCheck = new CompUiCtrlCheckbox(noiseEditWnd, UiCoords.Zero, false) { Text = "Show Slope" };
            CompActionOnChange.MonitorValue(slopeOutputCheck.Value, value => { if (value) m.Output = DFXVariants.NoiseTestOutputType.SlopeOutput; });
            wndLayout[12, 0] = slopeOutputCheck;

            CompUiCtrlCheckbox.Group(valueOutputCheck, normalsOutputCheck, slopeOutputCheck);


            CompUiCtrlCheckbox timeNoiseCheck = new CompUiCtrlCheckbox(noiseEditWnd, UiCoords.Zero, false) { Text = "Pause Time" };
            CompActionOnChange.MonitorValue(timeNoiseCheck.Value, value => Context.Time.TimeFlowRate = value ? 0.0f : 1.0f);
            wndLayout[13, 0] = timeNoiseCheck;

            wndLayout.Apply();
            noiseEditWnd.Show();
        }
    }

    class CompMtlNoiseTest : CompMaterial
    {
        private DFXVariants.NoiseTestOutputType outType;

        public CompMtlNoiseTest(Component parent) : base(parent)
        {
            UpdateEachFrame = true;
        }

        public override string EffectName => "NoiseTest";

        public DFXVariants.NoiseTestOutputType Output
        {
            get
            {
                return outType;
            }
            set
            {
                outType = value;
                SetVariantValue(DFXVariants.NoiseTestOutputType.Name, outType.Value);
            }
        }

        public int NoiseType { get; set; }

        public GpuNoiseDistribution Distribution;

        protected override void UpdateParams()
        {
            Shader.SetParam("noiseType", NoiseType);
            GpuNoiseDistribution distr = Distribution;
            if (NoiseType == 6)
                distr.AmplitudeMul = 1.0f;
            distr.Normalize();
            distr.SetToShader("noiseDistr", Shader);
        }
    }



}
