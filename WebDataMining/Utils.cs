﻿using HtmlAgilityPack;
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
            settings.Text = "|";

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("------------------------------------------------------------------------------------\n");
            writer.WriteConsole("WEB DATA", settings);
            Console.WriteLine(" ");
            writer.WriteConsole("MINING", settings);
            Console.WriteLine("\n------------------------------------------------------------------------------------");
            Console.WriteLine($" BY GLERYSTON MATOS | VERSÃO {versao}                                               |");
            Console.WriteLine("------------------------------------------------------------------------------------");
        }

        public static void Carregando()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.Write(" Carregando");
            Thread.Sleep(500);
            Console.Write(".");
            Thread.Sleep(500);
            Console.Write(".");
            Thread.Sleep(500);
            Console.Write(".");
            Thread.Sleep(500);
            Console.Write(".");
            Thread.Sleep(500);
            Console.Write(".");
            Thread.Sleep(500);
            Console.Write(".");

            Console.ForegroundColor = ConsoleColor.Yellow;
        }

        public static void Adeus(string versao)
        {
            BemVindo(versao);

            Console.WriteLine(" ATÉ A PRÓXIMA                                                                     |");
            Console.WriteLine("------------------------------------------------------------------------------------");

            Console.ResetColor();
            Thread.Sleep(2000);
        }

        public static void Topo(string versao)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--------------------------------------------------------");
            Console.WriteLine($" WEB DATA MINING | BY GLERYSTON MATOS | VERSÃO {versao} |");
            Console.WriteLine("--------------------------------------------------------");
        }

        public static string Confirmacao(string pergunta)
        {
            Console.ForegroundColor = ConsoleColor.Gray;

            Console.Write($"{pergunta} (S/N): ");
            string resposta = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            return resposta.ToUpper();
        }

        public static string Pergunta(string pergunta)
        {
            Console.ForegroundColor = ConsoleColor.Gray;

            Console.Write($"{pergunta} ");
            string resposta = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            return resposta;
        }

        public static IList<string> ObterLinksDeImagens(string html)
        {
            IList<string> listaSrc = new List<string>();
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