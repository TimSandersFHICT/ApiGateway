using System;

namespace KweetAPI.Model
{
    public class Kweet
    {
        public int id { get; set; }
        public int UserID { get; set; }
        public string text { get; set; }
        public DateTime created { get; set; }

        public Kweet() { }
        public Kweet(long userId, string text)
        {
            UserID = id;
            this.text = text;
            this.created = DateTime.Now;
        }
    }
}