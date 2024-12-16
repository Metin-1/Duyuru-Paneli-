using Newtonsoft.Json.Linq;
using System.IO;

namespace Login2.Services
{
    public class DuyuruService
    {
        private readonly string _filePath;

        public DuyuruService()
        {
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "Jsondb");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            _filePath = Path.Combine(directory, "duyurular.json");
        }

        public JObject ReadDuyurular()
        {
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException("JSON dosyası bulunamadı: " + _filePath);
            }

            var json = File.ReadAllText(_filePath);
            return JObject.Parse(json);
        }

        public void UpdateDuyurular(JObject duyurular)
        {
            var json = duyurular.ToString();
            try
            {
                File.WriteAllText(_filePath, json); // JSON verilerini dosyaya yazdır
                Console.WriteLine("JSON dosyası başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("JSON dosyası güncellenirken hata oluştu: " + ex.Message);
            }
        }
    }
}
