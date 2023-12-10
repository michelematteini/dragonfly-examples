using Dragonfly.BaseModule;
using Dragonfly.Engine.Core;
using Dragonfly.Engine.Test.GraphicTests;
using Dragonfly.Graphics;
using Dragonfly.Graphics.Math;
using Dragonfly.Utils;
using Dragonfly.Utils.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Dragonfly.Engine.Test
{

    public partial class FrmTestGUI : Form
    {
        private const string LOG_FILE_PATH = "test_log.txt";

        private Panel3D canvas;
        private List<TestRecord> guiRecords;
        private List<GraphicsTest> tests;
        private Queue<int> toBeExecuted; // list of tests to be executed
        private List<List<Int2>> supportedApiResolution;

        public FrmTestGUI()
        {
            InitializeComponent();
            InitCanvas();

            guiRecords = new List<TestRecord>();
            tests = new List<GraphicsTest>();
            supportedApiResolution = new List<List<Int2>>();
            toBeExecuted = new Queue<int>();

            LoadAvailableAPIs();

            AddTest(new ClearBlueTest());
            AddTest(new RotatingObjectTest());
            AddTest(new TextureLoaderTest());
            AddTest(new FullScreenTest());
            AddTest(new ViewportTest());
            AddTest(new PathTest());
            AddTest(new SpriteTextTest());
            AddTest(new PhongMaterialTest());
            AddTest(new ShadowmapTest());
            AddTest(new ObjForestTest());
            AddTest(new ProceduralTest());
            AddTest(new UiTest());
            AddTest(new PhysicalMaterialTest());
            AddTest(new RadianceMapTest());
            AddTest(new HemisphereSampleTest());
            AddTest(new TerrainTest());
            AddTest(new VBufferBakerTest());
            AddTest(new EngineOverheadTest());
            AddTest(new NoiseTest());
            AddTest(new HeighmapVisualizerTest());
            AddTest(new PlanetTest());
            InitLog();
        }

        private void InitLog()
        {
            if(File.Exists(LOG_FILE_PATH))
            {
                foreach (string logLine in File.ReadLines(LOG_FILE_PATH))
                    Log(logLine);
            }

            Log(string.Format("============= Dragonfly test section - {0} =============", DateTime.Now.ToString()));
        }

        private void InitCanvas()
        {
            canvas = new Panel3D();
            canvas.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            canvas.Antialising = false;
            canvas.BackColor = System.Drawing.Color.Black;
            canvas.CaptureErrors = false;
            canvas.Location = pnlTests.Location;
            canvas.Name = "pnlCanvas";
            canvas.RenderOnMainThread = true;
            canvas.Size = pnlTests.Size;
            canvas.StartupPath = PathEx.DefaultResourceFolder;
            canvas.TabIndex = 1;
            canvas.Visible = false;
            canvas.SceneSetup += Canvas_SceneSetup;
            canvas.EngineErrorOccurred += Canvas_EngineErrorOccurred;
            this.Controls.Add(canvas);
        }

        private void LoadAvailableAPIs()
        {
            foreach (IGraphicsAPI api in GraphicsAPIs.GetList())
            {
                if (api.IsSupported)
                {
                    supportedApiResolution.Add(api.DefaultDisplayResolutions);
                    cbGraphicAPI.Items.Add(api);
                }
            }
            cbGraphicAPI.SelectedIndex = 0;
        }

        private void Canvas_EngineErrorOccurred(Exception error)
        {
            MessageBox.Show(error.ToString());
        }

        private void Canvas_SceneSetup()
        {
            GraphicsTest curTest = tests[toBeExecuted.Peek()];

            curTest.Context = canvas.Engine;
            curTest.TestMode = !chkKeepTesting.Checked;
            curTest.Context.GetModule<BaseMod>().Initialize(curTest.EngineUsage);
            curTest.CreateScene();

            if (!chkKeepTesting.Checked) // add an event that stop the test after its duration  
                new CompActionOnEvent(new CompEventTimed(canvas.Engine.Scene.Root, curTest.TestDurationSeconds).Event.DisposeOnceOccurred(), OnTestCompleted);    

            // stop test pressing esc
            new CompActionOnEvent(new CompEventKeyPressed(canvas.Engine.Scene.Root,  VKey.VK_ESCAPE).Event, OnTestCompleted);

            // update to the required resolution
            if (!curTest.OverrideScreen)
            {
                if (cbResolution.SelectedIndex > 0)
                {
                    Int2 requiredResolution = supportedApiResolution[cbGraphicAPI.SelectedIndex][cbResolution.SelectedIndex - 1];
                    canvas.Engine.Scene.Resolution = requiredResolution;
                    if (!chkFullScreen.Checked)
                        this.Size = this.Size + new Size(requiredResolution.X, requiredResolution.Y) - canvas.Size;
                }
                else
                    canvas.Engine.Scene.ResizeStyle = ResizeStyle.Automatic;
                
                canvas.Engine.Scene.FullScreen = chkFullScreen.Checked;
                canvas.Engine.Scene.VSyncEnabled = chkVSync.Checked;
            }
            curTest.Context.GetModule<BaseMod>().Sound.Paused = !chkSound.Checked;
        }

        private void AddTest(GraphicsTest test)
        {
            AddGuiRecord(test.Name, tests.Count);
            tests.Add(test);
        }

        private void AddGuiRecord(string name, int id)
        {
            int recordHeight = 55, margin = 5;

            TestRecord testRecord = new TestRecord();
            testRecord.Size = new System.Drawing.Size(pnlTests.Width, recordHeight);
            testRecord.Location = new System.Drawing.Point(0, guiRecords.Count * (recordHeight + margin) + margin);
            testRecord.TestName = name;
            testRecord.TestID = id;
            testRecord.TestStart += StartTest;
            testRecord.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            guiRecords.Add(testRecord);
            pnlTests.Controls.Add(testRecord);
            pnlTests.AutoScroll = true;
        }

        private void StartTest(int testID)
        {
            toBeExecuted.Clear();
            toBeExecuted.Enqueue(testID);
            RunNextTest();
        }

        private void OnTestCompleted()
        {
            BeginInvoke(new Action(StopTest));
        }

        private void StopTest()
        {        
            GraphicsTest completedTest = tests[toBeExecuted.Dequeue()];
            LogResults(completedTest);
            canvas.DestroyGraphics();
            SetGuiMode(false);
            if (toBeExecuted.Count > 0)
                RunNextTest();
        }

        private void cbGraphicAPI_SelectedIndexChanged(object sender, EventArgs e)
        {
            GraphicsAPIs.SetDefault((IGraphicsAPI)cbGraphicAPI.SelectedItem);

            // update display reslutions
            cbResolution.Items.Clear();
            cbResolution.Items.Add("Auto");
            for (int i = 0; i < supportedApiResolution[cbGraphicAPI.SelectedIndex].Count; i++)
                cbResolution.Items.Add(supportedApiResolution[cbGraphicAPI.SelectedIndex][i]);

            cbResolution.SelectedIndex = 0;
        }

        private void btnRunAllTests_Click(object sender, EventArgs e)
        {
            toBeExecuted.Clear();
            for (int i = 0; i < tests.Count; i++)
                if (guiRecords[i].TestEnabled) toBeExecuted.Enqueue(i);

            RunNextTest();
        }

        private void RunNextTest()
        {
            if (toBeExecuted.Count == 0) return;

            canvas.CaptureErrors = chkCaptureErrors.Checked;
            SetGuiMode(true); // once visible, the control start rendering automatically
        }

        private void SetGuiMode(bool testing)
        {
            pnlTests.Visible = !testing;
            canvas.Visible = testing;
            pnlSettings.Enabled = !testing;
            if (testing) canvas.Focus();
        }


        private void LogResults(GraphicsTest test)
        {
            if (test.TestResults == null)
                return;
            
            string testResultLog = string.Format("{0} {1} API: {4}, Resolution: {3}, Avg. FPS: {2}", 
                DateTime.Now.ToShortTimeString(), 
                test.Name, 
                test.TestResults.FpsAvg, 
                test.Context.Scene.Resolution.X + "x" + test.Context.Scene.Resolution.Y,
                cbGraphicAPI.Text
            );

            Log(testResultLog);
        }            

        private void Log(string text)
        {
            lstLog.Items.Add(text);
        }

        private void saveToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] logLines = new string[lstLog.Items.Count];
            for (int i = 0; i < lstLog.Items.Count; i++)
                logLines[i] = lstLog.Items[i].ToString();

            File.WriteAllLines(LOG_FILE_PATH, logLines);
        }

        private void reloadFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lstLog.Items.Clear();
            InitLog();
        }
    }
}
