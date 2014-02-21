using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PassPhotoMVC;
using PassPhotoMVC.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.IO;
using System.Web;
using PassPhotoMVC.Tests.Helpers;
using System.Drawing;

namespace PassPhotoMVC.Tests.Helpers
{
    [TestClass]
    public class PhotoUtilsTest
    {

        Stream stream1;
        Stream stream2;
        Stream stream3;
        TestsUtils mytestutils = new TestsUtils();        

        [TestInitialize]
        public void SetUp()
        {
            // preset the test image stream for the upload test
            byte[] mybytes = File.ReadAllBytes(mytestutils.GetLocalRootPathToTestFile("\\Images\\testimage1.jpg"));
            stream1 = new MemoryStream(mybytes);
            mybytes = File.ReadAllBytes(mytestutils.GetLocalRootPathToTestFile("\\Images\\testimage3.jpg"));
            stream2 = new MemoryStream(mybytes);
            mybytes = File.ReadAllBytes(mytestutils.GetLocalRootPathToTestFile("\\Images\\testimage4.gif"));
            stream3 = new MemoryStream(mybytes);
        }

        [TestCleanup]
        public void TearDown()
        {

            stream1.Dispose();
            stream2.Dispose();
            stream3.Dispose();
           
        }

        
        [TestMethod]
        public void UploadImageHelper()
        {
            // Arrange
            PhotoUtils myutils = new PhotoUtils();
            if (File.Exists(mytestutils.GetLocalRootPathToFile("\\uploaded_images\\raw\\") + "image_upload_test.jpg"))
            {
                File.Delete(mytestutils.GetLocalRootPathToFile("\\uploaded_images\\raw\\") + "image_upload_test.jpg");
            }

            // setup mock object files for testing upload
            var file1 = new Mock<HttpPostedFileBase>();
            file1.Setup(x => x.InputStream).Returns(stream1);
            file1.Setup(x => x.ContentLength).Returns((int)stream1.Length);
            file1.Setup(x => x.FileName).Returns("testimage1.jpg");
            var file2 = new Mock<HttpPostedFileBase>();
            file2.Setup(x => x.InputStream).Returns(stream2);
            file2.Setup(x => x.ContentLength).Returns((int)stream2.Length);
            file2.Setup(x => x.FileName).Returns("testimage3.jpg");
            var file3 = new Mock<HttpPostedFileBase>();
            file3.Setup(x => x.InputStream).Returns(stream3);
            file3.Setup(x => x.ContentLength).Returns((int)stream3.Length);
            file3.Setup(x => x.FileName).Returns("testimage4.gif");

            String imgfolder = mytestutils.GetLocalRootPathToFile("\\uploaded_images\\raw\\");

            String result1 = myutils.UploadImage(file1.Object, "image_upload_test.jpg", imgfolder, 262144, 280);
            String result2 = myutils.UploadImage(file2.Object, "image_upload_test2.jpg", imgfolder, 262144, 280);
            String result3 = myutils.UploadImage(file3.Object, "image_upload_test3.jpg", imgfolder, 262144, 280);

            Boolean isFileUploaded = File.Exists(mytestutils.GetLocalRootPathToFile("\\uploaded_images\\raw\\") + "image_upload_test.jpg");
            Boolean isFileUploaded2 = File.Exists(mytestutils.GetLocalRootPathToFile("\\uploaded_images\\raw\\") + "image_upload_test2.jpg");
            Boolean isFileUploaded3 = File.Exists(mytestutils.GetLocalRootPathToFile("\\uploaded_images\\raw\\") + "image_upload_test3.jpg");

            Assert.IsTrue(isFileUploaded);
            Assert.IsFalse(isFileUploaded2);
            Assert.IsFalse(isFileUploaded2);
            Assert.AreEqual("OK: File uploaded!", result1);
            Assert.IsTrue(result2.StartsWith("ERROR: File is larger than"));
            Assert.IsTrue(result3.StartsWith("ERROR: Cannot accept files of this type."));

        }

