using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Yomu
{
    class Bookmarks
    {
        private Dictionary<string, int> bookmarks;

        public Bookmarks()
        {
            var bookmarkFile = File.Open("bookmarks", FileMode.OpenOrCreate);
            if (bookmarkFile.Length > 0)
            {
                BinaryFormatter bf = new BinaryFormatter();
                bookmarks = bf.Deserialize(bookmarkFile) as Dictionary<string, int>;
                bookmarkFile.Position = 0;
                bookmarkFile.Flush();
            }
            else
            {
                bookmarks = new Dictionary<string, int>();
            }
            bookmarkFile.Close();
        }

        ~Bookmarks()
        {
            var bookmarkFile = File.Open("bookmarks", FileMode.Create);
            bookmarkFile.Seek(0, SeekOrigin.Begin);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(bookmarkFile, bookmarks);
            bookmarkFile.Close();
        }

        public int GetBookmarkedPage(string filename)
        {
            if (!bookmarks.ContainsKey(filename)) return 0;
            return bookmarks[filename];
        }

        public void SetBookmarkedPage(string filename, int page)
        {
            bookmarks[filename] = page;
        }
    }
}
