using System;
using System.Drawing;

namespace HdrShot
{
    public class DeepRangeColor
    {
        public int Red { get; set; }

        public int Green { get; set; }

        public int Blue { get; set; }

        public Vector3 Relation
        {
            get
            {
                //var max = (float)(Red + Green + Blue);
                var max = (float)Math.Max(Red, Math.Max(Green, Blue));
                var value = 1f;

                var vector = new Vector3
                {
                    X = (max > 0 ? Red / max : 0),
                    Y = (max > 0 ? Green / max : 0),
                    Z = (max > 0 ? Blue / max : 0)
                };

                /*vector.X = Red == Math.Max(Red, Math.Max(Green, Blue)) ? vector.X / value : vector.X * value;
                vector.Y = Green == Math.Max(Red, Math.Max(Green, Blue)) ? vector.Y / value : vector.Y * value;
                vector.Z = Blue == Math.Max(Red, Math.Max(Green, Blue)) ? vector.Z / value : vector.Z * value;*/

                return vector;
            }
        }

        public float GetBrightness()
        {
            var redBrightness = Red / 255f;
            var greenBrightness = Green / 255f;
            var blueBrightness = Blue / 255f;

            return (redBrightness + greenBrightness + blueBrightness) / 3;
        }

        public static DeepRangeColor operator +(DeepRangeColor deepRangeColor, Color color)
        {
            var number = 5;

            return new DeepRangeColor
            {
                Red = deepRangeColor.Red + (color.R / number) * number,
                Green = deepRangeColor.Green + (color.G / number) * number,
                Blue = deepRangeColor.Blue + (color.B / number) * number
            };
        }

        public override bool Equals(object obj)
        {
            var data = obj as DeepRangeColor;

            if (data != null)
            {
                return data.Red == Red
                    && data.Green == Green
                    && data.Blue == Blue;
            }

            return base.Equals(obj);    
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}