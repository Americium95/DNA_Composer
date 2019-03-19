using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using static ConsoleApp1.WaveFormatChunk;

namespace ConsoleApp1
{
    class Program
    {
        static string FileName = "DNA1";
        static public int Age = 0;

        static int DnaCount = 40;/*표본크기*/

        static int DnaLength = 400;/*DNA길이*/

        static int par = 20;/*변이율*/
        static int MaxNum_2 = 0;/**/

        static float Public_MaxPoint = -90000;/*최다 득점*/

        static bool Active = true;

        static string MaxDna;

        static public float[] PriDnaPoint = new float[DnaCount];
        static public String[] PriDna = new String[DnaCount];
        static public String[] NewDna = new String[DnaCount];
        static public Thread EVO_Thread;
        static Stopwatch sw = new Stopwatch();

        static void Main(string[] args)
        {

            while (true)
            {
                string str = Console.ReadLine();
                switch (str)
                {

                    case "시작":

                        sw.Start();

                        Console.WriteLine("Writeing_Start");
                        EVO_Thread = new Thread(MakePriDNA, 1024 * 1024 * 1024);
                        EVO_Thread.Start();
                        break;

                    case "정지":
                        Active = false;
                        
                        Thread v = new Thread(ViewGrap);
                        v.Start();
                        v.Join();
                        Console.WriteLine("결과 출력중");


                        break;

                }
            }

        }

        /*유전자 생성*/
        static void MakePriDNA()
        {
            if (!File.Exists("D:\\" + FileName + ".txt"))
            {
                for (int i = 0; i < DnaCount; i++)
                {
                    char[] str = new char[DnaLength];
                    Console.Write(i + ": ");
                    for (int k = 0; k < DnaLength; k++)
                    {
                        int code = (int)(new Random().Next(1, 24));
                        if (k % 6 == 0)
                            code = 0;
                        Console.Write("[" + code + "]");
                        str[k] = Convert.ToChar(code);
                    }
                    Console.WriteLine();
                    PriDna[i] = new string(str);
                }

                Console.WriteLine("\n무작위 유전자 " + DnaCount + "개 구성 완료");
            }
            else
            {
                Console.WriteLine("유전자 리로드");
                for (int i = 0; i < DnaCount; i++)
                {
                    PriDna[i] = File.ReadAllText("D:\\" + FileName + ".txt");
                }
            }
            Check();

        }

        static int DeltaCodonPoint(List<int> DeltaCodon)
        {
            int PublicPoint = 1000000;
            string[] FileNode = Directory.GetFiles("D:\\MusicSample");
            for (int i_0=0;i_0< Directory.GetFiles("D:\\MusicSample").Length;i_0++)
            {
                int point = 0;
                string[] SampleNode = File.ReadAllText(FileNode[i_0]).Split('0');

                for (int i_1=0;i_1< SampleNode.Length-1;i_1++)
                {
                    //Console.WriteLine(SampleNode[i_0][i_1].ToString());
                    point+=Math.Abs((int.Parse(SampleNode[i_0][i_1].ToString())- int.Parse(SampleNode[i_0][i_1+1].ToString()))-DeltaCodon[i_1%DeltaCodon.Count ]);
                }
                if(point<PublicPoint)
                {
                    PublicPoint = point;
                }
            }
            return PublicPoint;
        }

