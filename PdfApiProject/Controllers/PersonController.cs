using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PdfApiProject.Model;
using System.Runtime.InteropServices;
using iTextSharp.text;   
using iTextSharp.text.pdf;  
using iTextSharp.text.pdf.parser;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection.PortableExecutable;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;
using static iTextSharp.text.pdf.AcroFields;
using System.Linq;

namespace PdfApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        List<Person> persons = new List<Person>()
        {
                new Person(){ Id = 1, TCKN="16699922258", Name="Aslı" ,Surname="can",Date ="01/09/2022"},
                new Person(){ Id = 2, TCKN="16796628106", Name="Yusuf" ,Surname="Yakupoğlu", Date ="01/09/2022"},
                new Person(){ Id = 3, TCKN="16793628260", Name="Yunus" ,Surname="Yakupoğlu", Date ="01/09/2022"},
                new Person(){ Id = 4, TCKN="16232323232", Name="eksik" ,Surname="json", Date ="01/09/2022"},
                new Person(){ Id = 5, TCKN="16232323265", Name="eksik2" ,Surname="json2", Date ="01/09/2022"}
        };
    
        private async Task<string> PDfCreatePath(string pdfTckn)
        {
          
            foreach (var item in persons)
            {
                var listİtemTckn = item.TCKN;
                var JsonMatchPdf = pdfTckn.Contains(listİtemTckn);
                NullJSon(JsonMatchPdf);
                if (JsonMatchPdf == true && item != null)
                {
                    //bordro-2022-9-TCKN-GUID.pdf
                    var pdfPath = "bordro" + item.TCKN + item.Name + item.Surname;
                    return await CreatePdf(pdfPath);
                }            
            }

            return null ;                
             
        }

        private async void NullJSon(bool jsonMatchTckn)
        {
            foreach (var item in persons)
            {
                if (jsonMatchTckn == false)
                {
                    //bordro-2022-9-TCKN-GUID.pdf
                    var pdfPath = "bordro" + item.TCKN + item.Name + item.Surname;
                    await CreatePdf(pdfPath);

                }
            }
        }

        private async Task<string> CreatePdf(string item)
        {
            iTextSharp.text.Document pdfDocument = new iTextSharp.text.Document();
            var PdfPath = "C:\\Users\\Musa\\Desktop\\TestPdf\\" + item + ".pdf";
            PdfWriter.GetInstance(pdfDocument, new FileStream(PdfPath, FileMode.Create));

            if (pdfDocument.IsOpen() == false)
            {
                string text = item;
                pdfDocument.Open();
                text = await Turkishletters(text);
                pdfDocument.Add(new Paragraph(text));


                pdfDocument.Close();
            }

            return null;
        }

        private async Task<string>Turkishletters(string text)
        {

            text = text.Replace("İ", "\u0130");

            text = text.Replace("ı", "\u0131");

            text = text.Replace("Ş", "\u015e");

            text = text.Replace("ş", "\u015f");

            text = text.Replace("Ğ", "\u011e");

            text = text.Replace("ğ", "\u011f");

            text = text.Replace("Ö", "\u00d6");

            text = text.Replace("ö", "\u00f6");

            text = text.Replace("ç", "\u00e7");

            text = text.Replace("Ç", "\u00c7");

            text = text.Replace("ü", "\u00fc");

            text = text.Replace("Ü", "\u00dc");

            return text;
        }
        
        /// <summary>
        /// verilen pdfText string'i içerisinden tckn bulup string olarak döndür.
        /// </summary>
        private async Task<string> FindTckn(string pdfText)
        {
            string pattern = @"\d{11}"; 
            Match m = Regex.Match(pdfText, pattern);
            if (m.Success)
            {
                await PDfCreatePath(m.Value);
                return m.Value;

            }
       
            return null;
        }
        
        private async Task<List<string>> GetTcknList(string pdfPath)
        {
            List<string> tcknList = new List<string>();

            string[] pdfContents = await ReadPdf(pdfPath);
            foreach (string pdfContent in pdfContents)
            {
                string tckn = await FindTckn(pdfContent);
                tcknList.Add(tckn);
            }

            return tcknList;
        }

        private async Task <string[]> ReadPdf(string pdfPath)
        {
            using (PdfReader reader = new PdfReader(pdfPath))
            {
                string[] pdfContents = new string[reader.NumberOfPages];
        
                for (int i = 0; i < reader.NumberOfPages; i++)
                {
                    pdfContents[i] = PdfTextExtractor.GetTextFromPage(reader, i + 1);
                }

                return pdfContents;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get()       
        {
            
            return Ok(GetTcknList("C:\\Users\\Musa\\Desktop\\ReadPdf\\bordro.pdf"));
        }

    }
}
