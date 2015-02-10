﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using GalaxyConquest.StarSystems;
using GalaxyConquest.Tactics;
using NAudio;
using NAudio.Wave;

// для работы с библиотекой FreeGLUT 
using Tao.FreeGlut;

namespace GalaxyConquest
{
    [Serializable]
    public partial class Form1 : Form
    {
        //-----added

        //Цвета для флотов
        public static Brush activeFleetBrush = Brushes.GreenYellow;
        public static Brush passiveFleetBrush = Brushes.MediumSeaGreen;
        public static Brush neutralFleetBrush = Brushes.Silver;
        public static Brush enemyFleetBrush = Brushes.OrangeRed;

        public static Color activeFleetColor = Color.GreenYellow;
        public static Color passiveFleetColor = Color.MediumSeaGreen;
        public static Color neutralFleetColor = Color.Silver;
        public static Color enemyFleetColor = Color.OrangeRed;

        private int dispersion = 7;    //Размер выделяемой области вокруг звезды

        //Число, которое отвечает за скорость передвижения флотов во время шага (чем больше, тем медленнее)
        //При малых значениях движение происходит мгновенно
        //Фактически это множитель количества пересчетов координат (fly * speed раз перечсчитываются)
        int speed = 2000000;

        private float centerX;
        private float centerY;
        //-------------

        public static double credit = 0;
        public static int energy = 0;
        public static int minerals = 0;

        public ModelGalaxy galaxy;
        public Bitmap galaxyBitmap;
        public double spinX = 0.0;
        public double spinY = Math.PI / 4;

        public float scaling = 1f;
        //---edited
        public float horizontal = 0;  //для увеличения плавности прокручивания стал float (был int)
        public float vertical = 0;    //

        public float dynamicStarSize = 5; //Variable for dynamic of fix scale 
        public double starDistanse;
        public int maxDistanse = 150;
        public double s2x, s2y, s2z;
        public double warp;

        public int selectedFleet = 0;
        public static int star_selected; ////-----------------попытаюсь заменить на ссылку на объект звезды
        public static StarSystem selectedStar;  // <- ссылка на объект

        public int mouseX;
        public int mouseY;
        //Brushes for stars colors
        public static SolidBrush BlueBrush = new SolidBrush(Color.FromArgb(255, 123, 104, 238));
        public static SolidBrush LightBlueBrush = new SolidBrush(Color.FromArgb(180, 135, 206, 235));
        public static SolidBrush WhiteBrush = new SolidBrush(Color.FromArgb(255, 225, 250, 240));
        public static SolidBrush LightYellowBrush = new SolidBrush(Color.FromArgb(180, 255, 255, 0));
        public static SolidBrush YellowBrush = new SolidBrush(Color.FromArgb(255, 255, 255, 0));
        public static SolidBrush OrangeBrush = new SolidBrush(Color.FromArgb(255, 255, 140, 0));
        public static SolidBrush RedBrush = new SolidBrush(Color.FromArgb(255, 255, 0, 0));
        public static SolidBrush SuperWhiteBrush = new SolidBrush(Color.FromArgb(255, 255, 255, 255));
        public static SolidBrush GoldBrush = new SolidBrush(Color.Gold);

        public static Shop shop_form;

        public static Player player = new Player();//contain player staff
        public Tech_Tree tt = new Tech_Tree();
        IWavePlayer waveOutDevice;
        AudioFileReader audioFileReader;

        public static Form1 SelfRef         //need for get var from other classes
        {
            get;
            set;
        }

        public Form1()
        {
            InitializeComponent();
            shop_form = new Shop();
            Buildings builds = new Buildings();
            SelfRef = this;
            tech_progressBar.Visible = false;
            tech_label.Visible = false;
            listView.Visible = false;
            this.MouseWheel += new MouseEventHandler(this.onMouseWheel); // for resizing of galaxy at event change wheel mouse
            waveOutDevice = new WaveOutEvent();
            audioFileReader = new AudioFileReader("Sounds/Untitled45.mp3");
            waveOutDevice.Init(audioFileReader);
            waveOutDevice.Play();
            statusStrip1.Items[0].Text = "Выбран 1 флот";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            centerX = galaxyImage.Width / 2;
            centerY = galaxyImage.Height / 2;
        }

        public override Size MinimumSize
        {
            get
            {
                return base.MinimumSize;
            }
            set
            {
                base.MinimumSize = new Size(this.Width, this.Height);

            }

        }
        
        //------------------------------------Sound------------------------------------

