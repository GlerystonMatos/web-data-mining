namespace WebDataMining
{
    public class Program
    {
        private static string _versao = "1.0.8.0";

        private static Dictionary<string, string> _opcoes = new Dictionary<string, string>
        {
            { "1", "Realizar download de capítulos de mangas" },
            { "2", "Abrir diretório de download dos arquivos" },
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
                    await DownloadCapituloMangaLinkPagina.Iniciar(_versao, "");
                    break;
                case "2":
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

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("\nOpção selecionada invalida, selecione uma das opções disponíveis!");

                codigo = SelecionarOpcao();
            }

            return codigo;
        }

        public static string SelecionarOpcao()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("\nOpções:");

            foreach (var opcao in _opcoes)
                if (!string.IsNullOrEmpty(opcao.Key))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($"{opcao.Key} - ");
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write($"{opcao.Value}\n");
                }
                else
                    Console.WriteLine(" ");

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\nInforme uma das opções disponíveis: ");

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            return Console.ReadLine();
        }
    }
}