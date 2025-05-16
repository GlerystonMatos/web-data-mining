using System.Diagnostics;
using System.Reflection;

namespace WebDataMining
{
    public class DownloadCapituloMangaLinkPagina
    {
        public static async Task<string> Iniciar(string versao, string manga, string capitulo)
        {
            Utils.Topo(versao);
            Console.WriteLine("\n3: Realizar download de capítulos de mangas (Link da página)\n");

            if (string.IsNullOrEmpty(manga))
                manga = Utils.Pergunta("Informe o nome do manga:");

            if (string.IsNullOrEmpty(capitulo))
                capitulo = Utils.Pergunta("Informe o capítulo do manga:");

            Console.Clear();
            Utils.Topo(versao);
            Console.WriteLine("\n3: Realizar download de capítulos de mangas (Link da página)");
            Console.WriteLine($"Manga: {manga} - Capítulo: {int.Parse(capitulo).ToString("D3")}\n");

            string linkPagina = Utils.Pergunta("Informe o link da página:");

            HttpClient httpClient = new HttpClient();
            string html = await httpClient.GetStringAsync(linkPagina);
            IList<string> links = Utils.ObterLinksDeImagens(html);

            Console.Clear();
            Utils.Topo(versao);
            Console.WriteLine("\n3: Realizar download de capítulos de mangas (Link da página)\n");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($" Manga: {manga} - Capítulo: {int.Parse(capitulo).ToString("D3")} ");
            Console.Write($" Baixando");

            string caminhoExe = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string pastaManga = $"{caminhoExe}\\Download\\{manga}\\{manga} - Capítulo {int.Parse(capitulo).ToString("D3")}\\";

            Directory.CreateDirectory(pastaManga);

            IList<string> erros = new List<string>();
            erros.Clear();

            int pagina = 0;
            foreach (string link in links)
            {
                pagina += 1;
                string extensao = link.Split(".")[link.Split(".").Length - 1];

                try
                {
                    Thread.Sleep(500);
                    byte[] bytes = await httpClient.GetByteArrayAsync(link);

                    if (!extensao.Equals("webp"))
                        await File.WriteAllBytesAsync($"{pastaManga}\\{pagina.ToString()}.{extensao}", bytes);
                    else
                        await File.WriteAllBytesAsync($"{pastaManga}\\{pagina.ToString()}.jpg", bytes);

                    Console.Write(".");
                }
                catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(".");
                    Console.ForegroundColor = ConsoleColor.Green;

                    erros.Add($"Recurso não encontrado (404) para o link: {link}");
                }
                catch (HttpRequestException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(".");
                    Console.ForegroundColor = ConsoleColor.Green;

                    erros.Add($"Erro HTTP ao acessar o link: {link}. Código: {ex.StatusCode}");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(".");
                    Console.ForegroundColor = ConsoleColor.Green;

                    erros.Add($"Erro inesperado: {ex.Message}");
                }
            }

            if (erros.Count() > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n\nOcorreram alguns erros:");

                foreach (string erro in erros)
                    Console.WriteLine($"{erro}");

                Console.WriteLine("");
            }
            else
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