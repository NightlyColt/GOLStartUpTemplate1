using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GOLStartUpTemplate1
{
    public partial class SeedDialog : Form
    {
        
        public SeedDialog()
        {
            InitializeComponent();
        }

        // property to access/set numeric up down for universe's seed
        public int RandomSeed
        {
            get
            {
                return (int)numericUpDown1.Value;
            }
            set
            {
                numericUpDown1.Value = value;
            }
        }

        private void RandomButton_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            RandomSeed = random.Next();
            numericUpDown1.Value = RandomSeed;
        }
    }
}
