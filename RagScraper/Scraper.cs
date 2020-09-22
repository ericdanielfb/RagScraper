using System;
using System.Collections.Generic;
using System.Text;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using HtmlAgilityPack;
using System.IO;
using System.Text.Json;

namespace RagScraper
{
    class Scraper
    {
        ScrapingBrowser _browser = new ScrapingBrowser();
        const string ArchivePath = @"C:\Users\Eric Daniel\source\repos\RagScraper\RagScraper\RagMonsters.txt";
        public List<string> UpdateArchive()
        {
            int maxPages = 152;
            string url = "https://playragnarokonlinebr.com/database/thor/monstros";
            var MainPageLinks = new List<string>();

            Console.WriteLine("Inicializando Web Scraping.");
            for (int i = 0; i < maxPages; i++)
            {
                var NewLinks = GetMainPageLinks(url + $"?page={i + 1}");
                MainPageLinks.AddRange(NewLinks);

                drawTextProgressBar(i, maxPages);
            }
            Console.WriteLine();
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(ArchivePath))
            {
                foreach (string link in MainPageLinks)
                {
                    file.WriteLine(link);
                }
            }
            Console.WriteLine($"Web Scraping concluído. {MainPageLinks.Count} registros salvos.");
            Console.WriteLine($"Caminho do arquivo: {ArchivePath}");

            return MainPageLinks;
        }
        public List<MonsterInfo> GetMonsterPageInfo(List<string> urls)
        {
            var infos = new List<MonsterInfo>();
            int count = 1;

            Console.WriteLine("Gerando arquivo JSON.");
            foreach (var url in urls)
            {
                try
                {
                    var htmlNode = GetHtml(url);
                    var monsterInfo = new MonsterInfo();

                    monsterInfo.Name = htmlNode
                        .OwnerDocument
                        .DocumentNode
                        .SelectSingleNode("//html/body/main/main/section/div/div/h1")
                        .InnerText
                        .Trim();

                    monsterInfo.Level = int.Parse(htmlNode
                        .OwnerDocument
                        .DocumentNode
                        .SelectNodes("//html/body/main/main/section/div/div")[1]
                        .SelectNodes("//ul/li")[1]
                        .InnerText
                        .Trim()
                        .Replace(".", ""));

                    monsterInfo.BaseExp = int.Parse(htmlNode
                        .OwnerDocument
                        .DocumentNode
                        .SelectNodes("//html/body/main/main/section/div/div")[1]
                        .SelectNodes("//ul/li")[9]
                        .InnerText
                        .Trim()
                        .Replace(".", ""));

                    monsterInfo.ClassExp = int.Parse(htmlNode
                        .OwnerDocument
                        .DocumentNode
                        .SelectNodes("//html/body/main/main/section/div/div")[1]
                        .SelectNodes("//ul/li")[11]
                        .InnerText
                        .Trim()
                        .Replace(".", ""));

                    monsterInfo.Url = url;
                    infos.Add(monsterInfo);
                    drawTextProgressBar(count, urls.Count);
                    count++;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Erro ao acessar a url: {url}");
                }
            }

            string jsonString;
            jsonString = JsonSerializer.Serialize(infos);
            File.WriteAllText("monsters.json", jsonString);
            Console.WriteLine("Arquivo JSON gerado.");
            return infos;
        }
        List<string> GetMainPageLinks(string url)
        {
            var homePageLinks = new List<String>();
            var html = GetHtml(url);
            var links = html.CssSelect("a");

            foreach (var link in links)
            {
                if (link.Attributes["href"].Value.Contains("detalhes"))
                {
                    homePageLinks.Add(@"https://playragnarokonlinebr.com/database/thor/" + link.Attributes["href"].Value);
                }
            }
            return homePageLinks;
        }

        HtmlNode GetHtml(string url)
        {
            WebPage webpage = _browser.NavigateToPage(new Uri(url));
            return webpage.Html;
        }

        private void drawTextProgressBar(int progress, int total)
        {
            //draw empty progress bar
            Console.CursorLeft = 0;
            Console.Write("["); //start
            Console.CursorLeft = 32;
            Console.Write("]"); //end
            Console.CursorLeft = 1;
            float onechunk = 30.0f / total;

            //draw filled part
            int position = 1;
            for (int i = 0; i < onechunk * progress; i++)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw unfilled part
            for (int i = position; i <= 31; i++)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw totals
            Console.CursorLeft = 35;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(progress.ToString() + " of " + total.ToString() + "    "); //blanks at the end remove any excess
        }
    }


}