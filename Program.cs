// See https://aka.ms/new-console-template for more information
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.VisualBasic;

namespace DiscordBot
{

    public class Program
    {
        private DiscordSocketClient _client;
        public static Task Main(string[] args) => new Program().MainAsync();


        public async Task MainAsync()
        {
            var config = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.All
            };
            _client = new DiscordSocketClient(config);
            _client.Log += Log;

            DotNetEnv.Env.Load();
            var token = Environment.GetEnvironmentVariable("token");

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            // Aqui embaixo vai os eventos
            _client.MessageUpdated += MessageUpdated;
            _client.MessageReceived += MessageReceived;
            _client.UserJoined += UserJoined;
            _client.ReactionAdded += Reaction;
            _client.UserLeft += UserGetOut;


            await Task.Delay(-1);
        }

        private Task UserGetOut(SocketGuild guild, SocketUser user)
        {
            ulong channelIdToUserGetOut = 1154207411419222097;
            var channel = _client.GetChannel(channelIdToUserGetOut) as IMessageChannel;

            if (channel != null)
            {
                channel.SendMessageAsync($"O usuário {user.Username} saiu do servidor!");

            }
            return Task.CompletedTask;
        }


        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task Reaction(Cacheable<IUserMessage, ulong> msg, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
        {
            var message = await msg.GetOrDownloadAsync();

            Console.WriteLine($"TESTE: \n Autor: {message.Author.Username} | CANAL: {message.Channel.Name} \n Conteúdo: {message.Content}\n Reação: {reaction.Emote} | by: {reaction.User.Value.Username}");
        }

        private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            var message = await before.GetOrDownloadAsync();
            Console.WriteLine($"Uma mensagem foi editada! {message} => {after}");
        }
        private async Task MessageReceived(SocketMessage msg)
        {

            // if (msg.Author.Id == _client.CurrentUser.Id) return;
            if (msg.Author.Id != _client.CurrentUser.Id)
            {

                var channel = _client.GetChannel(msg.Channel.Id) as IMessageChannel;
                await channel.SendMessageAsync($"Recebi sua mensagem");

            }

        }

        private async Task UserJoined(SocketGuildUser user)
        {
            ulong channelIdToWelcome = 1154079976040173640;
            var channel = _client.GetChannel(channelIdToWelcome) as IMessageChannel;

            if (channel != null)
            {
                await channel.SendMessageAsync($"Seja muito bem-vindo(a) ao nosso servidor, {user.Mention}!");

            }

        }


        // var channel = msg.Channel as SocketGuildChannel;
        // Console.WriteLine($"[{msg.CreatedAt.DateTime.ToLongTimeString()}] Servidor: {channel.Guild}\n no canal: {channel.Name}\n {msg.Author} disse: {msg.Content}");
        // return Task.CompletedTask;




    }
}
