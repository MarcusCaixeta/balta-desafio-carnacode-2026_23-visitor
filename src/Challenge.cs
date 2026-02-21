// DESAFIO: Sistema de Documentos - SOLUÇÃO: Padrão Visitor

using System;
using System.Collections.Generic;

namespace DesignPatternChallenge
{
    public interface IDocumentElement
    {
        void Accept(IDocumentVisitor visitor);
    }

    public interface IDocumentVisitor
    {
        void Visit(Paragraph p);
        void Visit(Image img);
        void Visit(Table tbl);
    }

    public class Paragraph : IDocumentElement
    {
        public string Text { get; set; }
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public Paragraph(string text) { Text = text; FontFamily = "Arial"; FontSize = 12; }
        public void Accept(IDocumentVisitor v) => v.Visit(this);
    }

    public class Image : IDocumentElement
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Alt { get; set; }
        public Image(string url, int w, int h) { Url = url; Width = w; Height = h; Alt = ""; }
        public void Accept(IDocumentVisitor v) => v.Visit(this);
    }

    public class Table : IDocumentElement
    {
        public List<List<string>> Cells { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public Table(int r, int c)
        {
            Rows = r; Columns = c;
            Cells = new List<List<string>>();
            for (int i = 0; i < r; i++)
            {
                var row = new List<string>();
                for (int j = 0; j < c; j++) row.Add($"C{i},{j}");
                Cells.Add(row);
            }
        }
        public void Accept(IDocumentVisitor v) => v.Visit(this);
    }

    public class HtmlExportVisitor : IDocumentVisitor
    {
        private readonly List<string> _parts = new();

        public string GetResult() => string.Join("", _parts);

        public void Visit(Paragraph p) => _parts.Add($"<p style='font-family:{p.FontFamily};font-size:{p.FontSize}px'>{p.Text}</p>");
        public void Visit(Image img) => _parts.Add($"<img src='{img.Url}' width='{img.Width}' height='{img.Height}' alt='{img.Alt}' />");
        public void Visit(Table tbl)
        {
            var html = "<table>";
            foreach (var row in tbl.Cells)
            {
                html += "<tr>";
                foreach (var cell in row) html += $"<td>{cell}</td>";
                html += "</tr>";
            }
            _parts.Add(html + "</table>");
        }
    }

    public class WordCountVisitor : IDocumentVisitor
    {
        public int TotalWords { get; private set; }

        public void Visit(Paragraph p) => TotalWords += p.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        public void Visit(Image img) { }
        public void Visit(Table tbl)
        {
            foreach (var row in tbl.Cells)
                foreach (var cell in row)
                    TotalWords += cell.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        }
    }

    public class ValidationVisitor : IDocumentVisitor
    {
        public bool IsValid { get; private set; } = true;

        public void Visit(Paragraph p) { if (string.IsNullOrEmpty(p.Text) || p.Text.Length >= 1000) IsValid = false; }
        public void Visit(Image img) { if (string.IsNullOrEmpty(img.Url) || img.Width <= 0 || img.Height <= 0) IsValid = false; }
        public void Visit(Table tbl) { if (tbl.Rows <= 0 || tbl.Columns <= 0) IsValid = false; }
    }

    public class Document
    {
        public string Title { get; set; }
        public List<IDocumentElement> Elements { get; } = new();

        public void AddElement(IDocumentElement el) => Elements.Add(el);

        public void Accept(IDocumentVisitor visitor)
        {
            foreach (var el in Elements)
                el.Accept(visitor);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Documentos (Visitor Pattern) ===\n");

            var doc = new Document { Title = "Relatório" };
            doc.AddElement(new Paragraph("Texto do relatório."));
            doc.AddElement(new Image("grafico.png", 800, 600));
            doc.AddElement(new Table(3, 4));

            var htmlVisitor = new HtmlExportVisitor();
            doc.Accept(htmlVisitor);
            Console.WriteLine("HTML: " + htmlVisitor.GetResult().Substring(0, 80) + "...");

            var wordVisitor = new WordCountVisitor();
            doc.Accept(wordVisitor);
            Console.WriteLine($"Palavras: {wordVisitor.TotalWords}");

            var validVisitor = new ValidationVisitor();
            doc.Accept(validVisitor);
            Console.WriteLine($"Válido: {validVisitor.IsValid}");

        }
    }
}
