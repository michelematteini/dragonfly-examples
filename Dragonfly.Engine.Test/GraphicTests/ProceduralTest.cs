using Dragonfly.BaseModule;
using Dragonfly.Engine.Core;
using Dragonfly.Modules.Procedural;
using Dragonfly.Graphics;
using Dragonfly.Graphics.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Dragonfly.Engine.Test.GraphicTests
{
    public class ProceduralTest : GraphicsTest
    {
        private const string PROC_MODELS_PATH = "models/proc/test";

        private struct MatPreset
        {
            public MaterialFactory Factory;
            public bool ShowTerrain, ShowSky;
            public Float3 BackgroundColor;
        }

        // scene components
        private CompMaterial wireMaterial;
        private MatPreset[] materiaPresets;
        private int curMaterialPreset;
        private CompMesh ground;
        private CompSphericalBackground skyDome;
        private CompScreenshot renderer;

        private class ProcModelState
        {
            public CompMeshList Mesh;
            public List<CompMaterial> OriginalMaterials; // saved mesh materials when replaced with wire material
            public ProceduralMeshDescription Description;
        }

        // proc models state
        private Component procModelsNode;
        private List<ProcModelState> procModelsStates;

        public ProceduralTest()
        {
            Name = "Component Tests: Procedural Trees";
            EngineUsage = BaseMod.Usage.Generic3D;
            TestDurationSeconds = 10.0f;
        }

        public override void CreateScene()
        {
            BaseMod baseMod = Context.GetModule<BaseMod>();
            InitializeMaterialPresets();

            /** SCENE CREATION **/
            skyDome = new CompSphericalBackground(Context.Scene.Root, "textures/sky304.jpg", 1024);

            // create camera
            baseMod.MainPass.Camera = new CompCamPerspective(new CompTransformEditorMovement(Context.Scene.Root, new Float3(-12.0f, 5.0f, -6.0f), new Float3(3.0f, 0, 4.0f)));

            // ground plane
            ground = new CompMesh(Context.Scene.Root, new CompMtlPhong(Context.Scene.Root, "textures/gravel_moss1.dds", "textures/gravel_moss1_norm.dds", 20.0f).DisplayIn(baseMod.MainPass));
            Primitives.Quad(ground.AsObject3D(),
                new Float3(-100.0f, 0.0f, -100.0f), new Float3(-100.0f, 0.0f, 100.0f), new Float3(100.0f, 0.0f, 100.0f),
                new Float2(-10.0f, -10.0f), new Float2(-10.0f, 10.0f), new Float2(10.0f, 10.0f)
            );

            //add lights
            CompLightAmbient ambient = new CompLightAmbient(Context.Scene.Root, new Float3("#202b38"));
            CompLightDirectional sun = new CompLightDirectional(CompTransformStatic.FromDirection(Context.Scene.Root, new Float3(1.0f, -0.5f, 1.0f)), new Float3("#f9f6e0") * 2.0f);
            sun.CastShadow = true;

            // add test commands ui
            CompUiWindow testCmdWindow = new CompUiWindow(baseMod.UiContainer, "40em 8em", "10px 10px");
            testCmdWindow.Title = this.Name;
            CompUiCtrlButton btnWireframe = new CompUiCtrlButton(testCmdWindow, UiPositioning.Inside(testCmdWindow, "0 0"), "Toggle wireframe");
            new CompActionOnEvent(btnWireframe.Clicked, ToggleWireframe);
            CompUiCtrlButton btnReseed = new CompUiCtrlButton(testCmdWindow, UiPositioning.RightOf(btnWireframe, "0.5em"), "Reseed models");
            new CompActionOnEvent(btnReseed.Clicked, ReseedProcModels);
            CompUiCtrlButton btnReload = new CompUiCtrlButton(testCmdWindow, UiPositioning.RightOf(btnReseed, "0.5em"), "Reload models");
            new CompActionOnEvent(btnReload.Clicked, ReloadProcModels);
            CompUiCtrlButton btnChangeMat = new CompUiCtrlButton(testCmdWindow, UiPositioning.RightOf(btnReload, "0.5em"), "Change material");
            new CompActionOnEvent(btnChangeMat.Clicked, ChangeMaterialPreset);
            CompUiCtrlButton btnScreenshot = new CompUiCtrlButton(testCmdWindow, UiPositioning.RightOf(btnChangeMat, "0.5em"), "Copy screenshot");
            new CompActionOnEvent(btnScreenshot.Clicked, TakeScreenshot);
            testCmdWindow.Show();
            wireMaterial = new CompMtlBasic(Context.Scene.Root, Color.Magenta.ToFloat3()).DisplayIn(baseMod.MainPass);
            wireMaterial.FillMode = FillMode.Wireframe;
            renderer = new CompScreenshot(Context.Scene.Root);

            // create procedural models
            procModelsNode = new Component(Context.Scene.Root);
            procModelsStates = new List<ProcModelState>();
            ReloadProcModels();
        }

        private void InitializeMaterialPresets()
        {
            BaseMod baseMod = Context.GetModule<BaseMod>();
            materiaPresets = new MatPreset[5];

            // phong display preset
            materiaPresets[0].Factory = new CompMtlPhong.Factory() { MaterialClass = baseMod.MainPass.MainClass };
            materiaPresets[0].ShowSky = true;
            materiaPresets[0].ShowTerrain = true;
            // normal map display preset
            materiaPresets[1].Factory = new CompMtlGBuffer.Factory(DFXVariants.GBufferOutput.TangentSpaceNormalMap) { MaterialClass = baseMod.MainPass.MainClass };
            materiaPresets[1].BackgroundColor = new Float3(0.5f, 0.5f, 1.0f);
            // albedo map display preset
            materiaPresets[2].Factory = new CompMtlGBuffer.Factory(DFXVariants.GBufferOutput.Albedo) { MaterialClass = baseMod.MainPass.MainClass };
            materiaPresets[2].BackgroundColor = new Float3(1.0f, 0, 1.0f);
            // roughness map display preset
            materiaPresets[3].Factory = new CompMtlGBuffer.Factory(DFXVariants.GBufferOutput.Roughness) { MaterialClass = baseMod.MainPass.MainClass };
            materiaPresets[3].BackgroundColor = Color.Gray.ToFloat3();
            // translucency map display preset
            materiaPresets[4].Factory = new CompMtlGBuffer.Factory(DFXVariants.GBufferOutput.Translucency) { MaterialClass = baseMod.MainPass.MainClass };
            materiaPresets[4].BackgroundColor = Float3.Zero;

            curMaterialPreset = 0;
        }

        private void TakeScreenshot()
        {
            renderer.TakeScreenshot(OnScreenShotReady, new Int2(2048, 2048));
        }

        private void OnScreenShotReady(System.Drawing.Bitmap screenshot)
        {
            Form.ActiveForm.BeginInvoke(new Action<System.Drawing.Bitmap>(SaveScreenshotToClipboard), screenshot);
        }

        private void SaveScreenshotToClipboard(System.Drawing.Bitmap screenshot)
        {
            Clipboard.SetImage(screenshot);
        }

        private void ReloadProcModels()
        {
            // remover current models from the scene
            DisposeProcModels();

            // load all proc meshes from a specific folder
            foreach (string modelPath in Directory.GetFiles(Context.GetResourcePath(PROC_MODELS_PATH), "*.xml"))
            {
                ProcModelState procModel = new ProcModelState();
                procModel.Description = ProceduralMeshDescription.LoadFromFile(modelPath);
                procModelsStates.Add(procModel);
            }

            // regenerate models
            RegenerateProcModels();
        }

        private void RegenerateProcModels()
        {
            foreach (ProcModelState procModel in procModelsStates)
            {
                if (procModel.Mesh != null) procModel.Mesh.Dispose();
                procModel.Mesh = ProceduralMesh.Generate(procModelsNode, procModel.Description, materiaPresets[curMaterialPreset].Factory);
            }
        }

        private void ReseedProcModels()
        {
            foreach (ProcModelState procModel in procModelsStates)
                procModel.Description.Reseed();

            RegenerateProcModels();
        }

        private void DisposeProcModels()
        {
            procModelsNode.DisposeChildren();
            procModelsStates.Clear();
        }

        private void ToggleWireframe()
        {
            foreach (ProcModelState procModel in procModelsStates)
            {
                if (procModel.Mesh[0].MainMaterial.FillMode == FillMode.Solid)
                {
                    procModel.OriginalMaterials = procModel.Mesh.GetMainMaterials();
                    procModel.Mesh.SetMainMaterial(wireMaterial);
                }
                else
                {
                    procModel.Mesh.SetMainMaterials(procModel.OriginalMaterials);
                }
            }
        }

        private void ChangeMaterialPreset()
        {
            curMaterialPreset = (curMaterialPreset + 1) % materiaPresets.Length;
            RegenerateProcModels();
            Context.GetModule<BaseMod>().MainPass.ClearValue = materiaPresets[curMaterialPreset].BackgroundColor.ToFloat4(1.0f);
            skyDome.Active = materiaPresets[curMaterialPreset].ShowSky;
            ground.Active = materiaPresets[curMaterialPreset].ShowTerrain;
        }

    }
}
