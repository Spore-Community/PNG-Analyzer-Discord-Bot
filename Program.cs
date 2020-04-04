using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

// Invite link: https://discordapp.com/oauth2/authorize?client_id=502450987643043857&scope=bot

namespace DiscordBot
{
    public class Program
    {
        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            var client = new DiscordSocketClient();

            client.Log += Log;
            client.MessageReceived += MessageReceived;

            string token = "DISCORD TOKEN GOES HERE";
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg);
            return Task.CompletedTask;
        }

        public async Task MessageReceived(SocketMessage message)
        {
            foreach(var attachment in message.Attachments)
            {
                if(attachment.Height==128 && attachment.Width==128){

                    var filePath = "C:\\Users\\KyleN\\OneDrive\\Documents\\Spore Files\\PNG Decoder\\Cached\\"+attachment.Filename;
                    Console.WriteLine($"Downloading {attachment.Url} and saving to {filePath}");
                    using (var webClient = new WebClient())
                    {
                        var uri = new Uri(attachment.Url);
                        webClient.DownloadProgressChanged += wc_DownloadProgressChanged;
                        try{
                            webClient.DownloadFile(uri, filePath);
                        }
                        catch(Exception e){
                            Console.WriteLine(e.Message);
                        }
                    }
                    try{
                        var asset = new Asset(filePath);
                        await message.Channel.SendMessageAsync(asset.getInfo());
                        if(message.Content.Contains("!xml")) await message.Channel.SendFileAsync(filePath+".xml");
                        if(message.Content.Contains("!advanced")) await message.Channel.SendMessageAsync(asset.getInfoAdvanced());
                        if(message.Content.Contains("!api")) await message.Channel.SendMessageAsync(asset.getApiUrls());
                        if(message.Content.Contains("!pollinator")) await message.Channel.SendMessageAsync(asset.getPollinatorUrls());
                    }
                    catch{
                        await message.Channel.SendMessageAsync($"That creation couldn't be recognized.");
                    }

                }
            }
        }

        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.WriteLine($"{e.ProgressPercentage} ({e.BytesReceived}/{e.TotalBytesToReceive})");
        }
    }
}
