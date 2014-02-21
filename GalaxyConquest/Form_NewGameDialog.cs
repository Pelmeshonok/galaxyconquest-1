﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GalaxyConquest
{
    public partial class Form_NewGameDialog : Form
    {
        int galaxytype = 0;

        public Form_NewGameDialog()
        {
            InitializeComponent();
        }

        private void buttonGalaxyTypeLeft_Click(object sender, EventArgs e)
        {
            if (galaxytype == 0)
            {
                pictureBoxGalaxyType.Image = Properties.Resources.icon_newgame_sphere;
                labelGalaxyType.Text = "Sphere";
                galaxytype = 1;
            }
            else
            {
                pictureBoxGalaxyType.Image = Properties.Resources.icon_newgame_spiral;
                labelGalaxyType.Text = "Spiral";
                galaxytype = 0;
            }
        }

        private void buttonGalaxyTypeRight_Click(object sender, EventArgs e)
        {
            if (galaxytype == 0)
            {
                pictureBoxGalaxyType.Image = Properties.Resources.icon_newgame_sphere;
                labelGalaxyType.Text = "Sphere";
                galaxytype = 1;
            }
            else
            {
                pictureBoxGalaxyType.Image = Properties.Resources.icon_newgame_spiral;
                labelGalaxyType.Text = "Spiral";
                galaxytype = 0;
            }
        }
    }
}