using Dragonfly.BaseModule;
using Dragonfly.Engine.Core;
using Dragonfly.Graphics.Math;
using Dragonfly.Terrain;
using System;
using System.Collections.Generic;

namespace Dragonfly.Engine.Test.GraphicTests
{
    public class PlanetTest : GraphicsTest
    {
        private const int TESSELLATION = 16;
        private const float MAX_VERTEX_DENSITY = 64.0f;

        private List<CompPlanet> planetAndMoons;
        private CompTerrainLODUpdater terrainLODUpdater;
        private CompUiCtrlCheckbox lodCheck, wireframeCheck;
        private CompUiCtrlLabel altitudeLabel, planetNameLabel, speedLabel, radiusLabel;
        private CompUiWindow testWnd;
        private BaseMod baseMod;
        private CompCamPerspective camera;
        private CompPlanetGravityFrame gravityFrame;

        public PlanetTest()
        {
            Name = "Component Tests: Planet Component";
            EngineUsage = BaseMod.Usage.PhysicalRendering;
        }

        public override void CreateScene()
        {
            Component root = Context.Scene.Root;
            baseMod = Context.GetModule<BaseMod>();
            baseMod.Settings.Shadows.MaxShadowDistance = 20000;
            baseMod.Settings.Shadows.MaxOccluderDistance = 40000;
            baseMod.Settings.Shadows.CascadePerFrameCount = 2;
            baseMod.PostProcess.ExposureValue = ExposureHelper.EVClody;
            baseMod.PostProcess.Starfield.Enabled = true;
            CompRenderPass mainPass = Context.GetModule<BaseMod>().MainPass;
            mainPass.ClearValue = Float4.Zero;

            AddDebugInfoWindow();

            // add a camera
            TiledFloat3 camPos = new TiledFloat3(Float3.Zero, new Int3(20000, 0, -160000));
            CompTransformEditorMovement cameraController = new CompTransformEditorMovement(root, camPos, Float3.Zero, 4.0f);
            cameraController.Movement.SpeedMps.Set(new CompCumulativeMouseWheel(cameraController, 120000.0f)); // camera speed can be changed with the mouse wheel
            gravityFrame = new CompPlanetGravityFrame(cameraController, Float3.UnitY);
            CompTimeSmoothing<Float3> planetUp = new CompTimeSmoothing<Float3>(cameraController, 5.0f, Float3.UnitY, (v1, v2, alpha) => FMath.Normalize(FMath.Lerp(v1, v2, alpha)));
            planetUp.OverrideTargetFunction = () => gravityFrame.UpVector;
            cameraController.UpVector.Set(planetUp); // up vector will react to a near planet gravity
            cameraController.Movement.Modifiers.Add(gravityFrame); // let the gravity correct the orbit around planets
            camera = new CompCamPerspective(cameraController) { FarPlane = float.PositiveInfinity };
            mainPass.Camera = camera;

            // add lights
            CompLightDirectional sun = new CompLightDirectional(CompTransformStatic.FromDirection(root, new Float3(1.0f, -0.05f, 1.0f)), new Float3("#ffffff") * ExposureHelper.LuxAtSunset);
            sun.CastShadow = true;
            baseMod.PostProcess.Flares.Lights.Add(sun);


            // add planets and moons
            {
                planetAndMoons = new List<CompPlanet>();
                FractalPlanetFactory planetFactory = new FractalPlanetFactory(Context.GetResourcePath("data/planet_templates"));

                // initialize the terrain LOD updater with distance-based LOD, which use an estimation of the future position to update the terrain
                {
                    CompFutureWorldPosition lodPosition = new CompFutureWorldPosition(cameraController, cameraController.Movement.Position);
                    DistanceLOD terrainLOD = new DistanceLOD(lodPosition);
                    terrainLOD.MaxVertexDensity = MAX_VERTEX_DENSITY;
                    terrainLOD.DensityModifiers.Add(new DistanceLODSlopeModifier()); // improve density on slopes
                    terrainLODUpdater = new CompTerrainLODUpdater(root, terrainLOD);
                    // future position is based on the time it takes to change LOD
                    lodPosition.DeltaTime.Set(new CompFunction<float>(lodPosition, () => 2.0f * terrainLODUpdater.MinLodSwitchTimeSeconds));
                }

                // create a baker pool for the terrain tiles
                BakerScreenSpacePool bakers = new BakerScreenSpacePool();

                // create the main planet
                ulong exoplanetSeed = 849;
                AddPlanet("Exoplanet A" + exoplanetSeed, planetFactory, planetFactory.CreateSeed(exoplanetSeed, "earth1"), root, bakers, TiledFloat3.Zero);

                // create moons
                {
                    TiledFloat3 planetUnitVector = new TiledFloat3() { X = planetAndMoons[0].Radius, Z = planetAndMoons[0].Radius };
                    /*A*/AddPlanet("Moon Alpha", planetFactory, planetFactory.CreateSeed(100, "moonA"), root, bakers, planetUnitVector * new Float3(2.5f, 0.0f, 1.0f));
                    /*B*/AddPlanet("Moon Beta", planetFactory, planetFactory.CreateSeed(210, "moonB"), root, bakers, planetUnitVector * new Float3(-3.6f, 0.0f, 3.5f));
                    /*C*/AddPlanet("Moon Gamma", planetFactory, planetFactory.CreateSeed(308, "moonC"), root, bakers, planetUnitVector * new Float3(-5.2f, 0.0f, -6.8f));
                    /*D*/AddPlanet("Moon Delta", planetFactory, planetFactory.CreateSeed(409, "moonD"), root, bakers, planetUnitVector * new Float3(-14.0f, 0.0f, 4.8f));
                    /*E*/AddPlanet("Moon Theta", planetFactory, planetFactory.CreateSeed(500, "moonE"), root, bakers, planetUnitVector * new Float3(5.5f, 0.0f, 16.2f));
                }
            }

            // add UI with test commands
            testWnd = new CompUiWindow(baseMod.UiContainer, "300px 200px", UiPositioning.Below(TestResults.Window, "10px"));
            testWnd.Title = this.Name;
            {
                CompUiCtrlLabel wireframeLabel = new CompUiCtrlLabel(testWnd, "Wireframe mode:");
                wireframeCheck = new CompUiCtrlCheckbox(testWnd, UiCoords.Zero);
                new CompActionOnEvent(wireframeCheck.CheckedChanged, ToggleWireframe);

                CompUiCtrlLabel lodLabel = new CompUiCtrlLabel(testWnd, "LOD Updates:");
                lodCheck = new CompUiCtrlCheckbox(testWnd, "0", true);
                new CompActionOnEvent(lodCheck.CheckedChanged, ToggleLODUpdates);

                CompUiCtrlLabel hideUiLabel = new CompUiCtrlLabel(testWnd, "Press 'U' to hide the UI");
                
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
                layout[2, 0] = hideUiLabel;
                layout.Apply();
            }
            testWnd.Show();

            // add a key binding to toggle ui visibility
            new CompActionOnEvent(new CompEventKeyPressed(root, Utils.VKey.K_U).Event, ToggleUI);

            // on-screen informations on the current planet
            planetNameLabel = new CompUiCtrlLabel(baseMod.UiContainer, "N/A", "2% 81%");
            planetNameLabel.Font.Size = "4%";
            radiusLabel = new CompUiCtrlLabel(baseMod.UiContainer, "Radius: xxxxx Km", "3% 86%");
            radiusLabel.Font.Size = "3%";
            altitudeLabel = new CompUiCtrlLabel(baseMod.UiContainer, "Altitude: xxxxxxxxxx", "3% 90%");
            altitudeLabel.Font.Size = "3%";
            speedLabel = new CompUiCtrlLabel(baseMod.UiContainer, "Speed: xxxxxxxxxxxxxxxxxxxxx", "3% 94%");
            speedLabel.Font.Size = "3%";

            // component to manage per frame updates
            new OnNewFrame(root, this);
        }

