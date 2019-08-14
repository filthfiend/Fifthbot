﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using FifthBot.Resources.Datatypes;
using FifthBot.Resources.Settings;
using FifthBot.Core.Data;
using FifthBot.Core.Utils;
using FifthBot.Resources.Database;

using Newtonsoft.Json;

namespace FifthBot
{
    class Program
    {


        private DiscordSocketClient Client;
        private CommandService Commands;

        static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            string JSON = "";
            string SettingsLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).Replace(@"bin\Debug\netcoreapp2.2",
                @"\Data\Settings.json");

            /*
            if (!File.Exists(SettingsLocation))
            {

            }
            */
            using (var fileStream = new FileStream(SettingsLocation, FileMode.Open, FileAccess.Read))
            using (var ReadSettings = new StreamReader(fileStream))
            {
                JSON = ReadSettings.ReadToEnd();
            }

            Setting Settings = JsonConvert.DeserializeObject<Setting>(JSON);

            String token = Settings.token;




            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug,
                
            });


            Commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug,
            });

            Client.MessageReceived += Client_MessageReceived;
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
                
                //AddModulesAsync(Assembly.GetEntryAssembly());

            Client.Ready += Client_Ready;
            Client.Log += Client_Log;

            Client.JoinedGuild += Client_JoinedGuild;



            Client.ReactionAdded += Client_ReactionAdded;

            /*
            string Token = "";
            using (var Stream = new FileStream((Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)).Replace(@"bin\Debug\netcoreapp2.1", @"Data\Token.txt"), FileMode.Open, FileAccess.Read))
            using (var ReadToken = new StreamReader(Stream))
            {
                Token = ReadToken.ReadToEnd();
            }
            */



            await Client.LoginAsync(TokenType.Bot, Settings.token);
            await Client.StartAsync();











            await Task.Delay(-1);
        }

        private async Task Client_JoinedGuild(SocketGuild arg)
        {
            var botGuilds = Client.Guilds.ToList();

            foreach (SocketGuild botGuild in botGuilds)
            {
                var serverRecord = DataMethods.GetServer(botGuild.Id);

                if (serverRecord == null)
                {
                    await DataMethods.AddServer(botGuild.Id, botGuild.Name);

                    Console.WriteLine("server added - " + botGuild.Name);

                    var introChannels = botGuild.TextChannels.Where(x => x.Name.Contains("intros")).ToList();

                    Console.WriteLine("wrote list of channels");

                    foreach (SocketTextChannel introChannel in introChannels)
                    {
                        await DataMethods.AddIntroChannel(botGuild.Id, introChannel.Id);
                    }

                    Console.WriteLine("added channels to database");

                }
            }
        }

        private Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            var socketReact = arg3;

            if (Vars.menuBuilder.IsActive)
            {
                //SocketGuildUser user = Client.guild
            















            }

















            throw new NotImplementedException();
        }

        private async Task Client_Log(LogMessage Message)
        {
            Console.WriteLine($"{DateTime.Now} at {Message.Source} {Message.Message}");
        }

        private async Task Client_Ready()
        {
            await Client.SetGameAsync("Fuck you, that's my name!", "http://www.google.com", ActivityType.Watching);

            var botGuilds = Client.Guilds.ToList();

            foreach(SocketGuild botGuild in botGuilds)
            {
                var serverRecord = DataMethods.GetServer(botGuild.Id);

                if (serverRecord == null)
                {
                    await DataMethods.AddServer(botGuild.Id, botGuild.Name);

                    Console.WriteLine("server added - " + botGuild.Name );

                    var introChannels = botGuild.TextChannels.Where( x => x.Name.Contains("intros")).ToList();

                    Console.WriteLine("wrote list of channels");

                    foreach( SocketTextChannel introChannel in introChannels )
                    {
                        await DataMethods.AddIntroChannel(botGuild.Id, introChannel.Id);
                    }

                }

            }

        }

        private async Task Client_MessageReceived(SocketMessage MessageParam)
        {
            var Message = MessageParam as SocketUserMessage;

            var Context = new SocketCommandContext(Client, Message);

            if (Context.Message == null || Context.Message.Content == "") return;
            if (Context.User.IsBot) return;


            int ArgPos = 0;
            if (!(Message.HasStringPrefix("!!", ref ArgPos) || Message.HasMentionPrefix(Client.CurrentUser, ref ArgPos)))
            {
                var guildUser = Context.User as SocketGuildUser;

                if (!(

                    guildUser.GuildPermissions.Administrator ||
                    guildUser.Roles.Any(x => x.Name == "Seven Deadly Sins") ||
                    guildUser.Roles.Any(x => x.Name == "Tester")
                    
                    ))
                {
                    return;
                }

                // to AddKinkData
                if(Vars.activeCommands.Where(x => x.ActorID == Context.User.Id && x.CommandName == "addkink" && x.ChannelID == Context.Channel.Id).Count() > 0)
                {
                    UtilMethods kinkToAdd = new UtilMethods();

                    Console.WriteLine(" we're entering AddKinkData ");
                    await kinkToAdd.AddKinkData(Context);
                }

                // to AddGroupData
                if (Vars.activeCommands.Where(x => x.ActorID == Context.User.Id && x.CommandName == "addgroup" && x.ChannelID == Context.Channel.Id).Count() > 0)
                {
                    UtilMethods groupToAdd = new UtilMethods();

                    Console.WriteLine(" we're entering AddGroupData ");
                    await groupToAdd.AddGroupData(Context);
                }

                // to EditKinkData
                if (Vars.activeCommands.Where(x => x.ActorID == Context.User.Id && x.CommandName == "editkink" && x.ChannelID == Context.Channel.Id).Count() > 0)
                {
                    UtilMethods kinkToEdit = new UtilMethods();

                    Console.WriteLine(" we're entering EditKinkData ");
                    await kinkToEdit.EditKinkData(Context);
                }

                // to EditGroupData
                if (Vars.activeCommands.Where(x => x.ActorID == Context.User.Id && x.CommandName == "editgroup" && x.ChannelID == Context.Channel.Id).Count() > 0)
                {
                    UtilMethods groupToEdit = new UtilMethods();

                    Console.WriteLine(" we're entering EditGroupData ");
                    await groupToEdit.EditGroupData(Context);
                }

                // to GroupMenuCreator
                if (
                    Vars.menuBuilder.IsActive && 
                    Vars.menuBuilder.ChannelID == Context.Channel.Id &&
                    Vars.menuBuilder.UserID == Context.User.Id
                    )
                {
                    MenuCreateMethods menuToCreate = new MenuCreateMethods();

                    Console.WriteLine(" we're entering GroupMenuCreator ");
                    await menuToCreate.GroupMenuCreator(Context);
                }


                /*
                if (!(Vars.usersAddingKinks.Where(x => x == Context.User.Id).Count() > 0) )
                {
                    return;
                }

                UtilMethods kinkToAdd = new UtilMethods();

                await kinkToAdd.AddKinkData(Context);
                */


                return;

            }


            var Result = await Commands.ExecuteAsync(Context, ArgPos, null);
            if (!Result.IsSuccess)
            {
                Console.WriteLine($"{DateTime.Now} at Commands] Something went wrong executing a command. Text: {Context.Message.Content} | Error: {Result.ErrorReason}");
            }


        }
    }
}
