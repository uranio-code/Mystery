using Mystery.Configuration;
using Mystery.Content;
using Mystery.Encryption;
using Mystery.Json;
using Mystery.Register;
using Mystery.Users;
using Mystery.Web;
using MysteryWebLogic.Authetication;
using MysteryWebLogic.Modules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Runtime.InteropServices;
using Mystery.Files;
using Mystery.Instance;
using Mystery.Messaging;
using MysteryDMS.Model;
using MongoDB.Bson;

namespace MysteryConsole
{
    class MysteryHelloWordProgram
    {

        IContentDispatcher cd ;
        IGlobalContentCreator cc ;
        IConfigurationProvider conf_p;
        IFileRepository file_repo;
        MysteryJsonConverter converter;

        public MysteryHelloWordProgram() {
            this.getMystery().AssemblyRegister.Register(typeof(DMSVersion).Assembly);
            cd = this.getGlobalObject<IContentDispatcher>();
            cc = this.getGlobalObject<IGlobalContentCreator>();
            conf_p = this.getGlobalObject<IConfigurationProvider>();
            file_repo = this.getGlobalObject<IFileRepository>();
            converter = this.getGlobalObject<MysteryJsonConverter>();
        }
        
        public void createSomeVersion() {
            

            var titles = new List<string>() {
                "Quality Plan",
                "Requirements Specification",
                "Business Plan",
                "Schedule",
            };

            var statues = new List<string>() {
                "In Work",
                "Under review",
                "Approved"
            };

            var rnd = new Random();
            var users = new List<User>(cd.GetAll<User>());
            var folders  = new List<DMSFolder>(cd.GetAll<DMSFolder>());

            for (var i = 0; i < 20; i++) {
                var ver = cc.getNewContent<DMSVersion>();
                ver.title = titles[rnd.Next(titles.Count)];
                ver.author = users[rnd.Next(users.Count)];
                ver.wf_Status = statues[rnd.Next(statues.Count)];
                ver.version_number = rnd.Next(10);
                var folder = folders[rnd.Next(folders.Count)];
                ver.parent_folders.Add(folder);
                folder.versions.Add(ver);

                cd.Add(ver);
                cd.Add(folder);
                Console.WriteLine(i);
            }
            

            //ver.title = "action!!!";
            //cd.Add(ver);
        }
        public void AddCommentsToAllVersion() {
            var rnd = new Random();
            var users = new List<User>(cd.GetAll<User>());
            var version = new List<DMSVersion>(cd.GetAll<DMSVersion>());

            foreach (var v in version)
            {
                for (var i = 0; i < rnd.Next(10) + 1; i++)
                {
                    var comment = cc.getNewContent<DMSComment>();
                    comment.owner = users[rnd.Next(users.Count)];
                    comment.comment_body = Guid.NewGuid().ToString();
                    comment.parent_dms_content = v;
                    cd.Add(comment);
                }

            }

        }
        public void createSomeFolderTree() {
            var rnd = new Random();
            var users = new List<User>(cd.GetAll<User>());

            var titles = new List<string>() {
                "Quality Folder",
                "Requirements Folder",
                "Business Folder",
                "Schedule Folder",
            };
            //level 0
            foreach(var title in titles){
                var folder = cc.getNewContent<DMSFolder>();
                folder.title = titles[rnd.Next(titles.Count)];
                folder.rowner = users[rnd.Next(users.Count)];
                cd.Add(folder);
                //level 1
                foreach (var title2 in titles)
                {
                    var level1 = cc.getNewContent<DMSFolder>();
                    level1.title = titles[rnd.Next(titles.Count)];
                    level1.rowner = users[rnd.Next(users.Count)];
                    folder.add_subfolder(level1);
                    //level 2
                    foreach (var title3 in titles)
                    {
                        var level2 = cc.getNewContent<DMSFolder>();
                        level2.title = titles[rnd.Next(titles.Count)];
                        level2.rowner = users[rnd.Next(users.Count)];
                        level1.add_subfolder(level2);
                        foreach (var title4 in titles)
                        {
                            var level3 = cc.getNewContent<DMSFolder>();
                            level3.title = titles[rnd.Next(titles.Count)];
                            level3.rowner = users[rnd.Next(users.Count)];
                            level2.add_subfolder(level3);
                        }
                    }
                }
            }
        }

