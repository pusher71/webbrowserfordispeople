using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace webBro
{
    public partial class Form2 : Form
    {
        int state;
        float borderLeft = -200;
        float borderRight = 200;
        float borderUp = 320;
        float borderDown = 100;
        public Form2()
        {
            InitializeComponent();
            state = 0;
            helloBox.Image = new Bitmap(Properties.Resources.state0);
        }

        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            webBro ifrm = Owner as webBro;
            if (e.KeyCode == Keys.Q && ifrm.finger != null && ifrm.finger.TipPosition.x != 0 && ifrm.finger.TipPosition.y != 0)
            {
                if (state == 0)
                {
                    borderLeft = ifrm.finger.TipPosition.x;
                    borderUp = ifrm.finger.TipPosition.y;
                    state = 1;
                    helloBox.Image = new Bitmap(Properties.Resources.state1);
                }
                else if (state == 1)
                {
                    borderRight = ifrm.finger.TipPosition.x;
                    borderDown = ifrm.finger.TipPosition.y;
                    ifrm.borderLeft = borderLeft;
                    ifrm.borderUp = borderUp;
                    ifrm.borderRight = borderRight;
                    ifrm.borderDown = borderDown;
                    ifrm.firstLeap = false;
                    Close();
                }
            }
        }
    }
}
