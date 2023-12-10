using Dragonfly.BaseModule;
using Dragonfly.Engine.Core;
using Dragonfly.Graphics.Math;

namespace Dragonfly.Engine.Test.GraphicTests
{
    public class PhysicalMaterialTest : GraphicsTest
    {
        private CompCamPerspective camera;
        private CompMtlPhysical physicalMaterial;

        public PhysicalMaterialTest()
        {
            Name = "Material Tests: Physical Material";
            TestDurationSeconds = 10.0f;
            EngineUsage = BaseMod.Usage.PhysicalRendering;
        }

        public override void CreateScene()
        {
            BaseMod baseMod = Context.GetModule<BaseMod>();

            /** SCENE CREATION **/
            baseMod.MainPass.ClearValue = new Float4("#37587a");
            baseMod.Settings.Shadows.MaxShadowDistance = 50.0f;
            baseMod.PostProcess.ExposureValue = ExposureHelper.EVSunset;

            // create camera
            camera = new CompCamPerspective(CompTransformStatic.FromLookAt(Context.Scene.Root, new Float3(0, 0.5f, 3.0f), new Float3(0, 0, 0)));
            baseMod.MainPass.Camera = camera;

            // background 
            CompSphericalBackground background = new CompSphericalBackground(Context.Scene.Root, "textures/syferfontein.hdr");
            background.RadianceLod = 1;

            // lights
            CompLightDirectional sun = new CompLightDirectional(CompTransformStatic.FromDirection(Context.Scene.Root, new Float3(-1.0f, -0.9f, -1.0f)), new Float3("#f9f6e0") * ExposureHelper.LuxInDirectSunlight);
            sun.RadiusMeters.Set(sun.RadiusMeters.GetValue() * 10);
            sun.CastShadow = true;
            CompLightHDRI backgroundLight = new CompLightHDRI(Context.Scene.Root, "textures/syferfontein.hdr");

            // create rotating node
            CompTransformDynamic rotation = CompTransformDynamic.RotationY(Context.Scene.Root, () => 0.2f * Context.Time.SecondsFromStart.ToFloat());

            // create obj mesh
            CompMeshList model = new CompMeshList(rotation);
            physicalMaterial = new CompMtlPhysical(model);
            physicalMaterial.Albedo.Value = new Float3("#051525");
            physicalMaterial.Roughness.Value = 0;
            model.AddMesh("models/material_display.obj", physicalMaterial.DisplayIn(baseMod.MainPass));

            // create material properties UI
            CompUiWindow materialWindow = new CompUiWindow(baseMod.UiContainer, "30em 20em", "10px");
            materialWindow.Title = "Physical Material";
            {
                // AlbedoMultiplier
                CompUiCtrlLabel lblAlbedo = new CompUiCtrlLabel(materialWindow, "Albedo Multiplier: ");
                CompUiCtrlColorSwatch colAlbedo = new CompUiCtrlColorSwatch(materialWindow);
                colAlbedo.SelectedColor.Set(physicalMaterial.Albedo);
                CompActionOnChange.MonitorValue(colAlbedo.SelectedColor, color => physicalMaterial.Albedo.Value = color);

                // Roughness
                CompUiCtrlLabel lblRoughMul = new CompUiCtrlLabel(materialWindow, "Roughness: ");
                CompUiCtrlLabel lblRoughDisplay = new CompUiCtrlLabel(materialWindow, "");
                CompUiCtrlSlider sliderRough = new CompUiCtrlSlider(materialWindow);
                sliderRough.Percent = physicalMaterial.Roughness;
                CompActionOnChange.MonitorValue(sliderRough.Value, roughness =>
                {
                    physicalMaterial.Roughness.Value = roughness;
                    lblRoughDisplay.Text.Set(((int)(roughness * 100.0f)).ToString() + "%");
                }).Execute();
                
                // SpecularColor
                CompUiCtrlLabel lblSpecular = new CompUiCtrlLabel(materialWindow, "Specular color: ");
                CompUiCtrlColorSwatch colSpecular = new CompUiCtrlColorSwatch(materialWindow);
                colSpecular.SelectedColor.Set(physicalMaterial.Specular);
                CompActionOnChange.MonitorValue(colSpecular.SelectedColor, color => physicalMaterial.Specular.Value = color);
                
                // Position all controls
                UiGridLayout layout = new UiGridLayout(materialWindow, 10, 3, UiPositioning.Inside(materialWindow, "0em 1em"));
                layout.SetRowHeight("2em");
                layout.SetColumnWidth(0, "9em");
                layout.SetColumnWidth(1, "11em");
                layout.SetColumnWidth(2, "5em");
                layout[0, 0] = lblAlbedo;
                layout[0, 1] = colAlbedo;
                layout[1, 0] = lblRoughMul;
                layout[1, 1] = sliderRough;
                layout[1, 2] = lblRoughDisplay;
                layout[2, 0] = lblSpecular;
                layout[2, 1] = colSpecular;
                layout.Apply();
            }
            materialWindow.Show();
        }
    }
}
