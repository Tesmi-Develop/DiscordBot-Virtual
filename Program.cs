using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace DiscordBot_Virtual
{
    public class Programm
    {
        private DiscordSocketClient client = new DiscordSocketClient();

        private Data data;
        private Config config;

        private string pathData = Path.Combine(Environment.CurrentDirectory, "data.json");
        private string pathConfig = Path.Combine(Environment.CurrentDirectory, "config.json");

        private static void Main(string[] args) => new Programm().MainAsync();

        public async Task MainAsync()
        {
            client.MessageReceived += CommandsHandler;
            client.Log += Log;
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomainProcessExit);

            config = ReadConfig();
            data = ReadData();

            string token = config.Token ?? "";

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            Console.ReadKey();
        }

        private async Task CommandsHandler(SocketMessage message)
        {
            if (message.Author.IsBot) return;

            string[] splitingResult = message.Content.Split(' ');

            SocketGuildUser user = (SocketGuildUser) message.Author;
            SocketGuild server = user.Guild;
            IChannel channel = message.Channel;

            if (user == null || server == null) return;

            switch (splitingResult[0])
            {
                case "!поменятьКанал":
                    if (user.Guild.Owner != user)
                    {
                        await message.Channel.SendMessageAsync("Только владелец может использовать эту команду");
                        break;
                    }

                    if (!data.ChannelServersIdeas.ContainsKey(server.Id))
                    {
                    data.ChannelServersIdeas.Add(server.Id, message.Channel.Id);
                    }

                    data.ChannelServersIdeas[server.Id] = message.Channel.Id;

                    await message.Channel.SendMessageAsync("Канал был изменён");

                    await message.DeleteAsync();

                    break;

                case "!идея":
                    if (!data.ChannelServersIdeas.TryGetValue(server.Id, out ulong channelIdeasId))
                    {
                        await message.Channel.SendMessageAsync("Для начало нужно установить канал команндой: \"!ПоменятьКанал\"");
                        break;
                    }

                    IMessageChannel? channelIdeas = (IMessageChannel?)await client.GetChannelAsync(channelIdeasId);

                    if (channelIdeas == null)
                    {
                        await message.Channel.SendMessageAsync("Для начало нужно установить канал команндой: \"!ПоменятьКанал\"");
                        break;
                    }

                    if (splitingResult.Length == 1)
                    {
                        await message.Channel.SendMessageAsync("Неверное количество аргументов: !идея <идея>");
                        break;
                    }

                    string messageForSend = "";

                    for (int i = 1; i < splitingResult.Length; i++)
                    {
                        messageForSend += $"{splitingResult[i]} ";
                    }

                    IUserMessage sentMessage = await channelIdeas.SendMessageAsync(messageForSend);

                    await sentMessage.AddReactionAsync(new Emoji("✅"));
                    await sentMessage.AddReactionAsync(new Emoji("❌"));

                    await message.DeleteAsync();
                    break;
                    
            }
            return;
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private void SaveData(Data data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);

            File.WriteAllText(pathData, json);
        }
        private Data ReadData()
        {
            if (!File.Exists(pathData)) return new Data(new Dictionary<ulong, ulong>());

            return JsonConvert.DeserializeObject<Data>(File.ReadAllText(pathData));
        }

        private void CurrentDomainProcessExit(object sender, EventArgs e)
        {
            SaveData(data);
            Console.WriteLine("exit");
        }
        
        private Config ReadConfig()
        {
            if (!File.Exists(pathConfig)) 
            {
                string config = JsonConvert.SerializeObject(new Config(""), Formatting.Indented);
                File.WriteAllText(pathConfig, config);

                Console.WriteLine(1);

                return new Config("");
            }

            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(pathConfig));
        }
    }
}