        private void ToggleUI()
        {
            testWnd.Visible = !testWnd.Visible;
            TestResults.Window.Visible = !TestResults.Window.Visible;
        }

        private void AddPlanet(string name, FractalPlanetFactory factory, FractalPlanetFactory.Seed seed, Component root, BakerScreenSpacePool bakers, TiledFloat3 position)
        {
#if TRACING
            System.Diagnostics.Debug.Print("Planet {0} seed: {1}", name, seed.Hash.Value);
#endif

            CompPlanet planet = factory.CreatePlanet(seed, root, position, TESSELLATION, MAX_VERTEX_DENSITY, bakers, terrainLODUpdater);
            planet.Name = name;
            planetAndMoons.Add(planet);
        }

        private void ToggleWireframe()
        {
            foreach (CompPlanet planet in planetAndMoons)
            {
                for (int i = 0; i < planet.Terrains.Count; i++)
                {
                    planet.Terrains[i].WireframeModeEnabled = wireframeCheck.Checked;
                }
            }
        }

        private void ToggleLODUpdates()
        {
            terrainLODUpdater.FreezeLOD = !lodCheck.Checked;
        }

        private class OnNewFrame : Component, ICompUpdatable
        {
            private PlanetTest test;
            private TiledFloat3 prevViewPos;
            private CompTimeSmoothing<float> smoothSpeed;
            private CompTimeSmoothing<float> smoothFov;
            private CompTaskScheduler.ITask selectPlanetTask;
            private CompPlanet displayedPlanet, nextPlanet;
            private PreciseFloat displayedPlanetTimestamp;

            private const float PLANET_INFO_DELAY = 2.0f;

            public OnNewFrame(Component parent, PlanetTest test) : base(parent) 
            { 
                this.test = test;
                smoothSpeed = new CompTimeSmoothing<float>(this, 3.0f, 0.0f, FMath.Lerp);
                smoothFov = new CompTimeSmoothing<float>(this, 1.0f, FMath.PI_OVER_4, FMath.Lerp);
                test.camera.FOV.Set(smoothFov);
                selectPlanetTask = GetComponent<CompTaskScheduler>().CreateTask("SelectCurrentPlanet", SelectCurrentPlanet, PLANET_INFO_DELAY);
            }

