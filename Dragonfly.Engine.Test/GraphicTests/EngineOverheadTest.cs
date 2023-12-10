using Dragonfly.BaseModule;
using Dragonfly.Engine.Core;
using Dragonfly.Graphics.Math;
using System;
using System.Collections.Generic;

namespace Dragonfly.Engine.Test
{
    public class EngineOverheadTest : GraphicsTest
    {
        public EngineOverheadTest()
        {
            Name = "Performance test: Engine Overhead Test";
            EngineUsage = BaseMod.Usage.Generic3D;
            TestDurationSeconds = 20.0f;
        }

        public override void CreateScene()
        {
            AddDebugInfoWindow();

            BaseMod baseMod = Context.GetModule<BaseMod>();
            CompRenderPass mainPass = baseMod.MainPass;
            Component root = Context.Scene.Root;

            /** SCENE CREATION **/
            mainPass.ClearValue = new Float4("#e3f3f9");

            // create camera
            CompTransformEditorMovement cameraController = new CompTransformEditorMovement(root, Float3.Zero + 10 * Float3.UnitY, Float3.UnitZ + 10 * Float3.UnitY);
            CompCamPerspective camera = new CompCamPerspective(cameraController) { FarPlane = 3000.0f };
            cameraController.Look.AspectRatio = camera.AspectRatio;
            mainPass.Camera = camera;

            // start box generation
            new AddBoxComponent(root);
        }
    }

    public class AddBoxComponent : Component, ICompUpdatable
    {
        private FRandom rnd;
        private Float3[] fractalVerts;
        private Float3 nextBoxPos;
        private int lastAdditionFrame = 0;

        public AddBoxComponent(Component parent) : base(parent)
        {
            MinFrameRate = 30;
            MaxBoxSize = 10;
            InstancesPerFrame = 7;
            StopAfterFrame = 100;
            MaxBoxDistance = new Float3(800, 300, 1000);
            rnd = new FRandom();
            fractalVerts = new Float3[]
            {
                new Float3(0, 0, MaxBoxSize),
                new Float3(-MaxBoxDistance.X, MaxBoxDistance.Y, MaxBoxDistance.Z),
                new Float3(MaxBoxDistance.X, MaxBoxDistance.Y, MaxBoxDistance.Z)
            };
            nextBoxPos = fractalVerts[0];
        }

        public int MinFrameRate { get; set; }

        public float MaxBoxSize { get; set; }

        public Float3 MaxBoxDistance { get; set; }

        public int InstancesPerFrame { get; set; }

        public int StopAfterFrame { get; set; }

        public UpdateType NeededUpdates => (Context.Time.FramesPerSecond > MinFrameRate && lastAdditionFrame > (Context.Time.FrameIndex - StopAfterFrame)) ? UpdateType.FrameStart1 : UpdateType.None;

        public void Update(UpdateType updateType)
        {
            for (int i = 0; i < InstancesPerFrame; i++)
            {
                CompMesh newCube = new CompMesh(this);
                newCube.MainMaterial = new CompMtlBasic(newCube, rnd.NextSatColor());
                Float3 boxPos = nextBoxPos;
                boxPos.Y *= (boxPos.Y / MaxBoxDistance.Y);
                Primitives.Cuboid(newCube.AsObject3D(), boxPos, rnd.NextFloat3(Float3.Zero, (Float3)MaxBoxSize));
                nextBoxPos = 0.5f * (nextBoxPos + fractalVerts[rnd.NextInt(fractalVerts.Length)]);
            }

            lastAdditionFrame = Context.Time.FrameIndex;
        }
    }

}
