using System;

namespace UserAPI.Model
{
    public class User
    {
        public User()
        {

        }

        public string id { get; set; }

        public string username { get; set; }

        public string email { get; set; }

        public string bio { get; set; }

        public string location { get; set; }

        public DateTime createdAt { get; set; }
    }
}