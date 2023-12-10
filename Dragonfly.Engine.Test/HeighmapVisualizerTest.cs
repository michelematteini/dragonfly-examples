using Dragonfly.BaseModule;
using Dragonfly.Engine.Core;
using Dragonfly.Graphics.Math;
using Dragonfly.Utils;
using System;
using System.Drawing;

namespace Dragonfly.Engine.Test
{
    public class HeighmapVisualizerTest : GraphicsTest
    {
        private const float TERRAIN_SIZE = 500.0f;
        private const int TERRAIN_TILE_SIZE = 128;
        private const string background = "#6ca9de";
        private static readonly string DEFAULT_HMAP = "yosemite_hm.jpg";

        private CompMeshList ground;
        private CompMaterial groundMat;
        private float[,] heights;
        private CompUiCtrlTextInput txtHMapName, txtHeightMul;

        public HeighmapVisualizerTest()
        {
            Name = "Basic Tests: Heighmap Visualizer";
            TestDurationSeconds = 10.0f;
            EngineUsage = BaseMod.Usage.Generic3D;
        }

        public override void CreateScene()
        {
            BaseMod baseModule = Context.GetModule<BaseMod>();
            Component root = Context.Scene.Root;

            /** SCENE CREATION **/
            baseModule.MainPass.ClearValue = new Float4(background);
            baseModule.PostProcess.ExposureValue = 4.0f;

            // background 
            CompSphericalBackground skyDome = new CompSphericalBackground(root, "textures/sky304.jpg", 1024, 2.2f, 2.0f);

            // add heightmap selection UI
            CompUiWindow hmapSelectionWnd = new CompUiWindow(baseModule.UiContainer, "24em 12em", "10px") { Title = "Heightmap visualizer" };
            txtHMapName = new CompUiCtrlTextInput(hmapSelectionWnd, DEFAULT_HMAP, UiPositioning.Inside(hmapSelectionWnd, "1em"));
            CompUiCtrlButton btnUpdateHMap = new CompUiCtrlButton(hmapSelectionWnd, UiPositioning.RightOf(txtHMapName, "0.5em"), "Load");
            new CompActionOnEvent(btnUpdateHMap.Clicked, UpdateHMap);
            CompUiCtrlLabel lblHeightMul = new CompUiCtrlLabel(hmapSelectionWnd, "Height multiplier: ", UiPositioning.Below(txtHMapName, "0.5em"));
            txtHeightMul = new CompUiCtrlTextInput(hmapSelectionWnd, "100.0", UiPositioning.RightOf(lblHeightMul));
            hmapSelectionWnd.Show();

            // terrain material
            groundMat = new CompMtlPhong(root, Float3.One).DisplayIn(baseModule.MainPass);

            // generate terrain 
            UpdateHMap();

            // create camera
            Component cameraController = null;
            {
                Float3 cameraPos = new Float3(224.0f, 25.0f, 102.5f), cameraTarget = cameraPos + new Float3(-0.84f, 0.31f, -0.44f);
                cameraPos.Y += HeightAt(cameraPos.XZ);
                cameraTarget.Y += HeightAt(cameraPos.XZ);
                cameraController = new CompTransformEditorMovement(root, cameraPos, cameraTarget);
            }
            baseModule.MainPass.Camera = new CompCamPerspective(cameraController);

            // add lights
            CompLightAmbient ambient = new CompLightAmbient(root, new Float3("#2485a8"));
            CompLightDirectional sun = new CompLightDirectional(CompTransformStatic.FromDirection(root, new Float3(1.0f, -1.5f, 1.0f)), new Float3("#f9f6e0") * 20.0f);
        }

        private void UpdateHMap()
        {
            if (ground != null)
                ground.Dispose();
            ground = new CompMeshList(Context.Scene.Root);
            float heightMul = txtHeightMul.Text.ParseInvariantFloat();
            
            // generate terrain 
            Bitmap terrainHmap = new Bitmap(Context.GetResourcePath("textures/" + txtHMapName.Text));
            int terrainWidth = terrainHmap.Width, terrainHeight = terrainHmap.Height;
            heights = new float[terrainWidth, terrainHeight];
            terrainHmap.ToFloatMatrix(heights, heightMul);
            terrainHmap.Dispose();
            IntRect terrainTileRect = new IntRect();
            for (terrainTileRect.Y = 0; terrainTileRect.Y < terrainHeight; terrainTileRect.Y += TERRAIN_TILE_SIZE)
            {
                terrainTileRect.Height = Math.Min(TERRAIN_TILE_SIZE + 1, terrainHeight - terrainTileRect.Y);
                for (terrainTileRect.X = 0; terrainTileRect.X < terrainWidth; terrainTileRect.X += TERRAIN_TILE_SIZE)
                {
                    terrainTileRect.Width = Math.Min(TERRAIN_TILE_SIZE + 1, terrainWidth - terrainTileRect.X);
                    CompMesh terrainTile = ground.AddMesh();
                    terrainTile.MainMaterial = groundMat;
                    Primitives.Terrain(terrainTile.AsObject3D(), Float3.Zero, heights, terrainTileRect, TERRAIN_SIZE, TERRAIN_SIZE, 16.0f);
                }
            }
        }

        private float HeightAt(Float2 loc)
        {
            Float2 hmapCoords = (loc / TERRAIN_SIZE + Float2.One * 0.5f) * new Float2(heights.GetLength(0) - 1, heights.GetLength(1) - 1) + Float2.One * 0.5f;
            return heights[(int)hmapCoords.X, (int)hmapCoords.Y];
        }

    }

}