        private void sound_button_Click(object sender, EventArgs e)
        {
            if (waveOutDevice.PlaybackState == PlaybackState.Playing)
            {
                waveOutDevice.Pause();
                sound_button.Text = "Unmute";
            }
            else
            {
                waveOutDevice.Play();
                sound_button.Text = "Mute";
            }
        }

        //-----------------------------Main Menu --------------------------------------

        private void mainMenuQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void mainMenuNew_Click(object sender, EventArgs e)
        {
            Form_NewGameDialog nd = new Form_NewGameDialog();
            if (nd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                player = new Player();

                galaxy = new ModelGalaxy(player);
                galaxy.GenerateNew(nd.galaxyName, nd.namePlayer, nd.getGalaxyType(), nd.getGalaxySize(), nd.getStarsCount(), nd.getGalaxyRandomEvents());

                galaxyNameLablel.Text = galaxy.name;
                groupBox1.Text = player.name;

                UpdateLabels();

                GameTimer.Start();
            }
        }

        private void mainMenuSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sDlg = new SaveFileDialog();
            if (sDlg.ShowDialog() == DialogResult.OK)
            {
                string fileName = sDlg.FileName;

                FileStream fs = new FileStream(fileName, FileMode.CreateNew);


                //XmlSerializer xs = new XmlSerializer(typeof(ModelGalaxy));
                //xs.Serialize(fs, galaxy);                
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, galaxy);

