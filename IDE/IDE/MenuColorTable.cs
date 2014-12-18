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

            using (var b = new SolidBrush(Color.FromArgb(20, 20, 20)))
            {
                e.Graphics.FillRectangle(b, e.ConnectedArea);
            }
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

        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            base.OnRenderItemCheck(e);

            using (var b = new SolidBrush(Color.Black))
            {
                e.Graphics.FillRectangle(b, new Rectangle(3, 1, 20, 20));
                e.Graphics.FillRectangle(b, new Rectangle(4, 2, 18, 18));
            }

            e.Graphics.DrawImage(e.Image, new Point(5, 3));
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
            int colorValue = 0;

            if (e.Item.Enabled)
            {
                //                var color = Color.FromArgb(0, 0, 0);

                var rect = new Rectangle(0, 0, e.Item.Width, e.Item.Height);

                if (e.Item.Selected)
                {
                    //                    color = Color.FromArgb(80, 80, 80);
                    colorValue = 80;
                }
                else if (!e.Item.IsOnDropDown)
                {
                    //                    color = Color.FromArgb(40, 40, 40);
                    colorValue = 40;
                }
                else
                {
                    //                  color = Color.FromArgb(20, 20, 20);
                    colorValue = 20;
                }

                if ((e.Item as ToolStripMenuItem).DropDown.Visible && !e.Item.IsOnDropDown)
                {
                    rect.Inflate(-1, -1);

                    //                    color = Color.FromArgb(20, 20, 20);
                    colorValue = 20;
                }

                using (var b = new SolidBrush(Color.FromArgb(colorValue, colorValue, colorValue)))
                {
                    e.Graphics.FillRectangle(b, rect);
                }
            }
        }
    }
}