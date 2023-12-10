using Dragonfly.BaseModule;
using Dragonfly.Graphics.Math;

namespace Dragonfly.Engine.Test.GraphicTests
{
    public class RadianceMapTest : GraphicsTest
    {
        public RadianceMapTest()
        {
            Name = "Component Test: Radiance Maps";
            TestDurationSeconds = 10.0f;
            EngineUsage = BaseMod.Usage.Generic3D;
        }
        
        public override void CreateScene()
        {
            BaseMod baseMod = Context.GetModule<BaseMod>();
            baseMod.PostProcess.ExposureValue = ExposureHelper.EVSunny;

            AddDebugInfoWindow();

            // create a background from the radiance map
            CompSphericalBackground bg = new CompSphericalBackground(Context.Scene.Root, "textures/syferfontein.hdr");

            // create a window to show the radiance file content (its 2d layout)
            CompUiWindow filePreviewWnd;
            {
                filePreviewWnd = new CompUiWindow(baseMod.UiContainer, "500px 350px", UiPositioning.Below(TestResults.Window, "10px"));
                filePreviewWnd.Title = "Radiance file content";

                // cubemap picture
                CompMtlImgCopy imageMat = new CompMtlImgCopy(baseMod.UiContainer, false);
                imageMat.Image.SetSource("textures/syferfontein.hdr");
                CompUiCtrlPicture cubemapDisplayImg = new CompUiCtrlPicture(filePreviewWnd, UiPositioning.Inside(filePreviewWnd, "0"), imageMat);
                cubemapDisplayImg.Width = "90%";
                cubemapDisplayImg.SizingStyle = ImageSizingStyle.AutoHeight;

                // LOD slider
                CompUiCtrlLabel lblSliderTitle = new CompUiCtrlLabel(filePreviewWnd, "Background LOD: ", UiPositioning.Inside(filePreviewWnd, "0 70%"));
                CompUiCtrlSlider lodSlider = new CompUiCtrlSlider(filePreviewWnd, UiPositioning.RightOf(lblSliderTitle));
                lodSlider.MaxValue = 8.0f;
                CompActionOnChange.MonitorValue(lodSlider.Value, (lod) => bg.RadianceLod = lod);
                UiPositioning.AlignCenterVertically(lblSliderTitle, lodSlider);
            }
            filePreviewWnd.Show();

            // create a camera to rotate the background
            baseMod.MainPass.Camera = new CompCamPerspective(new CompTransformEditorMovement(Context.Scene.Root, Float3.Zero, Float3.UnitZ));
        }
    }
}
