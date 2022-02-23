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
    public partial class SeedDialog : Form
    {
        public SeedDialog()
        {
            InitializeComponent();
        }

        public int Seed
        {
            get
            {
                return (int)numericUpDownSeed.Value;
            }

            set
            {
                numericUpDownSeed.Value = value;
            }
        }
    }
}
