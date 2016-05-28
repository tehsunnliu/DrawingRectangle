using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ResizableRectangles
{
    class RectangleList
    {
        public Rectangle rectangle;
        public List<Rectangle> subRectList;
        private int m_nOffset = 5;

        public RectangleList()
        {
        }

        public RectangleList(int x, int y, int width, int height)
        {
            Size sz = new Size(m_nOffset, m_nOffset);
            rectangle = new Rectangle(x, y, width, height);

            subRectList = new List<Rectangle>(8);                                                                                                       // Set Resizable points for rectangle.
            subRectList.Add(new Rectangle(new Point(rectangle.X - m_nOffset, rectangle.Y - m_nOffset), sz));                                            // Top Left
            subRectList.Add(new Rectangle(new Point(rectangle.X + (rectangle.Width / 2) - (m_nOffset / 2), rectangle.Y - m_nOffset), sz));              // Top Center
            subRectList.Add(new Rectangle(new Point(rectangle.X + rectangle.Width, rectangle.Y - m_nOffset), sz));                                      // Top Right

            subRectList.Add(new Rectangle(new Point(rectangle.X - m_nOffset, rectangle.Y + (rectangle.Height / 2) - (m_nOffset / 2)), sz));             // Center Left
            subRectList.Add(new Rectangle(new Point(rectangle.X + rectangle.Width, rectangle.Y + (rectangle.Height / 2) - (m_nOffset / 2)), sz));       // Center Right

            subRectList.Add(new Rectangle(new Point(rectangle.X - m_nOffset, rectangle.Y + rectangle.Height), sz));                                     // Bottom Left
            subRectList.Add(new Rectangle(new Point(rectangle.X + (rectangle.Width / 2) - (m_nOffset / 2), rectangle.Y + rectangle.Height), sz));       // Bottom Center
            subRectList.Add(new Rectangle(new Point(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height), sz));                           // Bottom Right
        }

        public void SetNewLocationOfSubRectList()
        {
            Size sz = new Size(m_nOffset, m_nOffset);
            subRectList[0] = new Rectangle(new Point(rectangle.X - m_nOffset, rectangle.Y - m_nOffset), sz);
            subRectList[1] = new Rectangle(new Point(rectangle.X + (rectangle.Width / 2) - (m_nOffset / 2), rectangle.Y - m_nOffset), sz);              // Top Center
            subRectList[2] = new Rectangle(new Point(rectangle.X + rectangle.Width, rectangle.Y - m_nOffset), sz);                                      // Top Right

            subRectList[3] = new Rectangle(new Point(rectangle.X - m_nOffset, rectangle.Y + (rectangle.Height / 2) - (m_nOffset / 2)), sz);             // Center Left
            subRectList[4] = new Rectangle(new Point(rectangle.X + rectangle.Width, rectangle.Y + (rectangle.Height / 2) - (m_nOffset / 2)), sz);       // Center Right

            subRectList[5] = new Rectangle(new Point(rectangle.X - m_nOffset, rectangle.Y + rectangle.Height), sz);                                     // Bottom Left
            subRectList[6] = new Rectangle(new Point(rectangle.X + (rectangle.Width / 2) - (m_nOffset / 2), rectangle.Y + rectangle.Height), sz);       // Bottom Center
            subRectList[7] = new Rectangle(new Point(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height), sz);
        }

        public void ResizeRect()
        {

        }
    }
}
