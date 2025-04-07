using System.Diagnostics;
using System.Reflection;

namespace WebDataMining
{
    public class DownloadCapituloManga
    {
        public static async Task<string> Iniciar(string versao, string manga, string capitulo)
        {
            Utils.Topo(versao);
            Console.WriteLine("\n1: Realizar download de capítulos de mangas\n");

            if (string.IsNullOrEmpty(manga))
                manga = Utils.Pergunta("Informe o nome do manga:");

            if (string.IsNullOrEmpty(capitulo))
                capitulo = Utils.Pergunta("Informe o capítulo do manga:");

            string html = Utils.Pergunta("Informe o elemento com os links de download das páginas:");
            IList<string> links = Utils.ObterLinksDeImagens(html);

            Console.Clear();
            Utils.Topo(versao);
            Console.WriteLine("\n1: Realizar download de capítulos de mangas\n");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($" Manga: {manga} - Capítulo: {capitulo} ");
            Console.Write($" Baixando");

            string caminhoExe = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string pastaManga = $"{caminhoExe}\\Download\\{manga} {capitulo}\\";

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

            Console.WriteLine("\n");
            string abrirDiretorio = Utils.Confirmacao("Deseja abrir o diretório de download?");
            if (abrirDiretorio.Equals("S"))
                Process.Start("explorer.exe", pastaManga);

            string opcao = Utils.Confirmacao("Deseja baixar mais capítulos?");

            while (opcao.ToUpper().Equals("S"))
                opcao = await Iniciar(versao, manga, (int.Parse(capitulo) + 1).ToString());

            return opcao;
        }
    }
}