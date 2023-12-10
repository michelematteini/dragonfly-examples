using Dragonfly.BaseModule;
using Dragonfly.Engine.Core;
using Dragonfly.Graphics.Math;

namespace Dragonfly.Engine.Test.GraphicTests
{
    public class ShadowmapTest : GraphicsTest
    {
        public ShadowmapTest()
        {
            EngineUsage = BaseMod.Usage.Generic3D;
            Name = "Component test: Shadowmaps";
            TestDurationSeconds = 45.0f;
        }

        CompMeshList CreateRoom(CompMtlPhong wallMaterial, Component root)
        {
            CompMeshList room = new CompMeshList(root);
            Primitives.Cuboid(room.AddMesh().AsObject3D(), new Float3(0, -0.2f, -2.5f), new Float3(7.0f, 0.4f, 10.0f));
            Primitives.Cuboid(room.AddMesh().AsObject3D(), new Float3(0, 3.0f + 0.2f, 0), new Float3(7.0f, 0.4f, 5.0f));
            Primitives.Cuboid(room.AddMesh().AsObject3D(), new Float3(-3.7f, 1.5f, 0), new Float3(0.4f, 3.8f, 5.0f));
            Primitives.Cuboid(room.AddMesh().AsObject3D(), new Float3(3.7f, 1.5f, 0), new Float3(0.4f, 3.8f, 5.0f));
            Primitives.Cuboid(room.AddMesh().AsObject3D(), new Float3(0, 1.5f, 2.7f), new Float3(7.8f, 3.8f, 0.4f));
            room.SetMainMaterial(wallMaterial);
            return room;
        }

        void CreateRoom(CompMtlPhong.Factory phongFactory, CompMtlPhong wallMaterial, Component root, int type)
        {
            switch (type)
            {
                case 0:
                    CreateForestSpotRoom(phongFactory, wallMaterial, root);
                    break;
                case 1:
                    CreateOmniRoom(phongFactory, wallMaterial, root);
                    break;          
                case 2:
                    CreateCartMultispotRoom(phongFactory, wallMaterial, root);
                    break;
                case 3:
                    CreateRadiusTestRoom(phongFactory, wallMaterial, root);
                    break;
            }
        }

        void CreateOmniRoom(CompMtlPhong.Factory phongFactory, CompMtlPhong wallMaterial, Component root)
        {
            // walls
            CreateRoom(wallMaterial, root);

            // model that cast shadows
            CompMeshList model = new CompMeshList(root, "models/qilin.obj", phongFactory, Float4x4.RotationY(FMath.PI) * Float4x4.Scale(0.5f));

            // light movement logic
            CompTransformDynamic lightsRotation = new CompTransformDynamic(root, () =>
            {
                float time = Context.Time.SecondsFromStart.ToFloat();
                Float4x4 transform = Float4x4.Translation(0.5f * Float3.UnitX * FMath.Sin(time * 1.3f));
                transform *= Float4x4.RotationY(0.8f * time);
                transform *= Float4x4.Translation(1.4f * Float3.UnitY * FMath.Sin(time));
                return transform;
            });

            // add point light
            Float3 lightPos = new Float3(1.4f, 1.5f, 0);
            CompLightPoint pointLight = new CompLightPoint(CompTransformStatic.FromPosition(lightsRotation, lightPos), new Float3("#ff4001") * 40.0f);
            pointLight.CastShadow = true;
            pointLight.Radius.Set(0.05f);
            pointLight.AddDebugMesh();
        }

        void CreateRadiusTestRoom(CompMtlPhong.Factory phongFactory, CompMtlPhong wallMaterial, Component root)
        {
            // walls
            CreateRoom(wallMaterial, root);

            // model that cast shadows
            CompMeshList model = new CompMeshList(root, "models/qilin.obj", phongFactory, Float4x4.RotationY(FMath.PI) * Float4x4.Scale(0.5f));

            // add point light
            CompLightPoint pointLight = new CompLightPoint(CompTransformStatic.FromPosition(root, new Float3(1.4f, 0.5f, 0)), new Float3("#eeeeff") * 20.0f);
            pointLight.CastShadow = true;
            CompTimeSeconds lrTime = new CompTimeSeconds(pointLight, 0.5f);
            pointLight.Radius.Set(new CompFunction<float>(pointLight, () => 0.03f * (1.0f + FMath.Sin(lrTime.GetValue()))));
            pointLight.AddDebugMesh();
        }


