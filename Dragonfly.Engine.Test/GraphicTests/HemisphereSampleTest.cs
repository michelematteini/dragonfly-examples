using Dragonfly.BaseModule;
using Dragonfly.Engine.Core;
using Dragonfly.Graphics.Math;
using System;

namespace Dragonfly.Engine.Test.GraphicTests
{
    public class HemisphereSampleTest : GraphicsTest
    {
        private CompMesh samplesMesh;
        private CompUiCtrlSlider sampleCountSlider;
        private CompUiCtrlCheckbox bothHemispheresCheck;

        public HemisphereSampleTest()
        {
            Name = "Basic Tests: Random sample on hemisphere";
            EngineUsage = BaseModule.BaseMod.Usage.Generic3D;
        }

        public override void CreateScene()
        {
            Component root = Context.Scene.Root;
            BaseMod baseMod = Context.GetModule<BaseMod>();
            CompRenderPass mainPass = Context.GetModule<BaseMod>().MainPass;
            mainPass.ClearValue = new Float4("#e3f3f9");

            AddDebugInfoWindow();
            
            // add a camera
            mainPass.Camera = new CompCamPerspective(new CompTransformEditorMovement(root, new Float3(0, 3.0f, -3.0f), Float3.Zero));

            // prepare a ground plane with an hemisphere in the middle
            CompMesh sphereMesh = new CompMesh(root);
            IObject3D sphereMeshObj = sphereMesh.AsAsyncObject3D();
            Primitives.Spheroid(sphereMeshObj, Float3.Zero, (Float3)2.0f, 5000);
            CompMtlMasking sphereMat = new CompMtlMasking(sphereMesh, Color.White.ToFloat3());
            sphereMat.ShadingEnabled.Value = true;
            sphereMesh.MainMaterial = sphereMat;

            // prepare a mesh which highlights the samples
            samplesMesh = new CompMesh(root);
            samplesMesh.MainMaterial = new CompMtlBasic(samplesMesh, Color.Red.ToFloat3());

            // diplay test params window
            CompUiWindow testParamsWindow = new CompUiWindow(baseMod.UiContainer, "25em 12em", UiPositioning.Below(TestResults.Window, "0.5em"));
            testParamsWindow.Title = Name;
            {
                // Number of samples:
                CompUiCtrlLabel lblSampleCount = new CompUiCtrlLabel(testParamsWindow, "Num. of samples: ", UiPositioning.Inside(testParamsWindow, "0em 1em"));
                sampleCountSlider = new CompUiCtrlSlider(testParamsWindow, UiPositioning.RightOf(lblSampleCount));
                sampleCountSlider.Width = "12em";
                sampleCountSlider.MinValue = 1;
                sampleCountSlider.MaxValue = 32;
                sampleCountSlider.Percent = 0.5f;
                UiPositioning.AlignCenterVertically(lblSampleCount, sampleCountSlider);
                CompActionOnChange.MonitorValue(sampleCountSlider.Value, nsamples => UpdateSamples());

                // sample full sphere
                CompUiCtrlLabel lblBothHemispere = new CompUiCtrlLabel(testParamsWindow, "Sample full sphere: ", UiPositioning.Below(lblSampleCount, "2em"));
                bothHemispheresCheck = new CompUiCtrlCheckbox(testParamsWindow, UiPositioning.RightOf(lblBothHemispere));
                UiPositioning.AlignCenterVertically(lblBothHemispere, bothHemispheresCheck);
                new CompActionOnEvent(bothHemispheresCheck.CheckedChanged, UpdateSamples);
            }

            UpdateSamples();
            testParamsWindow.Show();
        }

        private void UpdateSamples()
        {
            int nSamples = (int)sampleCountSlider.Value.GetValue();
            nSamples *= nSamples; // grow samples quadratically
            bool bothHemispheres = bothHemispheresCheck.Checked;

            IObject3D samplesMeshObj = samplesMesh.AsAsyncObject3D();
            for (int i = 0; i < nSamples; i++)
            {
                float x = (float)i / Math.Max(1, nSamples - 1) * (bothHemispheres ? 2.0f : 1.0f);

                float upAngleUpper = FMath.PI_OVER_2 * FMath.Sqrt(x);
                float upAngleLower = FMath.PI_OVER_2 * (2.0f - FMath.Sqrt(2.0f - x));
                float upAngle = (bothHemispheres && x > 1.0f) ? upAngleLower : upAngleUpper;
                
                float yRotAngle = i * FMath.PHI * FMath.TWO_PI;

                Float3 samplePos = new Float3(FMath.Sin(upAngle) * FMath.Cos(yRotAngle), FMath.Cos(upAngle), FMath.Sin(upAngle) * FMath.Sin(yRotAngle));
                Primitives.Spheroid(samplesMeshObj, samplePos, (Float3)0.05f, 50);
            }
        }
    }
}
