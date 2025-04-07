namespace WebDataMining
{
    public class Program
    {
        private static string _versao = "1.0.0.1";

        private static Dictionary<string, string> _opcoes = new Dictionary<string, string>
        {
            { "1", "Realizar download de capítulos de mangas" },
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
                    await DownloadCapituloManga.Iniciar(_versao, "", "");
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
                Console.WriteLine($"{opcao.Key}: {opcao.Value}");

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\nInforme uma das opções disponíveis: ");
            string opcaoSelecionada = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            return opcaoSelecionada;
        }
    }
}