        void CreateForestSpotRoom(CompMtlPhong.Factory phongFactory, CompMtlPhong wallMaterial, Component root)
        {
            // walls
            CreateRoom(wallMaterial, root);

            CompMeshList tree1 = new CompMeshList(root, "models/tree_conifer1.obj", phongFactory, Float4x4.Scale(0.2f));
            CompMeshList tree2 = new CompMeshList(root, "models/tree_conifer1.obj", phongFactory, Float4x4.Scale(0.3f) * Float4x4.RotationY(1.0f) * Float4x4.Translation(-0.5f, 0, -1.5f));
            CompMeshList tree3 = new CompMeshList(root, "models/tree_conifer1.obj", phongFactory, Float4x4.Scale(0.15f) * Float4x4.RotationY(-2.0f) * Float4x4.Translation(0.5f, 0, -3.0f));
            CompMeshList tree4 = new CompMeshList(root, "models/tree_conifer1.obj", phongFactory, Float4x4.Scale(0.25f) * Float4x4.RotationY(-3.0f) * Float4x4.Translation(1.0f, 0, -6.0f));
            CompMeshList tree5 = new CompMeshList(root, "models/tree_conifer1.obj", phongFactory, Float4x4.Scale(0.2f) * Float4x4.RotationY(-4.0f) * Float4x4.Translation(2.0f, 3.4f, 0.5f));

            Float3 spotPos1 = new Float3(1.8f, 2.0f, 0);
            CompLightSpot spotLight1 = new CompLightSpot(CompTransformStatic.FromPosAndDir(root, spotPos1, -spotPos1), new Float3("#c0ff00") * 40.0f, (20.0f).ToRadians(), (80.0f).ToRadians());
            spotLight1.CastShadow = true;
            spotLight1.Radius.Set(0.1f);
            spotLight1.AddDebugMesh();

            Float3 spotPos2 = new Float3(-1.8f, 1.0f, -3.0f);
            Float3 spotDir2 = FMath.Normalize(new Float3(0, 2.0f, 0) - spotPos2);
            CompLightSpot spotLight2 = new CompLightSpot(CompTransformStatic.FromPosAndDir(root, spotPos2, spotDir2), new Float3("#00e7ff") * 40.0f, (10.0f).ToRadians(), (80.0f).ToRadians());
            spotLight2.CastShadow = true;
            spotLight2.Radius.Set(0.04f);
            spotLight2.AddDebugMesh();
        }

        void CreateCartMultispotRoom(CompMtlPhong.Factory phongFactory, CompMtlPhong wallMaterial, Component root)
        {
            // walls
            CreateRoom(wallMaterial, root);

            // shadow casting object
            CompMeshList cart = new CompMeshList(root, "models/medieval_cart.obj", phongFactory, Float4x4.Scale(0.8f));

            // rotating spot lights
            CompTransformDynamic lightsRotation = CompTransformDynamic.RotationY(root, () => 0.8f * Context.Time.SecondsFromStart.ToFloat());

            int spotsCount = 5;
            for (int i = 0; i < spotsCount; i++)
            {
                float angleRadians = FMath.TWO_PI * i / spotsCount;
                Float2 dir2D = new Float2(FMath.Cos(angleRadians), FMath.Sin(angleRadians));
                Float3 lPos = new Float3(dir2D.X, 0.5f, dir2D.Y) * 2.1f;
                CompLightSpot spotLight = new CompLightSpot(CompTransformStatic.FromPosAndDir(lightsRotation, lPos, - lPos + Float3.UnitY), new Float3("#ff005e") * 20.0f, (5.0f).ToRadians(), (70.0f).ToRadians());
                spotLight.CastShadow = true;
                spotLight.Radius.Set(0.04f);
                spotLight.AddDebugMesh();
            }
        }

