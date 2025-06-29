using HtmlAgilityPack;
using System.Diagnostics;
using System.Reflection;
using TextToAsciiArt;

namespace WebDataMining
{
    public static class Utils
    {
        public static void Introducao(string versao)
        {
            BemVindo(versao);
            Carregando();
        }

        public static void BemVindo(string versao)
        {
            IArtWriter writer = new ArtWriter();
            ArtSetting settings = new ArtSetting();
            settings.Text = "/";

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkCyan;

            Console.WriteLine("");
            writer.WriteConsole("WEB DATA", settings);
            Console.WriteLine("");
            writer.WriteConsole("MINING", settings);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"\n\n VERSÃO: {versao}\n");
        }

        public static void Carregando()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write(" Carregando: [");

            Thread.Sleep(500);
            Console.Write("//");
            Thread.Sleep(500);
            Console.Write("//");

            Thread.Sleep(500);
            Console.Write("//");
            Thread.Sleep(500);
            Console.Write("//");

            Thread.Sleep(500);
            Console.Write("//");
            Thread.Sleep(500);
            Console.Write("//");

            Console.Write("] 100%");
            Thread.Sleep(1000);
        }

        public static void Adeus(string versao)
        {
            BemVindo(versao);

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(" ATÉ A PRÓXIMA !!!");
            Console.ForegroundColor = ConsoleColor.Gray;
            Thread.Sleep(2000);
        }

        public static void Topo(string versao)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("---------------------------------------------------------");
            Console.Write("| ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("WEB DATA MINING");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(" | ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("BY GLERYSTON MATOS");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(" | ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"VERSÃO {versao}");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(" |\n");
            Console.WriteLine("---------------------------------------------------------");
        }

        public static string Confirmacao(string pergunta)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"{pergunta} (S/N): ");

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            string resposta = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            return resposta.ToUpper();
        }

        public static string Pergunta(string pergunta)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"{pergunta} ");

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            string resposta = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            return resposta;
        }

        public static HashSet<string> ObterLinksDeImagens(string html)
        {
            HashSet<string> listaSrc = new HashSet<string>();
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            HtmlNodeCollection imagens = htmlDoc.DocumentNode.SelectNodes("//img[@src]");
            if (imagens != null)
            {
                foreach (HtmlNode img in imagens)
                {
                    string src = img.GetAttributeValue("src", "");
                    if ((!string.IsNullOrWhiteSpace(src)) && (!src.Contains("data:image")) && (!src.Contains("avatar")))
                        listaSrc.Add(src.Trim());
                }
            }

            HtmlNodeCollection dataImagens = htmlDoc.DocumentNode.SelectNodes("//img[@data-src]");
            if (dataImagens != null)
            {
                foreach (HtmlNode img in dataImagens)
                {
                    string src = img.GetAttributeValue("data-src", "");
                    if ((!string.IsNullOrWhiteSpace(src)) && (!src.Contains("data:image")) && (!src.Contains("avatar")))
                        listaSrc.Add(src.Trim());
                }
            }

            return listaSrc;
        }

        public static void AbrirDiretorioDownloadArquivos()
        {
            string caminhoExe = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string pastaManga = $"{caminhoExe}\\Download\\";

            Directory.CreateDirectory(pastaManga);
            Process.Start("explorer.exe", pastaManga);
        }
    }
}