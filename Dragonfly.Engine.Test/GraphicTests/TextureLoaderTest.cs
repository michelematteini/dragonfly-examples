using Dragonfly.BaseModule;
using Dragonfly.Engine.Core;
using Dragonfly.Graphics.Math;
using System;
using System.Collections.Generic;

namespace Dragonfly.Engine.Test.GraphicTests
{
    public class TextureLoaderTest : GraphicsTest
    {
        CompMtlBasic cubeMaterial;
        CompTimer timer;
        List<string> testTextures;

        public TextureLoaderTest()
        {
            Name = "Component Tests: Texture loader";
            EngineUsage = BaseMod.Usage.Generic3D;
            testTextures = new List<string>();
        }

        public override void CreateScene()
        {
            testTextures.Clear();
            testTextures.Add("textures/testTex1.png");
            testTextures.Add("textures/testTex2.png");
            testTextures.Add("textures/testTex3.png");
            testTextures.Add("textures/testTex_ok.png");

            /** SCENE CREATION **/
            CompRenderPass mainPass = Context.GetModule<BaseMod>().MainPass;
            mainPass.ClearValue = new Float4("#37587a");

            // create camera
            mainPass.Camera = new CompCamPerspective(CompTransformStatic.FromLookAt(Context.Scene.Root, new Float3(0, 5.0f, 20.0f), Float3.Zero));

            // create rotating node
            CompTransformDynamic rotation = CompTransformDynamic.RotationY(Context.Scene.Root, () => 0.3f * Context.Time.SecondsFromStart.ToFloat());
            
            // create box
            CompMesh simpleBox = new CompMesh(rotation);
            cubeMaterial = new CompMtlBasic(simpleBox, Color.White.ToFloat3());
            cubeMaterial.CullMode = Graphics.CullMode.None;
            simpleBox.MainMaterial = cubeMaterial;
            Primitives.Cuboid(simpleBox.AsObject3D(), Float3.Zero, Float3.One * 3.0f);

            // timer that change textures
            timer = new CompTimer(simpleBox, 1, Timer_Elapsed);
        }

        private void Timer_Elapsed()
        {
            if (testTextures.Count == 0) return;
            cubeMaterial.ColorTexture.SetSource(testTextures[0]);
            testTextures.RemoveAt(0);
            if (testTextures.Count == 0)
                cubeMaterial.Color.Value = Color.Green.ToFloat3();
        }

    }
}