        /*포인트 부여*/
        static void Check()
        {
            if (Active == true)
            {
                float MaxPoint = -999999;
                int MaxNum = 0;

                float Public_Instantaneous_rate_of_change_Max = 0;
                float Public_Analog_Checker = 0;
                float Public_Instantaneous_rate_of_change_directional = 0;
                float Public_DoubleCount = 0;

                for (int i = 0; i < DnaCount; i++)
                {
                    List<int> codon = new List<int>();
                    List<int> daltacodon = new List<int>();
                    PriDnaPoint[i] = 0;
                    float Instantaneous_rate_of_change_Max= 0;
                    float Analog_Checker = 0;
                    float Instantaneous_rate_of_change_directional = 0;
                    float DoubleCount = 0;
                    List<List<int>> LastCodon = new List<List<int>>();



                    for (int k = 0; k < DnaLength; k++)
                    {

                        float LastAverage = 0;
                        
                        codon.Add(PriDna[i][k]);
                        if(k < DnaLength-1)
                            daltacodon.Add(PriDna[i][k]-PriDna[i][k+1]);
                        if (PriDna[i][k] == 0)
                        {
                                
                            
                           /*위에*/
                            PriDnaPoint[i] -= DeltaCodonPoint(daltacodon)*100;
                            codon = new List<int>();
                            daltacodon = new List<int>();

                        }

                    }
                    if (MaxPoint < PriDnaPoint[i])
                    {
                        MaxPoint = PriDnaPoint[i];
                        MaxNum_2 = MaxNum;
                        MaxNum = i;

                        Public_Instantaneous_rate_of_change_Max = Instantaneous_rate_of_change_Max;
                        Public_Analog_Checker = Analog_Checker;
                        Public_Instantaneous_rate_of_change_directional = Instantaneous_rate_of_change_directional;
                        Public_DoubleCount = DoubleCount;
                    }
                    //Console.WriteLine(PriDnaPoint[i]);

                }
                if (Public_MaxPoint < MaxPoint)
                {
                    Public_MaxPoint = MaxPoint;

                    File.WriteAllText("D:\\" + FileName + ".txt", PriDna[MaxNum]);
                    Console.WriteLine("제" + Age + "세대 최고득점 유전자: " + MaxNum + "번 유전자 " + MaxPoint + "포인트" + "  2등:" + MaxNum_2 + "\n경과시간:" + sw.Elapsed.ToString());
                    Console.WriteLine("불연속" + Public_Instantaneous_rate_of_change_Max);
                    Console.WriteLine("유사도" + Public_Analog_Checker);
                    Console.WriteLine("방향성" + Public_Instantaneous_rate_of_change_directional);
                    Console.WriteLine("요거" + Public_DoubleCount);
                }

                MaxDna = PriDna[MaxNum];
                //Console.ReadLine();
                Transition(MaxNum);
            }
        }

        /*유전자 풀*/
        static void Transition(int Num)
        {
            NewDna[0] = PriDna[Num];
            for (int i = 1; i < DnaCount; i++)
            {
                char[] Rna = new Char[DnaLength];

                Rna = Supply(Num, MaxNum_2);

                for (int k = 0; k < DnaLength * par / 100; k++)
                {
                    int count = new Random().Next(0, DnaLength - 1);
                    int code = (int)(new Random().Next(1, 24));
                    if (count % 6 == 0)
                        code = 0;
                    Rna[count] = Convert.ToChar(code);
                }
                NewDna[i] = new string(Rna);

            }
            PriDna = NewDna;
            Age += 1;
            Check();
        }

        static void ViewGrap()/*시각화후 저장*/
        {
            Console.WriteLine("코돈 복호화중");
            Thread.Sleep(100);
            char[] str = MaxDna.ToCharArray();

            for (int i = 0; i < str.Length - 1; i += 2)
            {
                for (int f = 0; f < Convert.ToChar(str[i]); f++)
                {
                    Console.Write("■");
                }
                Console.Write("(");
                for (int f = 0; f < Convert.ToChar(str[i + 1]); f++)
                {
                    Console.Write("■");
                }
                Console.Write(")    ");
                

                Console.WriteLine("");

            }
            WaveGenerator();
            Console.WriteLine("계속_Y/N");
            if (Console.ReadKey(true).Key == ConsoleKey.Y)
            {
                Active = true;
                EVO_Thread = new Thread(Check);
                EVO_Thread.Start();
            }

        }

        static char[] Supply(int Num_1, int Num_2)/*유전자 교차*/
        {
            char[] code = new Char[DnaLength];
            for (int i = 0; i < DnaLength; i++)
            {
                if (new Random().Next(0, 1) == 0)
                    code[i] = PriDna[Num_1][i];
                else
                {
                    code[i] = PriDna[Num_2][i];
                }
            }

            return code;
        }


    public class Instantaneous_rate_of_change
    {
        public static float Max(int[] Ary)/*최대 순간변화율*/
        {
            int Max = -1;
            for (int i = 0; i < Ary.Length - 1; i++)
            {
                int num = Math.Abs(Ary[i] - Ary[i + 1]);
                if (Max < num)
                {
                    Max = num;
                }
            }
            return Max;
        }

