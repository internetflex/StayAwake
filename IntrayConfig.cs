using System.Windows.Forms;

namespace SystemTray
{
    public partial class IntrayConfig : Form
    {
        public IntrayConfig()
        {
            InitializeComponent();
            WaitInterval = 1;
            StayAwakeActive = true;
        }

        public long WaitInterval { get; set; }
        public bool StayAwakeActive { get; set; }

        private void IntrayConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            WaitInterval = (long)waitInterval.Value;
            StayAwakeActive = checkBox1.CheckState == CheckState.Checked;
        }
    }
}
