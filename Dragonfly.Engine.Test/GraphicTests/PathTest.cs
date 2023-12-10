using Dragonfly.BaseModule;
using Dragonfly.Engine.Core;
using Dragonfly.Graphics.Math;
using System.Runtime.Remoting.Contexts;

namespace Dragonfly.Engine.Test.GraphicTests
{
    public class PathTest : GraphicsTest
    {
        public PathTest()
        {
            Name = "Component Tests: Path walking test";
            EngineUsage = BaseMod.Usage.Generic3D;
            TestDurationSeconds = 10.0f;
        }

        public override void CreateScene()
        {
            CompRenderPass mainPass = Context.GetModule<BaseMod>().MainPass;

            /** SCENE CREATION **/
            mainPass.ClearValue = new Float4("#37587a");

            // create camera
            mainPass.Camera = new CompCamPerspective(CompTransformStatic.FromLookAt(Context.Scene.Root, new Float3(-12.0f, 5.0f, -6.0f), new Float3(3.0f, 0, 4.0f)));

            // ground plane
            CompMesh ground = new CompMesh(Context.Scene.Root, new CompMtlBasic(Context.Scene.Root, "textures/gravel_moss1.dds").DisplayIn(mainPass));
            Primitives.Quad(ground.AsObject3D(),
                new Float3(-100.0f, -5.0f, -100.0f), new Float3(-100.0f, -5.0f, 100.0f), new Float3(100.0f, -5.0f, 100.0f),
                new Float2(-25.0f, -25.0f), new Float2(-25.0f, 25.0f), new Float2(25.0f, 25.0f)
            );

            // create a path
            Path3D path = new Path3D();
            path.Points.Add(new Float3(20.0f, 0, 0));
            path.Points.Add(new Float3(6.00f, 0, 0));
            path.Points.Add(new Float3(6.00f, 0, 8.0f));
            path.Points.Add(new Float3(0.00f, 0, 4.0f));
            path.Points.Add(new Float3(4.00f, 0, -2.0f));
            path.SmoothingRadius = 3.0f;

            // create a path-following node
            CompPathWalker pathDirection = new CompPathWalker(Context.Scene.Root, path, 5.0f, 1.0f);
            pathDirection.PathWalkingMode = PathWarkingMode.Tangent;
            CompPathWalker pathPosition = new CompPathWalker(Context.Scene.Root, path, 5.0f, 1.0f);
            CompTransformDynamic matrixMovement = new CompTransformDynamic(Context.Scene.Root, () =>
            {
                Float4x4 transform = Float4x4.RotationY(-FMath.PI_OVER_2);
                transform *= Float4x4.Rotation(Float3.UnitZ, pathDirection.GetValue(), Float3.UnitY);
                transform *= Float4x4.Translation(pathPosition.GetValue());
                return transform;
            });

            // create model
            CompMeshList model = new CompMeshList(matrixMovement);
            model.AddMesh("models/medieval_cart.obj");
        }

    }
}
