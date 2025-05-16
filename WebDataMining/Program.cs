namespace WebDataMining
{
    public class Program
    {
        private static string _versao = "1.0.5.0";

        private static Dictionary<string, string> _opcoes = new Dictionary<string, string>
        {
            { "1", "Realizar download de capítulos de mangas (Html)" },
            { "2", "Realizar download de capítulos de mangas (Link base)" },
            { "3", "Realizar download de capítulos de mangas (Link da página)" },
            { "4", "Abrir diretório de download dos arquivos" },
            { "", "" },
            { "0", "Sobre" },
        };

        public static async Task Main(string[] args)
        {
            Utils.Introducao(_versao);
            await Iniciar();
            Utils.Adeus(_versao);
        }

        public static async Task Iniciar()
        {
            string opcao = Menu();

            switch (opcao)
            {
                case "1":
                    await DownloadCapituloMangaHtml.Iniciar(_versao, "", "");
                    break;
                case "2":
                    await DownloadCapituloMangaLinkBase.Iniciar(_versao, "", "");
                    break;
                case "3":
                    await DownloadCapituloMangaLinkPagina.Iniciar(_versao, "", "");
                    break;
                case "4":
                    Utils.AbrirDiretorioDownloadArquivos();
                    break;
                case "0":
                    Utils.BemVindo(_versao);
                    break;
            }

            string confirmar = Utils.Confirmacao("Deseja selecionar outra opção?");

            if (confirmar.ToUpper().Equals("S"))
                await Iniciar();
        }

        public static string Menu()
        {
            Utils.Topo(_versao);
            string codigo = SelecionarOpcao();

            while (!_opcoes.ContainsKey(codigo))
            {
                Utils.Topo(_versao);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nOpção selecionada invalida, selecione uma das opções disponíveis!");

                Console.ForegroundColor = ConsoleColor.Yellow;
                codigo = SelecionarOpcao();
            }

            return codigo;
        }

        public static string SelecionarOpcao()
        {
            Console.WriteLine("\nOpções:");

            foreach (var opcao in _opcoes)
                if (!string.IsNullOrEmpty(opcao.Key))
                    Console.WriteLine($"{opcao.Key} - {opcao.Value}");
                else
                    Console.WriteLine(" ");

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\nInforme uma das opções disponíveis: ");
            string opcaoSelecionada = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            return opcaoSelecionada;
        }
    }
}