            public UpdateType NeededUpdates => UpdateType.FrameStart1;

            private void SelectCurrentPlanet()
            {
                IReadOnlyList<CompPlanet> planets = GetComponents<CompPlanet>();
                BaseMod baseMod = Context.GetModule<BaseMod>();
                TiledFloat3 viewPos = baseMod.MainPass.Camera.Position;
                ViewFrustum cameraVolume = baseMod.MainPass.Camera.ViewFrustum;

                // find the size-relative closest planet
                nextPlanet = null;
                float closestRelativeDist = 0.0f;
                for (int i = 0; i < planets.Count; i++)
                {
                    TiledFloat3 planetVec = viewPos - planets[i].Center;
                    TiledFloat planetDist = planetVec.Length;
                    TiledFloat planetRadius = planets[i].Radius;

                    float relativeDist = planetDist.ToFloat() / planetRadius.ToFloat();
                    bool isPlanetVisible = cameraVolume.Intersects(planets[i].Occluder.GetValue().ToSphere(viewPos.Tile));

                    // don't show labels unless close enought and visible
                    if (relativeDist > 5.0f || !isPlanetVisible && relativeDist > 1.5f)
                        continue;

                    if (nextPlanet == null || relativeDist < closestRelativeDist)
                    {
                        closestRelativeDist = relativeDist;
                        nextPlanet = planets[i];
                    }
                }
            }

            public void Update(UpdateType updateType)
            {
                BaseMod baseMod = Context.GetModule<BaseMod>();
                TiledFloat3 viewPos = baseMod.MainPass.Camera.Position;
                Float3 deltaViewPos = (viewPos - prevViewPos).ToFloat3();

                // update the planet currently displayed on the UI
                if (selectPlanetTask.State == CompTaskScheduler.TaskState.Completed)
                {
                    if (displayedPlanet != nextPlanet)
                        displayedPlanetTimestamp = Context.Time.RealSecondsFromStart;
                    displayedPlanet = nextPlanet;
                    selectPlanetTask.Reset();
                }
                selectPlanetTask.QueueExecution();

                // fake field of view dilation at relativistic speeds
                float fovExp = FMath.Dot(FMath.Normalize(deltaViewPos), test.camera.Direction);
                fovExp *= FMath.Smoothstep(0.0f, 0.2f, smoothSpeed.GetValue() / FMath.C);
                smoothFov.TargetValue = FMath.PI_OVER_4 * FMath.Pow(2.0f, fovExp);
                Float3 shiftedColor = fovExp < 0 ? FMath.Lerp(new Float3(1.0f, 0.3f, 0.0f), Float3.One, 1.0f + fovExp) : FMath.Lerp(Float3.One, new Float3(0.0f, 0.2f, 1.5f), fovExp);

                baseMod.PostProcess.Tonemapping.ColorMultiplier = shiftedColor;

                // update the UI labels
                {

                    if (displayedPlanet == null)
                    {
                        // hide planet information
                        test.planetNameLabel.Visible = false;
                        test.altitudeLabel.Visible = false;
                        test.radiusLabel.Visible = false;
                    }
                    else
                    {
                        float planetInfoAlpha = (Context.Time.RealSecondsFromStart - displayedPlanetTimestamp).ToFloat() / PLANET_INFO_DELAY;
  
                        // update planet name
                        test.planetNameLabel.Visible = true;
                        test.planetNameLabel.Font.Alpha = planetInfoAlpha;
                        test.planetNameLabel.Text.Set(displayedPlanet.Name);
                        // update planet radius
                        test.radiusLabel.Visible = true;
                        test.radiusLabel.Font.Alpha = planetInfoAlpha;
                        test.radiusLabel.Text.InsertRightAligned(8, 5, FMath.Floor(displayedPlanet.Radius.ToFloat() * 0.001f), 0);
                        // update altitude
                        float altitude = ((viewPos - displayedPlanet.Center).Length - displayedPlanet.Radius).ToFloat();
                        test.altitudeLabel.Visible = true;
                        test.altitudeLabel.Font.Alpha = planetInfoAlpha;
                        test.altitudeLabel.Text.InsertValueAndUnit(10, 10, altitude, 1, UiTextUnits.DistanceUnits);
                    }

                    // update speed
                    if (Context.Time.LastFrameDuration > 0)
                    {
                        float deltaS = FMath.Length(deltaViewPos);
                        float deltaT = Context.Time.LastFrameDuration;
                        float speedMS = deltaS / deltaT;
                        smoothSpeed.TargetValue = speedMS;
                    }
                    test.speedLabel.Text.InsertValueAndUnit(7, 21, smoothSpeed.GetValue(), 1, UiTextUnits.SpeedUnits);
                    test.speedLabel.Font.Color = shiftedColor / shiftedColor.CMax();
                    test.speedLabel.Invalidate();
                }

                prevViewPos = viewPos;
            }
        }
    }
}
