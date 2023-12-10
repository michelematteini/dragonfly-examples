using Dragonfly.BaseModule;
using Dragonfly.Engine.Core;
using Dragonfly.Graphics.Math;
using Dragonfly.Terrain;

namespace Dragonfly.Engine.Test.GraphicTests
{
    public class TerrainTest : GraphicsTest
    {
        private const double TERRAIN_SIZE = 1048576.0;
        private const float TERRAIN_HEIGHT = 9000.0f;
        private const int TESSELLATION = 16;
        private const float MAX_VERTEX_DENSITY = 64.0f;

        CompTerrainLODUpdater terrainLODUpdater;
        private CompTerrain terrain;
        private CompUiCtrlCheckbox lodCheck, wireframeCheck;
        
        public TerrainTest()
        {
            Name = "Component Tests: Terrain Component";
            EngineUsage = BaseMod.Usage.PhysicalRendering;
            TestDurationSeconds = 25.0f;
        }

        public override void CreateScene()
        {
            Component root = Context.Scene.Root;
            BaseMod baseMod = Context.GetModule<BaseMod>();
            baseMod.Settings.Shadows.MaxShadowDistance = 20000;
            baseMod.Settings.Shadows.MaxOccluderDistance = 40000;
            baseMod.Settings.Shadows.CascadePerFrameCount = 2;
            baseMod.PostProcess.ExposureValue = ExposureHelper.EVSunset;
            CompRenderPass mainPass = Context.GetModule<BaseMod>().MainPass;
            mainPass.ClearValue = new Float4("#e3f3f9");
            baseMod.PostProcess.Fog.Enabled = true;
            baseMod.PostProcess.Fog.Color.Value = new Float3(1.0f, 0.85f, 0.6f) * ExposureHelper.EVToLux(12.75f);
            baseMod.PostProcess.Fog.Multiplier.Value = 0.0002f;
            baseMod.PostProcess.Fog.GradientCoeff = 0.001f;

            AddDebugInfoWindow();

            // add a camera
            Float3 camPos = new Float3(0, TERRAIN_HEIGHT, 0);
            CompTransformEditorMovement cameraController = new CompTransformEditorMovement(root, camPos, camPos + new Float3(30, 0, -100), 2.0f);
            cameraController.Movement.SpeedMps.Set(new CompCumulativeMouseWheel(cameraController, 1200.0f)); // camera speed can be changed with the mouse wheel
            mainPass.Camera = new CompCamPerspective(cameraController) { FarPlane = float.PositiveInfinity };

            // background 
            CompSphericalBackground background = new CompSphericalBackground(Context.Scene.Root, "textures/kloppenheim.hdr");

            // add lights
            CompLightDirectional sun = new CompLightDirectional(CompTransformStatic.FromDirection(root, new Float3(1.0f, -0.5f, 1.0f)), new Float3("#f2a160") * ExposureHelper.LuxAtSunset);
            sun.CastShadow = true;
            CompLightHDRI backgroundLight = new CompLightHDRI(Context.Scene.Root, "textures/kloppenheim.hdr");

            // add terrain component to be tested
            {
                TiledRect3 terrainArea = new TiledRect3(new Float3(-0.5f, 0, -0.5f) * new TiledFloat(TERRAIN_SIZE), Float3.UnitX, Float3.UnitZ, (Float2)TERRAIN_SIZE);
                FractalDataSourceParams fractalParams = new FractalDataSourceParams();
                fractalParams.AlbedoLUTPath = "textures/terrain/lut/terrainAlbedo1.png";
                fractalParams.PeaksMaxHeightMeters = TERRAIN_HEIGHT;
                fractalParams.OceanMaxDepthMeters = TERRAIN_HEIGHT;
                fractalParams.FeaturesMaxExtensionMeters = 4000.0f;
                fractalParams.PeaksPercent = 0.2f;
                fractalParams.Seed = 16;
                CompFractalDataSource terrainData = new CompFractalDataSource(root, fractalParams, (Int2)256, TESSELLATION, MAX_VERTEX_DENSITY);

                // use a distance based lod, which use an estimation of the future position to update the terrain
                CompFutureWorldPosition lodPosition = new CompFutureWorldPosition(cameraController, cameraController.Movement.Position);
                DistanceLOD terrainLOD = new DistanceLOD(lodPosition);
                terrainLOD.MaxVertexDensity = MAX_VERTEX_DENSITY;
                terrainLOD.DensityModifiers.Add(new DistanceLODHeightModifier()); // improve density on slopes
                terrainLODUpdater = new CompTerrainLODUpdater(root, terrainLOD);
                lodPosition.DeltaTime.Set(new CompFunction<float>(lodPosition, () => 2.0f * terrainLODUpdater.MinLodSwitchTimeSeconds));

                // material factory
                TerrainPhysicalMaterialFactory mfactory = new TerrainPhysicalMaterialFactory(terrainData);
                mfactory.SetDetail("mossgravel1");

                // create the terrain
                TerrainParams terrainParams = new TerrainParams()
                {
                    Area = terrainArea,
                    LodUpdater = terrainLODUpdater,
                    DataSource = terrainData.Source,
                    MaterialFactory = mfactory
                };
                terrain = new CompTerrain(root, terrainParams);
            }

            // add UI with test commands
            CompUiWindow testWnd = new CompUiWindow(baseMod.UiContainer, "300 150", UiPositioning.Below(TestResults.Window, "10px"));
            testWnd.Title = this.Name;
            {
                CompUiCtrlLabel wireframeLabel = new CompUiCtrlLabel(testWnd, "Wireframe mode:");
                wireframeCheck = new CompUiCtrlCheckbox(testWnd, "0");
                new CompActionOnEvent(wireframeCheck.CheckedChanged, ToggleWireframe);

                CompUiCtrlLabel lodLabel = new CompUiCtrlLabel(testWnd, "LOD Updates:");
                lodCheck = new CompUiCtrlCheckbox(testWnd, "0", true);
                new CompActionOnEvent(lodCheck.CheckedChanged, ToggleLODUpdates);

                // Position all controls
                UiGridLayout layout = new UiGridLayout(testWnd, 10, 3, UiPositioning.Inside(testWnd, "0em 1em"));
                layout.SetRowHeight("2em");
                layout.SetColumnWidth(0, "9em");
                layout.SetColumnWidth(1, "11em");
                layout.SetColumnWidth(2, "5em");
                layout[0, 0] = wireframeLabel;
                layout[0, 1] = wireframeCheck;
                layout[1, 0] = lodLabel;
                layout[1, 1] = lodCheck;
                layout.Apply();
            }
            testWnd.Show();

            // add background audio
            CompAudio windBg = new CompAudio(root, "audio/windy_loop_1.wav");
            windBg.Effects.Add(new CompAudioFxDirGradient(windBg, Float3.Zero, -40.0f, Float3.UnitY * 1200.0f, 0));
            windBg.Effects.Add(new CompAudioFxVolumeRnd(windBg, 0.4f, -12.0f, 0.0f));
            windBg.PlayLoop();
        }

        private void ToggleWireframe()
        {
            terrain.WireframeModeEnabled = wireframeCheck.Checked;
        }

        private void ToggleLODUpdates()
        {
            terrainLODUpdater.FreezeLOD = !lodCheck.Checked;
        }
    }
}
