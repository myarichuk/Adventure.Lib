using System;
using System.Collections.Generic;
using System.Numerics;

namespace Adventure.World
{
    public static class Utils
    {
        private static readonly Random Random = new Random();
    
        //credit: adapted from here - https://stackoverflow.com/a/26127012/320103
        public static IEnumerable<Vector3> FibonacciSphere(int samples = 1000, bool randomize = true)
        {
            var rnd = 1.0f;
            if(randomize)
                rnd = Random.Next() * samples;
            var offset = 2.0f / samples;

            var increment = Math.PI * (3.0f - Math.Sqrt(5.0f));
            for (int i = 0; i < samples; i++)
            {
                var y =  i * offset - 1 + offset / 2;
                var r = Math.Sqrt(1 - Math.Pow(y, 2));
                var phi = (i + rnd) % samples * increment;

                var x = (float)(Math.Cos(phi) * r);
                var z = (float)(Math.Sin(phi) * r);
                
                yield return new Vector3(x,y,z);
            }
        }
    }
}
