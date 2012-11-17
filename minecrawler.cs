using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;

namespace MineCrawler
{
    public static class ExtensionMethods
    {
        public static string FixHTMLChars(this string S)
        {
            string CharCode;
            while (S.Extract("&#", out CharCode, ";"))
            {
                char ResultChar = (char)int.Parse(CharCode);
                string ResultString = ResultChar + "";
                S = S.Replace("&#" + CharCode + ";", ResultString);
            }
            return S;
        }

        public static bool ExtractInt(this string Source, string
Token1, out int Value, string Token2)
        {
            string Substring;
            Value = 0;
            return (Source.Extract(Token1, out Substring, Token2) &&
int.TryParse(Substring, out Value));
        }

        public static bool Extract(this string Source, string Token1,
out string Substring, string Token2)
        {
            int Pos = 0;
            return Source.Extract(Token1, out Substring, Token2, ref Pos);
        }

        public static bool Extract(this string Source, string Token1,
out string Substring, string Token2, ref int Pos)
        {
            int Token1Start = Source.IndexOf(Token1, Pos);
            if (Token1Start != -1)
            {
                int Token1End = Token1Start + Token1.Length;
                int Token2Start = Source.IndexOf(Token2, Token1End);
                if (Token2Start != -1)
                {
                    int Token2End = Token2Start + Token2.Length;
                    Substring = Source.Substring(Token1End,
Token2Start - Token1End);
                    Pos = Token2End;
                    return true;
                }
            }

            Substring = "";
            return false;
        }

        public static bool Extract(this string Source, string Token1,
out string Substring1, string Token2, out string Substring2, string
Token3)
        {
            int Pos = 0;
            return Source.Extract(Token1, out Substring1, Token2, out
Substring2, Token3, ref Pos);
        }

        public static bool Extract(this string Source, string Token1,
out string Substring1, string Token2, out string Substring2, string
Token3, ref int Pos)
        {
            int Token1Start = Source.IndexOf(Token1, Pos);
            if (Token1Start != -1)
            {
                int Token1End = Token1Start + Token1.Length;
                int Token2Start = Source.IndexOf(Token2, Token1End);
                if (Token2Start != -1)
                {
                    int Token2End = Token2Start + Token2.Length;
                    int Token3Start = Source.IndexOf(Token3, Token2End);
                    if (Token3Start != -1)
                    {
                        int Token3End = Token3Start + Token3.Length;
                        Substring1 = Source.Substring(Token1End,
Token2Start - Token1End);
                        Substring2 = Source.Substring(Token2End,
Token3Start - Token2End);
                        Pos = Token3End;
                        return true;
                    }
                }
            }

            Substring1 = "";
            Substring2 = "";
            return false;
        }

        public static bool Extract(this string Source, string Token1,
out string Substring1, string Token2, out string Substring2, string
Token3, out string Substring3, string Token4)
        {
            int Pos = 0;
            return Source.Extract(Token1, out Substring1, Token2, out
Substring2, Token3, out Substring3, Token4, ref Pos);
        }

        public static bool Extract(this string Source, string Token1,
out string Substring1, string Token2, out string Substring2, string
Token3, out string Substring3, string Token4, ref int Pos)
        {
            int Token1Start = Source.IndexOf(Token1, Pos);
            if (Token1Start != -1)
            {
                int Token1End = Token1Start + Token1.Length;
                int Token2Start = Source.IndexOf(Token2, Token1End);
                if (Token2Start != -1)
                {
                    int Token2End = Token2Start + Token2.Length;
                    int Token3Start = Source.IndexOf(Token3, Token2End);
                    if (Token3Start != -1)
                    {
                        int Token3End = Token3Start + Token3.Length;
                        int Token4Start = Source.IndexOf(Token4, Token3End);
                        if (Token4Start != -1)
                        {
                            int Token4End = Token4Start + Token4.Length;
                            Substring1 = Source.Substring(Token1End,
Token2Start - Token1End);
                            Substring2 = Source.Substring(Token2End,
Token3Start - Token2End);
                            Substring3 = Source.Substring(Token3End,
Token4Start - Token3End);
                            Pos = Token4End;
                            return true;
                        }
                    }
                }
            }

            Substring1 = "";
            Substring2 = "";
            Substring3 = "";
            return false;
        }

