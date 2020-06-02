using System;

namespace KweetAPI.Model
{
    public class Kweet
    {
        public int id { get; set; }
        public string userID { get; set; }
        public string text { get; set; }
        public DateTime created { get; set; }

        public Kweet() { }
        public Kweet(string userId, string text)
        {
            userID = userId;
            this.text = text;
            this.created = DateTime.Now;
        }
    }
}