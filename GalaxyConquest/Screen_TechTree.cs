﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

using SFML;
using Gwen;
using Gwen.Control;

using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using GalaxyConquest.Game;

using GalaxyConquest.Drawing;
using GalaxyConquest.SpaceObjects;


namespace GalaxyConquest
{
    class Screen_TechTree : Gwen.Control.DockBase
    {

        public Bitmap TechTreeBitmap = new Bitmap(Program.percentW(100), Program.percentH(100), PixelFormat.Format32bppArgb);
        public float scaling = 1f;
        public float horizontal = 0;
        public float vertical = 0;

        public int mouseX;
        public int mouseY;

        float centerX;
        float centerY;

        public Brush br;
        public System.Drawing.Font fnt = new System.Drawing.Font("Consolas", 10.0F);

        public Gwen.Control.ImagePanel img;
        Gwen.Control.Label label;

        Gwen.Control.TextBox techDescription;

        bool dragging = false;

        public Screen_TechTree(Base parent)
            : base(parent)
        {

            Gwen.Control.Button buttonOK = new Gwen.Control.Button(this);
            buttonOK.Text = "Back";
            buttonOK.Font = Program.fontButtonLabels;
            buttonOK.SetBounds(500, 500, 50, 50);
            buttonOK.Clicked += onButtonOKClick;

            Tech.Inint();
            SetSize(parent.Width, parent.Height);



            img = new Gwen.Control.ImagePanel(this);


            updateDrawing();

            img.SetPosition(Program.percentW(0), Program.percentH(0));
            img.SetSize(Program.percentW(100), Program.percentH(100));
            img.Clicked += new GwenEventHandler<ClickedEventArgs>(img_Clicked);
            img.MouseMoved += new GwenEventHandler<MovedEventArgs>(img_MouseMoved);
            img.MouseDown += new GwenEventHandler<ClickedEventArgs>(img_MouseDown);
            img.MouseUp += new GwenEventHandler<ClickedEventArgs>(img_MouseUp);
            img.MouseWheeled += new GwenEventHandler<MouseWheeledEventArgs>(img_MouseWheeled);

            techDescription = new Gwen.Control.TextBox(this);
            techDescription.SetPosition(Program.percentW(5), Program.percentH(80));
            techDescription.SetSize(200, 50);
            //techDescription.SetBounds(Program.percentW(5), Program.percentH(80), Program.percentH(20), Program.percentH(100));

            label = new Gwen.Control.Label(this);
            label.Text = "Tech_Tree Probe";
            label.SetPosition(Program.percentW(5), Program.percentH(5));
            label.TextColor = Color.FromArgb(200, 80, 0, 250);
            label.Font = Program.fontLogo;
        }

        void img_MouseUp(Base sender, ClickedEventArgs arguments)
        {
            dragging = false;
        }

        void img_MouseDown(Base sender, ClickedEventArgs arguments)
        {

        }

        void img_MouseWheeled(Base sender, MouseWheeledEventArgs arguments)
        {
            if (arguments.Delta > 0)
                scaling = (float)Math.Min(scaling + 0.1, 10);
            else
                scaling = (float)Math.Max(scaling - 0.1, 0.1);

            updateDrawing();

        }

        void img_MouseMoved(Base sender, MovedEventArgs arguments)
        {
            if (dragging)
            {
                horizontal += (arguments.X - mouseX) / scaling;
                vertical += (arguments.Y - mouseY) / scaling;

                mouseX = arguments.X;
                mouseY = arguments.Y;

                centerX = TechTreeBitmap.Width / 2 / scaling + horizontal;
                centerY = TechTreeBitmap.Height / 2 / scaling + vertical;

                updateDrawing();
            }
        }

        void img_Clicked(Base sender, ClickedEventArgs arguments)
        {
            for (int i = 0; i < Tech.teches.tiers.Count; i++)
            {
                for (int j = 0; j < Tech.teches.tiers[i].Count; j++)
                {
                    for (int k = 0; k < Tech.teches.tiers[i][j].Count; k++)
                    {
                        for (int z = 0; z < Player.technologies.Count; z++)
                        {
                            if (i == Player.technologies[z][0] &&
                                j == Player.technologies[z][1] &&
                                k == Player.technologies[z][2])
                            {
                                br = Brushes.Yellow;
                                break;
                            }
                            else
                            {
                                br = Brushes.White;
                            }
                        }

                        Size RPStringLenght = TextRenderer.MeasureText(Tech.teches.tiers[i][j][k].RP, fnt);

                        RectangleF rectF1 = new RectangleF(centerX + j * 500 - Tech.teches.tiers[i][j].Count / 2 * 150 + k * 150,
                                centerY - i * 300,
                                150,
                                40
                                );

                        RectangleF rectF3 = new RectangleF(centerX + j * 500 - Tech.teches.tiers[i][j].Count / 2 * 150 + k * 150 - RPStringLenght.Width,
                                centerY - i * 300 + 40,
                                150 + RPStringLenght.Width,
                                60
                                );

                        if (arguments.X < (rectF3.Right) * scaling &&
                            arguments.X > (rectF3.Left) * scaling &&
                            arguments.Y < (rectF3.Bottom) * scaling &&
                            arguments.Y > (rectF1.Top) * scaling)
                        {
                            //tierClicked = i;
                            //techLineClicked = j;
                            //subtechClicked = k;

                            //label.Text = i+";"+j+";"+k;


                            techDescription.Text = Tech.teches.tiers[i][j][k].description;
                            //groupBox1.Visible = true;
                            //groupBox1.Text = Tech.teches.tiers[tierClicked][techLineClicked][subtechClicked].subtech;
                        }
                    }
                }
            }


            //label.Text = "DOWN";
            dragging = true;
            mouseX = arguments.X;
            mouseY = arguments.Y;
        }