        public static float Min(int[] Ary)/*최소 순간변화율*/
        {
            int Min = -1;
            for (int i = 0; i < Ary.Length - 1; i++)
            {
                int num = Math.Abs(Ary[i] - Ary[i+1]);
                if (Min > num)
                {
                    Min = num;
                }
            }
            return Min;
        }

        public static float Average(int[] Ary)/*평균 순간변화율*/
        {
            int Num = -1;
            for (int i = 0; i < Ary.Length - 1; i++)
            {
                Num += Math.Abs(Ary[i] - Ary[i+1]);

            }
            Num /= Ary.Length;
            return Num;
        }

        public static float Directional(List<int> Ary)/*방향성*/
        {
            float Num = 0;
            for (int i = 0; i < Ary.Count - 1; i++)
            {
                Num += (float)(((float)Ary[i] - Ary[i + 1])/ Math.Max(Math.Abs(Ary[i] - Ary[i + 1]), 1));
               
            }
            return Math.Abs(Num)/ Ary.Count;
        }
    }

    public class ListMath
    {
        public static float Average(int[] Ary)
        {
            float Sum = 0;
            for(int i=0;i<Ary.Length;i++)
            {
                Sum += Ary[i];
            }
            return Sum / Ary.Length;
        }

        public static float Analog_Checker(List<List<int>> Ary_0, int[] Ary_1)
        {
            float sum_s = 0;
            float MP = 0;
            for (int i_0 = 0; i_0 < Ary_0.Count-1; i_0++)
            {
                float sum = 0;
                for (int i_1 = 0; i_1 < Ary_1.Length - 1; i_1++)
                {
                    
                    int num_0 = Math.Abs(Ary_0[i_0][i_1 % Ary_0[i_0].Count] - Ary_0[i_0][(i_1 + 1) % Ary_0[i_0].Count]);
                    
                    int num_1 = Math.Abs(Ary_1[i_1 % Ary_1.Length] - Ary_1[(i_1 + 1) % Ary_1.Length]);
                    sum += Math.Abs(num_0 - num_1);
                }
                MP = Ary_0[i_0].Count;
                sum_s+=sum / MP;
            }
            return sum_s / Ary_0.Count; ;
        }
    }



        /*파형생성*/

        static WaveHeader header;

        static WaveFormatChunk format;

        static WaveDataChunk data;

        public static void WaveGenerator()
        {


            header = new WaveHeader();

            format = new WaveFormatChunk();

            data = new WaveDataChunk();




            char[] str = MaxDna.ToCharArray();
            uint numSamples = format.dwSamplesPerSec * format.wChannels;




            data.shortArray = new short[((str.Length / 4 + 1) * format.wChannels * numSamples) + (numSamples - 1 + format.wChannels)];



            int amplitude = 37440;

            double freq = 440.0f;








            for (int i_0 = 0; i_0 < str.Length; i_0++)
            {
                Console.WriteLine(((i_0 * format.wChannels * numSamples) + (numSamples - 1 + format.wChannels)) + "/" + (str.Length) * format.wChannels * numSamples);
                for (uint i = 0; i < numSamples - 1; i++)

                {
                    for (int channel = 0; channel < format.wChannels; channel++)

                    {
                        if ((float)Convert.ToInt32(str[i_0]) != 0)
                        {
                            freq = (55 * Math.Pow(2, ((float)Convert.ToInt32(str[i_0])+12 - 1) / 12));
                            double t = (Math.PI * 2 * freq) / (format.dwSamplesPerSec * format.wChannels);
                            double Byte_Data = data.shortArray[((i_0 * format.wChannels * numSamples) / 6) + i + channel];
                            if (Byte_Data == 0)
                                data.shortArray[((i_0 * format.wChannels * numSamples) / 6) + i + channel] = (short)((amplitude * Piano.Piano_0(t, i) + data.shortArray[((i_0 * format.wChannels * numSamples) / 6) + i + channel - 1]) / 2);
                            else
                            {
                                data.shortArray[((i_0 * format.wChannels * numSamples) / 6) + i + channel] = (short)((amplitude * Piano.Piano_0(t, i) + data.shortArray[((i_0 * format.wChannels * numSamples) / 6) + i + channel - 1] + Byte_Data) / 3);
                            }
                            if (i_0 == str.Length)
                            {
                                if (Byte_Data == 0)
                                    data.shortArray[((i_0 * format.wChannels * numSamples) / 6) + i + channel] = (short)((amplitude * Piano.Piano_0(t, i) + (format.wChannels - channel) / format.wChannels) / 2);
                                else
                                {
                                    data.shortArray[((i_0 * format.wChannels * numSamples) / 6) + i + channel] = (short)((amplitude * Piano.Piano_0(t, i) + data.shortArray[((i_0 * format.wChannels * numSamples) / 6) + i + channel - 1] + (format.wChannels - channel) / format.wChannels) / 3);
                                }
                            }
                        }
                    }

                }
            }


            data.dwChunkSize = (uint)(data.shortArray.Length * (format.wBitsPerSample / 8));
            Save("D:\\" + FileName + ".wav");

        }


