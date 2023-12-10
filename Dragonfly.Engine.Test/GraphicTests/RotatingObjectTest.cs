using Dragonfly.BaseModule;
using Dragonfly.Engine.Core;
using Dragonfly.Graphics.Math;

namespace Dragonfly.Engine.Test.GraphicTests
{
    public class RotatingObjectTest : GraphicsTest
    {
        CompCamPerspective camera;

        public RotatingObjectTest()
        {
            EngineUsage = BaseMod.Usage.Generic3D;
            Name = "Basic Tests: Rotating model";
        }

        public override void CreateScene()
        {
            CompRenderPass mainPass = Context.GetModule<BaseMod>().MainPass;

            /** SCENE CREATION **/
            mainPass.ClearValue = new Float4("#37587a");

            // create camera
            Float3 camPos = new Float3(0, 5.0f, 8.0f), camTarget = new Float3(0, 2.0f, 0);
            camera = new CompCamPerspective(CompTransformStatic.FromLookAt(Context.Scene.Root, camPos, camTarget));
            mainPass.Camera = camera;

            // create rotating node
            CompTransformDynamic rotation = CompTransformDynamic.RotationY(Context.Scene.Root, () => 0.2f * Context.Time.SecondsFromStart.ToFloat());

            // create obj mesh
            CompMtlBasic.Factory mtlFactory = new CompMtlBasic.Factory { MaterialClass = mainPass.MainClass };
            new CompMeshList(rotation).AddMesh("models/qilin.obj", mtlFactory);
        }

    }
}
