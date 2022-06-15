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
    public partial class OptionDialog : Form
    {
        public OptionDialog()
        {
            InitializeComponent();
        }

        // property to access/set numeric up down for the time interval
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

        // property to access/set numeric up down for the universe's Width
        public int UWidth
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

        // property to access/set numeric up down for the universe's Height
        public int UHeight
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