        /*파일저장*/
        public static void Save(string filePath)

        {


            FileStream fileStream = new FileStream(filePath, FileMode.Create);


            BinaryWriter writer = new BinaryWriter(fileStream);



            // RIFF write

            writer.Write(header.sGroupID.ToCharArray());

            writer.Write(header.dwFileLength);

            writer.Write(header.sRiffType.ToCharArray());



            // FMT write

            writer.Write(format.sChunkID.ToCharArray());

            writer.Write(format.dwChunkSize);

            writer.Write(format.wFormatTag);

            writer.Write(format.wChannels);

            writer.Write(format.dwSamplesPerSec);

            writer.Write(format.dwAvgBytesPerSec);

            writer.Write(format.wBlockAlign);

            writer.Write(format.wBitsPerSample);



            // DATA write

            writer.Write(data.sChunkID.ToCharArray());

            writer.Write(data.dwChunkSize);

            foreach (short dataPoint in data.shortArray)

            {

                writer.Write(dataPoint);

            }

            writer.Seek(4, SeekOrigin.Begin);

            uint filesize = (uint)writer.BaseStream.Length;

            writer.Write(filesize - 8);



            writer.Close();

            fileStream.Close();

        }

    }

    public class Piano
    {
        public static double Piano_0(double t, double i)
        {
            double Sum = 0;
            //for (float n = 0; n < 5; n++)
            {
                Sum = Piano_1(t, i, 1);
            }
            return Sum;
        }

        public static double Piano_1(double t, double i, float n)
        {
            return (double)Math.Sin(2 * t * i) * Math.Sin(t * i);
        }
    }

    public class WaveHeader
    {

        public string sGroupID;

        public uint dwFileLength;

        public string sRiffType;



        public WaveHeader()

        {

            dwFileLength = 0;

            sGroupID = "RIFF";

            sRiffType = "WAVE";

        }

    }

    public class WaveFormatChunk

    {

        public string sChunkID;               /*"fmt "*/

        public uint dwChunkSize;           /*헤더의 크기*/

        public ushort wFormatTag;          /*PCM은 1*/

        public ushort wChannels;           /*채널 수*/

        public uint dwSamplesPerSec;

        public uint dwAvgBytesPerSec;

        public ushort wBlockAlign;

        public ushort wBitsPerSample;



        public WaveFormatChunk()

        {

            sChunkID = "fmt ";

            dwChunkSize = 16;

            wFormatTag = 1;

            wChannels = 2;

            dwSamplesPerSec = 22050;

            wBitsPerSample = 16;

            wBlockAlign = (ushort)(wChannels * (wBitsPerSample / 8));

            dwAvgBytesPerSec = dwSamplesPerSec * wBlockAlign;

        }

        public class WaveDataChunk

        {

            public string sChunkID;       /*"data"*/

            public uint dwChunkSize;    /*data 크기*/

            public short[] shortArray;    /*데이터*/



            public WaveDataChunk()

            {

                shortArray = new short[0];

                dwChunkSize = 0;

                sChunkID = "data";

            }

        }
    }

}

