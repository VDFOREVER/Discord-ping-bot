using System.Net.NetworkInformation;
using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace pingbot
{
    class bot
    {
        Ping p = new Ping();
        DiscordSocketClient client;
        string curFile = @"./config.json";
        static void Main(string[] args) => new bot().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            if (File.Exists(curFile) == true)
            {
                dynamic jsonfile = JsonConvert.DeserializeObject(File.ReadAllText(curFile));
                client = new DiscordSocketClient();
                //client.MessageReceived += CommandsHandler;
                client.Log += Log;
                var token = $"{jsonfile["BotToken"]}";
                await client.LoginAsync(TokenType.Bot, token);
                await client.StartAsync();
                client.Ready += _client_Ready1;
                await PingIP();
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Сonfig file not found");
                Console.ReadKey();
            }
        }
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private Task CommandsHandler(SocketMessage msg)
        {
            throw new NotImplementedException();
        }

        private static IPStatus PingStatus;

        public async Task PingIP()
        {
            int offline = 0;
            int timeout = 10000;
            dynamic jsonfile = JsonConvert.DeserializeObject(File.ReadAllText("./config.json"));
            string IPaddress = $"{jsonfile["ServerIP"]}";
            Ping s = new Ping();
            while (true)
            {
                PingStatus = await s.SendPingAsync(IPaddress).ContinueWith(pingTask => pingTask.Result.Status);
                Console.WriteLine(PingStatus);
                if (PingStatus.ToString() == "TimedOut")
                {
                    offline = 1;
                }

                if (offline == 1 && PingStatus.ToString() == "Success")
                {
                    offline = 0;
                    await _client_Ready();
                }
                await Task.Delay(20000);
            }
        }

        private async Task _client_Ready()
        {
            dynamic jsonfile = JsonConvert.DeserializeObject(File.ReadAllText("./config.json"));
            var guild = client.GetGuild((ulong)jsonfile["GuildId"]); // guild id
            var channel = guild.GetTextChannel((ulong)jsonfile["ChannelId"]); // channel id
            await channel.SendMessageAsync($"({DateTime.Now.ToString("dd/MM/yyyy")})IP is down");
        }

        private async Task _client_Ready1()
        {
            dynamic jsonfile = JsonConvert.DeserializeObject(File.ReadAllText("./config.json"));
            var guild = client.GetGuild((ulong)jsonfile["GuildId"]); // guild id
            var channel = guild.GetTextChannel((ulong)jsonfile["ChannelId"]); // channel id

            await channel.SendMessageAsync("Bot is running");
        }
    }
}