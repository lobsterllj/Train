using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Train.Trains
{
    public partial class SetLocForm : Form
    {
        private TrainLocation tl;
        public SetLocForm(TrainLocation trainLocation)
        {
            InitializeComponent();
            tl = trainLocation;
            lblTrainLength.Text = "列车长度：" + TrainInfo.L_TRAIN;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            double kilo = 0, meter = 0;
            try
            {
                kilo = Convert.ToDouble(tbKilo.Text);
                meter = Convert.ToDouble(tbMeter.Text);
                double tmp = kilo * 1000 + meter;
                // make sure train left end and right end are in valid range
                if ( tmp<0 || tmp+TrainInfo.L_TRAIN>TrainDynamics.LINE_LENGTH)
                {
                    MessageBox.Show("out of line range!");
                    return;
                }
                tl.LeftLoc = tmp;
                tl.RightLoc = tl.LeftLoc + TrainInfo.L_TRAIN;
                tl.IsSetLoc = true;
                Close();
            }
            catch (FormatException fe)
            {
                MessageBox.Show(fe.Message);
            }
        }
    }
}
