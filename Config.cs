using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot_Virtual
{
    internal class Config
    {
        public string Token { get; set; }

        public Config(string token)
        {
            Token = token;
    }
    }
}
