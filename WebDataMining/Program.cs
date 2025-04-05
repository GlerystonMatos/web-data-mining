using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using TextToAsciiArt;

namespace WebDataMining
{
    public class Program
    {
        public static string extensaoDownload = "";
        public static int tentativasDownload = 0;

        public static async Task Main(string[] args)
        {
            Introducao();
            await Iniciar();
            await Continuar();
            Adeus();
        }

        #region Program

        public static void ShowProgram()
        {
            IArtWriter writer = new ArtWriter();

            ArtSetting settings = new ArtSetting();
            settings.Text = "|";

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.Clear();
            Console.WriteLine("------------------------------------------------------------------------------------\n");
            writer.WriteConsole("WEB", settings);
            Console.WriteLine(" ");
            writer.WriteConsole("DATA", settings);
            Console.WriteLine(" ");
            writer.WriteConsole("MINING", settings);
            Console.WriteLine("\n-----------------------------------------------------------------------------------");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("                                                               BY GLERYSTON MATOS    ");
        }

        public static void Introducao()
        {
            ShowProgram();
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
            string opcao = ObterOpcoes();
            Console.ResetColor();

            switch (opcao)
            {
                case "1":
                    await DownloadCapitulosMangas();
                    break;
            }
        }

        public static async Task Continuar()
        {
            string opcao = SelecionarOutraOpcao("Deseja selecionar outra opção?");

            while (opcao.ToUpper().Equals("S"))
            {
                await Iniciar();
                opcao = SelecionarOutraOpcao("Deseja selecionar outra opção?");
            }
        }

        public static void Adeus()
        {
            ShowProgram();
            Console.WriteLine("ATÉ A PRÓXIMA ");

            Console.ResetColor();
            Thread.Sleep(2000);
        }

        #endregion

        #region Functions

        public static string ObterOpcoes()
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

        public static string SelecionarOutraOpcao(string pergunta)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(pergunta);

            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("(S/N):");
            return Console.ReadLine();
        }

        public static string FazerPergunta(string pergunta)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"\n{pergunta}");

            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("R: ");
            return Console.ReadLine();
        }

        public static async Task<byte[]> Download(string link, int pagina, string extensao)
        {
            HttpClient httpClient = new HttpClient();

            try
            {
                if (tentativasDownload > 3)
                    throw new Exception("Numero máximo de tentativas de download excedido!");

                tentativasDownload += 1;
                extensaoDownload = extensao;
                return await httpClient.GetByteArrayAsync($"{link}{pagina}.{extensao}");
            }
            catch 
            {
                if (extensao.Equals("jpg"))
                    return await Download(link, pagina, "png");
                else if (extensao.Equals("png"))
                    return await Download(link, pagina, "webp");
                else if (extensao.Equals("webp"))
                    return await Download(link, pagina, "jpg");
                else
                    return null;
            }
        }

        #endregion

        #region DownloadCapitulosMangas

        public static async Task<string> DownloadCapitulosMangas()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Download de capítulos de mangas");

            string manga = FazerPergunta("Informe o nome do manga:");
            string quantidadeCapitulos = FazerPergunta("Quantos capítulos deseja baixar?");

            string capitulo = "";
            string link = "";
            string quantidadePaginas = "";

            for (int i = 0; i < int.Parse(quantidadeCapitulos); i++)
            {
                link = FazerPergunta("Informe o link de download da última página do capítulo:");

                extensaoDownload = link.Split(".")[3];
                capitulo = link.Split("_")[2];

                quantidadePaginas = link.Split("_")[3];
                quantidadePaginas = quantidadePaginas.Replace($".{extensaoDownload}", "");

                link = link.Replace(link.Split("_")[3], "");

                await FazerDownload(manga, capitulo, link, quantidadePaginas, extensaoDownload);
            }

            string opcao = SelecionarOutraOpcao("Deseja baixar mais capítulos?");

            while (opcao.ToUpper().Equals("S"))
                opcao = await DownloadCapitulosMangas();

            return opcao;
        }

        public static async Task FazerDownload(string manga, string capitulo, string link, string quantidadePaginas, string extensao)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\nBaixando");

            string caminhoExe = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string caminhoDownload = $"{caminhoExe}\\Download\\{manga} {capitulo}\\";

            Directory.CreateDirectory(caminhoDownload);
            for (int pagina = 0; pagina < (int.Parse(quantidadePaginas) + 1); pagina++)
            {
                Console.Write(".");
                
                tentativasDownload = 0;
                byte[] bytes = await Download(link, pagina, extensao);
                
                await File.WriteAllBytesAsync($"{caminhoDownload}\\{pagina.ToString()}.{extensaoDownload}", bytes);
            }

            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(" ");
        }

        #endregion
    }
}