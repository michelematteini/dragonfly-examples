using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dragonfly.Engine.Test
{
    public partial class TestRecord : UserControl
    {
        public string TestName
        {
            get { return lblTitle.Text; }
            set { lblTitle.Text = value; }
        }

        public int TestID { get; set; }

        public event Action<int> TestStart;

        public bool TestEnabled
        {
            get { return chkEnabled.Checked; }
            set { chkEnabled.Checked = value; }
        }

        public TestRecord()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (TestStart != null) TestStart(TestID);
        }
    }
}
