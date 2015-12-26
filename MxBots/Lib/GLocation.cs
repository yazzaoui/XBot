namespace GLib
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct GLocation
    {
        public float X;
        public float Y;
        public float Z;
        public float Facing;
       
        public GLocation(float _X, float _Y, float _Z)
        {
            this.X = _X;
            this.Y = _Y;
            this.Z = _Z;
            this.Facing = 0f;
        }

        
        public GLocation(float _X, float _Y, float _Z, float _Facing)
        {
            this.X = _X;
            this.Y = _Y;
            this.Z = _Z;
            this.Facing = _Facing;
        }

        public double distanceFrom(float nX, float nY, float nZ)
        {
            return Math.Sqrt((Math.Pow((double) (this.X - nX), 2.0) + Math.Pow((double) (this.Y - nY), 2.0)) + Math.Pow((double) (this.Z - nZ), 2.0));
        }

        public double distanceFrom(GLocation pos)
        {
            return Math.Sqrt((Math.Pow((double) (this.X - pos.X), 2.0) + Math.Pow((double) (this.Y - pos.Y), 2.0)) + Math.Pow((double) (this.Z - pos.Z), 2.0));
        }

        public override string ToString()
        {
            return string.Concat(new object[] { "( ", this.X, ", ", this.Y, ", ", this.Z, " )" });
        }

        public bool isValid()
        {
            return (((this.X != 0f) && (this.Y != 0f)) && (this.Z != 0f));
        }
    }
}

