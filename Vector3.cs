namespace HdrShot
{
    public class Vector3
    {
        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public Vector3 Clone()
        {
            return new Vector3
            {
                X = X,
                Y = Y,
                Z = Z
            };
        }
    }
}