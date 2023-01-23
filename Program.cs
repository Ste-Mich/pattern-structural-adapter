using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Newtonsoft.Json;

/*  Adapter umožňuje rozšíření o novou funkci bez změny stávajícího projektu. 
 *  V tomto případě:
 *  - XMLDataReader čte seznamy knih ve formátu XML, chceme přidat více knih z externího zdroje
 *  - Jsou však v JSON. Data převedeme na XML, abychom nemuseli měnit stávající kód.
 */

namespace Adapter
{
    class Program
    {
        static void Main(string[] args)
        {
            new XMLDataReader("../../../XMLSampleData.xml");

            var Adapter = new AdapterJSONtoXML("../../../JSONSampleData.json", "../../../XMLConverted.xml");
            Adapter.Init();
            new XMLDataReader("../../../XMLConverted.xml");
        }
    }

    class XMLDataReader
    {
        public XMLDataReader(string path)
        {
            XmlDocument xmlDoc = new();
            xmlDoc.Load(path);
            foreach (XmlNode xmlNode in xmlDoc.DocumentElement.ChildNodes)
            {
                Console.WriteLine("- " 
                    + xmlNode.ChildNodes[0].InnerText + " by " 
                    + xmlNode.ChildNodes[1].InnerText + " with rating "
                    + xmlNode.ChildNodes[2].InnerText);
            }
        }
    }

    public class AdapterJSONtoXML
    {
        private string XMLPath { get; set; }
        private string JSONPath { get; set; }

        public AdapterJSONtoXML(string jsonpath, string xmlpath)
        {
            XMLPath = xmlpath;
            JSONPath = jsonpath;
        }

        private List<Book> Books { get; set; }

        private class Book
        {
            public string Title { get; set; }
            public string Author { get; set; }
            public string Rating { get; set; }
        }

        public void Init()
        {
            GetJSONData();
            ConvertToXML(Books);
        }

        private void ConvertToXML(List<Book> Books)
        {
            using (XmlWriter writer = XmlWriter.Create(XMLPath))
            {
                writer.WriteStartElement("bookstore");
                foreach (var book in Books)
                {
                    writer.WriteStartElement("book");
                    writer.WriteElementString("title", book.Title);
                    writer.WriteElementString("author", book.Author);
                    writer.WriteElementString("rating", book.Rating);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.Flush();
            }
        }

        private void GetJSONData()
        {
            Books = new();
            Book book;

            using (StreamReader r = new StreamReader(JSONPath))
            {
                string json = r.ReadToEnd();
                dynamic array = JsonConvert.DeserializeObject(json);
                foreach (var b in array.bookstore.book)
                {
                    book = new();
                    book.Title = b.title;
                    book.Author = b.author;
                    book.Rating = b.rating;
                    Books.Add(book);
                }
            }
        }
    }

    class JSONDataProvider
    {
        //Dává rozdílný formát dat - JSONSampleData.json
    }
}
