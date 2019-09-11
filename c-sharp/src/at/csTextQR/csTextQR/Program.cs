using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csTextQR
{
    class Program
    {
        static void Main(string[] args)
        {

            int width = 32;
            string text = "https://www.fiskaltrust.at";
            char[] ft = fiskaltrust.ifPOS.Utilities.QR_TextChars(text,width,true);

            int line = 0;
            Console.WriteLine(text);
            Console.WriteLine("============================================================");
            while(line*width<ft.Length)
            {
                Console.WriteLine(ft, width * line++, width);
            }
            Console.WriteLine("press key ===================================================");
            Console.ReadKey();



            width = 64;
            text = "_R1-AT1_ft-1_ft136#297_2016-06-03T04:52:34_29,30_0,00_0,00_0,00_0,00_igFMecg=_2159b9f6_dr9imioA8Lw=_FS3AkgpKCewQKuz9h6B8TUCiSIdYzntPrhB1Gfmd27rKZ3bXXDO564nwSVU9B4ORavFRH9o+zp4NVcRxwiO/1A==";
            ft = fiskaltrust.ifPOS.Utilities.QR_TextChars(text, width, true);

            line = 0;
            Console.WriteLine(text);
            Console.WriteLine("============================================================");
            while (line * width < ft.Length)
            {
                Console.WriteLine(ft, width * line++, width);
            }
            Console.WriteLine("press key ===================================================");
            Console.ReadKey();

        }
    }
}
