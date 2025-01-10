using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecast
{
    public static class CIndicePearson
    {
        public static double CalcolaIndicePearson(float[] X, float[] Y)
        {
            if (X.Length < Y.Length)
            {
                Array.Resize(ref Y, X.Length); // Must be the same length
            }
            else if (Y.Length < X.Length)
            {
                Array.Resize(ref X, Y.Length);
            }

            int n = X.Length;

            // Calcolo delle somme necessarie
            double sommaX = X.Sum();
            double sommaY = Y.Sum();
            double sommaXY = X.Zip(Y, (x, y) => x * y).Sum(); // Sum based on matching indexes
            double sommaX2 = X.Select(x => x * x).Sum();
            double sommaY2 = Y.Select(y => y * y).Sum();

            // Calcolo dell'indice di Pearson
            double numeratore = (n * sommaXY) - (sommaX * sommaY);
            double denominatore = Math.Sqrt((n * sommaX2 - Math.Pow(sommaX, 2)) * (n * sommaY2 - Math.Pow(sommaY, 2)));

            if (denominatore == 0)
            {
                return 0; // Se il denominatore è 0, la correlazione non è definita (avviso: divisione per zero)
            }

            return numeratore / denominatore;
        }
    }
}
