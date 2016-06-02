using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ResizableRectangles
{
    enum MouseMode
    {
        NONE,
        DRAWING,
        OBJ_SELECT,
        PANNING
    }

    enum ResizeBorder  // keep track of which border of the box is to be resized.
    {
        RB_NONE = 0,
        RB_TOP_LEFT = 1,
        RB_TOP_CENTER = 2,
        RB_TOP_RIGHT = 3,
        RB_CENTER_LEFT = 4,
        RB_CENTER_RIGHT = 5,
        RB_BOTTOM_LEFT = 6,
        RB_BOTTOM_CENTER = 7,
        RB_BOTTOM_RIGHT = 8,
        RB_OBJ_MOVE = 9
    }

    public partial class Form1 : Form
    {
        List<RectangleList> rectList = new List<RectangleList>();

        Point startPoint = new Point(0,0);
        Point currentPoint = new Point(0, 0);

        MouseMode mouseModeEnum = MouseMode.NONE;
        ResizeBorder resizeBorderEnum = ResizeBorder.RB_NONE;

        bool IsMouseDown = false;

        int selectedRectCounter;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.MouseWheel += PictureBox1_MouseWheel;
        }

        private void toolStripButtonDrawRect_Click(object sender, EventArgs e)
        {
            if(mouseModeEnum == MouseMode.NONE || mouseModeEnum == MouseMode.OBJ_SELECT || mouseModeEnum == MouseMode.PANNING)
            {
                mouseModeEnum = MouseMode.DRAWING;
                toolStripButtonDrawRect.BackColor = Color.Orange;
            }
            else
            {
                mouseModeEnum = MouseMode.NONE;
                toolStripButtonDrawRect.BackColor = Color.Transparent;
            }
        }

        private void toolStripButtonLoadImage_Click(object sender, EventArgs e)
        {
            if(mouseModeEnum == MouseMode.DRAWING)
            {
                mouseModeEnum = MouseMode.NONE;
                toolStripButtonDrawRect.BackColor = Color.Transparent;
            }
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image | *.bmp";
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(ofd.FileName);
            }
        }

        private void toolStripButtonClearCanvas_Click(object sender, EventArgs e)
        {
            if (mouseModeEnum == MouseMode.DRAWING)
            {
                mouseModeEnum = MouseMode.NONE;
                toolStripButtonDrawRect.BackColor = Color.Transparent;
            }

            rectList.Clear();
            pictureBox1.Image = null;
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                IsMouseDown = true;
                switch(mouseModeEnum)
                {
                    case MouseMode.DRAWING:
                        startPoint = currentPoint = e.Location;
                        break;
                    case MouseMode.PANNING:
                        startPoint = e.Location;
                        break;
                    case MouseMode.NONE:
                    case MouseMode.OBJ_SELECT:
                        if (mouseModeEnum == MouseMode.OBJ_SELECT && (pictureBox1.Cursor == Cursors.SizeNS || pictureBox1.Cursor == Cursors.SizeWE || pictureBox1.Cursor == Cursors.SizeNWSE || pictureBox1.Cursor == Cursors.SizeNESW))
                        {
                            startPoint = currentPoint = e.Location;
                            return; // do nothing, it requires resizing of the selected object.
                        }
                        startPoint = new Point(e.X, e.Y);
                        if (FindSelectedRect(new Point(e.X, e.Y)))
                        {   
                            mouseModeEnum = MouseMode.OBJ_SELECT;
                        }
                        else
                        {
                            mouseModeEnum = MouseMode.NONE;
                        }
                        pictureBox1.Invalidate();
                        break;
                }
            }
            if(e.Button == MouseButtons.Right)
            {
                if (mouseModeEnum == MouseMode.DRAWING || mouseModeEnum == MouseMode.PANNING)
                {
                    mouseModeEnum = MouseMode.NONE;
                    Cursor = Cursors.Default;
                    toolStripButtonDrawRect.BackColor = Color.Transparent;
                }
                if(mouseModeEnum == MouseMode.OBJ_SELECT)
                {
                    ContextMenuStrip menu = new ContextMenuStrip();
                    ToolStripMenuItem item, submenu;
                    //submenu = new ToolStripMenuItem();
                    //submenu.Text = "Sub-menu 1";
                    item = new ToolStripMenuItem();
                    item.Text = "Delete Rectangle";
                    //submenu.DropDownItems.Add(item);
                    //item = new ToolStripMenuItem();
                    //item.Text = "Sub-item 2";
                    //submenu.DropDownItems.Add(item);
                    menu.Items.Add(item);
                    item = new ToolStripMenuItem();
                    item.Text = "Delete All";
                    menu.Items.Add(item);
                    menu.Show(Cursor.Position);
                    menu.ItemClicked += Menu_ItemClicked;
                }
            }
        }

        private void Menu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.ToString() == "Delete Rectangle")
            {
                rectList.RemoveAt(selectedRectCounter);
            }
            if (e.ClickedItem.ToString() == "Delete All")
            {
                rectList.Clear();
            }
            pictureBox1.Invalidate();
            mouseModeEnum = MouseMode.NONE;
            Cursor = Cursors.Default;
        }

        private bool FindSelectedRect(Point pt)
        {
            selectedRectCounter = 0;
            foreach (RectangleList rect in rectList)
            {
                if(rect.rectangle.Contains(pt))
                {
                    return true;
                }
                selectedRectCounter++;
            }
            return false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (IsMouseDown)
                {
                    switch (mouseModeEnum)
                    {
                        case MouseMode.DRAWING:
                            currentPoint = e.Location;
                            break;
                        case MouseMode.PANNING:
                            panel1.AutoScrollPosition = new Point(-panel1.AutoScrollPosition.X - e.X + startPoint.X, -panel1.AutoScrollPosition.Y - e.Y + startPoint.Y);
                            break;
                        case MouseMode.OBJ_SELECT:
                            switch(resizeBorderEnum)
                            {
                                case ResizeBorder.RB_OBJ_MOVE:
                                    rectList[selectedRectCounter].rectangle.X += e.X - startPoint.X;
                                    rectList[selectedRectCounter].rectangle.Y += e.Y - startPoint.Y;
                                    startPoint = new Point(e.X, e.Y); // Required
                                    break;
                                case ResizeBorder.RB_TOP_LEFT:
                                    rectList[selectedRectCounter].rectangle.Height = rectList[selectedRectCounter].rectangle.Y + rectList[selectedRectCounter].rectangle.Height - e.Y;
                                    rectList[selectedRectCounter].rectangle.Y = e.Y;
                                    rectList[selectedRectCounter].rectangle.Width = rectList[selectedRectCounter].rectangle.X + rectList[selectedRectCounter].rectangle.Width - e.X;
                                    rectList[selectedRectCounter].rectangle.X = e.X;
                                    if(rectList[selectedRectCounter].rectangle.Width < 0) resizeBorderEnum = ResizeBorder.RB_TOP_RIGHT;
                                    if(rectList[selectedRectCounter].rectangle.Height < 0) resizeBorderEnum = ResizeBorder.RB_BOTTOM_LEFT;
                                    break;
                                case ResizeBorder.RB_TOP_CENTER:
                                    rectList[selectedRectCounter].rectangle.Height = rectList[selectedRectCounter].rectangle.Y + rectList[selectedRectCounter].rectangle.Height - e.Y;
                                    rectList[selectedRectCounter].rectangle.Y = e.Y;
                                    if (rectList[selectedRectCounter].rectangle.Height < 0)
                                    {
                                        resizeBorderEnum = ResizeBorder.RB_BOTTOM_CENTER;
                                    }
                                    break;
                                case ResizeBorder.RB_TOP_RIGHT:
                                    rectList[selectedRectCounter].rectangle.Height = rectList[selectedRectCounter].rectangle.Y + rectList[selectedRectCounter].rectangle.Height - e.Y;
                                    rectList[selectedRectCounter].rectangle.Y = e.Y;
                                    rectList[selectedRectCounter].rectangle.Width = e.X - rectList[selectedRectCounter].rectangle.X;
                                    if (rectList[selectedRectCounter].rectangle.Width < 0) resizeBorderEnum = ResizeBorder.RB_TOP_LEFT;
                                    if (rectList[selectedRectCounter].rectangle.Height < 0) resizeBorderEnum = ResizeBorder.RB_BOTTOM_RIGHT;
                                    break;
                                case ResizeBorder.RB_CENTER_LEFT:
                                    rectList[selectedRectCounter].rectangle.Width = rectList[selectedRectCounter].rectangle.X + rectList[selectedRectCounter].rectangle.Width - e.X;
                                    rectList[selectedRectCounter].rectangle.X = e.X;
                                    if (rectList[selectedRectCounter].rectangle.Width < 0)
                                    {
                                        resizeBorderEnum = ResizeBorder.RB_CENTER_RIGHT;
                                    }
                                    break;
                                case ResizeBorder.RB_CENTER_RIGHT:
                                    rectList[selectedRectCounter].rectangle.Width = e.X - rectList[selectedRectCounter].rectangle.X;
                                    if (rectList[selectedRectCounter].rectangle.Width < 0)
                                    {
                                        resizeBorderEnum = ResizeBorder.RB_CENTER_LEFT;
                                    }
                                    break;
                                case ResizeBorder.RB_BOTTOM_LEFT:
                                    rectList[selectedRectCounter].rectangle.Width = rectList[selectedRectCounter].rectangle.X + rectList[selectedRectCounter].rectangle.Width - e.X;
                                    rectList[selectedRectCounter].rectangle.X = e.X;
                                    rectList[selectedRectCounter].rectangle.Height = e.Y - rectList[selectedRectCounter].rectangle.Y;
                                    if (rectList[selectedRectCounter].rectangle.Width < 0) resizeBorderEnum = ResizeBorder.RB_BOTTOM_RIGHT;
                                    if (rectList[selectedRectCounter].rectangle.Height < 0) resizeBorderEnum = ResizeBorder.RB_TOP_LEFT;
                                    break;
                                case ResizeBorder.RB_BOTTOM_CENTER:
                                    rectList[selectedRectCounter].rectangle.Height = e.Y - rectList[selectedRectCounter].rectangle.Y;
                                    if(rectList[selectedRectCounter].rectangle.Height < 0)
                                    {
                                        resizeBorderEnum = ResizeBorder.RB_TOP_CENTER;
                                    }
                                    break;
                                case ResizeBorder.RB_BOTTOM_RIGHT:
                                    rectList[selectedRectCounter].rectangle.Height = e.Y - rectList[selectedRectCounter].rectangle.Y;
                                    rectList[selectedRectCounter].rectangle.Width = e.X - rectList[selectedRectCounter].rectangle.X;
                                    if (rectList[selectedRectCounter].rectangle.Width < 0) resizeBorderEnum = ResizeBorder.RB_BOTTOM_LEFT;
                                    if (rectList[selectedRectCounter].rectangle.Height < 0) resizeBorderEnum = ResizeBorder.RB_TOP_RIGHT;
                                    break;
                            }
                            rectList[selectedRectCounter].SetNewLocationOfSubRectList();
                            break;
                    }
                    pictureBox1.Invalidate();
                }
            }
            else // Mouse is not down but only moving
            {
                if (mouseModeEnum == MouseMode.OBJ_SELECT)
                {
                    startPoint = new Point(e.X, e.Y);
                    foreach(RectangleList rect in rectList)
                    {
                        if (rect.subRectList[0].Contains(startPoint))
                        {
                            Cursor = Cursors.SizeNWSE;  // Top-Left
                            resizeBorderEnum = ResizeBorder.RB_TOP_LEFT;
                            break;
                        }
                        else if (rect.subRectList[1].Contains(startPoint))
                        {
                            Cursor = Cursors.SizeNS;    // Top-Center
                            resizeBorderEnum = ResizeBorder.RB_TOP_CENTER;
                            break;
                        }
                        else if (rect.subRectList[2].Contains(startPoint))
                        {
                            Cursor = Cursors.SizeNESW;  // Top-Right
                            resizeBorderEnum = ResizeBorder.RB_TOP_RIGHT;
                            break;
                        }
                        else if (rect.subRectList[3].Contains(startPoint))
                        {
                            Cursor = Cursors.SizeWE;    // Center-Left
                            resizeBorderEnum = ResizeBorder.RB_CENTER_LEFT;
                            break;
                        }
                        else if (rect.subRectList[4].Contains(startPoint))
                        {
                            Cursor = Cursors.SizeWE;    // Center-Right
                            resizeBorderEnum = ResizeBorder.RB_CENTER_RIGHT;
                            break;
                        }
                        else if (rect.subRectList[5].Contains(startPoint))
                        {
                            Cursor = Cursors.SizeNESW;  // Botton-Left
                            resizeBorderEnum = ResizeBorder.RB_BOTTOM_LEFT;
                            break;
                        }
                        else if (rect.subRectList[6].Contains(startPoint))
                        {
                            Cursor = Cursors.SizeNS;    // Bottom-Center
                            resizeBorderEnum = ResizeBorder.RB_BOTTOM_CENTER;
                            break;
                        }
                        else if (rect.subRectList[7].Contains(startPoint))
                        {
                            Cursor = Cursors.SizeNWSE;  // Bottom-Right
                            resizeBorderEnum = ResizeBorder.RB_BOTTOM_RIGHT;
                            break;
                        }
                        else if (rect.rectangle.Contains(startPoint))
                        {
                            Cursor = Cursors.SizeAll;
                            resizeBorderEnum = ResizeBorder.RB_OBJ_MOVE;
                            break;
                        }
                        else
                        {
                            Cursor = Cursors.Default;
                            resizeBorderEnum = ResizeBorder.RB_NONE;
                        }
                    }
                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (mouseModeEnum == MouseMode.DRAWING)
                {
                    RectangleList rect = getRectangle();
                    if (rect.rectangle.Width > 0 && rect.rectangle.Height > 0)
                    {
                        rectList.Add(rect);
                    }
                    pictureBox1.Invalidate();
                }
                else if (mouseModeEnum == MouseMode.OBJ_SELECT)
                {

                }
                sortRectangles();
            }
        }

        private RectangleList getRectangle()
        {
            return new RectangleList(
                Math.Min(startPoint.X, currentPoint.X),
                Math.Min(startPoint.Y, currentPoint.Y),
                Math.Abs(startPoint.X - currentPoint.X),
                Math.Abs(startPoint.Y - currentPoint.Y));
        }

        private void sortRectangles()
        {
            rectList.Sort((X, Y) => ((X.rectangle.Height * X.rectangle.Width).CompareTo(Y.rectangle.Height * Y.rectangle.Width)));
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (rectList.Count > 0)
            {
                int counter = 0;
                foreach (RectangleList rect in rectList)
                {
                    e.Graphics.DrawRectangle(Pens.Lime, rect.rectangle);
                    if (selectedRectCounter == counter && mouseModeEnum == MouseMode.OBJ_SELECT)
                    {
                        e.Graphics.DrawRectangles(Pens.Blue, rect.subRectList.ToArray());
                    }
                    counter++;
                }
            }
            if (mouseModeEnum == MouseMode.DRAWING)
            {
                RectangleList rect = getRectangle();
                e.Graphics.DrawRectangle(Pens.Lime, rect.rectangle);
            }
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            if (mouseModeEnum == MouseMode.DRAWING) Cursor = Cursors.Cross;
            else if (mouseModeEnum == MouseMode.PANNING) Cursor = Cursors.Hand;
            pictureBox1.Focus();
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                if (e.Delta <= 0)
                {
                    //set minimum size to zoom
                    if (pictureBox1.Width < 800)
                        return;
                }
                else
                {
                    //set maximum size to zoom
                    if (pictureBox1.Width > 10000)
                        return;
                }

                ZoomInOut(e.Delta, e.Location);

                if (pictureBox1.Width > panel1.Width || pictureBox1.Height > panel1.Height)
                {
                    Cursor = Cursors.Hand;
                    mouseModeEnum = MouseMode.PANNING;
                }
                else
                {
                    mouseModeEnum = MouseMode.NONE;
                    Cursor = Cursors.Default;
                }
                pictureBox1.Invalidate();
            }
        }

        
        private void ZoomInOut(int zoomFactor, Point zoomPt)
        {
            if (pictureBox1.Width < 800 && zoomFactor == -120) return;
            if (pictureBox1.Width > 10000 && zoomFactor == 120) return;

            double[] xLocPercent = new double[rectList.Count];
            double[] yLocPercent = new double[rectList.Count];
            double[] rectWidthPercent = new double[rectList.Count];
            double[] rectHeightPercent = new double[rectList.Count];

            double Width = pictureBox1.Width;
            double Height = pictureBox1.Height;
                
            for (int i = 0; i < rectList.Count; i++)
            {
                double X = rectList[i].rectangle.X;
                double Y = rectList[i].rectangle.Y;
                double W = rectList[i].rectangle.Width;
                double H = rectList[i].rectangle.Height;

                xLocPercent[i] = (X / Width) * 100;
                yLocPercent[i] = (Y / Height) * 100;
                rectWidthPercent[i] = (W / Width) * 100;
                rectHeightPercent[i] = (H / Height) * 100;
            }

            if (zoomFactor != 0)
            {
                Width = pictureBox1.Width += Convert.ToInt32(pictureBox1.Width * zoomFactor / 1000);
                Height = pictureBox1.Height += Convert.ToInt32(pictureBox1.Height * zoomFactor / 1000);

                for (int i = 0; i < rectList.Count; i++)
                {
                    rectList[i].rectangle.Width = Convert.ToInt32((pictureBox1.Width * rectWidthPercent[i]) / 100);
                    rectList[i].rectangle.Height = Convert.ToInt32((pictureBox1.Height * rectHeightPercent[i]) / 100);
                    rectList[i].rectangle.X = Convert.ToInt32((Width * xLocPercent[i]) / 100);
                    rectList[i].rectangle.Y = Convert.ToInt32((Height * yLocPercent[i]) / 100);
                    rectList[i].SetNewLocationOfSubRectList();
                }

                // To do: Set location of picturebox with respect to mouse location to give mouse location zoom effect.
                
                //pictureBox1.Location = new Point((pictureBox1.Parent.ClientSize.Width / 2) - (pictureBox1.Width / 2), (pictureBox1.Parent.ClientSize.Height / 2) - (pictureBox1.Height / 2));
                //panel1.AutoScrollPosition = new Point(- panel1.HorizontalScroll.Maximum/2, - panel1.VerticalScroll.Maximum/2);
            }
            else
            {
                //Reset zoom
                pictureBox1.Width = panel1.Width;
                pictureBox1.Height = panel1.Height;
                for (int i = 0; i < rectList.Count; i++)
                {
                    rectList[i].rectangle.Width = Convert.ToInt32((pictureBox1.Width * rectWidthPercent[i]) / 100);
                    rectList[i].rectangle.Height = Convert.ToInt32((pictureBox1.Height * rectHeightPercent[i]) / 100);
                    rectList[i].rectangle.X = Convert.ToInt32((pictureBox1.Width * xLocPercent[i]) / 100);
                    rectList[i].rectangle.Y = Convert.ToInt32((pictureBox1.Height * yLocPercent[i]) / 100);
                    rectList[i].SetNewLocationOfSubRectList();
                }
                pictureBox1.Location = new Point(0, 0);
            }
            // Set scroll bar values.
        }

        private void toolStripButtonSaveImage_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();

            if(saveFile.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(saveFile.FileName + ".bmp");
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ZoomInOut(120, new Point(0,0));
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            ZoomInOut(-120, new Point(0, 0));
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            ZoomInOut(0, new Point(0, 0));
        }

        private void toolStripButtonOan_Click(object sender, EventArgs e)
        {
            mouseModeEnum = MouseMode.PANNING;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            mouseModeEnum = MouseMode.NONE;
        }
    }
}