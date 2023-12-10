using Dragonfly.BaseModule;
using Dragonfly.Engine.Core;
using Dragonfly.Graphics.Math;

namespace Dragonfly.Engine.Test.GraphicTests
{
    public class ClearBlueTest : GraphicsTest
    {
        public ClearBlueTest()
        {
            Name = "Basic Tests: Clear blue test";
            EngineUsage = BaseMod.Usage.Generic3D;
        }

        public override void CreateScene()
        {
            CompRenderPass mainView = Context.GetModule<BaseMod>().MainPass;
            mainView.ClearValue = new Float4("#84c4ef");
        }
    }
}
