using System.Diagnostics;
using System.Reflection;

namespace WebDataMining
{
    public class DownloadCapituloMangaLinkPagina
    {
        public static async Task<string> Iniciar(string versao, string manga, string capitulo)
        {
            Utils.Topo(versao);
            Console.WriteLine("\n1: Realizar download de capítulos de mangas (Link da página)\n");

            if (string.IsNullOrEmpty(manga))
                manga = Utils.Pergunta("Informe o nome do manga:");

            if (string.IsNullOrEmpty(capitulo))
                capitulo = Utils.Pergunta("Informe o capítulo do manga:");

            string linkPagina = Utils.Pergunta("Informe o link da página:");

            HttpClient httpClient = new HttpClient();
            string html = await httpClient.GetStringAsync(linkPagina);
            IList<string> links = Utils.ObterLinksDeImagens(html);

            Console.Clear();
            Utils.Topo(versao);
            Console.WriteLine("\n1: Realizar download de capítulos de mangas (Link da página)\n");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($" Manga: {manga} - Capítulo: {int.Parse(capitulo).ToString("D3")} ");
            Console.Write($" Baixando");

            string caminhoExe = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string pastaManga = $"{caminhoExe}\\Download\\{manga} - Capítulo {int.Parse(capitulo).ToString("D3")}\\";

            Directory.CreateDirectory(pastaManga);

            int pagina = 0;
            foreach (string link in links)
            {
                pagina += 1;
                Console.Write(".");
                string extensao = link.Split(".")[link.Split(".").Length - 1];

                Thread.Sleep(500);
                byte[] bytes = await httpClient.GetByteArrayAsync(link);

                if (!extensao.Equals("webp"))
                    await File.WriteAllBytesAsync($"{pastaManga}\\{pagina.ToString()}.{extensao}", bytes);
                else
                    await File.WriteAllBytesAsync($"{pastaManga}\\{pagina.ToString()}.jpg", bytes);
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