namespace ApiMarvel.DTO
{
    public class Results
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public Comics comics { get; set; }
        public Series series { get; set; }
        public Stories stories { get; set; }
        public Events events { get; set; }
    }
}
