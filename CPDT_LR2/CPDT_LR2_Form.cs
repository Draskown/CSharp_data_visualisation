using System.Drawing;
using System.Windows.Forms;

namespace CPDT_LR2
{
    public partial class CPDT_LR2_Form : Form
    {
        public CPDT_LR2_Form()
        {
            InitializeComponent();

            this.boxLog.Text += "12:13> received 2023 points\r\n12:13> received 2023 points\r\n12:13> received 2023 points\r\n";

            //this.boxLog.Lines[0] = "12:13> received 2023 points";
            //this.boxLog.Lines[1] = "12:13> received 2023 points";
        }
    }
}
