using HtmlAgilityPack;
using System.Diagnostics;
using System.Reflection;
using TextToAsciiArt;

namespace WebDataMining
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Introducao();
            await Iniciar();
            await Continuar();
            Adeus();
        }

        #region Program

        public static void BemVindo()
        {
            IArtWriter writer = new ArtWriter();

            ArtSetting settings = new ArtSetting();
            settings.Text = "|";

            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("------------------------------------------------------------------------------------\n");
            writer.WriteConsole("WEB", settings);
            Console.WriteLine(" ");
            writer.WriteConsole("DATA", settings);
            Console.WriteLine(" ");
            writer.WriteConsole("MINING", settings);
            Console.WriteLine("\n------------------------------------------------------------------------------------");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("  V 1.0.0.1                                                    BY GLERYSTON MATOS    ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("------------------------------------------------------------------------------------");
        }

        public static void Introducao()
        {
            BemVindo();
            Carregando();
        }

        public static void Carregando()
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Green;

            Console.Write("  Carregando");
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

            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static async Task Iniciar()
        {
            string opcao = Menu();
            Console.ResetColor();

            switch (opcao)
            {
                case "1":
                    await DownloadCapitulosMangas("");
                    break;
            }
        }

        public static string Menu()
        {
            Dictionary<string, string> opcoes = new Dictionary<string, string>
            {
                { "1", "Download capítulos mangas" },
            };

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Selecione uma opção:");

            Console.ForegroundColor = ConsoleColor.Yellow;

            foreach (var opcao in opcoes)
                Console.WriteLine($"{opcao.Key}: {opcao.Value}");

            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\nInforme uma opção: ");
            string codigo = Console.ReadLine();

            while (!opcoes.ContainsKey(codigo))
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Opção selecionada invalida, selecione uma das opções disponíveis:");

                Console.ForegroundColor = ConsoleColor.Yellow;

                foreach (var uf in opcoes)
                    Console.WriteLine($"{uf.Key}: {uf.Value}");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\nInforme uma opção: ");
                codigo = Console.ReadLine();
            }

            return codigo;
        }

        public static async Task Continuar()
        {
            string opcao = Confirmacao("Deseja selecionar outra opção?");

            while (opcao.ToUpper().Equals("S"))
            {
                await Iniciar();
                opcao = Confirmacao("Deseja selecionar outra opção?");
            }
        }

        public static void Adeus()
        {
            BemVindo();
            Console.WriteLine("ATÉ A PRÓXIMA ");
            Console.WriteLine("------------------------------------------------------------------------------------");

            Console.ResetColor();
            Thread.Sleep(2000);
        }

        #endregion

        #region Functions

        public static string Confirmacao(string pergunta)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(pergunta);

            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("(S/N):");

            string resposta = Console.ReadLine();
            return resposta.ToUpper();
        }

        public static string Pergunta(string pergunta)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"\n{pergunta}");

            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("R: ");
            return Console.ReadLine();
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
                    if (!string.IsNullOrWhiteSpace(src))
                        listaSrc.Add(src);
                }
            }

            return listaSrc;
        }

        #endregion

        #region DownloadCapitulosMangas

        public static async Task<string> DownloadCapitulosMangas(string manga)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Download de capítulos de mangas");

            if (string.IsNullOrEmpty(manga))
                manga = Pergunta("Informe o nome do manga:");

            string capitulo = Pergunta("Informe o capítulo do manga:");

            string html = Pergunta("Informe o elemento com os links de download das páginas:");
            IList<string> links = ObterLinksDeImagens(html);

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Download de capítulos de mangas");

            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"\n({manga} Cap: {capitulo})");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"\nBaixando");

            string caminhoExe = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string pastaDownload = $"{caminhoExe}\\Download";
            string pastaManga = $"{pastaDownload}\\{manga} {capitulo}\\";

            Directory.CreateDirectory(pastaManga);

            int pagina = 0;
            foreach (string link in links)
            {
                pagina += 1;
                Console.Write(".");
                string extensao = link.Split(".")[3];

                HttpClient httpClient = new HttpClient();
                byte[] bytes = await httpClient.GetByteArrayAsync(link);
                await File.WriteAllBytesAsync($"{pastaManga}\\{pagina.ToString()}.{extensao}", bytes);
            }

            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(" ");

            string abrirDiretorio = Confirmacao("Deseja abrir o diretório de download?");
            if (abrirDiretorio.Equals("S"))
                Process.Start("explorer.exe", $"{pastaDownload}\\");

            string opcao = Confirmacao("Deseja baixar mais capítulos?");

            while (opcao.ToUpper().Equals("S"))
                opcao = await DownloadCapitulosMangas(manga);

            return opcao;
        }

        #endregion
    }
}