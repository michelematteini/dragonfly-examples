using Dragonfly.BaseModule;
using Dragonfly.Engine.Core;
using Dragonfly.Graphics.Math;
using Dragonfly.Utils;
using System.Drawing;

namespace Dragonfly.Engine.Test.GraphicTests
{
    public class ObjForestTest : GraphicsTest
    {
        private const float TERRAIN_SIZE = 500.0f;
        private const string background = "#6ca9de";

        private CompMesh ground;
        private float[,] heights;

        private CompAudio forestBg, windBg, ambientMusic;

        public ObjForestTest()
        {
            Name = "Graphic Tests: Forest test";
            TestDurationSeconds = 53.0f;
            EngineUsage = BaseMod.Usage.Generic3D;
        }

        public override void CreateScene()
        {
            AddDebugInfoWindow();

            BaseMod baseModule = Context.GetModule<BaseMod>();
            Component root = Context.Scene.Root;

            /** SCENE CREATION **/
            baseModule.MainPass.ClearValue = new Float4(background);
            baseModule.PostProcess.Fog.Enabled = true;
            baseModule.PostProcess.Fog.Color.Value = new Float3(background);
            baseModule.PostProcess.Fog.Multiplier.Value = 0.02f;
            baseModule.PostProcess.Fog.GradientCoeff = 0.02f;

            // initial loading screen
            CreateLoadingScreen(root, StartAudio, "textures/forestTestLoading.png").ShowLoadingScreen();

            // background 
            CompSphericalBackground skyDome = new CompSphericalBackground(root, "textures/sky304.jpg", 1024, 2.2f, 2.0f);

            // terrain material
            CompMtlPhong groundMat = new CompMtlPhong(root, "textures/terrain_splat1.dds", "textures/terrain_splat1_nrm.dds", 1.0f);
            groundMat.DiffuseColor.Value = new Float3("#9b95a0");
            ground = new CompMesh(root, groundMat.DisplayIn(baseModule.MainPass));

            // terrain mesh
            Bitmap terrainHmap = new Bitmap(Context.GetResourcePath("textures/yosemite_hm.jpg"));
            heights = new float[terrainHmap.Width, terrainHmap.Height];
            terrainHmap.ToFloatMatrix(heights, 160.0f);
            terrainHmap.Dispose();
            Primitives.Terrain(ground.AsObject3D(), Float3.Zero, heights, TERRAIN_SIZE, TERRAIN_SIZE, 16.0f);

            // create camera
            Component cameraController = null;
            if (TestMode)
            {
                CompTransformCameraRig cameraRig = new CompTransformCameraRig(root, new CompTimeSeconds(root, 1.0f/*, 5.0f*/));
                cameraRig.AddTrackingShot(8.0f, new Path3D(new Float3(221.0f, 38.0f, 114.0f), new Float3(191.0f, 36.0f, 84.0f)), 5.0f);
                cameraRig.AddPanShot(10.0f, new Path3D(new Float3(-26.0f, 181.0f, -175.0f), new Float3(69.0f, 182.0f, 108.0f)), 5.0f, new Float3(-0.39f, -0.73f, 0.56f));

                Path3D trackShot3Path = new Path3D(
                    new Float3(145.0f, 132.0f, -147.0f),
                    new Float3(144.0f, 128.0f, -125.0f),
                    new Float3(129.0f, 96.0f, -77.0f),
                    new Float3(114.5f, 65.0f, -61.0f),
                    new Float3(99.0f, 58.0f, -45.0f)
                );
                trackShot3Path.SmoothingRadius = 10.0f;
                cameraRig.AddTrackingShot(20.0f, trackShot3Path, 7.0f);
                cameraRig.AddArcShort(14.0f, new Path3D(new Float3(30.0f, 20f, 13.7f), new Float3(-6.6f, 12.85f, 29.0f)), 3.0f, new Float3(0, 15.0f, 0));
                cameraController = cameraRig;
            }
            else
            {
                Float3 cameraPos = new Float3(224.0f, 25.0f, 102.5f), cameraTarget = cameraPos + new Float3(-0.84f, 0.31f, -0.44f);
                cameraPos.Y += HeightAt(cameraPos.XZ);
                cameraTarget.Y += HeightAt(cameraPos.XZ);
                cameraController = new CompTransformEditorMovement(root, cameraPos, cameraTarget);
            }
            baseModule.MainPass.Camera = new CompCamPerspective(cameraController);

            // create trees  
            CompMtlPhong.Factory mf = new CompMtlPhong.Factory { MaterialClass = baseModule.MainPass.MainClass };
            mf.ClipTransparency = true;

            FRandom rnd = new FRandom(2);
            CompMeshList[] trees = new CompMeshList[]
            {
                new CompMeshList(root, "models/tree_conifer1.obj", mf),
                new CompMeshList(root, "models/tree_conifer2.obj", mf),
                new CompMeshList(root, "models/tree_dead1.obj", mf)
            };

            for (int i = 0; i < 600; i++)
            {
                Float3 rndPosition = new Float3(rnd.NextFloat() - 0.5f, 0, rnd.NextFloat() - 0.5f) * TERRAIN_SIZE * 0.8f;
                rndPosition.Y = HeightAt(rndPosition.XZ);
                Float4x4 treeTransform = Float4x4.RotationY(rnd.NextInt() * FMath.TWO_PI) * Float4x4.Scale(rnd.NextFloat() * 1.5f + 0.4f) * Float4x4.Translation(rndPosition);
                trees[rnd.NextInt() % trees.Length].AddInstance(treeTransform);
            }

            // add other models
            CompMeshList house1 = new CompMeshList(root, "models/medieval_house.obj", mf, Float4x4.Translation(0, HeightAt(Float2.Zero), 0));
            CompMeshList cart = new CompMeshList(root, "models/medieval_cart.obj", mf, Float4x4.Translation(ToTerrain(-5.0f, 20.0f)));

            // add lights
            CompLightAmbient ambient = new CompLightAmbient(root, new Float3("#2485a8"));
            CompLightDirectional sun = new CompLightDirectional(CompTransformStatic.FromDirection(root, new Float3(1.0f, -1.5f, 1.0f)), new Float3("#f9f6e0") * 20.0f);
            sun.CastShadow = true;

            // add background audio
            forestBg = new CompAudio(house1, "audio/forest_loop_1.wav");
            forestBg.VolumeDecibels = -10.0f;
            forestBg.Effects.Add(new CompAudioFxDirGradient(forestBg, Float3.Zero, 0, Float3.UnitY * 120.0f, -20.0f));
            

            windBg = new CompAudio(house1, "audio/windy_loop_1.wav");
            windBg.Effects.Add(new CompAudioFxDirGradient(windBg, Float3.Zero, -40.0f, Float3.UnitY * 120.0f, 0));
            windBg.Effects.Add(new CompAudioFxVolumeRnd(windBg, 0.4f, -12.0f, 0.0f));
            

            if (TestMode)
            {
                ambientMusic = new CompAudio(house1, "audio/ambient_loop_1.wav");
                ambientMusic.Effects.Add(new CompAudioFxFadeIn(ambientMusic, 3.0f));
                ambientMusic.VolumeDecibels = -5.0f;
                ambientMusic.PlayLoop();
            }
        }

        private void StartAudio()
        {
            forestBg.PlayLoop();
            windBg.PlayLoop();
            if(TestMode)
                ambientMusic.PlayLoop();
        }

        private float HeightAt(Float2 loc)
        {
            Float2 hmapCoords = (loc / TERRAIN_SIZE + Float2.One * 0.5f) * new Float2(heights.GetLength(0) - 1, heights.GetLength(1) - 1) + Float2.One * 0.5f;
            return heights[(int)hmapCoords.X, (int)hmapCoords.Y];
        }

        private Float3 ToTerrain(float x, float z)
        {
            return new Float3(x, HeightAt(new Float2(x, z)), z);
        }

    }

}
