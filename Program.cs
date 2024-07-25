using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CompareList
{
    class Program
    {
        static string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        static void Main(string[] args)
        {
            Console.WriteLine("請輸入增加時間(分):");
            int addTime;
            bool parsedSuccessfully = int.TryParse(Console.ReadLine(), out addTime);

            while (parsedSuccessfully == false)
            {
                Console.WriteLine("請輸入數字!");
                Console.WriteLine("請輸入增加時間(分):");
                parsedSuccessfully = int.TryParse(Console.ReadLine(), out addTime);
            }

            string compareList = Path.Combine(assemblyPath, "CompareList");
            if (!Directory.Exists(compareList))
            {
                Console.WriteLine("目標資料夾不存在!");
                Console.ReadKey();
            }
            else
            {
                //取案號
                string caseNO = new DirectoryInfo(assemblyPath).Name;

                //用cmd產生 附件三
                var psiNpm0 = new ProcessStartInfo
                {
                    FileName = "cmd",
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false
                };
                var pNpmRun0 = Process.Start(psiNpm0);
                pNpmRun0.StandardInput.WriteLine($@"DIR Update\SourceCode\*.* /S –D > 附件三-{caseNO}.txt");
                pNpmRun0.StandardInput.WriteLine("exit");
                pNpmRun0.WaitForExit();

                //用cmd產生 附件四-CompareList.txt
                var psiNpm1 = new ProcessStartInfo
                {
                    FileName = "cmd",
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false
                };
                var pNpmRun1 = Process.Start(psiNpm1);
                pNpmRun1.StandardInput.WriteLine(@"DIR CompareList\*.* /S –D > 附件四-CompareList.txt");
                pNpmRun1.StandardInput.WriteLine("exit");
                pNpmRun1.WaitForExit();

                DirectoryInfo d = new DirectoryInfo(compareList);
                FileInfo[] files = d.GetFiles("*.*"); //Getting files

                //新資料夾
                string newCompareList = Path.Combine(assemblyPath, "CompareListCheck");
                if (!Directory.Exists(newCompareList))
                    Directory.CreateDirectory(newCompareList);

                DateTime oTime = files.OrderBy(o => o.LastWriteTime).Last().LastWriteTime;   //基礎時間
                DateTime nTime = oTime.AddMinutes(addTime);
                Random rnd = new Random();
                //複製至另一資料夾並改變修改時間
                for (int i = 0; i < files.Length; i++)
                {
                    string copyFile = Path.Combine(newCompareList, files[i].Name);
                    File.Copy(files[i].FullName, copyFile, true);
                    nTime = nTime.AddSeconds(rnd.Next(15, 30));  //增加隨機秒數
                    File.SetLastWriteTime(copyFile, nTime);
                    File.SetCreationTime(copyFile, nTime);
                    File.SetLastAccessTime(copyFile, nTime);
                }

                //用cmd產生 附件四-CompareList.txt
                var psiNpm2 = new ProcessStartInfo
                {
                    FileName = "cmd",
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false
                };
                var pNpmRun2 = Process.Start(psiNpm2);
                pNpmRun2.StandardInput.WriteLine(@"DIR CompareListCheck\*.* /S –D > 附件四-CompareListCheck.txt");
                pNpmRun2.StandardInput.WriteLine("exit");
                pNpmRun2.WaitForExit();
            }

        }
    }
}
