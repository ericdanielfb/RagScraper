using System;
using System.Collections.Generic;
using System.IO;

namespace RagScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            Scraper scraper = new Scraper();
            //string[] lines = File.ReadAllLines(@"C:\Users\Eric Daniel\source\repos\RagScraper\RagMonsters.txt");
            //var monsterUrls = new List<string>();
            //foreach (string line in lines)
            //{
            //    monsterUrls.Add(line);
            //}
            var monsterUrls = scraper.UpdateArchive();

            var monsters = scraper.GetMonsterPageInfo(monsterUrls);
        }
    }
}