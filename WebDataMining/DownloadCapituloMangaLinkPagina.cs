using System.Diagnostics;
using System.Reflection;

namespace WebDataMining
{
    public class DownloadCapituloMangaLinkPagina
    {
        private static int _pagina = 0;
        private static string _manga = "";
        private static string _versao = "";
        private static string _capitulo = "";
        private static string _caminhoExe = "";
        private static string _pastaManga = "";

        private static HttpClient _httpClient;
        private static HashSet<string> _erros;
        private static HashSet<string> _links;
        private static HashSet<int> _paginasComErro;

        public static async Task<string> Iniciar(string versao, string manga, string capitulo)
        {
            _httpClient = new HttpClient();

            ObterInformacoes(versao, manga, capitulo);
            string linkCapitulo = ObterLinkCapitulo();

            string html = await _httpClient.GetStringAsync(linkCapitulo);
            _links = Utils.ObterLinksDeImagens(html);

            await BaixarCapituloAsync();
            DownloadConcluido();

            string opcao = Utils.Confirmacao("Deseja baixar mais capítulos?");
            while (opcao.ToUpper().Equals("S"))
                opcao = await Iniciar(_versao, _manga, (int.Parse(_capitulo) + 1).ToString());

            return opcao;
        }

        private static void Topo(string versao)
        {
            Console.Clear();
            Utils.Topo(versao);
            Console.Write("| ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Opção: Realizar download de capítulos de mangas");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("       |\n");
            Console.WriteLine("---------------------------------------------------------");
        }

        private static void ObterInformacoes(string versao, string manga, string capitulo)
        {
            Topo(versao);
            _versao = versao;

            Console.WriteLine("");

            if (string.IsNullOrEmpty(manga))
                _manga = Utils.Pergunta("Informe o nome do manga:");
            else
                _manga = manga;

            if (string.IsNullOrEmpty(capitulo))
                _capitulo = Utils.Pergunta("Informe o capítulo do manga:");
            else
                _capitulo = capitulo;
        }

        private static string ObterLinkCapitulo()
        {
            Topo(_versao);

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($"\nManga: {_manga} - Capítulo: {int.Parse(_capitulo).ToString("D3")}");

            return Utils.Pergunta("Informe o link do capítulo:");
        }

        private static async Task BaixarCapituloAsync()
        {
            InicializarDadosDownload();
            await RealizarDownloadsAsync();
            ExibirErros();
        }

        private static void InicializarDadosDownload()
        {
            _caminhoExe = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _pastaManga = $"{_caminhoExe}\\Download\\{_manga}\\{_manga} - Capítulo {int.Parse(_capitulo).ToString("D3")}\\";

            _paginasComErro = new HashSet<int>();
            _erros = new HashSet<string>();

            _pagina = 0;

            Directory.CreateDirectory(_pastaManga);
        }

        private static async Task RealizarDownloadsAsync()
        {
            foreach (string link in _links)
            {
                try
                {
                    await RealizarDownloadAsync(link);
                }
                catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ItemConcluidoErro();
                    _paginasComErro.Add(_pagina);
                    _erros.Add($"Recurso não encontrado (404) para o link: {link}");
                }
                catch (HttpRequestException ex)
                {
                    ItemConcluidoErro();
                    _paginasComErro.Add(_pagina);
                    _erros.Add($"Erro HTTP ao acessar o link: {link}. Código: {ex.StatusCode}");
                }
                catch (Exception ex)
                {
                    ItemConcluidoErro();
                    _paginasComErro.Add(_pagina);
                    _erros.Add($"Erro inesperado: {ex.Message}");
                }
            }
        }

        private static async Task RealizarDownloadAsync(string link)
        {
            Topo(_versao);

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"\n Manga: {_manga} - Capítulo: {int.Parse(_capitulo).ToString("D3")}");
            Console.Write($" Baixando: ");

            IndicadorDownload();
            PercentualConclusao();

            Thread.Sleep(10);

            string extensao = link.Split(".")[link.Split(".").Length - 1];
            byte[] bytes = await _httpClient.GetByteArrayAsync(link);
            _pagina += 1;

            if (!extensao.Equals("webp"))
                await File.WriteAllBytesAsync($"{_pastaManga}\\{_pagina.ToString()}.{extensao}", bytes);
            else
                await File.WriteAllBytesAsync($"{_pastaManga}\\{_pagina.ToString()}.jpg", bytes);

            ItensConcluidos();
        }

        private static void IndicadorDownload()
        {
            if (_pagina % 2 == 0)
                Console.Write("\\");
            else
                Console.Write("/");
        }

        private static void PercentualConclusao()
        {
            int percentual = ((_pagina * 100) / _links.Count());
            Console.Write($" {percentual}% ");
        }

        private static void ItensConcluidos()
        {
            for (int i = 1; i <= _pagina; i++)
            {
                Thread.Sleep(10);

                if (_paginasComErro.Contains(i))
                    ItemConcluidoErro();
                else
                    Console.Write(".");
            }
        }

        private static void ItemConcluidoErro()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(".");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
        }

        private static void ExibirErros()
        {
            if (_erros.Count() > 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"\n\nOcorreram alguns erros:");

                foreach (string erro in _erros)
                    Console.WriteLine($"{erro}");

                Console.WriteLine("");
            }
            else
                Console.WriteLine("\n");
        }

        private static void DownloadConcluido()
        {
            Topo(_versao);

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write($"\nManga: {_manga} - Capítulo: {int.Parse(_capitulo).ToString("D3")} - Download: ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"100%\n");

            string abrirDiretorio = Utils.Confirmacao("Deseja abrir o diretório de download?");
            if (abrirDiretorio.Equals("S"))
                Process.Start("explorer.exe", _pastaManga);
        }
    }
}