using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace HdrShot
{
    class Program
    {
        static Bitmap filled;

        static void Main(string[] args)
        {
            var images = Directory.GetFiles("../../../images/test").Select(x => new Bitmap(Image.FromFile(x))).ToList();

            var deepRangeBitmap = new DeepRangeBitmap(images[0].Width, images[0].Height);
            var brightnessMap = new Bitmap(images[0].Width, images[0].Height);
            var test = new Bitmap(images[0].Width, images[0].Height);
            var pre = new Bitmap(images[0].Width, images[0].Height);

            var brightnessMaps = new List<Bitmap>();

            for (var i = 0; i < images.Count; i++)
            {
                brightnessMaps.Add(new Bitmap(images[0].Width, images[0].Height));

                for (var y = 0; y < images[i].Height; y++)
                {
                    for (var x = 0; x < images[i].Width; x++)
                    {
                        if (deepRangeBitmap.AddColor(x, y, images[i].GetPixel(x, y)) == 0)
                        {
                            brightnessMaps[i].SetPixel(x, y, images[i].GetPixel(x, y));
                        }
                    }
                }

                brightnessMaps[i].Save($"map{i}.png");
                Console.WriteLine($"{i} images out of {images.Count}");
            }

            var average = 1 / deepRangeBitmap.Pixels.Average(x => x.Average(s => s.GetBrightness()));
            var mostBrightnessColor = deepRangeBitmap.BrightnessPixel;
            var mostBrightnessPixel = mostBrightnessColor.GetBrightness();
            var target = 0.7f;
            var maxChange = 0.3f;

            var map = new List<List<Color>>();
            var matrix = new float[,]
            {
                { 1.2f, 1.2f, 1.2f},
                { 1.2f, 0.2f, 1.2f},
                { 1.2f, 1.2f, 1.2f}
            };

            var width = (int)Math.Ceiling(matrix.GetLength(0) / 2f);
            var height = (int)Math.Ceiling(matrix.GetLength(1) / 2f);

            for (var y = 0; y < brightnessMap.Height; y++)
            {
                map.Add(new List<Color>());

                for (var x = 0; x < brightnessMap.Width; x++)
                {
                    var brightness = deepRangeBitmap.Pixels[y][x].GetBrightness() / mostBrightnessPixel;

                    //brightness += brightness > average ? -brightness * min : brightness * min;

                    var diff = target - brightness;

                    var a = Math.Abs(diff / brightness) <= maxChange ? diff : brightness * maxChange;

                    brightness += diff > 0 ? Math.Abs(a) : -Math.Abs(a);

                    var red = (int)Add(deepRangeBitmap.Pixels[y][x].Relation.X * (255 * brightness));
                    var green = (int)Add(deepRangeBitmap.Pixels[y][x].Relation.Y * (255 * brightness));
                    var blue = (int)Add(deepRangeBitmap.Pixels[y][x].Relation.Z * (255 * brightness));

                    //brightnessMap.SetPixel(x, y, color);
                    //test.SetPixel(x, y, Color.FromArgb(red, green, blue));
                    map[y].Add(Color.FromArgb(red, green, blue));
                    pre.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }

            for (var y = 0; y < map.Count; y++)
            {
                for (var x = 0; x < map[y].Count; x++)
                {
                    var red = 0f;
                    var green = 0f;
                    var blue = 0f;

                    var count = 0;

                    for (int my = y - height, myi = 0; my < map.Count && myi < matrix.GetLength(0); my++, myi++)
                    {
                        for (int mx = x - width, mxi = 0; mx < map[y].Count && mxi < matrix.GetLength(1); mx++, mxi++)
                        {
                            if (my < 0 || mx < 0)
                            {
                                continue;
                            }

                            red += matrix[myi, mxi] * map[my][mx].R;
                            green += matrix[myi, mxi] * map[my][mx].G;
                            blue += matrix[myi, mxi] * map[my][mx].B;

                            count++;
                        }
                    }

                    red /= count - 1;
                    green /= count - 1;
                    blue /= count - 1;

                    test.SetPixel(x, y, Color.FromArgb((int)red, (int)green, (int)blue));
                }
            }

            //brightnessMap.Save("map.png");
            pre.Save("pre.png");
            test.Save("test.png");
        }

        static float Add(float value)
        {
            return value > 255 ? 255 : value;
        }
    }
}