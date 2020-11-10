using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Tetris
{   static class Program
    {
        public static ConsoleColor[,] warna = new ConsoleColor[23, 10];
        public static int[,] grid = new int[23, 10];
        public static int[,] runtuh = new int[23, 10];
        public static Stopwatch dropTimer = new Stopwatch();
        public static int waktuturun, dropRate = 300, linesCleared = 0, score = 0, level = 1;
        public static bool isDropped = false, isKeyPressed = false, final = true;
        public static tetris t;
        public static char sqr = '■';

        public class tetris
        {
            public static int[,] I = new int[1, 4] { { 1, 1, 1, 1 } };
            public static int[,] O = new int[2, 2] { { 1, 1 }, { 1, 1 } };
            public static int[,] T = new int[2, 3] { { 0, 1, 0 }, { 1, 1, 1 } };
            public static int[,] S = new int[2, 3] { { 0, 1, 1 }, { 1, 1, 0 } };
            public static int[,] Z = new int[2, 3] { { 1, 1, 0 }, { 0, 1, 1 } };
            public static int[,] J = new int[2, 3] { { 1, 0, 0 }, { 1, 1, 1 } };
            public static int[,] L = new int[2, 3] { { 0, 0, 1 }, { 1, 1, 1 } };
            public static List<int[,]> bentuktetris = new List<int[,]>() { I, O, T, S, Z, J, L };
            public int[,] shape;
            public ConsoleColor tempwarna;
            public List<int[]> location = new List<int[]>();
            public tetris()
            {
                Random rnd = new Random();
                shape = bentuktetris[rnd.Next(0, 7)];
                tempwarna = (ConsoleColor)rnd.Next(1, 16);
                for (int i = 26; i < 33; ++i)
                {
                    for (int j = 3; j < 10; j++)
                    {
                        Console.SetCursorPosition(i, j);
                        Console.Write(" ");
                    }
                }
                bingkai();
                
                for (int i = 0; i < shape.GetLength(0); i++)
                {
                    for (int j = 0; j < shape.GetLength(1); j++)
                    {
                        if (shape[i, j] == 1)
                        {
                            Console.ForegroundColor = tempwarna;
                            Console.SetCursorPosition((((10 - shape.GetLength(1)) / 2) + j) * 2 + 20, i + 5);
                            Console.Write(sqr);
                        }
                    }
                }
                Console.ResetColor();
            }
            
            public void Spawn()
            {
                for (int i = 0; i < shape.GetLength(0); i++)
                {
                    for (int j = 0; j < shape.GetLength(1); j++)
                    {
                        if (shape[i, j] == 1)
                        {
                            location.Add(new int[] { i, (10 - shape.GetLength(1)) / 2 + j });
                        }
                    }
                }
                Update();
            }
            public void Update()
            {
                for (int i = 0; i < 23; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        Program.grid[i, j] = 0;
                    }
                }

                if (akses())
                {
                    bool flag = false;
                    for (int i = 0; i < 4; i++)
                    {
                        if (Program.runtuh[location[i][0], location[i][1]] == 1 && location[i][0] >= 1)
                        {
                            flag = true;
                            break;
                        }
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        if (runtuh[location[i][0], location[i][1]] == 0)
                        {

                            if (flag)
                            {
                                int koorx = 0;
                                if (location[i][0] == 1 && runtuh[location[i][0], location[i][1]] == 1) koorx = location[i][0];
                                else koorx = location[i][0] - 1;
                                if (koorx <= 0) koorx = 0;
                                Program.grid[koorx, location[i][1]] = 1;
                                Program.warna[koorx, location[i][1]] = tempwarna;

                            }
                            else
                            {
                                Program.warna[location[i][0], location[i][1]] = tempwarna;
                                Program.grid[location[i][0], location[i][1]] = 1;
                            }
                        }
                    }
                    Program.Draw(); 
                }
            }

            public void Drop()
            {

                if (cekbawah())
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Program.runtuh[location[i][0], location[i][1]] = 1;
                        Program.warna[location[i][0], location[i][1]] = tempwarna;
                    }
                    Program.isDropped = true;
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        location[i][0] += 1;
                    }
                    Update();
                }
            }

            public void Rotate()
            {
                List<int[]> templocation = new List<int[]>();
                for (int i = 0; i < shape.GetLength(0); i++)
                {
                    for (int j = 0; j < shape.GetLength(1); j++)
                    {
                        if (shape[i, j] == 1)
                        {
                            templocation.Add(new int[] { i, (10 - shape.GetLength(1)) / 2 + j });
                        }
                    }
                }
    
                if (shape == bentuktetris[3])
                {

                    for (int i = 0; i < location.Count; i++)
                    {
                        templocation[i] = TransformMatrix(location[i], location[3]);
                    }
                }
                else if (shape == bentuktetris[1]) return;
                else
                {
                    for (int i = 0; i < location.Count; i++)
                    {
                        templocation[i] = TransformMatrix(location[i], location[2]);
                    }
                }
                for (int i = 0; bataskiri(templocation) != false || bataskanan(templocation) != false || batasbawah(templocation) != false; i++)
                {
                    if (bataskiri(templocation) == true)
                    {
                        for (int j = 0; j < location.Count; j++)
                        {
                            templocation[j][1] += 1;
                        }
                    }
                    if (bataskanan(templocation) == true)
                    {
                        for (int j = 0; j < location.Count; j++)
                        {
                            templocation[j][1] -= 1;
                        }
                    }
                    if (batasbawah(templocation) == true)
                    {
                        for (int j = 0; j < location.Count; j++)
                        {
                            templocation[j][0] -= 1;
                        }
                    }
                    if (i == 3)
                    {
                        return;
                    }
                }

                location = templocation;
            }

            public int[] TransformMatrix(int[] coord, int[] sumbu)
            {

                int[] maincoord = { coord[0] - sumbu[0], coord[1] - sumbu[1] };
                maincoord = new int[] { maincoord[1], -maincoord[0] };
                return new int[] { maincoord[0] + sumbu[0], maincoord[1] + sumbu[1] };
            }

            public bool cekbawah()
            {
                for (int i = 0; i < 4; i++)
                {
                    if (location[i][0] + 1 >= 23)
                        return true;
                    if (location[i][0] + 1 < 23)
                    {
                        if (Program.runtuh[location[i][0] + 1, location[i][1]] == 1)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            public bool akses()
            {
                for (int i = 3; i < 7; i++)
                {
                    if (Program.runtuh[0, i] == 1)
                    {
                        return false;
                    }
                }
                return true;
            }
            public bool cekkanan()
            {
                for (int i = 0; i < 4; i++)
                {
                    if (location[i][1] == 9)
                    {
                        return true;
                    }
                    else if (Program.runtuh[location[i][0], location[i][1] + 1] == 1)
                    {
                        return true;
                    }
                }
                return false;
            }
            public bool cekkiri()
            {
                for (int i = 0; i < 4; i++)
                {
                    if (location[i][1] == 0)
                    {
                        return true;
                    }
                    else if (Program.runtuh[location[i][0], location[i][1] - 1] == 1)
                    {
                        return true;
                    }
                }
                return false;
            }

            public bool? bataskanan(List<int[]> location)
            {
                List<int> xcoords = new List<int>();
                for (int i = 0; i < 4; i++)
                {
                    xcoords.Add(location[i][1]);
                    if (location[i][1] > 9)
                    {
                        return true;
                    }
                    if (location[i][1] < 0)
                    {
                        return false;
                    }
                    if (location[i][0] >= 23)
                        return null;
                    if (location[i][0] < 0)
                        return null;
                }
                for (int i = 0; i < 4; i++)
                {
                    if (xcoords.Max() - xcoords.Min() == 3)
                    {
                        if (xcoords.Max() == location[i][1] | xcoords.Max() - 1 == location[i][1])
                        {
                            if (Program.runtuh[location[i][0], location[i][1]] == 1)
                            {
                                return true;
                            }
                        }

                    }
                    else
                    {
                        if (xcoords.Max() == location[i][1])
                        {
                            if (Program.runtuh[location[i][0], location[i][1]] == 1)
                            {
                                return true;
                            }
                        }
                    }

                }
                return false;

            }

            public bool? bataskiri(List<int[]> location)
            {
                List<int> xcoords = new List<int>();
                for (int i = 0; i < 4; i++)
                {
                    xcoords.Add(location[i][1]);
                    if (location[i][1] < 0)
                    {
                        return true;
                    }
                    if (location[i][1] > 9)
                    {
                        return false;
                    }
                    if (location[i][0] >= 23)
                        return null;
                    if (location[i][0] < 0)
                        return null;

                }
                for (int i = 0; i < 4; i++)
                {
                    if (xcoords.Max() - xcoords.Min() == 3)
                    {
                        if (xcoords.Min() == location[i][1] | xcoords.Min() + 1 == location[i][1])
                        {
                            if (Program.runtuh[location[i][0], location[i][1]] == 1)
                            {
                                return true;
                            }
                        }

                    }
                    else
                    {
                        if (xcoords.Min() == location[i][1])
                        {
                            if (Program.runtuh[location[i][0], location[i][1]] == 1)
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            
            public bool? batasbawah(List<int[]> location)
            {
                List<int> ycoords = new List<int>();
                for (int i = 0; i < 4; i++)
                {
                    ycoords.Add(location[i][0]);
                    if (location[i][0] >= 23)
                        return true;
                    if (location[i][0] < 0)
                        return null;
                    if (location[i][1] < 0)
                    {
                        return null;
                    }
                    if (location[i][1] > 9)
                    {
                        return null;
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    if (ycoords.Max() - ycoords.Min() == 3)
                    {
                        if (ycoords.Max() == location[i][0] || ycoords.Max() - 1 == location[i][0])
                        {
                            if (Program.runtuh[location[i][0], location[i][1]] == 1)
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (ycoords.Max() == location[i][0])
                        {
                            if (Program.runtuh[location[i][0], location[i][1]] == 1)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
        }

        public static void Draw()
        {
            for (int i = 0; i < 23; ++i)
            {
                for (int j = 0; j < 10; j++)
                {   
                    Console.SetCursorPosition(1 + 2 * j, i);
                    if (grid[i, j] == 1 || runtuh[i, j] == 1)                                     
                    {
                        Console.ForegroundColor = warna[i, j];
                        Console.Write(sqr);
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }

            }
            Console.ResetColor();
        }

        public static void bingkai()
        {
            for (int i = 0; i < 23; ++i)
            {
                Console.SetCursorPosition(0, i);
                Console.Write("|");
                Console.SetCursorPosition(20, i);
                Console.Write("|");
            }
            Console.SetCursorPosition(0, 23);
            for (int i = 0; i <= 20; i++)
            {
                Console.Write(sqr);
            }

        }
        public static void ubah()
        {
            while (true)
            {   
                waktuturun = (int)dropTimer.ElapsedMilliseconds;
                if (waktuturun > dropRate)
                {
                    dropTimer.Restart();
                    t.Drop();
                }
                if (isDropped == true)
                {
                    t = new tetris();
                    t.Spawn();
                    isDropped = false;
                }
                for (int j = 0; j < 10; j++)
                {
                    if (runtuh[0, j] == 1)
                        return;
                }

                Input();
                ClearBlock();
            }
        }
        public static void Input()
        {
            ConsoleKeyInfo tombol = new ConsoleKeyInfo();
            Console.SetCursorPosition(30, 20);
            if (Console.KeyAvailable)
            {
                tombol = Console.ReadKey();
                isKeyPressed = true;
            }
            else
                isKeyPressed = false;

            if (tombol.Key == ConsoleKey.LeftArrow && !t.cekkiri() && isKeyPressed)
            {
                for (int i = 0; i < 4; i++)
                {
                    t.location[i][1] -= 1;
                }
                t.Update();
            }
            else if (tombol.Key == ConsoleKey.RightArrow && !t.cekkanan() && isKeyPressed)
            {
                for (int i = 0; i < 4; i++)
                {
                    t.location[i][1] += 1;
                }
                t.Update();
            }
            if (tombol.Key == ConsoleKey.DownArrow && isKeyPressed)
            {
                t.Drop();
            }

            if (tombol.Key == ConsoleKey.Spacebar && isKeyPressed)
            {
                t.Rotate();
                t.Update();
            }
        }
        public static void ClearBlock()
        {
            int combo = 0;
            for (int i = 0; i < 23; i++)
            {
                int j;
                for (j = 0; j < 10; j++)
                {
                    if (runtuh[i, j] == 0)
                        break;
                }
                if (j == 10)
                {
                    linesCleared++;
                    combo++;
                    for (j = 0; j < 10; j++)
                    {
                        runtuh[i, j] = 0;
                    }
                    int[,] lokasibaru = new int[23, 10];
                    for (int k = 1; k < i; k++)
                    {
                        for (int l = 0; l < 10; l++)
                        {   
                            lokasibaru[k + 1, l] = runtuh[k, l];
                        }
                    }
                    for (int k = 1; k < i; k++)
                    {
                        for (int l = 0; l < 10; l++)
                        {
                            runtuh[k, l] = 0;
                        }
                    }
                    for (int k = 0; k < 23; k++)
                        for (int l = 0; l < 10; l++)
                            if (lokasibaru[k, l] == 1)
                                runtuh[k, l] = 1;
                    Draw();
                }
            }

            if (combo == 1)
                score += 40 * level;
            else if (combo == 2)
                score += 100 * level;
            else if (combo == 3)
                score += 300 * level;
            else if (combo > 3)
                score += 300 * combo * level;

            dropRate = 300 - 25 * level;
            if (linesCleared < 5)
            {
                level = 1;
                if (linesCleared == 4) dropRate -= 20;
            }
            else if (linesCleared < 10)
            {
                level = 2;
                if (linesCleared == 9) dropRate -= 20;
            }
            else if (linesCleared < 15)
            {
                level = 3;
                if (linesCleared == 14) dropRate -= 20;
            }
            else if (linesCleared < 25)
            {
                level = 4;
                if (linesCleared == 24) dropRate -= 20;
            }
            else if (linesCleared < 35)
            {
                level = 5;
                if (linesCleared == 34) dropRate -= 20;
            }
            else if (linesCleared < 50)
            {
                level = 6;
                if (linesCleared == 49) dropRate -= 20;
            }
            else if (linesCleared < 70)
            {
                level = 7;
                if (linesCleared == 69) dropRate -= 70;
            }
            else if (linesCleared < 90)
            {
                level = 8;
                if (linesCleared == 89) dropRate -= 20;
            }
            else if (linesCleared < 110)
            {
                level = 9;
                if (linesCleared == 109) dropRate -= 20;
            }
            else if (linesCleared < 150)
            {
                level = 10;
                if (linesCleared == 149) dropRate -= 20;
            }

            if (combo > 0)
            {
                Console.SetCursorPosition(25, 0);
                Console.WriteLine("Score " + score);
                Console.SetCursorPosition(25, 1);
                Console.WriteLine("LinesCleared " + linesCleared);
                Console.SetCursorPosition(25, 2);
                Console.WriteLine("Level " + level);
            }
        }

        static void Main()
        {
            bingkai();
            Console.CursorVisible = false;
            Console.SetCursorPosition(2, 5);
            Console.WriteLine("Tekan untuk Mulai");
            Console.ReadKey();
            Console.SetCursorPosition(2, 5);
            for (int i = 0; i < 23; i++) Console.Write("  ");
            Console.SetCursorPosition(25, 0);
            Console.WriteLine("Score " + score);
            Console.SetCursorPosition(25, 1);
            Console.WriteLine("LinesCleared " + linesCleared);
            Console.SetCursorPosition(25, 2);
            Console.WriteLine("Level " + level);
            Console.SetCursorPosition(25, 3);
            t = new tetris();
            dropTimer.Start();
            t.Spawn();
            ubah();

            Console.SetCursorPosition(25, 7);
            Console.WriteLine("Game Over");
            Console.SetCursorPosition(25, 8);
            Console.WriteLine("mau mengulang ? (Y/N)");
            Console.ResetColor();
            Console.SetCursorPosition(25, 9);

            string cmd = Console.ReadLine();


            if (cmd == "y" || cmd == "Y")
            {
                int[,] grid = new int[23, 10];
                runtuh = new int[23, 10];
                dropTimer = new Stopwatch();
                dropRate = 300;
                isDropped = false;
                isKeyPressed = false;
                linesCleared = 0;
                score = 0;
                level = 1;
                Console.Clear();
                Main();
            }
            else return;

        }
    }
}