        public static string QuotedString(this string S)
        {
            return '"' + S + '"';
        }

        public static string After(this string S, string Token)
        {
            int P = S.IndexOf(Token);
            if (P == -1)
                return "";
            else
                return S.Remove(0, P + Token.Length);
        }

    }

    public static class CSV
    {
        public static string Encode(string CellValue)
        {
            if (CellValue == "")
                return "";

            const string DQ = @"""";

            if (CellValue.IndexOf(DQ) != -1)
                return DQ + CellValue.Replace(DQ, DQ + DQ).Trim() + DQ;
            else
                if (CellValue.IndexOf(",") != -1)
                    return DQ + CellValue.Trim() + DQ;
                else
                    return CellValue.Trim();
        }

        public static string Encode(string[] CellValues)
        {
            string Result = "";
            for (int i = 0; i < CellValues.Length; i++)
            {
                if (i != 0)
                    Result += ',';
                Result += Encode(CellValues[i]);
            }
            return Result;
        }

        public static string Encode2(string[] CellValues)
        {
            string Result = "";
            for (int i = 0; i < CellValues.Length; i++)
            {
                if (i != 0)
                    Result += ',';
                Result += CellValues[i].QuotedString();
            }
            return Result;
        }
    }

    public struct Post
    {
        public int ID;
        public string URL;
        public string Title;
        public string Author;
        public DateTime Date;
        public int ReplyCount;
        public int ViewCount;

        public string ToCSVRecord()
        {
            return CSV.Encode2(new string[] { ID.ToString(), Title,
Author, Date.ToString("yyyy-MM-dd"), ReplyCount.ToString(),
ViewCount.ToString(), URL });
        }

    }

    public struct PostSorter : IComparer<Post>
    {
        public int Compare(Post A, Post B)
        {
            return A.ID - B.ID;
        }
    }

    public static class Program
    {

        public static string GetDesktopPath()
        {
            string P =
Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (P[P.Length - 1] != Path.DirectorySeparatorChar)
                P += Path.DirectorySeparatorChar;
            return P;
        }

        public static HttpStatusCode DownloadToStream(string URL, Stream Stream)
        {
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(URL);
            try
            {
                Request.AllowAutoRedirect = true;
                HttpWebResponse Response =
(HttpWebResponse)Request.GetResponse();
                Stream ResponseStream = Response.GetResponseStream();

                byte[] Buffer = new byte[8096];
                int BytesRead;
                while ((BytesRead = ResponseStream.Read(Buffer, 0,
Buffer.Length)) > 0)
                    Stream.Write(Buffer, 0, BytesRead);

                Response.Close();
                return Response.StatusCode;
            }
            catch (System.Net.WebException e)
            {
                return ((HttpWebResponse)e.Response).StatusCode;
            }
        }

        public static HttpStatusCode DownloadToFile(string URL, string FileName)
        {
            Stream FileStream = new FileStream(FileName, FileMode.Create);
            HttpStatusCode Result = DownloadToStream(URL, FileStream);
            FileStream.Close();
            return Result;
        }

        public static HttpStatusCode DownloadToString(string URL, out
string Content)
        {
            Stream Stream = new MemoryStream();
            HttpStatusCode Result = DownloadToStream(URL, Stream);
            if (Result == HttpStatusCode.OK)
            {
                Stream.Position = 0;
                Encoding UTF8 = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader StreamReader = new StreamReader(Stream, UTF8);
                Content = StreamReader.ReadToEnd();
                StreamReader.Close();
            }
            else
            {
                Content = "";
            }
            Stream.Close();
            return Result;
        }

        public static void ShowProgress(double Fraction)
        {
            if (Fraction < 0)
                Fraction = 0;

            if (Fraction > 1)
                Fraction = 1;

            // assume console is 80 columns wide
            const int ProgressBarSize = 73;
            const int N = 17;

            const char Tick = (char)0x2502;
            const char LeftHalf = (char)0x258C;
            const char RightHalf = (char)0x2590;
            const char BothHalves = (char)0x2588;

            StringBuilder S = new StringBuilder(ProgressBarSize);
            S.Length = ProgressBarSize;
            for (int I = 0; I < 5; I++)
                S[I] = ' ';

            for (int I = 0; I < 5; I++)
                S[I * (N + 1)] = Tick;

            int Count = (int)Math.Truncate(Fraction * 8 * (N + 1) + 0.5);

            int P = 0;

            if (Count > 0)
            {
                S[P] = RightHalf;
                Count--;
                P++;
            }

            while (Count >= 2)
            {
                S[P] = BothHalves;
                Count -= 2;
                P++;
            }

            if (Count == 1)
                S[P] = LeftHalf;

            string Percentage = (100 * Fraction).ToString("0.0") + "%";
            while (Percentage.Length < 6)
                Percentage = " " + Percentage;

            Console.Write("\r" + S.ToString() + Percentage);
        }

        public static void Scrape()
        {
            List<Post> Posts = new List<Post>();
            int PageCount = 1;
            for (int PageNum = 1; PageNum <= PageCount; PageNum++)
            {
                string URL =
"http://www.minecraftforum.net/forum/51-minecraft-mods/page__prune_day__100__sort_by__Z-A__sort_key__start_date__topicfilter__all__st__"
+ ((PageNum - 1) * 30).ToString();
                string HTML;

                if (DownloadToString(URL, out HTML) == HttpStatusCode.OK)
                {
                    if (PageNum == 1)
                    {
                        HTML.ExtractInt("<a href='#'>Page 1 of ", out
PageCount, " ");
                        Posts.Capacity = 30 * PageCount;
                    }

                    string PostContent;
                    int Pos = 0;
                    while (HTML.Extract("<tr itemscope
itemtype=\"http://schema.org/Article\" class='__topic  expandable' ",
out PostContent, "</tr>", ref Pos))
                    {
                        if (PostContent.Contains("<span
class='ipsBadge ipsBadge_green'>Pinned</span>"))
                            continue;

                        string TagContent, DateString;
                        Post P = new Post();

                        if (PostContent.Extract("<h4>", out
TagContent, "</h4>"))
                        {
                            TagContent.ExtractInt("tid-link-", out P.ID, "\"");
                            TagContent.Extract("href=\"", out P.URL, "\"");
                            TagContent.Extract("<span
itemprop=\"name\">", out P.Title, "</span>");
                        }

                        string Authorship;

                        PostContent.ExtractInt("onclick=\"return
ipb.forums.retrieveWhoPosted( " + P.ID.ToString() + " );\">", out
P.ReplyCount, " replies</a>");
                        PostContent.ExtractInt("<li class='views
desc'>", out P.ViewCount, " views</li>");


                        if (PostContent.Extract("Started by ", out
Authorship, ","))
                            P.Author = Authorship.Trim();

                        if (PostContent.Extract("<span
itemprop=\"dateCreated\">", out DateString, "</span>"))
                        {
                            if (DateString == "")
                                P.Date = DateTime.MinValue;
                            else
                                if (DateString.Contains("Today"))
                                    P.Date = DateTime.Today;
                                else
                                    if (DateString.Contains("Yesterday"))
                                        P.Date = DateTime.Today.AddDays(-1.0);
                                    else
                                        P.Date =
DateTime.ParseExact(DateString, "dd MMM yyyy",
System.Globalization.CultureInfo.InvariantCulture);
                        }

                        P.Title = P.Title.FixHTMLChars();

                        Posts.Add(P); // TODO: prevent duplicates
                    }
                }

                ShowProgress((PageNum + 0.0) / PageCount);
            }

            Console.WriteLine();
            Console.WriteLine("Sorting...");
            Posts.Sort(new PostSorter());

            Console.WriteLine("Writing CSV file...");
            StreamWriter Output = new
StreamWriter(Path.Combine(GetDesktopPath(), "minecraft_mods.csv"));
            try
            {
                foreach (Post P in Posts)
                    Output.WriteLine(P.ToCSVRecord());
            }
            finally
            {
                Output.Close();
            }

        }

        static void Main(string[] args)
        {
            Scrape();
        }
    }
}