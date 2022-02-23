using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace COS1_GameOfLife
{
    public partial class ModalDialog : Form
    {
        public ModalDialog()
        {
            InitializeComponent();
        }

/*        public int GetNumber()
        {
            return (int)numericUpDownNumber.Value;
        }

        public void SetNumber(int number)
        {
            numericUpDownNumber.Value = number;
        }*/
    
        public int Interval
        {
            get
            {
                return (int)numericUpDownInterval.Value;
            }

            set
            {
                numericUpDownInterval.Value = value;
            }
        }

        public int UniverseWidth
        {
            get
            {
                return (int)numericUpDownWidth.Value;
            }

            set
            {
                numericUpDownWidth.Value = value;
            }
        }

        public int UniverseHeight
        {
            get
            {
                return (int)numericUpDownHeight.Value;
            }

            set
            {
                numericUpDownHeight.Value = value;
            }
        }

    }
}