        protected override bool OnKeyReturn(bool down)
        {
            System.Windows.Forms.MessageBox.Show("URA");
            return base.OnKeyEscape(down);
        }

        private void updateDrawing()
        {


            Graphics g = Graphics.FromImage(TechTreeBitmap);
            g.FillRectangle(Brushes.Black, 0, 0, TechTreeBitmap.Width, TechTreeBitmap.Height);

            centerX = TechTreeBitmap.Width / 2 / scaling;
            centerY = TechTreeBitmap.Height / 2 / scaling;

            centerX += horizontal;
            centerY += vertical;

            g.ScaleTransform(scaling, scaling);
            br = Brushes.White;
            //достаем технологии из Tech.teches i - столбец(Tier); j - строка(TechLine); k - подстрока(Subtech)
            for (int i = 0; i < Tech.teches.tiers.Count; i++)
            {
                for (int j = 0; j < Tech.teches.tiers[i].Count; j++)
                {
                    for (int k = 0; k < Tech.teches.tiers[i][j].Count; k++)
                    {
                        for (int z = 0; z < Player.technologies.Count; z++)
                        {
                            if (i == Player.technologies[z][0] &&
                                j == Player.technologies[z][1] &&
                                k == Player.technologies[z][2])
                            {
                                br = Brushes.Yellow;
                                break;
                            }
                            else
                            {
                                br = Brushes.White;
                            }
                        }

                        Size RPStringLenght = TextRenderer.MeasureText(Tech.teches.tiers[i][j][k].RP, fnt);

                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Center;

                        RectangleF rectF1 = new RectangleF(centerX + j * 500 - Tech.teches.tiers[i][j].Count / 2 * 150 + k * 150,
                                centerY - i * 300,
                                150,
                                40
                                );

                        RectangleF rectF2 = new RectangleF(centerX - RPStringLenght.Width + j * 500 - Tech.teches.tiers[i][j].Count / 2 * 150 + k * 150,
                                centerY - i * 300,
                                RPStringLenght.Width,
                                40
                                );

                        RectangleF rectF3 = new RectangleF(centerX + j * 500 - Tech.teches.tiers[i][j].Count / 2 * 150 + k * 150 - RPStringLenght.Width,
                                centerY - i * 300 + 40,
                                150 + RPStringLenght.Width,
                                60
                                );

                        g.DrawString(Tech.teches.tiers[i][j][k].subtech, fnt, br,
                           rectF1, stringFormat);

                        g.DrawString(Tech.teches.tiers[i][j][k].RP, fnt, br,
                            rectF2, stringFormat);

                        g.DrawString(Tech.teches.tiers[i][j][k].description, fnt, br,
                             rectF3, stringFormat);


                        //----------------------------------------------------

                        g.DrawRectangle(Pens.AliceBlue, Rectangle.Round(rectF1));
                        g.DrawRectangle(Pens.AliceBlue, Rectangle.Round(rectF2));
                        g.DrawRectangle(Pens.AliceBlue, Rectangle.Round(rectF3));

                        /*
                        g.DrawString(Tech.teches.tiers[i][j][k].subtech, fnt, br,
                                    new PointF(centerX + 340 * j - (30 * k) + (30 * Tech.teches.tiers[i][j].Count / 2) + techStringLenght.Width, centerY + 300 - (80 + Tech.teches.tiers[i][j].Count + 1 * 10) * i));

                        g.DrawString(Tech.teches.tiers[i][j][k].RP, fnt, br,
                                    new PointF(centerX + 340 * j - RPStringLenght.Width - (30 * k) + (30 * Tech.teches.tiers[i][j].Count / 2) + techStringLenght.Width, centerY + 300 - (80 + Tech.teches.tiers[i][j].Count + 1 * 10) * i));

                        //----------------------------------------------------

                        g.DrawRectangle(Pens.AliceBlue, centerX + 340 * j - 2,
                            centerY + 300 - (80 + Tech.teches.tiers[i][j].Count + 1 * 10) * i - (30 * k) + (30 * Tech.teches.tiers[i][j].Count / 2) - 2, techStringLenght.Width + 2, techStringLenght.Height + 2);

                        g.DrawRectangle(Pens.AliceBlue, centerX + 340 * j - 2 - RPStringLenght.Width,
                            centerY + 300 - (80 + Tech.teches.tiers[i][j].Count + 1 * 10) * i - (30 * k) + (30 * Tech.teches.tiers[i][j].Count / 2) - 2, RPStringLenght.Width, RPStringLenght.Height + 2);
                    */
                    }

                }
            }
            img.Image = TechTreeBitmap;

            Program.m_Canvas.RenderCanvas();
            Program.m_Window.Display();
        }

        private void onButtonOKClick(Base control, EventArgs args)
        {
            Program.screenManager.LoadScreen("gamescreen");
        }

        public override void Dispose()
        {
            fnt.Dispose();
            base.Dispose();
        }

    }
}
