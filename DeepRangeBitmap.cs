using System.Collections.Generic;
using System.Drawing;

namespace HdrShot
{
    public class DeepRangeBitmap
    {
        public List<List<DeepRangeColor>> Pixels { get; private set; }

        public DeepRangeColor BrightnessPixel { get; private set; }

        public DeepRangeBitmap(int width, int height)
        {
            Pixels = new List<List<DeepRangeColor>>();

            for (var y = 0; y < height; y++)
            {
                Pixels.Add(new List<DeepRangeColor>());

                for (var x = 0; x < width; x++)
                {
                    Pixels[y].Add(new DeepRangeColor());
                }
            }
        }

        public int AddColor(int x, int y, Color color)
        {
            /*if (color.GetBrightness() > 0.86f)
            {
                return;
            }*/

            var brightness = new DeepRangeColor();

            brightness += color;

            /*if (brightness.GetBrightnessNormal() > 0.95f || brightness.GetBrightnessNormal() < 0.04f)
            {
                return 0;
            }*/

            Pixels[y][x] += color;

            if (Pixels[y][x].MaxRelation == null || Pixels[y][x].MaxRelationDifference < brightness.RelationDifference)
            {
                Pixels[y][x].MaxRelation = brightness.Relation;
            }

            if (Pixels[y][x].GetBrightness() > (BrightnessPixel?.GetBrightness() ?? 0))
            {
                BrightnessPixel = Pixels[y][x];
            }

            return 1;
        }
    }
}