﻿using System;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks;
using System.Linq;

using FifthBot.Resources.Database;
using FifthBot.Core.Utils;

using Discord;
using Discord.WebSocket;



namespace FifthBot.Core.Data
{
    public static class DataMethods
    {
        public static int GetStones(ulong UserId)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Stones.Where(x => x.UserId == UserId).Count() < 1)
                {
                    return 0;
                }
                return DbContext.Stones.Where(x => x.UserId == UserId).Select(x => x.Amount).FirstOrDefault();
            }
        }

        public static async Task SaveStones(ulong UserId, int Amount)
        {
            using (var DbContext = new SqliteDbContext())
            {

                if (DbContext.Stones.Where(x => x.UserId == UserId).Count() < 1)
                {
                    // this user doesn't have a row yet, create one for him
                    DbContext.Stones.Add(new Stone
                    {
                        UserId = UserId,
                        Amount = Amount
                    });
                }
                else
                {
                    Stone Current = DbContext.Stones.Where(x => x.UserId == UserId).FirstOrDefault();
                    Current.Amount += Amount;
                    DbContext.Stones.Update(Current);
                }

                await DbContext.SaveChangesAsync();
            }
        }
        public static (ulong, string) getAttacker(ulong TargetId)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Attacks.Where( x => x.TargetId == TargetId).Count() < 1)
                {
                    return (0, "");
                }
                else
                {
                    ulong AttackerID = DbContext.Attacks.Where(x => x.TargetId == TargetId).FirstOrDefault().AttackerId;

                    string Name = DbContext.Attacks.Where(x => x.TargetId == TargetId).FirstOrDefault().Name;

                    return (AttackerID, Name);

                }
            }
        }

        public static async Task SaveAttack(ulong AttackerId, ulong TargetId, string Name )
        {
            using (var DbContext = new SqliteDbContext())
            {
                DbContext.Attacks.Add(new Attack
                {
                    AttackerId = AttackerId,
                    TargetId = TargetId,
                    Name = Name,
                    DateandTime = DateTime.Now

                });

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task RemoveAttack(ulong TargetId)
        {
            using (var DbContext = new SqliteDbContext())
            {
                DbContext.Attacks.Remove(DbContext.Attacks.Where(x => x.TargetId == TargetId).FirstOrDefault());
                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task RemoveOldAttacks()
        {
            using (var DbContext = new SqliteDbContext())
            {
                DateTime CurrentDT = DateTime.Now;

                DbContext.Attacks.RemoveRange
                    (
                    DbContext.Attacks.Where(x => (CurrentDT - x.DateandTime).TotalSeconds > 30)
                    );
                await DbContext.SaveChangesAsync();
            }
        }



        public static ServerSetting GetServer (ulong serverID)
        {
            using (var DbContext = new SqliteDbContext())
            {

                if (DbContext.ServerSettings.Where(x => x.ServerID == serverID).Count() < 1)
                {
                    Console.WriteLine("null lol");
                    return null;
                }

                Console.WriteLine("not null lol");
                return DbContext.ServerSettings.Where(x => x.ServerID == serverID).FirstOrDefault();

            }

        }






        public static async Task AddServer(ulong serverID, string serverName /*, int purgerInterval = 50, string mutedRole = "Muted", string[] adminGroups = null, string[] modGroups = null, string[] introChannels = null*/ ) 
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.ServerSettings.Where(x => x.ServerID == serverID).Count() < 1)
                {
                    DbContext.ServerSettings.Add(new ServerSetting
                    {
                        ServerID = serverID,
                        ServerName = serverName,

                        /*
                        purgerInterval = purgerInterval,
                        mutedRole = "Muted",
                        adminGroups = adminGroups ?? new string[] { "Admins" },
                        modGroups = modGroups ?? new string[] { "Moderators" },
                        introChannels = introChannels ?? new string[] { "male-intros", "female-intros", "other-intros" }
                        */

                    });

                    



                }
                await DbContext.SaveChangesAsync();
            }

        }

        public static List<IntroChannel> GetIntroChannels(ulong serverID)
        {
            using (var DbContext = new SqliteDbContext())
            {

                if (DbContext.IntroChannels.Where(x => x.ServerID == serverID).Count() < 1)
                {
                    return null;
                }

                return DbContext.IntroChannels.Where(x => x.ServerID == serverID).ToList();

            }

        }

        public static ulong[] GetIntroChannelIDs(ulong serverID)
        {
            using (var DbContext = new SqliteDbContext())
            {

                if (DbContext.IntroChannels.Where(x => x.ServerID == serverID).Count() < 1)
                {
                    return null;
                }


                var channelRecords = DbContext.IntroChannels.Where(x => x.ServerID == serverID).ToList();

                List<ulong> channelIDs = new List<ulong>();

                foreach (IntroChannel channelRecord in channelRecords)
                {
                    channelIDs.Add(channelRecord.ChannelID);
                }

                return channelIDs.ToArray();



            }

        }

        public static async Task AddIntroChannel(ulong serverID, ulong channelID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.IntroChannels.Where(x => x.ChannelID == channelID).Count() < 1)
                {
                    DbContext.IntroChannels.Add(new IntroChannel
                    {
                        ServerID = serverID,
                        ChannelID = channelID,

                    });


                }
                await DbContext.SaveChangesAsync();
            }




        }

        public static async Task<bool> AddKink(string kinkName, string kinkDesc)
        {
            Console.WriteLine("we're getting to addkink");
            using (var DbContext = new SqliteDbContext())
            {


                if (DbContext.Kinks.Any(x => x.KinkName.Equals(kinkName, StringComparison.OrdinalIgnoreCase) ))
                {
                    return false;

                }

                DbContext.Kinks.Add(new Kink
                {
                    KinkName = kinkName,
                    KinkDesc = kinkDesc,

                });

                Console.WriteLine("we're getting to save changes async");
                await DbContext.SaveChangesAsync();

                return true;
            }

        }

        public static async Task AddGroup(string groupName, string groupDesc)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (!DbContext.KinkGroups.Any(x => x.KinkGroupName == groupName))
                {
                    DbContext.KinkGroups.Add(new KinkGroup
                    {
                        KinkGroupName = groupName,
                        KinkGroupDescrip = groupDesc,

                    });
                }




                await DbContext.SaveChangesAsync();
            }

        }

        public static Kink GetKink(string kinkName)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if( DbContext.Kinks.Where(x => x.KinkName.Equals(kinkName)).Count() < 1)
                {
                    return null;
                }

                return DbContext.Kinks.Where(x => x.KinkName.Equals(kinkName)).FirstOrDefault();


            }

        }

        public static KinkGroup GetGroup(string groupName)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.KinkGroups.Where(x => x.KinkGroupName.Equals(groupName)).Count() < 1)
                {
                    return null;
                }

                return DbContext.KinkGroups.Where(x => x.KinkGroupName.Equals(groupName)).FirstOrDefault();


            }

        }

        public static List<Kink> GetKinkList()
        {
            using (var DbContext = new SqliteDbContext())
            {


                if (DbContext.Kinks.Count() < 1)
                {
                    return null;
                }

                return DbContext.Kinks.ToList();


            }

        }

        public static List<Kink> GetKinksInGroup(ulong groupID)
        {
            using (var DbContext = new SqliteDbContext())
            {


                if (DbContext.Kinks.Where(x => x.KinkGroupID == groupID).Count() < 1)
                {
                    return null;
                }

                return DbContext.Kinks.Where(x => x.KinkGroupID == groupID).ToList();


            }

        }


        public static List<KinkWithEmoji> GetKinksInGroupWithEmojis(ulong groupID, ulong serverID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var myKinksList = DbContext.Kinks
                .Where
                    (
                        x => x.KinkGroupID == groupID
                    );

                Console.WriteLine("we're getting the kinks from the group");

                var myKinksAndEmojisAsGroupJoin = myKinksList
                .GroupJoin
                    (
                        DbContext.KinkEmojis.Where(sp => sp.ServerID == serverID),
                        Kink => Kink.KinkID,
                        KinkEmoji => KinkEmoji.KinkID,
                        (Kink, KinkEmoji) => new { Kink, KinkEmoji }
                    );

                Console.WriteLine("we're doing the group join");

                var myKEsasFlatList = myKinksAndEmojisAsGroupJoin
                .SelectMany
                    (
                        Kink => Kink.KinkEmoji.DefaultIfEmpty(),
                        (KinkHolder, KinkEmoji) => new KinkWithEmoji
                        {
                            KinkID = KinkHolder.Kink.KinkID,
                            KinkName = KinkHolder.Kink.KinkName,
                            KinkDesc = KinkHolder.Kink.KinkDesc,
                            KinkGroupID = KinkHolder.Kink.KinkGroupID,
                            GroupOrder = KinkHolder.Kink.GroupOrder,
                            ServerID = (KinkEmoji != null) ? KinkEmoji.ServerID : 0,
                            EmojiName = (KinkEmoji != null) ? KinkEmoji.EmojiName : "",
                        }
                    )
                .OrderBy(ke => ke.GroupOrder)
                .ThenBy(ke => ke.KinkName);
                

                Console.WriteLine("we're past the group join, ready to return");

                if (
                        myKEsasFlatList.Count() < 1
                   )
                {
                    return null;
                }

                Console.WriteLine("we're not returning null");

                return myKEsasFlatList.ToList();
                    
             }
        }






        public static List<KinkGroup> GetGroupList()
        {
            using (var DbContext = new SqliteDbContext())
            {


                if (DbContext.KinkGroups.Count() < 1)
                {
                    return null;
                }

                return DbContext.KinkGroups.ToList();


            }

        }

        public static async Task<bool> EditKink(ulong kinkID, string kinkName, string kinkDesc)
        {

            using (var DbContext = new SqliteDbContext())
            {
                if(!DbContext.Kinks.Any(x => x.KinkID == kinkID ))
                {
                    return false;
                }

                Kink kinkToEdit = DbContext.Kinks.Where(x => x.KinkID == kinkID).FirstOrDefault();
                kinkToEdit.KinkName = kinkName;
                kinkToEdit.KinkDesc = kinkDesc;


                await DbContext.SaveChangesAsync();

                return true;

            }

        }

        public static async Task<bool> EditGroup(ulong groupID, string groupName, string groupDesc)
        {

            using (var DbContext = new SqliteDbContext())
            {
                if (!DbContext.KinkGroups.Any(x => x.KinkGroupID == groupID))
                {
                    return false;
                }

                KinkGroup groupToEdit = DbContext.KinkGroups.Where(x => x.KinkGroupID == groupID).FirstOrDefault();
                groupToEdit.KinkGroupName = groupName;
                groupToEdit.KinkGroupDescrip = groupDesc;


                await DbContext.SaveChangesAsync();

                return true;

            }

        }

        public static async Task<bool> AddKinkToGroup(ulong groupID, string kinkName, ulong serverID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                Kink kinkToGroup = DbContext.Kinks.Where(x => x.KinkName == kinkName).FirstOrDefault();

                if (kinkToGroup == null)
                {
                    return false;
                }

                kinkToGroup.KinkGroupID = groupID;

                DbContext.KinkEmojis.RemoveRange( DbContext.KinkEmojis.Where(x => x.ServerID == serverID && x.KinkID == kinkToGroup.KinkID) );


                await DbContext.SaveChangesAsync();

                return true;
            }
        }

        public static async Task AddKinkMenu()
        {
            using (var DbContext = new SqliteDbContext())
            {
                //need to update the following:
                //group records - write kink or limit server/channel

                KinkGroupMenu groupMenu = DbContext.KinkGroupMenus.Where(x => x.KinkGroupID == Vars.menuBuilder.KinkGroupID && x.ServerID == Vars.menuBuilder.ServerID).FirstOrDefault();

                if (groupMenu == null && !Vars.menuBuilder.IsLimitMenu)
                {
                    //groupMenu = new

                    DbContext.KinkGroupMenus.Add(new KinkGroupMenu
                    {
                        KinkGroupID = Vars.menuBuilder.KinkGroupID,
                        ServerID = Vars.menuBuilder.ServerID,
                        KinkMsgID = Vars.menuBuilder.EmojiMenuID,
                        KinkChannelID = Vars.menuBuilder.ChannelID

                    }); ;
                }
                else if (groupMenu == null && Vars.menuBuilder.IsLimitMenu)
                {
                    DbContext.KinkGroupMenus.Add(new KinkGroupMenu
                    {
                        KinkGroupID = Vars.menuBuilder.KinkGroupID,
                        ServerID = Vars.menuBuilder.ServerID,
                        LimitMsgID = Vars.menuBuilder.EmojiMenuID,
                        LimitChannelID = Vars.menuBuilder.ChannelID

                    }); 
                }
                else if(!Vars.menuBuilder.IsLimitMenu)
                {
                    groupMenu.KinkGroupID = Vars.menuBuilder.KinkGroupID;
                    groupMenu.ServerID = Vars.menuBuilder.ServerID;
                    groupMenu.KinkChannelID = Vars.menuBuilder.ChannelID;
                    groupMenu.KinkMsgID = Vars.menuBuilder.EmojiMenuID;
                }
                else if (Vars.menuBuilder.IsLimitMenu)
                {
                    groupMenu.KinkGroupID = Vars.menuBuilder.KinkGroupID;
                    groupMenu.ServerID = Vars.menuBuilder.ServerID;
                    groupMenu.LimitChannelID = Vars.menuBuilder.ChannelID;
                    groupMenu.LimitMsgID = Vars.menuBuilder.EmojiMenuID;
                }

                DbContext.KinkEmojis.RemoveRange(DbContext.KinkEmojis.Where(x => x.ServerID == Vars.menuBuilder.ServerID && Vars.menuBuilder.KinksToUpdate.Any(y => y.KinkID == x.KinkID)));

                foreach(var kinkEmoji in Vars.menuBuilder.KinksToUpdate)
                {
                    DbContext.KinkEmojis.Add(new KinkEmoji
                    {
                        ServerID = Vars.menuBuilder.ServerID,
                        KinkID = kinkEmoji.KinkID,
                        EmojiName = kinkEmoji.EmojiName,
                        KinkGroupID = Vars.menuBuilder.KinkGroupID,
                    });
                }

                await DbContext.SaveChangesAsync();
                ReloadMenus();


            }
            
        }



        public static void ReloadMenus()
        {
            using (var DbContext = new SqliteDbContext() )
            {
                Vars.groupMenus = DbContext.KinkGroupMenus.ToList();

            }
        }

        public static ulong GetKinkFromMenu(KinkGroupMenu menu, IEmote myEmote )
        {
            using (var DbContext = new SqliteDbContext())
            {
                Console.WriteLine("emote name - " + myEmote.Name);
                Console.WriteLine("tostring - " + myEmote.ToString());

                var aKinkEmoji = DbContext.KinkEmojis.Where(x => x.ServerID == menu.ServerID && x.KinkGroupID == menu.KinkGroupID && x.EmojiName.Remove(x.EmojiName.IndexOf("<a") > -1 ? 1 : 0, x.EmojiName.IndexOf("<a") > -1 ? 1 : 0) == myEmote.ToString()).FirstOrDefault();

                if (aKinkEmoji == null)
                {
                    return 0;
                }

                return aKinkEmoji.KinkID;

            }
        }

        public static async Task AddUserKink(ulong userID, ulong kinkID, bool isLimit)
        {
            using (var DbContext = new SqliteDbContext())
            {
                DbContext.UserKinks.RemoveRange(DbContext.UserKinks.Where(x => x.UserID == userID && x.KinkID == kinkID));
                DbContext.UserKinks.Add(new UserKink
                {
                    UserID = userID,
                    KinkID = kinkID,
                    IsLimit = isLimit,
                });

                await DbContext.SaveChangesAsync();

            }
        }

        public static async Task RemoveUserKink(ulong userID, ulong kinkID, bool isLimit)
        {
            using (var DbContext = new SqliteDbContext())
            {
                DbContext.UserKinks.RemoveRange(DbContext.UserKinks.Where(x => x.UserID == userID && x.KinkID == kinkID && x.IsLimit == isLimit ));

                await DbContext.SaveChangesAsync();
            }
        }

        public static (string[], string[]) ValidateKinkNames(string[] parameters)
        {
            //List<string> isAKink = new List<string>();
            //List<string> isNotAKink = new List<string>();

            using (var DbContext = new SqliteDbContext())
            {
                string[] isAKink = parameters.Where(x => DbContext.Kinks.Any(y => x.Equals(y.KinkName, StringComparison.OrdinalIgnoreCase))).ToArray();
                string[] isNotAKink = parameters.Where(x => !DbContext.Kinks.Any(y => x.Equals(y.KinkName, StringComparison.OrdinalIgnoreCase))).ToArray();

                return (isAKink, isNotAKink);

            }

             
        }


        public static List<ulong> GetUserIDsFromKinknames(string[] parameters)
        {
            using (var DbContext = new SqliteDbContext())
            {
                Console.WriteLine(" trying to join ");
                var joinKinksAndNames = DbContext.UserKinks.Join
                    (
                        DbContext.Kinks,
                        a => a.KinkID,
                        b => b.KinkID,
                        (a, b) => new { a.UserID, a.KinkID, a.IsLimit, b.KinkName }
                    );
                //var complete1 = joinKinksAndNames.ToList();

                Console.WriteLine(" trying to group ");
                var groupUp = joinKinksAndNames
                    .GroupBy
                    (
                        c => c.UserID,
                        (myKey, stuff) => new { userKey = myKey, usersKinks = stuff }

                    );
                //var complete2 = groupUp.ToList();



                Console.WriteLine(" trying to where ");
                var narrowDown = groupUp
                    .Where
                    (
                       u =>
                       // every entry in parameters should match something somewhere in userkinks
                       parameters
                       .All(p => u.usersKinks.Any(k => k.KinkName.Equals(p,StringComparison.OrdinalIgnoreCase) && k.IsLimit == false))
                       &&
                       // nothing in parameters should match any of this user's limits
                       !parameters.
                       Any(p => u.usersKinks.Any(k => k.KinkName.Equals(p, StringComparison.OrdinalIgnoreCase) && k.IsLimit == true))
                    );
                //var complete3 = narrowDown.ToList();

                Console.WriteLine(" trying to select ");
                var selectIDs = narrowDown
                    .Select((a) => a.userKey);
             

                Console.WriteLine(" trying to add to list ");
                var addToList = selectIDs
                    .ToList();

                Console.WriteLine(" success ");
                return addToList;



            }



        }
        /*
        public static List<GroupKinks> GetUserKinks(ulong userID)
        {
            return GetUserKinksOrLimits(userID, false);
        }

        public static List<GroupKinks> GetUserLimits(ulong userID)
        {
            return GetUserKinksOrLimits(userID, true);
        }
        */
        public static List<GroupKinks> GetUserKinksAndLimits()
        {
            using (var DbContext = new SqliteDbContext())
            {
                var joinKinksAndGroups = DbContext.Kinks
                    .Join
                    (
                        DbContext.KinkGroups,
                        a => a.KinkGroupID,
                        b => b.KinkGroupID,
                        (a, b) => new
                        {
                            a.KinkID,
                            a.KinkName,
                            a.KinkGroupID,
                            a.KinkDesc,
                            a.GroupOrder,
                            b.KinkGroupName,
                            b.KinkGroupDescrip
                        }
                    );

                    var groupByGroup = joinKinksAndGroups
                    .GroupBy
                    (
                        x => new
                        {
                            x.KinkGroupID,
                            x.KinkGroupName,
                            x.KinkGroupDescrip,

                        },
                        x => new Kink
                        {
                            KinkGroupID = x.KinkGroupID,
                            KinkID = x.KinkID,
                            KinkName = x.KinkName,
                            KinkDesc = x.KinkDesc,
                            GroupOrder = x.GroupOrder,
                        },
                        (kgl, kinkList) => new GroupKinks
                        {
                            isLimit = false,
                            Group = new KinkGroup()
                            {
                                KinkGroupID = kgl.KinkGroupID,
                                KinkGroupDescrip = kgl.KinkGroupDescrip,
                                KinkGroupName = kgl.KinkGroupName
                            },
                            KinksForGroup = kinkList
                            .OrderBy(theKink => theKink.GroupOrder)
                            .ThenBy(theKink => theKink.KinkName)
                            .ToList()
                        }


                    )
                    .ToList();

                return groupByGroup;

            }
        }

        public static List<GroupKinks> GetUserKinksAndLimits(ulong userID)
        {
            using (var DbContext = new SqliteDbContext())
            {

                var userKinks = DbContext.UserKinks.Where(x => x.UserID == userID);

                var joinKinksAndNames = userKinks
                    .Join
                    (
                        DbContext.Kinks,
                        a => a.KinkID,
                        b => b.KinkID,
                        (a, b) => new { a.UserID, a.KinkID, a.IsLimit, b.KinkName, b.KinkDesc, b.KinkGroupID, b.GroupOrder }
                    );

                var joinKinksAndGroups = joinKinksAndNames
                    .Join
                    (
                        DbContext.KinkGroups,
                        a => a.KinkGroupID,
                        b => b.KinkGroupID,
                        (a, b) => new { a.UserID, a.KinkID, a.IsLimit, a.KinkName, a.KinkGroupID, a.KinkDesc, a.GroupOrder, b.KinkGroupName, b.KinkGroupDescrip }
                    );

                var groupByGroup = joinKinksAndGroups
                    .GroupBy
                    (
                        x => new
                        {
                            x.KinkGroupID,
                            x.KinkGroupName,
                            x.KinkGroupDescrip,
                            x.IsLimit,

                        },
                        x => new Kink
                        {
                            KinkGroupID = x.KinkGroupID,
                            KinkID = x.KinkID,
                            KinkName = x.KinkName,
                            KinkDesc = x.KinkDesc,
                            GroupOrder = x.GroupOrder,
                        },
                        (kgl, kinkList) => new GroupKinks
                        {
                            isLimit = kgl.IsLimit,
                            Group = new KinkGroup()
                            {
                                KinkGroupID = kgl.KinkGroupID,
                                KinkGroupDescrip = kgl.KinkGroupDescrip,
                                KinkGroupName = kgl.KinkGroupName
                            },
                            KinksForGroup = kinkList
                            .OrderBy( theKink => theKink.GroupOrder)
                            .ThenBy( theKink => theKink.KinkName )
                            .ToList()
                        }


                    )
                    .ToList();

                return groupByGroup;


            }






        }

        public static (KinkGroupMenu, string) getMenuRecord(ulong menuID, ulong serverID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                var aMenu = DbContext.KinkGroupMenus.Where(x => x.ServerID == serverID && (x.LimitMsgID == menuID || x.KinkMsgID == menuID)).FirstOrDefault();
                if(aMenu == null)
                {
                    return (null, null);
                }

                string aGroupName = DbContext.KinkGroups.Where(x => x.KinkGroupID == aMenu.KinkGroupID).FirstOrDefault().KinkGroupName;



                return (aMenu, aGroupName);


                // so what I need here is to get both the menu group ID and whether it's a
                // limit or a kink menu, hmmm


            }
        }


    }
}
