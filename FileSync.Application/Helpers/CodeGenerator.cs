using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync.Application.Helpers
{
    class CodeGenerator
    {
        private static readonly string[] Adjectives =
    {
        "RED", "BLUE", "GREEN", "GOLD", "SILVER",
        "QUICK", "BRIGHT", "CALM", "BOLD", "SWIFT",
        "WARM", "COOL", "FRESH", "WILD", "FREE", "NIMBLE",  "STEADY"
    };

        private static readonly string[] Nouns =
        {
        "TIGER", "EAGLE", "WOLF", "HAWK", "BEAR",
        "RIVER", "MOUNTAIN", "FOREST", "OCEAN", "STORM",
        "PHOENIX", "DRAGON", "FALCON", "LION", "SHARK", "COSMOS", "NOVA", "ORBIT",  "COMET",    "ASTRO"

    };

        private static readonly Random _random = new();


        public static string GenerateSpaceCode()
        {
            var adjective = Adjectives[_random.Next(Adjectives.Length)];
            var noun = Nouns[_random.Next(Nouns.Length)];
            var number = _random.Next(10, 99);
            return $"{adjective}-{noun}-{number}";




        }

        public static string GenerateDropCode()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var code = new char[9];

            for (int i = 0; i < code.Length; i++)
            {
                code[i] = chars[_random.Next(chars.Length)];
            }

            return new string(code);
        }


    }
}
