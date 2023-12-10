using Dragonfly.BaseModule;
using Dragonfly.Engine.Core;
using Dragonfly.Graphics.Math;
using Dragonfly.Graphics.Resources;

namespace Dragonfly.Engine.Test.GraphicTests
{
    public class VBufferBakerTest : GraphicsTest
    {
        public VBufferBakerTest()
        {
            Name = "Component Tests: CompBakerVertexBuffer";
            EngineUsage = BaseMod.Usage.Generic3D;
        }

        public override void CreateScene()
        {
            AddDebugInfoWindow();

            BaseMod baseMod = Context.GetModule<BaseMod>();
            Component root = Context.Scene.Root;
            
            baseMod.MainPass.ClearValue = new Float4("#002040");

            // create camera
            baseMod.MainPass.Camera = new CompCamPerspective(new CompTransformEditorMovement(root, new Float3(50, 30, -30), new Float3(50, 0, 50)));

            // ground plane with vertices baked with a custom material
            CompBakedGeometry meshGeom = new CompBakedGeometry(root, new Int2(100, 100));
            meshGeom.VertexBakingMaterial = new CompMtlGeomWaved(meshGeom);
            CompMesh ground = new CompMesh(root, new CompMtlBasic(root, new Float3("#80FF00")) { FillMode = Graphics.FillMode.Wireframe }.DisplayIn(baseMod.MainPass), meshGeom);
        }
    }

    class CompMtlGeomWaved : CompMaterial
    {
        public CompMtlGeomWaved(Component parent) : base(parent) { }

        public override string EffectName => "GeomWaved";

        protected override void UpdateParams() { }
    }

}
