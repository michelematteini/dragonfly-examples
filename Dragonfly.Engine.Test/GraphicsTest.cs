using Dragonfly.BaseModule;
using Dragonfly.Engine.Core;
using Dragonfly.Graphics.Math;
using System;

namespace Dragonfly.Engine.Test
{
    public abstract class GraphicsTest
    {
        public GraphicsTest()
        {
            Name = "Untitled Test";
            TestDurationSeconds = 6.0f;
            OverrideScreen = false;
        }

        public EngineContext Context { get; set; }

        /// <summary>
        /// Debug statistics collected by the test. 
        /// This can be optionally created and assigned to this property by the test implementation.
        /// </summary>
        public CompUiWndDebugInfo TestResults { get; protected set; }

        public string Name { get; protected set; }

        public abstract void CreateScene();
        
        public float TestDurationSeconds { get; protected set; }

        public bool TestMode { get; set; }

        /// <summary>
        /// Returns true if the test want to control resolution and full screen state.
        /// </summary>
        public bool OverrideScreen { get; protected set; }

        /// <summary>
        /// The context setup type expected by the test.
        /// </summary>
        public BaseMod.Usage EngineUsage { get; protected set; }

        #region Helper code

        protected CompUiLoadingScreen CreateLoadingScreen(Component parent, Action onLoadingCompleted, string backgroundTexturePath)
        {
            CompTextureRef.LoadSyncronously = true;

            CompUiLoadingScreen loadingScreen = Context.GetModule<BaseMod>().LoadingScreen;
            loadingScreen.HideAutomatically = true;

            if (onLoadingCompleted != null)
                new CompActionOnEvent(new CompEventLoadingScreenVisible(parent, EventTriggerType.End).Event, onLoadingCompleted);

            // add controls to the loading screen
            {
                CompUiCtrlPicture loadingBg = new CompUiCtrlPicture(loadingScreen.UiPanel, backgroundTexturePath);
                loadingBg.SizingStyle = ImageSizingStyle.FillScreen;
                loadingBg.CustomMeshZIndex++;
            
                CompUiCtrlPicture bgMask = new CompUiCtrlPicture(loadingScreen.UiPanel, "textures/loadingMask.png", "0 50%");
                bgMask.Size = "100% 50%";

                CompUiCtrlPicture bgMaskTop = new CompUiCtrlPicture(loadingScreen.UiPanel, "textures/loadingMask.png", "0 0", Float2.One, Float2.Zero);
                bgMaskTop.Size = "100% 50%";

                CompUiCtrlLabel loadingLabel = new CompUiCtrlLabel(loadingScreen.UiPanel, "Loading", "50% 94%");
                loadingLabel.Font.Size = "4%";
            }
            
            CompTextureRef.LoadSyncronously = false;

            return loadingScreen;
        }

        protected void AddDebugInfoWindow()
        {
            TestResults = new CompUiWndDebugInfo(Context.Scene.Root);
            TestResults.Window.Show();
        }

        #endregion

    }
}
