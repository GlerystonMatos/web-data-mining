using System.Diagnostics;
using System.Reflection;

namespace WebDataMining
{
    public class DownloadCapituloMangaLinkBase
    {
        public static async Task<string> Iniciar(string versao, string manga, string capitulo)
        {
            Utils.Topo(versao);
            Console.WriteLine("\n2: Realizar download de capítulos de mangas (Link base)\n");

            if (string.IsNullOrEmpty(manga))
                manga = Utils.Pergunta("Informe o nome do manga:");

            if (string.IsNullOrEmpty(capitulo))
                capitulo = Utils.Pergunta("Informe o capítulo do manga:");

            string linkBase = Utils.Pergunta("Informe o link base das páginas:");
            string primeiraPagina = Utils.Pergunta("Informe a primeira página do capítulo:");
            string ultimaPagina = Utils.Pergunta("Informe a última página do capítulo:");
            string extensao = Utils.Pergunta("Informe a extensão das imagens:");
            string zeros = Utils.Pergunta("Completar página com quantos zeros a esquerda (Informe o tamanho da string com os zeros):");

            Console.Clear();
            Utils.Topo(versao);
            Console.WriteLine("\n2: Realizar download de capítulos de mangas (Link base)\n");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($" Manga: {manga} - Capítulo: {capitulo} ");
            Console.Write($" Baixando");

            string caminhoExe = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string pastaManga = $"{caminhoExe}\\Download\\{manga} {capitulo}\\";

            Directory.CreateDirectory(pastaManga);

            for (int pagina = int.Parse(primeiraPagina); pagina <= int.Parse(ultimaPagina); pagina++)
            {
                Console.Write(".");

                string link = "";
                if (int.Parse(zeros) > 0)
                    link = $"{linkBase}{pagina.ToString($"D{int.Parse(zeros)}")}.{extensao}";
                else
                    link = $"{linkBase}{pagina.ToString()}.{extensao}";

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