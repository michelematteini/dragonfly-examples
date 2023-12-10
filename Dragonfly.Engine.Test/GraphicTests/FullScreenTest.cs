using Dragonfly.BaseModule;
using Dragonfly.Engine.Core;
using Dragonfly.Graphics.Math;
using System;

namespace Dragonfly.Engine.Test.GraphicTests
{
    public class FullScreenTest : GraphicsTest
    {
        CompCamPerspective camera;

        public FullScreenTest()
        {
            Name = "Basic Tests: Fullscreen transition";
            TestDurationSeconds = 16.0f;
            OverrideScreen = true;
            EngineUsage = BaseMod.Usage.Generic3D;
        }

        public override void CreateScene()
        {
            Component root = Context.Scene.Root;
            CompRenderPass mainPass = Context.GetModule<BaseMod>().MainPass;

            /** SCENE CREATION **/
            mainPass.ClearValue = new Float4("#37587a");

            // create camera
            camera = new CompCamPerspective(CompTransformStatic.FromLookAt(root, new Float3(0, 0, 5.0f), Float3.Zero));
            mainPass.Camera = camera;

            // create rotating node
            CompTransformDynamic rotation = CompTransformDynamic.RotationY(root, () => 0.5f * Context.Time.SecondsFromStart.ToFloat());

            // add a mesh
            CompMesh quad = new CompMesh(rotation);
            Primitives.Quad(quad.AsObject3D(),
                new Float3(-0.5f, -0.5f, 0), new Float3(0.5f, -0.5f, 0), new Float3(0.5f, 0.5f, 0), // vert
                Float2.Zero, Float2.UnitX, Float2.One // tex coords
            );
            CompMtlBasic material = new CompMtlBasic(rotation, Color.White.ToFloat3());
            material.ColorTexture.SetSource("textures\\green_grass1.dds");
            material.CullMode = Graphics.CullMode.None;
            quad.MainMaterial = material;

            // add  toggle full screen events
            new CompActionOnEvent(new CompEventTimed(root, 3).Event.DisposeOnceOccurred(), ToggleFullScreen);
            new CompActionOnEvent(new CompEventTimed(root, 10).Event.DisposeOnceOccurred(), ToggleFullScreen);
        }

        private void ToggleFullScreen()
        {
            Context.Scene.FullScreen = !Context.Scene.FullScreen;
        }

    }
}
