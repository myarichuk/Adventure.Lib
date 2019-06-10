using System;
using System.IO;
using Adventure.World;

namespace Adventure.Playground
{
    internal static class Program
    {
        public struct Foo
        {
            public int X;
            public long Y;
            public double Z;

            public override string ToString()
            {
                return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z}";
            }
        }

        static void Main(string[] args)
        {
            var map = new Map<Foo>("AA",10,10);
            map.Put(1,1,new Foo{ X = 1, Y = 123, Z = 4.56 });
        }
    }
}
