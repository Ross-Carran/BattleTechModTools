using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Planet_Boundary_finder
{
    class Program
    {
        static void Main(string[] args)
        {
            float num1 = 9999f;
            float num2 = -9999f;
            float num3 = 9999f;
            float num4 = -9999f;

            string planetName = "";
            string num1Planet = "None";
            string num2Planet = "None";
            string num3Planet = "None";
            string num4Planet = "None";

            int left = -9999;
            int right = 9999;
            int top = -9999;
            int bottom = 9999;

            Console.WriteLine("Hello World!");

            var planetJsons = new DirectoryInfo(args[0]).GetFiles();
            foreach (var planetJson in planetJsons)
            {
                Console.WriteLine(planetJson.ToString());

                JObject o1 = JObject.Parse(File.ReadAllText(planetJson.ToString()));
                Console.WriteLine("x: " + (string)o1["Position"]["x"]);
                Console.WriteLine("y: " + (string)o1["Position"]["y"]);
                Single x = float.Parse((string)o1["Position"]["x"]);
                Single y = float.Parse((string)o1["Position"]["y"]);
                planetName = (string)o1["Description"]["Name"];
                if ((double)x >= (double)left && (double)x <= (double)right &&
                    (double)y >= (double)top && (double)y <= (double)bottom)
                {
                    if ((double)x < (double)num1)
                    {
                        num1 = x;
                        num1Planet = planetName;
                    }


                    if ((double)x > (double)num2)
                    {
                        num2 = x;
                        num2Planet = planetName;
                    }

                    if ((double)y < (double)num3)
                    {
                        num3 = y;
                        num3Planet = planetName;
                    }

                    if ((double)y > (double)num4)
                    {
                        num4 = y;
                        num4Planet = planetName;
                    }

                }
            }

            Console.WriteLine("num1 x: " + num1.ToString() + " Planet Name: " + num1Planet);
            Console.WriteLine("num2 x: " + num2.ToString() + " Planet Name: " + num2Planet);
            Console.WriteLine("num3 y: " + num3.ToString() + " Planet Name: " + num3Planet);
            Console.WriteLine("num4 y: " + num4.ToString() + " Planet Name: " + num4Planet);
        }
    }
}