                fs.Close();
            }
        }

        private void mainMenuOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog sDlg = new OpenFileDialog();
            if (sDlg.ShowDialog() == DialogResult.OK)
            {
                string fileName = sDlg.FileName;

                FileStream fs = new FileStream(fileName, FileMode.Open);

                //XmlSerializer xs = new XmlSerializer(typeof(ModelGalaxy));
                //xs.Serialize(fs, galaxy);                
                BinaryFormatter bf = new BinaryFormatter();
                galaxy = (ModelGalaxy)bf.Deserialize(fs);

                fs.Close();

                //Redraw();
                galaxyNameLablel.Text = galaxy.name;
            }

        }

        private void mainMenuAbout_Click(object sender, EventArgs e)
        {
            Form_About af = new Form_About();
            af.ShowDialog();
        }

        private void MainMenuTechTree_Click(object sender, EventArgs e)
        {
            tt.ShowDialog();
        }

        //------------------------------------Events-----------------------------------
        
        //отрисовка
        private void galaxyImage_Paint(object sender, PaintEventArgs e)
        {
            if (galaxy == null)
            {
                return;
            }

            int r = 6;
            Pen pen = new Pen(Color.Gold);
            double ugol = 2 * Math.PI / (3);
            Point[] points = new Point[3];

            Graphics g = e.Graphics;

            g.FillRectangle(Brushes.Black, 0, 0, galaxyImage.Width, galaxyImage.Height);

            g.ScaleTransform(scaling, scaling);//resize image(zooming in/out)
            
            //рисуем звездные системы
            for (int i = 0; i < galaxy.stars.Count; i++)
            {
                float starSize = 0;

                double screenX;
                double screenY;
                double screenZ;

                StarSystem s = galaxy.stars[i];

                screenX = s.x * Math.Cos(spinX) - s.z * Math.Sin(spinX);
                screenZ = s.x * Math.Sin(spinX) + s.z * Math.Cos(spinX);
                screenY = s.y * Math.Cos(spinY) - screenZ * Math.Sin(spinY);

                starSize = s.type + dynamicStarSize;

                g.FillEllipse(s.br, centerX + (float)screenX - starSize / 2, centerY + (float)screenY - starSize / 2, starSize, starSize);

                RectangleF rectan = new RectangleF((centerX - 5 + (float)screenX - starSize / 2), (centerY - 5 + (float)screenY - starSize / 2), (starSize + 10), (starSize + 10));

                if (s == selectedStar)       //if (s == galaxy.stars[star_selected])
                {
                    g.DrawEllipse(pen, rectan);
                }

                rectan = new RectangleF((centerX - 3 + (float)screenX - starSize / 2), (centerY - 3 + (float)screenY - starSize / 2), (starSize + 6), (starSize + 6));

                if (player.player_stars.Contains(s))
                {
                    g.DrawEllipse(Pens.GreenYellow, rectan);
                    g.DrawString(s.name, new Font("Arial", 5.5F, FontStyle.Bold), Brushes.GreenYellow, new PointF(centerX + (float)screenX + 6, centerY + (float)screenY + 6));
                }
                else
                    g.DrawString(s.name, new Font("Arial", 5.0F), Brushes.White, new PointF(centerX + (float)screenX + 6, centerY + (float)screenY + 6));
            }


            //----------------Neutral fleets------------------
            for (int k = 0; k < galaxy.neutrals.Count; k++)
            {
                Fleet fleet = galaxy.neutrals[k];
                //if (s == galaxy.neutrals[k].s1)

                double screenXfl = fleet.x * Math.Cos(spinX) - fleet.z * Math.Sin(spinX) - 10;
                double screenZfl = fleet.x * Math.Sin(spinX) + fleet.z * Math.Cos(spinX);
                double screenYfl = fleet.y * Math.Cos(spinY) - screenZfl * Math.Sin(spinY) - 10;

                PointF[] compPointArrayShip = {  //точки для рисование корабля
                                    new PointF(centerX + (float)screenXfl + r * (float)Math.Cos(-1 * ugol), centerY + (float)screenYfl + r * (float)Math.Sin(-1 * ugol)),
                                    new PointF(centerX + (float)screenXfl + r * (float)Math.Cos(-2 * ugol), centerY + (float)screenYfl + r * (float)Math.Sin(-2 * ugol)),
                                    new PointF(centerX + (float)screenXfl + r * (float)Math.Cos(-3 * ugol), centerY + (float)screenYfl + r * (float)Math.Sin(-3 * ugol))};
                g.FillPolygon(neutralFleetBrush, compPointArrayShip);
                g.DrawString(fleet.name, new Font("Arial", 5.0F), neutralFleetBrush, new PointF(centerX - 3 + (float)screenXfl + r * (float)Math.Cos(-3 * ugol), centerY - 12 + (float)screenYfl + r * (float)Math.Sin(-3 * ugol)));

            }

            //----------------------Player Fleets----------------------
            for (int k = 0; k < player.player_fleets.Count; k++)
            {
                Fleet fleet = player.player_fleets[k];
                StarSystem flSys = fleet.s1;
                StarSystem targSys = fleet.s2;

                double screenXfl = fleet.x * Math.Cos(spinX) - fleet.z * Math.Sin(spinX) - 10;
                double screenZfl = fleet.x * Math.Sin(spinX) + fleet.z * Math.Cos(spinX);
                double screenYfl = fleet.y * Math.Cos(spinY) - screenZfl * Math.Sin(spinY) - 10;

                //if (playerSys != null)

                PointF[] compPointArrayShip = {  //точки для рисование корабля
                                        new PointF(centerX + (float)screenXfl + r * (float)Math.Cos(-1 * ugol), centerY + (float)screenYfl + r * (float)Math.Sin(-1 * ugol)),
                                        new PointF(centerX + (float)screenXfl + r * (float)Math.Cos(-2 * ugol), centerY + (float)screenYfl + r * (float)Math.Sin(-2 * ugol)),
                                        new PointF(centerX + (float)screenXfl + r * (float)Math.Cos(-3 * ugol), centerY + (float)screenYfl + r * (float)Math.Sin(-3 * ugol))};

                if (k == selectedFleet)
                {
                    g.FillPolygon(activeFleetBrush, compPointArrayShip);
                    g.DrawString(fleet.name, new Font("Arial", 9.0F, FontStyle.Bold), activeFleetBrush, new PointF(centerX - 3 + (float)screenXfl + r * (float)Math.Cos(-3 * ugol), centerY - 12 + (float)screenYfl + r * (float)Math.Sin(-3 * ugol)));

                }
                else
                {
                    g.FillPolygon(passiveFleetBrush, compPointArrayShip);
                    g.DrawString(fleet.name, new Font("Arial", 8.0F), passiveFleetBrush, new PointF(centerX - 3 + (float)screenXfl + r * (float)Math.Cos(-3 * ugol), centerY - 12 + (float)screenYfl + r * (float)Math.Sin(-3 * ugol)));
                }

                if (warp == 1 && k == selectedFleet && player.player_fleets[selectedFleet].starDistanse == 0)
                {
                    string dis = Math.Round(starDistanse, 3).ToString() + " св. лет\n<Ходов: " + ((int)(starDistanse / maxDistanse) + 1).ToString() + ">";//+-

                    double screenX, screens2X;
                    double screenY, screens2Y;
                    double screenZ, screens2Z;

                    screens2X = s2x * Math.Cos(spinX) - s2z * Math.Sin(spinX);
                    screens2Z = s2x * Math.Sin(spinX) + s2z * Math.Cos(spinX);
                    screens2Y = s2y * Math.Cos(spinY) - screens2Z * Math.Sin(spinY);

                    screenX = flSys.x * Math.Cos(spinX) - flSys.z * Math.Sin(spinX);
                    screenZ = flSys.x * Math.Sin(spinX) + flSys.z * Math.Cos(spinX);
                    screenY = flSys.y * Math.Cos(spinY) - screenZ * Math.Sin(spinY);

                    if (starDistanse < maxDistanse || true)//пока тестим
                    {
                        pen.Color = Color.Lime;
                        g.DrawString(dis, new Font("Arial", 6.0F), Brushes.Lime,
                            new PointF(centerX - 3 + (float)screens2X + r * (float)Math.Cos(-3 * ugol), centerY + 12 + (float)screens2Y + r * (float)Math.Sin(-3 * ugol)));
                    }
                    else
                    {
                        pen.Color = Color.Red;
                        g.DrawString(dis, new Font("Arial", 6.0F), Brushes.Red,
                            new PointF(centerX - 3 + (float)screens2X + r * (float)Math.Cos(-3 * ugol), centerY + 12 + (float)screens2Y + r * (float)Math.Sin(-3 * ugol)));
                    }

                    g.DrawLine(pen,
                        new PointF((centerX + (float)screenX), (centerY + (float)screenY)),
                        new PointF((centerX + (float)screens2X), (centerY + (float)screens2Y)));
                }


                if (targSys != null)
                {
                    double screenXflS1 = flSys.x * Math.Cos(spinX) - flSys.z * Math.Sin(spinX) - 10;
                    double screenZflS1 = flSys.x * Math.Sin(spinX) + flSys.z * Math.Cos(spinX);
                    double screenYflS1 = flSys.y * Math.Cos(spinY) - screenZflS1 * Math.Sin(spinY) - 10;

                    double screenXflS2 = targSys.x * Math.Cos(spinX) - targSys.z * Math.Sin(spinX) - 10;
                    double screenZflS2 = targSys.x * Math.Sin(spinX) + targSys.z * Math.Cos(spinX);
                    double screenYflS2 = targSys.y * Math.Cos(spinY) - screenZflS2 * Math.Sin(spinY) - 10;

                    pen.Color = Color.Red;
                    pen.DashStyle = DashStyle.Dash;

                    g.DrawLine(pen,
                            new PointF((centerX + (float)screenXflS1 + 10), (centerY + (float)screenYflS1) + 10),
                            new PointF((centerX + (float)screenXflS2 + 10), (centerY + (float)screenYflS2) + 10));
                }
                pen.Color = Color.Gold;
                pen.DashStyle = DashStyle.Solid;
            }

            //рисуем гиперпереходы
            for (int i = 0; i < galaxy.lanes.Count; i++)
            {
                StarWarp w = galaxy.lanes[i];

                g.DrawLine(Pens.Gray,
                    new PointF((centerX + (float)w.system1.x), (centerY + (float)w.system1.y)),
                    new PointF((centerX + (float)w.system2.x), (centerY + (float)w.system2.y)));
            }
        }

        //пересчет координат центров, когда изменяется размер окна
        private void Form1_Resize(object sender, EventArgs e)
        {
            UpdateCenters();
            //galaxyImage.Refresh();
        }

        private void galaxyImage_MouseDown(object sender, MouseEventArgs e)
        {
            mouseX = e.X;   //start x
            mouseY = e.Y;   //start y
        }

        private void galaxyImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (galaxy != null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    #region old
                    /*
                    int dx = mouseX - e.X;
                    int dy = mouseY - e.Y;
                    if (dx > 0)
                    {
                        horizontal -= 5;
                    }
                    if (dx < 0)
                    {
                        horizontal += 5;
                    }
                    if (dy > 0)
                    {
                        vertical -= 5;
                    }
                    if (dy < 0)
                    {
                        vertical += 5;
                    }
                    */
                    #endregion

                    if (ModifierKeys == Keys.Shift)
                    {
                        spinX += (e.X - mouseX) * 0.01;
                        spinY += (e.Y - mouseY) * 0.01;
                    }
                    else
                    {
                        horizontal += (e.X - mouseX) / scaling;
                        vertical += (e.Y - mouseY) / scaling;
                    }

                    mouseX = e.X;   //set start x again
                    mouseY = e.Y;   //set start y again

                    UpdateCenters();
                }

                for (int j = 0; j < galaxy.stars.Count; j++)
                {
                    StarSystem s = galaxy.stars[j];

                    if (mouseIsInStar(e, s))
                    {
                        if (player.player_fleets[selectedFleet].s1 == s)
                            return;

                        warp = 1;
                        starDistanse = Math.Sqrt(Math.Pow((s.x - player.player_fleets[selectedFleet].s1.x), 2) + Math.Pow((s.y - player.player_fleets[selectedFleet].s1.y), 2) + Math.Pow((s.z - player.player_fleets[selectedFleet].s1.z), 2));
                        statusStrip1.Items[1].Text = "x: " + s.x + " y: " + s.y;
                        break;
                    }
                    else
                    {
                        warp = 0;
                        statusStrip1.Items[1].Text = "";
                    }
                }
            }
        }

        private void onMouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (scaling >= 10)
                    return;
                else
                {
                    scaling += 0.2f;
                    if (dynamicStarSize >= 3) dynamicStarSize -= 0.4f;
                    else if (dynamicStarSize >= 2) dynamicStarSize -= 0.05f;
                    else if (dynamicStarSize >= 0) dynamicStarSize -= 0.01f;
                    //Redraw();
                }
            }
            else
            {
                if (scaling <= 0.4)
                    return;
                else
                {
                    scaling -= 0.2f;
                    if (dynamicStarSize <= 2)
                        dynamicStarSize += 0.01f;
                    else if (dynamicStarSize <= 3)
                        dynamicStarSize += 0.05f;
                    else if (dynamicStarSize <= 5)
                        dynamicStarSize += 0.4f;
                    //Redraw();
                }
            }
            UpdateCenters();
            //Redraw();
            //galaxyImage.Refresh();
        }

        private void galaxyImage_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            for (int j = 0; j < galaxy.stars.Count; j++)
            {
                #region old
                /*
                StarSystem s = galaxy.stars[j];
                
                double screenX;
                double screenY;
                double tX, tY, tZ;
                double starSize;
                
                float centerX = galaxyBitmap.Width / 2 / scaling;
                float centerY = galaxyBitmap.Height / 2 / scaling;

                centerX += horizontal;  //move galaxy left/right
                centerY += vertical;    //move galaxy up/down
                
                tX = s.x * Math.Cos(spinX) - s.z * Math.Sin(spinX);
                tZ = s.x * Math.Sin(spinX) + s.z * Math.Cos(spinX);
                tY = s.y * Math.Cos(spinY) - tZ * Math.Sin(spinY);

                screenX = tX;
                screenY = tY;

                starSize = s.type + dynamicStarSize;

                //--------------------------------------//

                //check for mouse in the star ellipce
                if (e.X / scaling > (centerX + (int)screenX - starSize / 2) &&
                    e.X / scaling < (centerX + (int)screenX + starSize / 2) &&
                    e.Y / scaling > (centerY + (int)screenY - starSize / 2) &&
                    e.Y / scaling < (centerY + (int)screenY + starSize / 2))
                    */
                #endregion

                if (mouseIsInStar(e, galaxy.stars[j]))
                {
                    selectedStar = null;     //star_selected = j;//store type for selected star

                    StarSystemForm ssm = new StarSystemForm(galaxy.stars[j]);
                    ssm.ShowDialog();

                    return;
                }
            }

            for (int i = 0; i < player.player_fleets.Count; i++)
            {
                if (mouseIsInFleet(e, player.player_fleets[i]))
                {
                    int scout = 0, assault = 0, health = 0;
                    for (int j = 0; j < player.player_fleets[i].ships.Count; j++)
                    {
                        health += player.player_fleets[i].ships[j].currentHealth;
                        if (player.player_fleets[i].ships[j] is ShipScout)
                            scout++;
                        else if (player.player_fleets[i].ships[j] is ShipAssaulter)
                            assault++;
                    }

                    MessageBox.Show("\n\nШтурмовых кораблей - " + assault + "\nИстребителей - " + scout + "\n\nОбщее количество здоровья: " + health + "hp", "Флот " + (i + 1));
                    return;
                }
            }

            for (int i = 0; i < galaxy.neutrals.Count; i++)
            {
                if (mouseIsInFleet(e, galaxy.neutrals[i]))
                {
                    int scout = 0, assault = 0, health = 0;
                    for (int j = 0; j < galaxy.neutrals[i].ships.Count; j++)
                    {
                        health += galaxy.neutrals[i].ships[j].currentHealth;
                        if (galaxy.neutrals[i].ships[j] is ShipScout)
                            scout++;
                        else if (galaxy.neutrals[i].ships[j] is ShipAssaulter)
                            assault++;
                    }

                    MessageBox.Show("\n\nШтурмовых кораблей - " + assault + "\nИстребителей - " + scout + "\n\nОбщее количество здоровья: " + health + "hp", "Нейтральный флот");
                    return;
                }
            }
        }

        private void galaxyImage_MouseClick(object sender, MouseEventArgs e)
        {
            for (int j = 0; j < galaxy.stars.Count; j++)
            {
                StarSystem s = galaxy.stars[j];

                if (mouseIsInStar(e, s))
                {
                    if ((conquer_progressBar.Visible == false) && (player.player_fleets[selectedFleet].starDistanse == 0))
                    {
                        starDistanse = Math.Sqrt(Math.Pow((s.x - player.player_fleets[selectedFleet].s1.x), 2) + Math.Pow((s.y - player.player_fleets[selectedFleet].s1.y), 2) + Math.Pow((s.z - player.player_fleets[selectedFleet].s1.z), 2));
                        //if (starDistanse <= maxDistanse)
                        {
                            player.player_fleets[selectedFleet].s2 = s;
                            player.player_fleets[selectedFleet].starDistanse = starDistanse;
                            selectedStar = s;    //star_selected = j;   //store type for selected star
                        }
                    }
                    else if (player.player_fleets[selectedFleet].s2 == s)///-----new
                    {
                        if (player.player_fleets[selectedFleet].onWay)
                            break;

                        player.player_fleets[selectedFleet].s2 = null;
                        player.player_fleets[selectedFleet].starDistanse = 0;
                        selectedStar = null;
                    }
                    return;
                }
            }

            for (int i = 0; i < player.player_fleets.Count; i++)
            {
                if (mouseIsInFleet(e, player.player_fleets[i]))
                {
                    selectedFleet = i;

                    selectedStar = player.player_fleets[i].s2;//----new

                    statusStrip1.Items[0].Text = "Выбран " + (i + 1) + " флот";

                    if (!player.player_stars.Contains(player.player_fleets[i].s1) && !player.player_fleets[i].onWay)
                        button2.Enabled = true;
                    else
                        button2.Enabled = false;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            conquer_progressBar.Visible = true;
            button3.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            conquer_progressBar.Visible = false;
            conquer_progressBar.Value = conquer_progressBar.Minimum;
            button3.Visible = false;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void Shop_button_Click(object sender, EventArgs e)
        {
            shop_form.ShowDialog();

            UpdateLabels();
        }

        //------------------- Fast access panel --------------------------

        private void fleetsButton_Click(object sender, EventArgs e)
        {
            if (listView.Tag.Equals(fleetsButton.Tag))
            {
                listView.Visible = false;
                listView.Tag = "";
                return;
            }
            else
                listView.Visible = true;
            listView.Items.Clear();
            if (listView.Visible)
            {
                listView.Tag = fleetsButton.Tag;
                for (int i = 0; i < player.player_fleets.Count; i++)
                {
                    listView.Items.Add(player.player_fleets[i].name);
                    listView.Items[i].ForeColor = passiveFleetColor;
                }

                listView.Items[selectedFleet].ForeColor = activeFleetColor;
            }
        }

        private void planetsButton_Click(object sender, EventArgs e)
        {
            if (listView.Tag.Equals(planetsButton.Tag))
            {
                listView.Visible = false;
                listView.Tag = "";
                return;
            }
            else
                listView.Visible = true;

            listView.Items.Clear();
            if (listView.Visible)
            {
                listView.Tag = planetsButton.Tag;
                for (int i = 0; i < player.player_stars.Count; i++)
                    listView.Items.Add(player.player_stars[i].name);
            }
        }
        
        //-----added ----отображает путь до системы игрока от выбранного флота
        private void listView_ItemMouseHover(object sender, ListViewItemMouseHoverEventArgs e)
        {
            if (listView.Tag.Equals(planetsButton.Tag))
            {
                StarSystem star = player.player_stars[e.Item.Index];
                s2x = star.x;
                s2y = star.y;
                s2z = star.z;

                if (star != player.player_fleets[selectedFleet].s1)
                {
                    warp = 1;
                    starDistanse = Math.Sqrt(Math.Pow((star.x - player.player_fleets[selectedFleet].s1.x), 2) + Math.Pow((star.y - player.player_fleets[selectedFleet].s1.y), 2) + Math.Pow((star.z - player.player_fleets[selectedFleet].s1.z), 2));
                    statusStrip1.Items[1].Text = "x: " + star.x + " y: " + star.y;
                }
                else
                {
                    warp = 0;
                    statusStrip1.Items[1].Text = "";
                }
            }
        }
        // выбор активного флота + цветовое отображение в списке
        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView.Tag.Equals(fleetsButton.Tag))
                if (listView.SelectedIndices.Count > 0)
                {
                    listView.Items[selectedFleet].ForeColor = passiveFleetColor;

                    selectedFleet = listView.SelectedIndices[0];
                    selectedStar = player.player_fleets[selectedFleet].s2;//----new
                    statusStrip1.Items[0].Text = "Выбран " + (selectedFleet + 1) + " флот";

                    listView.Items[selectedFleet].ForeColor = activeFleetColor;
                }
        }
        // выбор системы игрока в качестве цели для активного флота
        private void listView_MouseClick(object sender, MouseEventArgs e)
        {
            if (listView.Tag.Equals(planetsButton.Tag))
                if (listView.SelectedIndices.Count > 0)
                {
                    if (player.player_fleets[selectedFleet].starDistanse == 0)
                    {
                        StarSystem s = player.player_stars[listView.SelectedIndices[0]];
                        starDistanse = Math.Sqrt(Math.Pow((s.x - player.player_fleets[selectedFleet].s1.x), 2) + Math.Pow((s.y - player.player_fleets[selectedFleet].s1.y), 2) + Math.Pow((s.z - player.player_fleets[selectedFleet].s1.z), 2));
                        player.player_fleets[selectedFleet].s2 = s;
                        player.player_fleets[selectedFleet].starDistanse = starDistanse;
                        selectedStar = s;    //star_selected = j;   //store type for selected star
                    }
                    else if (player.player_fleets[selectedFleet].s2 == player.player_stars[listView.SelectedIndices[0]])
                    {
                        if (player.player_fleets[selectedFleet].onWay)
                            return;

                        player.player_fleets[selectedFleet].s2 = null;
                        player.player_fleets[selectedFleet].starDistanse = 0;
                        selectedStar = null;
                    }
                }
        }

        //-------------------------- Step ----------------------------------
        //------ шаг будем осуществлять в отдельном потоке ------------------
        private void step_button_Click(object sender, EventArgs e)
        {
            if (galaxy == null)
            {
                return;
            }
            //выключаем всю панель, пока запущен поток
            panel1.Enabled = false;
            button2.Enabled = false;//кнопка захвата по умолчанию будет неактивна. Включается только если система, в которой находится активный флот еще не захвачена

            StepWorker.RunWorkerAsync();//запускаем поток
        }
        // сам поток
        private void StepWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            galaxy.Time++;

            StarSystem s = selectedStar;    //StarSystem s = galaxy.stars[star_selected];
            Random r = new Random(DateTime.Now.Millisecond);

            //---------------изменение популяции в системах---------
            for (int j = 0; j < galaxy.stars.Count; j++)
            {
                for (int i = 0; i < galaxy.stars[j].planets_count; i++)
                {
                    galaxy.stars[j].PLN[i].POPULATION = galaxy.stars[j].PLN[i].Inc(galaxy.stars[j].PLN[i].POPULATION, r.NextDouble());
                }
            }

            //---------------получение бабосиков с захваченных систем---------
            for (int i = 0; i < Player.player_planets.Count; i++)
            {
                credit += Player.player_planets[i].PROFIT;//StarSystem.PLN[i].PROFIT;
            }

            //---------------процесс захвата системы---------  // ! FIX !
            if (conquer_progressBar.Visible == true && false)   
                if (conquer_progressBar.Value == conquer_progressBar.Maximum - 1)
                {
                    //захват происходит не той системы, которая выделена, а той, в которой находится активный флот (по логике)
                    player.player_stars.Add(player.player_fleets[selectedFleet].s1);      //player.player_stars.Add(galaxy.stars[star_selected]);
                    conquer_progressBar.Visible = false;
                    button3.Visible = false;
                    conquer_progressBar.Value = conquer_progressBar.Minimum;
                }
                else
                {
                    conquer_progressBar.Value = conquer_progressBar.Value + 1;
                }

            int fly = maxDistanse;

            //------------------движение галактики -----------------
            for (int i = 0; i < galaxy.stars.Count; i++)
            {
                //to add
            }

            //-------------------- движение флотов ---------------
            for (int k = 0; k < player.player_fleets.Count; k++)
            {
                Fleet fl = player.player_fleets[k];

                if (fl.s2 != null)
                {
                    fl.onWay = true;

                    if (fl.starDistanse < maxDistanse)
                        fly = (int)fl.starDistanse;

                    double dx = (fl.s2.x - fl.x) / fl.starDistanse / speed;
                    double dy = (fl.s2.y - fl.y) / fl.starDistanse / speed;
                    double dz = (fl.s2.z - fl.z) / fl.starDistanse / speed;
                    int calcFly = fly * speed;

                    for (int i = 0; i < calcFly; i++)
                    {
                        fl.x += dx;
                        fl.y += dy;
                        fl.z += dz;
                    }

                    fl.starDistanse -= fly;

                    if (fl.starDistanse <= 1)
                    {
                        fl.s1 = fl.s2;
                        fl.s2 = null;
                        fl.starDistanse = 0;
                        fl.onWay = false;
                    }
                }
            }

            if (tech_progressBar.Value < tt.learning_tech_time && tt.tech_clicked != 1000 && tt.subtech_clicked != 1000)
            {
                tech_progressBar.Value += 1;
            }

            if (tech_progressBar.Value == tt.learning_tech_time)
            {
                Player.technologies.Add(new int[] { tt.tech_clicked, tt.subtech_clicked });
                tech_progressBar.Value = 0;
                tech_progressBar.Visible = false;
                tech_label.Visible = false;
                tt.tech_clicked = 1000;
                tt.subtech_clicked = 1000;
            }
        }
        // функция, которая вызывается после того как поток завершился
        private void StepWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            //Обновляем лейблы
            UpdateLabels();
            //включам панель с кнопками
            panel1.Enabled = true;
            step_button.Focus();//задаём фокус для кнопки шага
            //проверяем нахождение нейтральных флотов и флотов игрока в одной системе
            for (int k = 0; k < galaxy.neutrals.Count; k++)
            {
                for (int l = 0; l < player.player_fleets.Count; l++)
                    if (galaxy.neutrals[k].s1 == player.player_fleets[l].s1)
                    {
                        if (player.player_fleets[l].starDistanse == 0)
                            if (MessageBox.Show("Ваш " + (l + 1) + " флот обнаружил нейтральный флот на планете " + player.player_fleets[l].s1.name + "!\nАтаковать его?", "", MessageBoxButtons.YesNo)
                                == System.Windows.Forms.DialogResult.Yes)
                            {
                                Fleet fl = galaxy.neutrals[k];

                                CombatForm cf = new CombatForm(player.player_fleets[l], fl);
                                cf.ShowDialog();

                                bool alive = false;

                                //Удалить флот из списка, если флот уничтожен
                                for (int i = 0; i < fl.ships.Count; i++)
                                {
                                    alive |= (fl.ships[i].currentHealth > 0);   
                                    //если хотябы у одго корабля хп больше 0, alive = true
                                }

                                if (!alive)
                                    galaxy.neutrals.RemoveAt(k);
                            }
                    }
            }
            //включаем кнопку захвата, если система в которой находится активный флот не захвачена
            if (!player.player_stars.Contains(player.player_fleets[selectedFleet].s1) && !player.player_fleets[selectedFleet].onWay)
                button2.Enabled = true;
            else
                button2.Enabled = false;
        }
        

        //----------------------Timer-------------------

        //отрисовка только по таймеру
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            galaxyImage.Refresh();
        }


        //-------------------------------other----------------------------------

        //Проверка находится ли курсор "e" в планете "star"
        private bool mouseIsInStar(MouseEventArgs e, StarSystem star)
        {
            s2x = star.x;
            s2y = star.y;
            s2z = star.z;

            double screenX = star.x * Math.Cos(spinX) - star.z * Math.Sin(spinX);
            double screenZ = star.x * Math.Sin(spinX) + star.z * Math.Cos(spinX);
            double screenY = star.y * Math.Cos(spinY) - screenZ * Math.Sin(spinY);
            double starSize = star.type + dynamicStarSize;

            return (e.X + dispersion) / scaling > (centerX + screenX - starSize / 2) &&
                   (e.X - dispersion) / scaling < (centerX + screenX + starSize / 2) &&
                   (e.Y + dispersion) / scaling > (centerY + screenY - starSize / 2) &&
                   (e.Y - dispersion) / scaling < (centerY + screenY + starSize / 2);
        }
        //то же самое, только с флотом
        private bool mouseIsInFleet(MouseEventArgs e, Fleet fleet)
        {
            double screenXfl = fleet.x * Math.Cos(spinX) - fleet.z * Math.Sin(spinX) - 10;
            double screenZfl = fleet.x * Math.Sin(spinX) + fleet.z * Math.Cos(spinX);
            double screenYfl = fleet.y * Math.Cos(spinY) - screenZfl * Math.Sin(spinY) - 10;
            
            return ((e.X + dispersion) / scaling > (centerX + screenXfl - 15 / 2) &&
                (e.X - dispersion) / scaling < (centerX + screenXfl + 15 / 2) &&
                (e.Y + dispersion) / scaling > (centerY + screenYfl - 15 / 2) &&
                (e.Y - dispersion) / scaling < (centerY + screenYfl + 15 / 2));
        }
        //обновляет центры
        private void UpdateCenters()
        {
            centerX = galaxyImage.Width / 2 / scaling + horizontal;
            centerY = galaxyImage.Height / 2 / scaling + vertical;
        }

        private void UpdateLabels()
        {
            dateLabel.Text = galaxy.Time.ToString() + " г.н.э.";
            CreditsStatus.Text = Math.Round(credit, 2).ToString() + " $";
            MineralStatus.Text = minerals + "Т";
            EnergyStatus.Text = energy + " Wt";
        }
    }
}
