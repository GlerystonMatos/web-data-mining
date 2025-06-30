using System.Diagnostics;
using System.Reflection;

namespace WebDataMining
{
    public class DownloadCapituloMangaLinkPagina
    {
        private static int _pagina = 0;
        private static int _quantidade = 0;

        private static string _manga = "";
        private static string _versao = "";
        private static string _capitulo = "";
        private static string _caminhoExe = "";
        private static string _pastaManga = "";

        private static HttpClient _httpClient;

        private static HashSet<string> _erros;
        private static HashSet<string> _links;
        private static HashSet<string> _concluidos;
        private static HashSet<int> _paginasComErro;

        public static async Task<string> Iniciar(string versao, string manga)
        {
            _httpClient = new HttpClient();
            _concluidos = new HashSet<string>();

            ObterInformacoes(versao, manga);
            string linkBase = ObterLinkCapitulo();

            for (int capitulos = 1; capitulos <= _quantidade; capitulos++)
            {
                string linkCapitulo = IncluirCapituloLink(linkBase);
                string html = await _httpClient.GetStringAsync(linkCapitulo);

                _links = Utils.ObterLinksDeImagens(html);
                await BaixarCapituloAsync();
                DownloadConcluido();

                _capitulo = (int.Parse(_capitulo) + 1).ToString();
            }

            ExibirConcluidos();

            if (_quantidade > 1)
                AbrirDiretorioDownloadArquivos();

            string opcao = Utils.Confirmacao("Deseja baixar mais capítulos?");
            while (opcao.ToUpper().Equals("S"))
                opcao = await Iniciar(_versao, _manga);

            return opcao;
        }

        private static void ObterInformacoes(string versao, string manga)
        {
            Topo(versao);
            _versao = versao;

            Console.WriteLine("");

            if (string.IsNullOrEmpty(manga))
                _manga = Utils.Pergunta("Informe o nome do manga:");
            else
                _manga = manga;

            _quantidade = int.Parse(Utils.Pergunta("Informe a quantidade de capítulos:"));
            _capitulo = Utils.Pergunta("Informe o primeiro capítulo:");
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

        private static string ObterLinkCapitulo()
        {
            Topo(_versao);

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($"\nManga: {_manga} - Capítulo: {int.Parse(_capitulo).ToString("D3")}");

            Console.WriteLine("\nInforme o link de download do primeiro capítulo");
            Console.WriteLine("Use {ref-cap} para que o capítulo seja atualizado automaticamente\n");

            return Utils.Pergunta("Link:");
        }

        private static string IncluirCapituloLink(string linkCapitulo)
            => linkCapitulo.Replace("{ref-cap}", _capitulo);

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
            for (int pagina = 1; pagina <= _pagina; pagina++)
            {
                Thread.Sleep(10);

                if (_paginasComErro.Contains(pagina))
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

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Aperte qualquer tecla para continuar");
                Console.ReadLine();
            }
            else
                Console.WriteLine("\n");
        }

        private static void DownloadConcluido()
        {
            string concluido = $"Manga: {_manga} - Capítulo: {int.Parse(_capitulo).ToString("D3")} - Download: ";
            _concluidos.Add(concluido);

            if (_quantidade == 1)
            {
                Topo(_versao);

                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write(concluido);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"100%\n");
                Console.WriteLine("");

                AbrirDiretorioDownloadManga();
            }
        }

        private static void AbrirDiretorioDownloadManga()
        {
            string abrirDiretorio = Utils.Confirmacao("Deseja abrir o diretório de download do manga?");
            if (abrirDiretorio.Equals("S"))
                Process.Start("explorer.exe", _pastaManga);
        }

        private static void AbrirDiretorioDownloadArquivos()
        {
            string abrirDiretorio = Utils.Confirmacao("Deseja abrir o diretório de download?");
            if (abrirDiretorio.Equals("S"))
                Utils.AbrirDiretorioDownloadArquivos();
        }

        private static void ExibirConcluidos()
        {
            if (_concluidos.Count() > 0)
            {
                Topo(_versao);
                Console.WriteLine("");

                foreach (string concluido in _concluidos)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write(concluido);
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write($"100%\n");
                }

                Console.WriteLine("");
            }
        }
    }
}