﻿// <auto-generated />
using System;
using FifthBot.Resources.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FifthBot.Migrations
{
    [DbContext(typeof(SqliteDbContext))]
    [Migration("20190818084454_addGIDtoKE")]
    partial class addGIDtoKE
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity("FifthBot.Resources.Database.Attack", b =>
                {
                    b.Property<ulong>("MessageID")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("AttackerId");

                    b.Property<string>("Category");

                    b.Property<DateTime>("DateandTime");

                    b.Property<string>("Name");

                    b.Property<ulong>("TargetId");

                    b.HasKey("MessageID");

                    b.ToTable("Attacks");
                });

            modelBuilder.Entity("FifthBot.Resources.Database.Command", b =>
                {
                    b.Property<ulong>("CommandID")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("ActorID");

                    b.Property<ulong>("ChannelID");

                    b.Property<string>("CommandData");

                    b.Property<string>("CommandName");

                    b.Property<int>("CommandStep");

                    b.Property<DateTime>("DateTime");

                    b.Property<ulong>("MessageID");

                    b.Property<ulong>("TargetID");

                    b.HasKey("CommandID");

                    b.ToTable("Commands");
                });

            modelBuilder.Entity("FifthBot.Resources.Database.IntroChannel", b =>
                {
                    b.Property<ulong>("ChannelID")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("ServerID");

                    b.HasKey("ChannelID");

                    b.ToTable("IntroChannels");
                });

            modelBuilder.Entity("FifthBot.Resources.Database.JoinedKinkUser", b =>
                {
                    b.Property<ulong>("JoinID")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsLimit");

                    b.Property<ulong>("KinkID");

                    b.Property<ulong>("UserID");

                    b.HasKey("JoinID");

                    b.ToTable("JoinedKinksUsers");
                });

            modelBuilder.Entity("FifthBot.Resources.Database.Kink", b =>
                {
                    b.Property<ulong>("KinkID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("KinkDesc");

                    b.Property<ulong>("KinkGroupID");

                    b.Property<string>("KinkName");

                    b.HasKey("KinkID");

                    b.ToTable("Kinks");
                });

            modelBuilder.Entity("FifthBot.Resources.Database.KinkEmoji", b =>
                {
                    b.Property<ulong>("JoinID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("EmojiName");

                    b.Property<ulong>("KinkGroupID");

                    b.Property<ulong>("KinkID");

                    b.Property<ulong>("ServerID");

                    b.HasKey("JoinID");

                    b.ToTable("KinkEmojis");
                });

            modelBuilder.Entity("FifthBot.Resources.Database.KinkGroup", b =>
                {
                    b.Property<ulong>("KinkGroupID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("KinkGroupDescrip");

                    b.Property<string>("KinkGroupName");

                    b.HasKey("KinkGroupID");

                    b.ToTable("KinkGroups");
                });

            modelBuilder.Entity("FifthBot.Resources.Database.KinkGroupMenu", b =>
                {
                    b.Property<ulong>("JoinID")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("KinkChannelID");

                    b.Property<ulong>("KinkGroupID");

                    b.Property<ulong>("KinkMsgID");

                    b.Property<ulong>("LimitChannelID");

                    b.Property<ulong>("LimitMsgID");

                    b.Property<ulong>("ServerID");

                    b.HasKey("JoinID");

                    b.ToTable("KinkGroupMenus");
                });

            modelBuilder.Entity("FifthBot.Resources.Database.ServerSetting", b =>
                {
                    b.Property<ulong>("ServerID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ServerName");

                    b.HasKey("ServerID");

                    b.ToTable("ServerSettings");
                });

            modelBuilder.Entity("FifthBot.Resources.Database.Stone", b =>
                {
                    b.Property<ulong>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Amount");

                    b.HasKey("UserId");

                    b.ToTable("Stones");
                });

            modelBuilder.Entity("FifthBot.Resources.Database.User", b =>
                {
                    b.Property<ulong>("UserID")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("ServerID");

                    b.HasKey("UserID");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
