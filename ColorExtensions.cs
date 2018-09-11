using System.Drawing;

namespace HdrShot
{
    public static class ColorExtensions
    {
        public static Color SetBrightness(this Color color, float brightness)
        {
            float red = color.R;
            float green = color.G;
            float blue = color.B;

            if (brightness < 0)
            {
                brightness = 1 + brightness;
                red *= brightness;
                green *= brightness;
                blue *= brightness;
            }
            else
            {
                red = (255 - red) * brightness + red;
                green = (255 - green) * brightness + green;
                blue = (255 - blue) * brightness + blue;
            }

            red = red > 255 ? 255 : red < 0 ? 0 : red;
            green = green > 255 ? 255 : green < 0 ? 0 : green;
            blue = blue > 255 ? 255 : blue < 0 ? 0 : blue;

            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }
    }
}