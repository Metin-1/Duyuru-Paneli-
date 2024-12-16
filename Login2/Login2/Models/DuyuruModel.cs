namespace Login2.Models
{
    public class DuyuruModel
    {
        public List<string> Textboxes { get; set; }
        public List<bool> CheckBoxes { get; set; }
        public List<int> Ids { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class Duyuru
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsChecked { get; set; }
    }
}
