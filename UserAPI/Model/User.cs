using System;

namespace UserAPI.Model
{
    public class User
    {
        public User()
        {

        }

        public int id { get; set; }

        public string username { get; set; }

        public string email { get; set; }

        public string password { get; set; }

        public string bio { get; set; }

        public string location { get; set; }

        public DateTime createdAt { get; set; }
    }
}