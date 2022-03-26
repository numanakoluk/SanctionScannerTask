using HtmlAgilityPack;
using Sahipinden.App;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Sahibinden.App
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            List<Product> products = new List<Product>();

            var url = "https://www.sahibinden.com";
            var web = new HtmlWeb();
            var doc = web.Load(url);

            //uiBox showcase class'ında a tagleri alındı.
            var nodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'uiBox showcase')]//a[@href]");


            //href ve title çekildi ve listeye atıldı.
            foreach (HtmlNode link in nodes)
            {
                string hrefValue = link.GetAttributeValue("href", string.Empty);
                string title = link.InnerText.Trim();
                products.Add(new Product { Url = url + hrefValue, Title = title });
            }

            for (int i = 1; i < products.Count; i++)
            {

                //Siteden olağan dışı erişim hatası döndüğünden istekler bekletildi.
                Thread.Sleep(20000);


                //detay sayfası açıldı.classifiedInfo dan price  çekildi.
                var detail = web.Load(products[i].Url);
                var detailNodes = detail.DocumentNode.SelectNodes("//div[contains(@class, 'classifiedInfo ')]");
                if (detailNodes != null)
                {
                    //150000 TL -boşluğun indexi bulundu ve priceString bulundu ve Convert edildi.
                    var innerText = detailNodes[0].InnerText.Trim();
                    var index = innerText.IndexOf(' ');
                    var priceString = innerText.Substring(0, index);


                    try
                    {
                        products[i].Price = Convert.ToDouble(priceString);
                    }
                    catch (Exception)
                    {
                        //Fiyat Convert edilemediğinde 0 olarak alındı.
                        products[i].Price = 0;
                    }
                    
                }
            }
            //Sıfır olmayan ürünler filtrelendi.
            var nonZeroproducts = products.Where(x => x.Price != 0);
            
              //Ortalama fiyat
              var average = nonZeroproducts.Average(x => x.Price);

            foreach (var item in nonZeroproducts)
            {
                Console.WriteLine($"Title:{item.Title} Price:{item.Price}");
            }
            
            Console.WriteLine("Avarage:" + average);


            File.WriteAllLines("Sahibinden.txt", nonZeroproducts.Select(x => x.Title +" "+ x.Price+ " " + x.Url));
            Console.ReadLine();
        }
        
    }
}