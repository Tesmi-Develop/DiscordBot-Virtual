using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot_Virtual
{
    internal class Data
    {
        public Dictionary<ulong, ulong> ChannelServersIdeas { get; set; }

        public Data(Dictionary<ulong, ulong> channelServersIdeas)
        {
            ChannelServersIdeas = channelServersIdeas;
        }
    }
}