        [TestMethod]
        public void ResizeImageUploadHelper()
        {
            // Arrange
            PhotoUtils myutils = new PhotoUtils();

            String myfilename = "image_upload_test.jpg";

            String imgfolder = mytestutils.GetLocalRootPathToFile("\\uploaded_images\\raw\\");

            String result1 = myutils.ResizeImageUpload(stream1, myfilename, imgfolder, 240);

            Boolean isFileUploaded = File.Exists(mytestutils.GetLocalRootPathToFile("\\uploaded_images\\raw\\") + myfilename);

            Bitmap image1 = (Bitmap)Image.FromFile(imgfolder + myfilename, true);

            Assert.AreEqual("OK: File uploaded!", result1);
            Assert.IsTrue(isFileUploaded);
            Assert.AreEqual(240, image1.Height);

        }

        [TestMethod]
        public void CropImageHelper()
        {
            //todo: copy testimage2.jpg from the Test Project Folder to the Application Folder
            
            // Arrange
            PhotoUtils myutils = new PhotoUtils();

            String myfilename = "testimage2.jpg";

            String imgfolder = mytestutils.GetLocalRootPathToFile("\\uploaded_images\\");

            String result1 = myutils.CropImage(myfilename, "raw", "cropped", imgfolder, 0, 0, 110, 140);

            Boolean isFileCropped = File.Exists(mytestutils.GetLocalRootPathToFile("\\uploaded_images\\cropped\\") + myfilename);

            Bitmap image1 = (Bitmap)Image.FromFile(imgfolder + "cropped\\" + myfilename, true);
            int imgh = image1.Height;
            image1.Dispose();

            Assert.AreEqual("OK - File cropped", result1);
            Assert.IsTrue(isFileCropped);
            Assert.AreEqual(280, imgh);

        }

        [TestMethod]
        public void CalculateResizedWidthHelper()
        {
            PhotoUtils myutils = new PhotoUtils();

            String myfilename = "testimage1.jpg";
            
            String imgfolder = mytestutils.GetLocalRootPathToTestFile("\\Images\\");

            int result1 = myutils.CalculateResizedWidth(myfilename, imgfolder, 280);

            myfilename = "testimage2.jpg";

            int result2 = myutils.CalculateResizedWidth(myfilename, imgfolder, 280);

            Assert.AreEqual(280, result1);
            Assert.AreEqual(421, result2);

        }

        [TestMethod]
        public void UpdatePreviewJsHelper()
        {
            PhotoUtils myutils = new PhotoUtils();

            String localjsfile = mytestutils.GetLocalRootPathToFile("\\js\\tiffjcroppreset.js");
            String ratio = Settings.AppSettings.passwidth + "/" + Settings.AppSettings.passheight;
            String previewwidth = Settings.AppSettings.previewwidth;
            String previewheight = Settings.AppSettings.previewheight;
                        
            String result1 = myutils.UpdatePreviewJs(220, 280, localjsfile);
            String result2 = myutils.UpdatePreviewJs(300, 400, localjsfile);

            Boolean isRatioOK1 = result1.Contains("aspectRatio: 220/280");
            Boolean isRatioOK2 = result2.Contains("aspectRatio: 220/280");
            Boolean isrxOK1 = result1.Contains("var rx = " + previewwidth + " / c.w;");
            Boolean isrxOK2 = result2.Contains("var rx = " + previewwidth + " / c.w;");
            Boolean isryOK1 = result1.Contains("var ry = " + previewheight + " / c.h;");
            Boolean isryOK2 = result2.Contains("var ry = " + previewheight + " / c.h;");
            Boolean isWidthOK1 = result1.Contains("width: Math.round(rx*220)");
            Boolean isWidthOK2 = result2.Contains("width: Math.round(rx*300)");
            Boolean isHeightOK1 = result1.Contains("height: Math.round(ry*280)");
            Boolean isHeightOK2 = result2.Contains("height: Math.round(ry*400)");
            Boolean istruesizeOK1 = result1.Contains("trueSize: [220, 280]");
            Boolean istruesizeOK2 = result2.Contains("trueSize: [300, 400]");

            Assert.IsTrue(isRatioOK1);
            Assert.IsTrue(isRatioOK2);
            Assert.IsTrue(isrxOK1);
            Assert.IsTrue(isrxOK2);
            Assert.IsTrue(isryOK1);
            Assert.IsTrue(isryOK2);
            Assert.IsTrue(isWidthOK1);
            Assert.IsTrue(isWidthOK2);
            Assert.IsTrue(isHeightOK1);
            Assert.IsTrue(isHeightOK2);
            Assert.IsTrue(istruesizeOK1);
            Assert.IsTrue(istruesizeOK2);
        
        }
        

    }

}
