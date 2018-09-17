using System;
using System.Drawing;

namespace HdrShot
{
    public class DeepRangeColor
    {
        public float Red { get; set; }

        public float Green { get; set; }

        public float Blue { get; set; }

        public Vector3 Relation
        {
            get
            {
                //var max = (float)(Red + Green + Blue);
                var max = (float)Math.Max(Red, Math.Max(Green, Blue));
                //var value = 1.5f;

                var vector = new Vector3
                {
                    X = (max > 0 ? Red / max : 0),
                    Y = (max > 0 ? Green / max : 0),
                    Z = (max > 0 ? Blue / max : 0)
                };

                /*vector.X = Red == max ? vector.X / value : vector.X * value;
                vector.Y = Green == max ? vector.Y / value : vector.Y * value;
                vector.Z = Blue == max ? vector.Z / value : vector.Z * value;*/

                return vector;
            }
        }

        public Vector3 MaxRelation
        {
            get
            {
                return maxRelation;
            }
            set
            {
                maxRelation = value;
            }
        }

        private Vector3 maxRelation;

        public float MaxRelationDifference
        {
            get
            {
                var diff = (MaxRelation.X + MaxRelation.Y + MaxRelation.Z) / 3f;

                return diff == 0 ? 0 : 1 - diff;
            }
        }

        public float RelationDifference
        {
            get
            {
                var diff = (Relation.X + Relation.Y + Relation.Z) / 3f;

                return diff == 0 ? 0 : 1 - diff;
            }
        }

        public float GetBrightness()
        {
            var redBrightness = Red / 255f;
            var greenBrightness = Green / 255f;
            var blueBrightness = Blue / 255f;

            return (redBrightness + greenBrightness + blueBrightness) / 3;
            //return Math.Max(redBrightness, Math.Max(greenBrightness, blueBrightness));
        }

        public float GetBrightnessNormal()
        {
            var red = Red - Math.Floor(Red / 255) * 255;
            var green = Green - Math.Floor(Green / 255) * 255;
            var blue = Blue - Math.Floor(Blue / 255) * 255;

            var redBrightness = (float)(red) / 255f;
            var greenBrightness = (float)(green) / 255f;
            var blueBrightness = (float)(blue) / 255f;

            return (redBrightness + greenBrightness + blueBrightness) / 3;
            //return Math.Max(redBrightness, Math.Max(greenBrightness, blueBrightness));
        }

        public void SetBrightnessByLocal(float targetBrightness)
        {
            var brightness = GetBrightness();
            var difference = targetBrightness - GetBrightness();
            var relation = MaxRelation.Clone();

            Red = relation.X * difference * (255 * (int)Math.Ceiling(brightness));
            Green = relation.Y * difference * (255 * (int)Math.Ceiling(brightness));
            Blue = relation.Z * difference * (255 * (int)Math.Ceiling(brightness));
        }

        public void SetBrightness(float targetBrightness)
        {
            var brightness = GetBrightness();
            var difference = targetBrightness - 1;
            var relation = MaxRelation.Clone();

            Red = relation.X * targetBrightness * (255 * (int)Math.Ceiling(brightness));
            Green = relation.Y * targetBrightness * (255 * (int)Math.Ceiling(brightness));
            Blue = relation.Z * targetBrightness * (255 * (int)Math.Ceiling(brightness));
        }

        public Color Normalize()
        {
            return Color.FromArgb((int)(Relation.X * 255), (int)(Relation.Y * 255), (int)(Relation.Z * 255));
        }

        public static DeepRangeColor operator +(DeepRangeColor deepRangeColor, Color color)
        {
            var number = 5;

            return new DeepRangeColor
            {
                Red = deepRangeColor.Red + (color.R / number) * number,
                Green = deepRangeColor.Green + (color.G / number) * number,
                Blue = deepRangeColor.Blue + (color.B / number) * number,
                MaxRelation = deepRangeColor.MaxRelation
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