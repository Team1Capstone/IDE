using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IDE
{
    public class CustomRenderer : ToolStripProfessionalRenderer
    {
        // Make sure the textcolor is black
        protected override void InitializeItem(ToolStripItem item)
        {
            base.InitializeItem(item);
            item.ForeColor = Color.White;
        }

        protected override void Initialize(ToolStrip toolStrip)
        {
            base.Initialize(toolStrip);
            toolStrip.ForeColor = Color.White;
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            base.OnRenderToolStripBackground(e);

            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(40, 40, 40)), e.AffectedBounds);
        }

        protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderItemBackground(e);

            //e.Graphics.FillRectangle(new SolidBrush(Color.Green), e.Item.ContentRectangle);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            base.OnRenderToolStripBorder(e);

            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(20, 20, 20)), e.ConnectedArea);
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = Color.White;

            base.OnRenderItemText(e);
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            base.OnRenderImageMargin(e);

            var b = new SolidBrush(Color.FromArgb(20, 20, 20));

            e.Graphics.FillRectangle(b, e.AffectedBounds);
        }

        // Render checkmark
        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            base.OnRenderItemCheck(e);

            if (e.Item.Selected)
            {
                var rect = new Rectangle(3, 1, 20, 20);
                var rect2 = new Rectangle(4, 2, 18, 18);
                SolidBrush b = new SolidBrush(Color.Black);
                SolidBrush b2 = new SolidBrush(Color.Black);

                e.Graphics.FillRectangle(b, rect);
                e.Graphics.FillRectangle(b2, rect2);
                e.Graphics.DrawImage(e.Image, new Point(5, 3));
            }
            else
            {
                var rect = new Rectangle(3, 1, 20, 20);
                var rect2 = new Rectangle(4, 2, 18, 18);
                SolidBrush b = new SolidBrush(Color.Black);
                SolidBrush b2 = new SolidBrush(Color.Black);

                e.Graphics.FillRectangle(b, rect);
                e.Graphics.FillRectangle(b2, rect2);
                e.Graphics.DrawImage(e.Image, new Point(5, 3));
            }
        }

        // Render separator
        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            base.OnRenderSeparator(e);

            var DarkLine = new SolidBrush(Color.Black);
            var WhiteLine = new SolidBrush(Color.White);
            var rect = new Rectangle(32, 3, e.Item.Width - 32, 1);
            //e.Graphics.FillRectangle(DarkLine, rect);
            //e.Graphics.FillRectangle(WhiteLine, rect);
        }

        // Render arrow
        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            e.ArrowColor = Color.White;
            base.OnRenderArrow(e);
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderMenuItemBackground(e);

            if (e.Item.Enabled)
            {
                var color = Color.FromArgb(0, 0, 0);
                var rect = new Rectangle(0, 0, e.Item.Width, e.Item.Height);

                if (!e.Item.IsOnDropDown && !e.Item.Selected)
                {
                    color = Color.FromArgb(40, 40, 40);
                }
                else if (!e.Item.IsOnDropDown && e.Item.Selected)
                {
                    color = Color.FromArgb(80, 80, 80);
                }
                else if (e.Item.IsOnDropDown && e.Item.Selected)
                {
                    color = Color.FromArgb(80, 80, 80);
                }
                else
                {
                    color = Color.FromArgb(20, 20, 20);
                }

                if ((e.Item as ToolStripMenuItem).DropDown.Visible && !e.Item.IsOnDropDown)
                {
                    rect.Inflate(-1, -1);

                    color = Color.FromArgb(20, 20, 20);
                }

                using (var b = new SolidBrush(color))
                {
                    e.Graphics.FillRectangle(b, rect);
                }
            }
        }
    }
}