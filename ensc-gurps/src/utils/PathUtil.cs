using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ensc_gurps.utils
{
    public class PathUtil
    {
        private static string _universe;
        private static string _adventure;
        private static string _root = "../.."; // from debug

        public static void SetRoot(string r)
        {
            _root = r;
        }

        public static void SetUniverse(string u)
        {
            _universe = u;
        }

        public static void SetAdventure(string a)
        {
            _adventure = a;
        }

        public static string GetUniversePath()
        {
            return _root + "/universes/" + _universe;
        }

        public static string GetDataPath(string file)
        {
            return GetUniversePath() + "/" + file;
        }

        public static string GetAdventurePath(string file)
        {
            return GetDataPath("adventures/" + _adventure + "/") + file;
        }

        public static string GetSavegamePath(string file)
        {
            return GetAdventurePath("savegame") + "/" + file;
        }

        public static List<string> GetUniverses()
        {
            return EnumerateDirs(_root + "/universes/");
        }

        public static List<string> GetAdventures()
        {
            return EnumerateDirs(GetDataPath("/adventures/"));
        }

        public static List<string> GetSaveGames()
        {
            return EnumerateFiles(GetAdventurePath("savegame") + "/");
        }

        private static List<string> EnumerateDirs(string path)
        {
            List<string> dirs = new List<string>(Directory.EnumerateDirectories(path));

            for (int i = 0; i < dirs.Count; i++)
                dirs[i] = dirs[i].Substring(dirs[i].LastIndexOf("/") + 1);

            return dirs;
        }

        private static List<string> EnumerateFiles(string path)
        {
            List<string> files = new List<string>(Directory.EnumerateFiles(path));

            for (int i = 0; i < files.Count; i++)
            {
                files[i] = files[i].Substring(files[i].LastIndexOf("/") + 1).Replace(".xml", "");
            }

            return files;
        }
    }
}
