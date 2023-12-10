using Dragonfly.BaseModule;
using Dragonfly.Engine.Core;
using Dragonfly.Graphics.Math;
using System.Collections.Generic;

namespace Dragonfly.Engine.Test.GraphicTests
{
    public class PhongMaterialTest : GraphicsTest
    {
        CompMaterial groundMat, whiteMat;
        List<CompMaterial> dragonMaterials;
        bool whiteMaterialActive;
        CompMesh ground;
        CompMeshList dragon;

        public PhongMaterialTest()
        {
            Name = "Material Tests: Phong Material";
            EngineUsage = BaseMod.Usage.Generic3D;
            TestDurationSeconds = 10.0f;
        }

        public override void CreateScene()
        {
            AddDebugInfoWindow();

            BaseMod baseMod = Context.GetModule<BaseMod>();
            CompRenderPass mainPass = baseMod.MainPass;
            Component root = Context.Scene.Root;
            whiteMaterialActive = false;

            /** SCENE CREATION **/
            mainPass.ClearValue = new Float4("#e3f3f9");

            // create camera
            mainPass.Camera = new CompCamPerspective(new CompTransformEditorMovement(root, new Float3(1.5f, 2.6f, 7.0f), new Float3(1.3f, 2.4f, 6.0f)));

            // ground plane
            groundMat = new CompMtlPhong(root, "textures/gravel_moss1.dds", "textures/gravel_moss1_norm.dds", 128.0f);
            ground = new CompMesh(root, groundMat.DisplayIn(mainPass));
            Primitives.Quad(ground.AsObject3D(),
                new Float3(-100.0f, 0, -100.0f), new Float3(-100.0f, 0, 100.0f), new Float3(100.0f, 0, 100.0f),
                new Float2(-25.0f, -25.0f), new Float2(-25.0f, 25.0f), new Float2(25.0f, 25.0f)
            );

            // create objects
            CompTransformStatic objTransform = new CompTransformStatic(root, Float4x4.RotationY(-0.4f));
            dragon = new CompMeshList(objTransform);
            dragon.AddMesh("models/qilin.obj", new CompMtlPhong.Factory { MaterialClass = baseMod.MainPass.MainClass });

            // add ambient light
            CompLightAmbient ambient = new CompLightAmbient(root, new Float3(0.05f, 0.05f, 0.1f));

            // add random lights
            CompTransformDynamic lightsRotation = CompTransformDynamic.RotationY(root, () => 0.4f * Context.Time.SecondsFromStart.ToFloat());

            FRandom rnd = new FRandom();
            for (int i = 0; i < 32; i++)
            {
                Float3 pos = Float3.Zero;
                pos.XZ = rnd.NextNorm2() * (1.0f + rnd.NextFloat() * 5.0f);
                pos.Y = 1.0f + rnd.NextFloat() * 2.0f;

                Float3 color = FMath.Lerp(rnd.NextSatColor(), Float3.One, 0.3f);
                new CompLightPoint(CompTransformStatic.FromPosition(lightsRotation, pos), color);
                CompMesh lightPoint = new CompMesh(lightsRotation);
                lightPoint.MainMaterial = new CompMtlBasic(lightsRotation, color);
                Primitives.Spheroid(lightPoint.AsObject3D(), pos, Float3.One / 6, 40);
            }  

            whiteMat = new CompMtlPhong(objTransform, Color.Gray.ToFloat3(), null, 128.0f, null).DisplayIn(mainPass);

            // add test commands ui
            CompUiWindow testCmdWindow = new CompUiWindow(baseMod.UiContainer, "22em 8em", UiPositioning.Below(TestResults.Window, "0.5em"));
            testCmdWindow.Title = this.Name;
            CompUiCtrlButton btnToggleMats = new CompUiCtrlButton(testCmdWindow, UiPositioning.Inside(testCmdWindow, "0 0.5em"), "Enable / Disable materials", "10em 2em");
            new CompActionOnEvent(btnToggleMats.Clicked, ToggleMaterials);
            testCmdWindow.Show();
        }

        private void ToggleMaterials()
        {
            if (whiteMaterialActive)
            {
                ground.MainMaterial = groundMat;
                dragon.SetMainMaterials(dragonMaterials);
                dragonMaterials = null;
            }
            else
            {
                ground.MainMaterial = whiteMat;
                dragonMaterials = dragon.GetMainMaterials();
                dragon.SetMainMaterial(whiteMat);
            }

            whiteMaterialActive = !whiteMaterialActive;
        }
    }
}