        public void putFloatingVersionsInFolders() {
            var versions = cd.GetAllByFilter<DMSVersion>(x => x.parent_folders == null || x.parent_folders.Count == 0);
            var folders = new List<DMSFolder>( cd.GetAll<DMSFolder>());
            var rnd = new Random();
            
            foreach (var v in versions) {
                var parent = folders[rnd.Next(folders.Count)];
                v.parent_folders.Add(parent);
                parent.versions.Add(v);
                cd.Add(v);
                cd.Add(parent);
            }
        }

        public void cleanDb() {
            var folders = new List<DMSFolder>(cd.GetAll<DMSFolder>());
            foreach (var f in folders) {
                if (f.add_permission_users.Count > 0 || f.edit_permission_users.Count > 0 || f.view_permission_users.Count > 0) {
                    f.add_permission_users = new MultiContentReference<User>();
                    f.edit_permission_users = new MultiContentReference<User>();
                    f.view_permission_users = new MultiContentReference<User>();
                    cd.Add(f);
                }
            }
            var version = new List<DMSVersion>(cd.GetAll<DMSVersion>());
            foreach (var f in version)
            {
                if (f.add_permission_users.Count > 0 || f.edit_permission_users.Count > 0 || f.view_permission_users.Count > 0)
                {
                    f.add_permission_users = new MultiContentReference<User>();
                    f.edit_permission_users = new MultiContentReference<User>();
                    f.view_permission_users = new MultiContentReference<User>();
                    cd.Add(f);
                }
            }
        }

        public void CreateASmallDb()
        {

            var admin = cc.getNewContent<User>();
            admin.account_type = UserType.admin;
            admin.fullname = "admin";
            admin.email = "admin@local.com";
            admin.password = this.getGlobalObject<IAuthetication>().Hashstring("");
            cd.Add(admin);

            var regular = cc.getNewContent<User>();
            regular.account_type = UserType.normal;
            regular.fullname = "regular";
            regular.email = "regular@local.com";
            regular.password = this.getGlobalObject<IAuthetication>().Hashstring("");
            cd.Add(regular);

            var guest = cc.getNewContent<User>();
            guest.account_type = UserType.normal;
            guest.fullname = "guest";
            guest.email = "guest@local.com";
            guest.password = this.getGlobalObject<IAuthetication>().Hashstring("");
            cd.Add(guest);

            var users = new List<User> { admin, regular, guest };

            //2 root folder
            var r1 = cc.getNewContent<DMSFolder>();
            r1.title = "Root folder 1";

            var r2 = cc.getNewContent<DMSFolder>();
            r2.title = "Root folder 2";

            var titles = new List<string>() {
                "Quality Folder",
                "Requirements Folder",
                "Business Folder",
                "Schedule Folder",
            };

            cd.Add(r1);
            cd.Add(r2);

            var rnd = new Random();

            //level 0
            foreach (var title in titles)
            {
                var folder = cc.getNewContent<DMSFolder>();
                folder.title = title;
                folder.rowner = users[rnd.Next(users.Count)];
                folder.parent_folders.Add(r1);
                r1.subfolders.Add(folder);
                cd.Add(folder);
            }

            createSomeVersion();
            AddCommentsToAllVersion();



        }

        public void Main() {

            //CreateASmallDb();
            //createSomeVersion();
            //AddCommentsToAllVersion();
            //createSomeFolderTree();
            //putFloatingVersionsInFolders();
            //cleanDb();

            Console.WriteLine("done");
            Console.ReadLine();

        }

    }


    class Program
    {
        static void Main(string[] args)
        {

            MysteryHelloWordProgram p = new MysteryHelloWordProgram();
            p.Main();


        }
    }
}
