using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CipherRSA
{
    class RSA
    {
        private int[] public_code = new int [] {0,0};
        private int[] priv_code = new int [] {0,0};

        public void setPublicCode(int e, int n){
            this.public_code[0] = e;
            this.public_code[1] = n;
        }

        public void setPrivateCode(int d, int n)
        {
            this.priv_code[0] = d;
            this.priv_code[1] = n;
        }

        /* funkcja naiwnego potęgowania modulo - potrzebna bo przy zwykłym potęgowaniu z biblioteki 
         * Math liczby wychodzą poza zakres i nie działa
         */
        private static int PowerModulo(int a, int b, int m)
        {
            long result = 1;
            long x = a % m;

            for (int i = 1; i <= b; i <<= 1)
            {
                x %= m;
                if ((b & i) != 0)
                {
                    result *= x;
                    result %= m;
                }
                x *= x;
            }

            return (int)result;
        }
        
        //największy wspólny dzielnik dwóch liczb a i b jeśli zwraca 1 - to brak wspólnego dzielnika
        private int NWD(int a, int b)
        {
            while (a != b)
            {
                if (a < b)
                {
                    b -= a;
                }
                else
                {
                    a -= b;
                }
            }
            return a;
        }

        // metoda wyznacza najmniejszą wartość nieparzystą nie mającą wspólnego dzielnika z "o"
        private int generateE(int o)
        {
            int e = 3;

            while(e % 2 == 0 || NWD(e, o) != 1){
                e += 2;
            }

            return e;
        }

        private int generateD(int e, int o)
        {
            int d = 0;

            while (d * e % o != 1)
            {
                d++;
            }

            return d;
        }

        //metoda generująca kody
        public void generateCodes(int prime_a, int prime_b)
        {
            int o = (prime_a - 1) * (prime_b - 1); //funkcja Eulera
            Console.WriteLine("O = " + o);

            int n = prime_a * prime_b; // moduł 
            Console.WriteLine("N = " + n);

            int e = generateE(o);
            //int e = 7;
            Console.WriteLine("e = " + e);

            int d = generateD(e, o);
            Console.WriteLine("d = " + d);
            //Console.WriteLine("Powinno być 1: " + d * e % o);

            setPublicCode(e, n);
            setPrivateCode(d, n);

        }

        // zaszyfruj wiadomość
        public void encryptMessage()
        {
            String oryginalMessage = "";
            String codedMessage = "";
            String encryptMessage = "";

            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader("wiadomosc.txt"))
                {
                    // Read the stream to a string
                    oryginalMessage = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return;
            }

            //-------------szyfrowanie--------------

            using (StreamWriter sw = new StreamWriter("zaszyfrowana wiadomosc.txt"))
            {
                foreach (char c in oryginalMessage)
                {
                    int unicode = c;
                    //Console.WriteLine(PowerModulo(unicode, public_code[0], public_code[1]));
                    encryptMessage = (Math.Pow(unicode, this.public_code[0]) % this.public_code[1]).ToString();
                    sw.WriteLine(encryptMessage);
                }
            }
            Console.WriteLine();
            Console.WriteLine("Wiadomość z pliku 'wiadomosc.txt' została zaszyfrowana i zapisana w pliku 'zaszyfrowana wiadomosc.txt'");
        }

        // rozszyfruj wiadomość
        public void decipherMessage()
        {
            String encryptMessage = "";
            String decipherMessage = "";
            String codedMessage = "";

            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader("zaszyfrowana wiadomosc.txt"))
                {
                    using (StreamWriter sw = new StreamWriter("rozszyfrowana wiadomosc.txt"))
                    {
                        //odczytywanie pliku linia po lini, dekodowanie znaków i zapisywanie do pliku
                        while ((encryptMessage = sr.ReadLine()) != null)
                        {
                            codedMessage = (PowerModulo(Int32.Parse(encryptMessage), this.priv_code[0], this.priv_code[1]) % this.priv_code[1]).ToString();
                            //Console.WriteLine("Odczytana i zdekodowana liczba: " + encryptMessage);

                            sw.Write(Convert.ToChar(Int32.Parse(codedMessage)));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return;
            }
            Console.WriteLine();
            Console.WriteLine("Zaszyfrowana wiadomość z pliku 'zaszyfrowana wiadomosc.txt' została rozszyfrowana i zapisana do pliku 'rozszyfrowana wiadomosc.txt'.");
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            int a = 0, b = 0;

            Console.WriteLine("Wprowadź liczbę pierwszą nr 1: ");
            a = Int32.Parse(Console.ReadLine());

            Console.WriteLine("Wprowadź liczbę pierwszą nr 2: ");
            b = Int32.Parse(Console.ReadLine());

            RSA cipher = new RSA();
            cipher.generateCodes(a, b);
            //cipher.generateCodes(13, 11);

            cipher.encryptMessage();
            cipher.decipherMessage();

            Console.ReadKey();
        }
    }
}