        public override void CreateScene()
        {
            AddDebugInfoWindow();

            Component root = Context.Scene.Root;
            BaseMod baseMod = Context.GetModule<BaseMod>();
            baseMod.Settings.Shadows.MaxShadowDistance = 500.0f;

            /** SCENE CREATION **/
            baseMod.MainPass.ClearValue = new Float4("#e3f3f9");

            // initial loading screen
            CreateLoadingScreen(root, null, "textures/smTestLoading.png").ShowLoadingScreen();

            // create camera
            Component cameraController = null;
            if (TestMode)
            {
                CompTransformCameraRig cameraRig = new CompTransformCameraRig(root, new CompTimeSeconds(root, 1.0f));
                cameraRig.AddArcShort(5.0f, new Path3D(new Float3(-5, 4, -75), new Float3(-34, 18, -86), new Float3(-67, 21, -76)).MakeSmooth(10.0f), 10.0f, new Float3(26, 7, 21));
                cameraRig.AddPanShot(12.0f, new Path3D(new Float3(-41, 3, -52), new Float3(23, 3, -52)), 5.0f, new Float3(0, -0.15f, 1));
                cameraRig.AddTrackingShot(20.0f, new Path3D(new Float3(-5, 4, -66), new Float3(-1.4f, 1.7f, -49), new Float3(9, 1.4f, -37), new Float3(16.5f, 1.4f, -26), new Float3(26, 1, -25), new Float3(22, 2, -8), new Float3(5.4f, 1.4f, -3.4f)).MakeSmooth(4.0f), 4.5f);
                cameraRig.AddArcShort(6.0f, new Path3D(new Float3(-20, 2, 4), new Float3(-8, 2, 3), new Float3(3.6f, 1.1f, 17.2f)).MakeSmooth(5.0f), 6.0f, new Float3(-18, 1.6f, 30));
                cameraController = cameraRig;
            }
            else
            {
                cameraController = new CompTransformEditorMovement(root, new Float3(0, 5.0f, -16.0f), new Float3(0, 0, 0));
                
            }
            baseMod.MainPass.Camera = new CompCamPerspective(cameraController);

            // background 
            CompSphericalBackground skyDome = new CompSphericalBackground(root, "textures/sky304.jpg", 2048, 2.2f, 1.5f);

            // create objects
            {
                //prepare materials
                CompMtlPhong whiteMat = new CompMtlPhong(root, Color.Gray.ToFloat3(), null, 128.0f, null);
                whiteMat.DisplayIn(baseMod.MainPass);
                CompMtlPhong.Factory phongFactory = new CompMtlPhong.Factory { MaterialClass = baseMod.MainPass.MainClass };
                phongFactory.ClipTransparency = true;

                // ground plane
                CompMesh groundPlane = new CompMesh(root, whiteMat);
                Primitives.Quad(groundPlane.AsObject3D(), new Float3(-100, -2, -100), new Float3(-100, -2, 100), new Float3(100, -2, 100));

                // create rooms
                int halfGrid = 2;
                for (int x = -halfGrid; x < halfGrid; x++)
                {
                    for (int y = -halfGrid; y < halfGrid; y++)
                    {
                        Float3 roomPos = new Float3((float)x, 0, y) * 20.0f;
                        CreateRoom(phongFactory, whiteMat, CompTransformStatic.FromPosition(root, roomPos), (x + y + 2 * halfGrid) % 4);
                    }
                }
            }

            // add ambient and directional lights
            CompLightAmbient ambient = new CompLightAmbient(root, new Float3("#b0d0ff"));
            CompLightDirectional sunLight = new CompLightDirectional(CompTransformStatic.FromDirection(root, new Float3(1.0f, -5.0f, -1.0f)), new Float3("#c2e1eb"));
            sunLight.CastShadow = true;

            // display the shadowmap atlas
            CompRenderBuffer shadowAtlas = baseMod.GetInternalBuffer("ShadowAtlas");
            CompUiWindow testCmdWindow = new CompUiWindow(baseMod.UiContainer, "33em 36em", UiPositioning.Below(TestResults.Window, "0.5em"));
            testCmdWindow.Title = "Shadow Atlas";
            CompUiCtrlPicture shadowAtlasImg = new CompUiCtrlPicture(testCmdWindow, new CompMtlImgHeatmap(testCmdWindow, Float4.UnitX * 80f, 0));
            shadowAtlasImg.Image.SetSource(new RenderTargetRef(shadowAtlas));
            shadowAtlasImg.SizingStyle = ImageSizingStyle.AutoHeight;
            shadowAtlasImg.Width = "30em";
            shadowAtlasImg.Position = UiPositioning.Inside(testCmdWindow, "0 0.5em");
            testCmdWindow.Show();
        }
    }
}
