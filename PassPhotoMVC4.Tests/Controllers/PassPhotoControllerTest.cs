using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PassPhotoMVC;
using PassPhotoMVC.Controllers;
using Moq;
using System.Web;
using System.Web.Hosting;
using System.IO;

namespace PassPhotoMVC.Tests.Controllers
{
    [TestClass]
    public class PassPhotoControllerTest
    {
        Stream _stream;

        [TestInitialize]
        public void SetUp()
        {
            // preset the test image stream for the upload test
            byte[] mybytes = File.ReadAllBytes(GetLocalRootPathToTestFile("\\Images\\testimage1.jpg"));
            _stream = new MemoryStream(mybytes);
            
            // delete any test files before the test
            if (File.Exists(GetLocalRootPathToFile("\\uploaded_images\\raw\\") + "image_upload_test.jpg"))
            {
                File.Delete(GetLocalRootPathToFile("\\uploaded_images\\raw\\") + "image_upload_test.jpg");
            }
            
        }
        
        [TestMethod]
        public void Index()
        {
            // Arrange
            PassPhotoController mycontroller = new PassPhotoController();

            // Use Mock the Request Object that is used on Index
            Mock<ControllerContext> cc = new Mock<ControllerContext>();
            cc.Setup(d => d.HttpContext.Request.Path).Returns("/");
            cc.Setup(e => e.HttpContext.Server.MapPath("~/js/tiffjcroppreset.js")).Returns(GetLocalRootPathToFile("\\js\\tiffjcroppreset.js"));  
            mycontroller.ControllerContext = cc.Object;

            SimpleWorkerRequest request = new SimpleWorkerRequest("", "", "", null, new StringWriter());
            HttpContext context = new HttpContext(request);
            HttpContext.Current = context;

            // Get full path to js file that is used in the controller
            String localjsfile= GetLocalRootPathToFile("\\js\\tiffjcroppreset.js");

            // Act
            ViewResult result = mycontroller.Index(localjsfile) as ViewResult;


            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ViewBag.Height);
            Assert.IsNotNull(result.ViewBag.Width);
            Assert.IsNotNull(result.ViewBag.PreviewJSMarkup);
            Assert.IsNotNull(result.ViewBag.RootPath);
        }

        [TestMethod]
        public void UploadImage()
        {
            // Arrange
            PassPhotoController mycontroller = new PassPhotoController();

            // Use Mock the Request Object that is used on Index
            Mock<ControllerContext> cc = new Mock<ControllerContext>();
            cc.Setup(d => d.HttpContext.Request.Path).Returns("/");
            String imgfolder = GetLocalRootPathToFile("\\uploaded_images\\");
            //cc.Setup(d => d.HttpContext.Server.MapPath("~/uploaded_images/")).Returns(imgfolder);
            mycontroller.ControllerContext = cc.Object;

            // setup mock object for uploaded file
            var file = new Mock<HttpPostedFileBase>();
            file.Setup(x => x.InputStream).Returns(_stream);
            file.Setup(x => x.ContentLength).Returns((int)_stream.Length);
            file.Setup(x => x.FileName).Returns("testimage1.jpg");            

            SimpleWorkerRequest request = new SimpleWorkerRequest("", "", "", null, new StringWriter());
            HttpContext context = new HttpContext(request);
            HttpContext.Current = context;

            // Get full path to js file that is used in the controller
            String localjsfile = GetLocalRootPathToFile("\\js\\tiffjcroppreset.js");            

            // Act
            ViewResult result = mycontroller.UploadImage(file.Object, "image_upload_test.jpg", "raw", imgfolder, localjsfile) as ViewResult;

            _stream.Dispose();

            Boolean isFileUploaded = File.Exists(GetLocalRootPathToFile("\\uploaded_images\\raw\\") + "image_upload_test.jpg");

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ViewBag.Height);
            Assert.IsNotNull(result.ViewBag.Width);
            Assert.IsNotNull(result.ViewBag.PreviewJSMarkup);
            Assert.IsNotNull(result.ViewBag.RootPath);
            Assert.IsNotNull(result.ViewBag.ImageName);
            Assert.IsNotNull(result.ViewBag.ImageUrl);
            Assert.IsNotNull(result.ViewBag.PreviewDisplay);
            Assert.IsTrue(isFileUploaded);
        }


        [TestMethod]
        public void CropImage()
        {
            // Arrange
            PassPhotoController mycontroller = new PassPhotoController();

            // Use Mock the Request Object that is used on Index
            Mock<ControllerContext> cc = new Mock<ControllerContext>();
            cc.Setup(d => d.HttpContext.Request.Path).Returns("/");
            String imgfolder = GetLocalRootPathToFile("\\uploaded_images\\");
            mycontroller.ControllerContext = cc.Object;
            
            SimpleWorkerRequest request = new SimpleWorkerRequest("", "", "", null, new StringWriter());
            HttpContext context = new HttpContext(request);
            HttpContext.Current = context;

            // Get full path to js file that is used in the controller
            String localjsfile = GetLocalRootPathToFile("\\js\\tiffjcroppreset.js");

            // Act
            ViewResult result = mycontroller.CropImage("testimage2.jpg", "raw", "0", "0", "220", "280", imgfolder, localjsfile) as ViewResult;
                       
            Boolean isFileCropped = File.Exists(GetLocalRootPathToFile("\\uploaded_images\\cropped\\") + "testimage2.jpg");

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ViewBag.Height);
            Assert.IsNotNull(result.ViewBag.Width);
            Assert.IsNotNull(result.ViewBag.PreviewJSMarkup);
            Assert.IsNotNull(result.ViewBag.RootPath);
            Assert.IsNotNull(result.ViewBag.ImageName);
            Assert.IsNotNull(result.ViewBag.ImageUrl);
            Assert.IsNotNull(result.ViewBag.PreviewDisplay);
            Assert.IsTrue(isFileCropped);

            
        }

               
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
            return mycurpath+filename;
        }

        /// <summary>
        /// Locate required files physically in the application folder         
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
