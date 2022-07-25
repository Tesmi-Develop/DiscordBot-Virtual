using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DiscrodBot
{
    public class Programm
    {
        DiscordSocketClient client;
        ulong ChannelId;
        static void Main(string[] args) => new Programm().MainAsync();

        public async Task MainAsync()
        {
            client = new DiscordSocketClient();
            client.MessageReceived += CommandsHandler;
            client.Log += Log;

            Console.WriteLine("Введите токен бота");
            var token = Console.ReadLine();

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            //await Task.Delay(-1);

            Console.ReadKey();
        }

        private async Task CommandsHandler(SocketMessage message)
        {
            if (message.Author.IsBot) return;

            string[] splitingResult = message.Content.Split(' ');

            switch (splitingResult[0])
            {
                case "!ПоменятьКанал":
                    {
                        var user = message.Author as SocketGuildUser;

                        if (user == null) break;

                        if (user.Guild.Owner != user)
                        {
                            await message.Channel.SendMessageAsync("Только владелец может использовать эту команду");
                            break;
                        }

                        ChannelId = message.Channel.Id;

                        await message.Channel.SendMessageAsync("Канал был изменён");

                        break;
                    }
                case "!идея":
                    {
                        var channel = await GetChannel(ChannelId);

                        if (channel == null)
                        {
                            await message.Channel.SendMessageAsync("Для начало нужно установить канал !ПоменятьКанал");
                            break;
                        }

                        if (channel is IMessageChannel msgChannel)
                        {
                            if (splitingResult.Length == 1)
                            {
                                await msgChannel.SendMessageAsync("Неверные аргументы, !идея (ваша идея)");
                                break;
                            }

                            string messageForSend = "";

                            for (int i = 1; i < splitingResult.Length; i++)
                            {
                                messageForSend += $"{splitingResult[i]} ";
                            }

                            IUserMessage sentMessage = await msgChannel.SendMessageAsync(messageForSend);

                            await sentMessage.AddReactionAsync(new Emoji("✅"));
                            await sentMessage.AddReactionAsync(new Emoji("❌"));
                        }

                        break;
                    }
            }
            return;
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async ValueTask<IChannel?> GetChannel(ulong id)
        {
            if (id == 0) return null;

            var channel = await client.GetChannelAsync(id);

            return channel;
        }
    }
}