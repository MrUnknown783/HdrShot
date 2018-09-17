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
            /*var asd = new DeepRangeColor { Red = 255, Green = 255, Blue = 255 };
            asd.SetBrightnessByLocal(1.5f);*/

            var images = Directory.GetFiles("../../../images/test").Select(x => new Bitmap(Image.FromFile(x))).ToList();

            var deepRangeBitmap = new DeepRangeBitmap(images[0].Width, images[0].Height);
            var brightnessMapPre = new Bitmap(images[0].Width, images[0].Height);
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
                        /*if (!(x > 196 && y > 357 && x < 268 && y < 375
                            || (x > 560 && y > 390 && x < 755 && y < 500)))
                        {
                            continue;
                        }*/

                        deepRangeBitmap.AddColor(x, y, images[i].GetPixel(x, y));

                        var b = images[i].GetPixel(x, y).GetBrightness();

                        brightnessMaps[i].SetPixel(x, y, Color.FromArgb((int)(b * 255), (int)(b * 255), (int)(b * 255)));

                        /*if (deepRangeBitmap.AddColor(x, y, images[i].GetPixel(x, y)) == 0)
                        {
                            brightnessMaps[i].SetPixel(x, y, images[i].GetPixel(x, y));
                        }
                        else
                        {
                            brightnessMaps[i].SetPixel(x, y, Color.Red);
                        }*/
                    }
                }

                brightnessMaps[i].Save($"map{i}.png");
                Console.WriteLine($"{i} images out of {images.Count}");
            }

            var average = deepRangeBitmap.Pixels.Average(x => x.Average(s => s.GetBrightness()));
            var mostBrightnessColor = deepRangeBitmap.BrightnessPixel;
            var mostBrightnessPixel = mostBrightnessColor.GetBrightness();
            var target = (average / mostBrightnessPixel) * 0.5f;
            var maxChange = 1f;

            var map = new List<List<Color>>();
            /*var matrix = new float[,]
            {
                {  -0.2f, -0.4f, -0.2f },
                {  -0.4f,  4.6f, -0.4f },
                {  -0.2f, -0.4f, -0.2f }
            };*/

            var matrix = new float[,]
            {
                {  0.2f, 0.4f, 0.6f, 0.4f, 0.2f },
                {  0.4f, 0.8f, 1.4f, 0.8f, 0.4f },
                {  0.6f, 1.4f, -16f, 1.4f, 0.6f },
                {  0.4f, 0.8f, 1.4f, 0.8f, 0.4f },
                {  0.2f, 0.4f, 0.6f, 0.4f, 0.2f },
            };

            var width = (int)Math.Floor(matrix.GetLength(0) / 2f);
            var height = (int)Math.Floor(matrix.GetLength(1) / 2f);

            for (var y = 0; y < brightnessMap.Height; y++)
            {
                map.Add(new List<Color>());

                for (var x = 0; x < brightnessMap.Width; x++)
                {
                    var brightness = deepRangeBitmap.Pixels[y][x].GetBrightness() / mostBrightnessPixel;

                    //brightness += brightness > average ? -brightness * min : brightness * min;

                    /*var diff = target - brightness;
                    
                    var a = Math.Abs(diff / brightness) <= maxChange ? diff : brightness * maxChange;
                    
                    brightness += diff > 0 ? Math.Abs(a) : -Math.Abs(a);*/

                    var red = (int)Add(deepRangeBitmap.Pixels[y][x].Relation.X * (255 * brightness));
                    var green = (int)Add(deepRangeBitmap.Pixels[y][x].Relation.Y * (255 * brightness));
                    var blue = (int)Add(deepRangeBitmap.Pixels[y][x].Relation.Z * (255 * brightness));

                    var deepColor = new DeepRangeColor { Red = red, Green = green, Blue = blue };

                    //deepColor.SetBrightness(0.3f);

                    var color = Color.FromArgb((int)deepColor.Red, (int)deepColor.Green, (int)deepColor.Blue);

                    brightnessMap.SetPixel(x, y, Color.FromArgb((int)(255 * brightness), (int)(255 * brightness), (int)(255 * brightness)));
                    test.SetPixel(x, y, color);
                    map[y].Add(color);
                }
            }

            brightnessMap.Save("brightnessMap.png");
            test.Save("test2.png");

            map = ApplyMatrix(map, height, width, matrix);

            for (var y = 0; y < map.Count; y++)
            {
                for (var x = 0; x < map[y].Count; x++)
                {
                    pre.SetPixel(x, y, map[y][x]);
                }
            }

            pre.Save("pre.png");
            
            /*for (var y = 0; y < brightnessMap.Height; y++)
            {
                map.Add(new List<Color>());

                for (var x = 0; x < brightnessMap.Width; x++)
                {
                    var brightness = deepRangeBitmap.Pixels[y][x].GetBrightness() / mostBrightnessPixel;

                    //brightness += brightness > average ? -brightness * min : brightness * min;

                    //var diff = target - brightness;
                    //
                    //var a = Math.Abs(diff / brightness) <= maxChange ? diff : brightness * maxChange;
                    //
                    //brightness += diff > 0 ? Math.Abs(a) : -Math.Abs(a);

                    var red = (int)Add(deepRangeBitmap.Pixels[y][x].Relation.X * (255 * brightness));
                    var green = (int)Add(deepRangeBitmap.Pixels[y][x].Relation.Y * (255 * brightness));
                    var blue = (int)Add(deepRangeBitmap.Pixels[y][x].Relation.Z * (255 * brightness));

                    var deepColor = new DeepRangeColor { Red = red, Green = green, Blue = blue };

                    deepColor.SetBrightness(0.5f);

                    var color = (int)(deepColor.GetBrightness() * 255);

                    //brightnessMap.SetPixel(x, y, color);
                    test.SetPixel(x, y, Color.FromArgb((int)deepColor.Red, (int)deepColor.Green, (int)deepColor.Blue));
                    map[y].Add(Color.FromArgb((int)deepColor.Red, (int)deepColor.Green, (int)deepColor.Blue));
                    pre.SetPixel(x, y, Color.FromArgb((int)deepColor.Red, (int)deepColor.Green, (int)deepColor.Blue));

                    //var color = (int)((new DeepRangeColor { Red = red, Green = green, Blue = blue }).GetBrightness() * 255);

                    brightnessMapPre.SetPixel(x, y, Color.FromArgb(color, color, color));
                }
            }

            for (var i = 0; i < 1; i++)
            {
                var buffer = new List<List<Color>>();

                for (var y = 0; y < map.Count; y++)
                {
                    buffer.Add(new List<Color>());

                    for (var x = 0; x < map[y].Count; x++)
                    {
                        buffer[y].Add(Color.Black);

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

                        red /= count;
                        green /= count;
                        blue /= count;

                        test.SetPixel(x, y, Color.FromArgb((int)red, (int)green, (int)blue));
                        buffer[y][x] = Color.FromArgb((int)red, (int)green, (int)blue);

                        var color = (int)((new DeepRangeColor { Red = red, Green = green, Blue = blue }).GetBrightness() * 255);

                        brightnessMap.SetPixel(x, y, Color.FromArgb(color, color, color));
                    }
                }

                map = buffer;
                brightnessMap.Save($"brightnessMap{i}.png");
            }

            brightnessMapPre.Save("brightnessMapPre.png");
            pre.Save("pre.png");*/
        }

        static float Add(float value)
        {
            return value > 255 ? 255 : value;
        }

        static List<List<Color>> ApplyMatrix(List<List<Color>> map, int height, int width, float[,] matrix)
        {
            var result = new List<List<Color>>();

            for (var y = 0; y < map.Count; y++)
            {
                result.Add(new List<Color>());

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

                    /*red /= count;
                    green /= count;
                    blue /= count;*/

                    red = red < 0 ? 0 : (red > 255 ? 255 : red);
                    green = green < 0 ? 0 : (green > 255 ? 255 : green);
                    blue = blue < 0 ? 0 : (blue > 255 ? 255 : blue);

                    var color = Color.FromArgb((int)red, (int)green, (int)blue);

                    if (color.R > 2 && color.G > 2 && color.B > 2)
                    {
                        color = color.SetBrightness(0.2f);
                    }

                    result[y].Add(color);

                    //var color = (int)((new DeepRangeColor { Red = red, Green = green, Blue = blue }).GetBrightness() * 255);
                }
            }

            return result;
        }
    }
}