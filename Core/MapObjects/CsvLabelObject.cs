using System.Collections.Generic;
using System;
using System.Drawing;
using System.IO;
using EnhancedMap.Core.MapObjects;

namespace EnhancedMap.Core.MapObjects
{

    public static class IconStorage
    {
        public static readonly Dictionary<string, Bitmap> Icons = new();

        public static void Load()
        {
            Icons.Clear();
            string iconDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Icon");
            if (!Directory.Exists(iconDir))
                return;

            foreach (string file in Directory.GetFiles(iconDir, "*.png"))
            {
                string key = Path.GetFileNameWithoutExtension(file).ToLowerInvariant();
                Icons[key] = new Bitmap(file);
            }
        }

        public static Bitmap GetIcon(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            Icons.TryGetValue(name.ToLowerInvariant(), out var bmp);
            return bmp;
        }
    }

    public class CsvLabelObject : LabelObject
    {
        public string IconName { get; set; }
        public int Zoom { get; set; } = 0; // adiciona essa prop no CsvLabelObject se ainda não tiver

        public CsvLabelObject(short x, short y, string text, string iconName) : base(x, y, text)
        {
            IconName = iconName;
        }

        public override bool Render(Graphics g, int x, int y, int canvasW, int canvasH)
        {
            if (!IsVisible || Map != Global.Facet)
                return false;

            int labelX = RelativePosition.X;
            int labelY = RelativePosition.Y;

            (labelX, labelY) = Geometry.RotatePoint(labelX, labelY, Global.Zoom, 1, Global.Angle);
            labelX += canvasW / 2;
            labelY += canvasH / 2;

            // Sempre desenha o ícone se existir
            if (!string.IsNullOrEmpty(IconName))
            {
                var icon = IconStorage.GetIcon(IconName);
                if (icon != null)
                {
                    g.DrawImage(icon, labelX - icon.Width / 2, labelY - icon.Height);
                }
            }

            // Detecta mouse hover
            Point mouse = Global.MainWindow.PointToClient(System.Windows.Forms.Control.MousePosition);
            int hoverRadius = 20;
            bool isHovering = Math.Abs(mouse.X - labelX) < hoverRadius && Math.Abs(mouse.Y - labelY) < hoverRadius;

            if (isHovering)
            {
                string text = Text;
                using Font font = new Font("Segoe UI", 8);
                SizeF textSize = g.MeasureString(text, font);
                int padding = 6;
                int textX = labelX - (int)(textSize.Width / 2);
                int textY = labelY - 20;

                Rectangle rect = new Rectangle(
                    textX - padding,
                    textY - padding,
                    (int)textSize.Width + padding * 2,
                    (int)textSize.Height + padding * 2
                );

                using Brush bgBrush = new SolidBrush(Color.FromArgb(200, 0, 0, 0));
                g.FillRectangle(bgBrush, rect);
                using Brush textBrush = new SolidBrush(ColorTranslator.FromHtml("#66CCFF"));
                g.DrawString(text, font, textBrush, textX, textY);
            }

            return true;
        }

    }
}
