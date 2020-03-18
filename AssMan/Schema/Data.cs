using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssMan.Schema
{ 
    public class AssManJSON
    {
        public static AssManJSON Instance;

        public List<AssetBundle> Assets { get; set; }
    }

    public class AssetBundle
    {
        public string Title { get; set; }
        public List<string> Filenames { get; set; }
        public string Description { get; set; }
        public List<string> Categories { get; set; }
    }
}
