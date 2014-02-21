using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PassPhotoMVC.Tests.Helpers
{
    class TestsUtils
    {
        /// <summary>
        /// Locate required files physically in the application folder         
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public String GetLocalRootPathToFile(String filename)
        {
            Uri myuri = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            String mycurpath = System.Uri.UnescapeDataString(Path.GetFullPath(myuri.AbsolutePath));
            mycurpath = mycurpath.Substring(0, mycurpath.IndexOf("\\PassPhotoMVC4.Tests"));
            mycurpath = mycurpath + "\\PassPhotoMVC4";
            return mycurpath + filename;
        }

        /// <summary>
        /// Locate required files physically in the test folder         
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public String GetLocalRootPathToTestFile(String filename)
        {
            Uri myuri = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            String mycurpath = System.Uri.UnescapeDataString(Path.GetFullPath(myuri.AbsolutePath));
            mycurpath = mycurpath.Substring(0, mycurpath.IndexOf("\\PassPhotoMVC4.Tests"));
            mycurpath = mycurpath + "\\PassPhotoMVC4.Tests";
            return mycurpath + filename;
        }
    }
}
