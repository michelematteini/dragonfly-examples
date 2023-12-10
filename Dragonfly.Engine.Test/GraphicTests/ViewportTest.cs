using System;
using Dragonfly.BaseModule;
using Dragonfly.Engine.Core;
using Dragonfly.Graphics.Math;

namespace Dragonfly.Engine.Test.GraphicTests
{
    public class ViewportTest : GraphicsTest
    {
        CompRenderPass mainPass;
        int disableIndex;

        public ViewportTest()
        {
            EngineUsage = BaseMod.Usage.Generic3D;
            Name = "Basic Tests: Viewports";
        }

        public override void CreateScene()
        {
            mainPass = Context.GetModule<BaseMod>().MainPass;
            disableIndex = 0;

            /** SCENE CREATION **/
            mainPass.ClearValue = new Float4("#37587a");

            // create rotating node
            CompTransformDynamic rotation = new CompTransformDynamic(Context.Scene.Root, () =>
            {
                Float4x4 transform = Float4x4.Scale(3.0f);
                transform *= Float4x4.RotationY(0.2f * Context.Time.SecondsFromStart.ToFloat());
                return transform;
            });
            
            // create obj mesh
            CompMtlBasic.Factory mtlFactory = new CompMtlBasic.Factory { MaterialClass = mainPass.MainClass };
            new CompMeshList(rotation).AddMesh("models/qilin.obj", mtlFactory);

            // create camera 1
            CompCamPerspective camera1 = new CompCamPerspective(CompTransformStatic.FromLookAt(Context.Scene.Root, new Float3(0, 5.0f, 20.0f), new Float3(0, 0, 0)));
            camera1.Viewport = new AARect(0, 0, 0.49f, 0.49f);
            mainPass.CameraList.Add(camera1);

            // create camera 2
            CompCamPerspective camera2 = new CompCamPerspective(CompTransformStatic.FromLookAt(Context.Scene.Root, new Float3(0, 20.0f, 5.0f), new Float3(0, 0, 0)));
            camera2.Viewport = new AARect(0.51f, 0, 1, 0.49f);
            mainPass.CameraList.Add(camera2);

            // create camera 3
            CompCamPerspective camera3 = new CompCamPerspective(CompTransformStatic.FromLookAt(Context.Scene.Root, new Float3(20.0f, 5.0f, 0), new Float3(0, 0, 0)));
            camera3.Viewport = new AARect(0, 0.51f, 0.49f, 1);
            mainPass.CameraList.Add(camera3);

            // create camera 4
            CompCamPerspective camera4 = new CompCamPerspective(CompTransformStatic.FromLookAt(Context.Scene.Root, new Float3(15.0f, -15.0f, 15.0f), new Float3(0, 0, 0)));
            camera4.Viewport = new AARect(0.51f, 0.51f, 1, 1);
            mainPass.CameraList.Add(camera4);


            CompTimer disableTimer = new CompTimer(Context.Scene.Root, 1.0f, DisableCameras);
        }

        private void DisableCameras()
        {
            if (disableIndex < mainPass.CameraList.Count)
                mainPass.CameraList[disableIndex++].Active = false;
        }
    }
}
