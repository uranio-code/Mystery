using Mystery.MysteryAction;
using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using Mystery.Register;
using Mystery.Content;
using System.Linq;

namespace Mystery.Files
{

    public class DownloadFileActionInput {
        public ContentReference content_reference { get; set; }
        public string property_name { get; set; }

        public HttpResponse response { get; set; }
    }

    public class DownloadFileAction : BaseMysteryAction<DownloadFileActionInput, MysteryFile>
    {
        static Dictionary<string, string> MIME_Types = new Dictionary<string, string>();
        const string default_MIME = "application/octet-stream";

        const string Inline_Extension = ".html.htm.jpeg.jpg.png.bmp.gif";

        static DownloadFileAction()
        {
            MIME_Types.Add(".txt", "text/plain");
            MIME_Types.Add(".vb", "text/plain");
            MIME_Types.Add(".html", "text/plain");
            MIME_Types.Add(".htm", "text/html");
            MIME_Types.Add(".doc", "application/vnd.ms-word");
            MIME_Types.Add(".docx", "application/vnd.ms-word");
            MIME_Types.Add(".xls", "application/vnd.ms-excel");
            MIME_Types.Add(".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            MIME_Types.Add(".ppt", "application/vnd.ms-powerpoint");
            MIME_Types.Add(".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation");
            MIME_Types.Add(".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow");
            MIME_Types.Add(".jpeg", "image/jpeg");
            MIME_Types.Add(".jpg", "image/jpeg");
            MIME_Types.Add(".png", "image/x-png");
            MIME_Types.Add(".bmp", "image/bmp");
            MIME_Types.Add(".gif", "image/gif");
            MIME_Types.Add(".pdf", "application/pdf");
            MIME_Types.Add(".avi", "image/avi");
            MIME_Types.Add(".zip", "application/zip");
        }


        private static string getMIMEType(string extension) {
            extension = extension.ToLower();
            if (MIME_Types.ContainsKey(extension))
                return MIME_Types[extension];
            return default_MIME;
        }

        private static string getFileDownloadHeader(string filename)
        {
            var extension = Path.GetExtension(filename);
            extension = extension.ToLower();
            string modeDisplay = Inline_Extension.Contains(extension) ? "inline" : "attachment";
            return modeDisplay + "; filename=" + "\"" + filename + "\"";
        }
            

        protected override ActionResult<MysteryFile> ActionImplemetation()
        {

            if (input == null)
                return ActionResultTemplates<MysteryFile>.InvalidInput;
            if (input.content_reference == null)
                return ActionResultTemplates<MysteryFile>.InvalidInput;
            if (string.IsNullOrEmpty(input.property_name))
                return ActionResultTemplates<MysteryFile>.InvalidInput;
            var cd = this.getGlobalObject<IContentDispatcher>();
            var content = cd.GetContent(input.content_reference.ContentType, input.content_reference.guid);
            if (content == null)
                return ActionResultTemplates<MysteryFile>.InvalidInput;

            var property = (from x in content.GetType().getMysteryPropertyAttributes<ContentProperty>()
                            where x.name == input.property_name
                            select x).FirstOrDefault();

            if (property == null)
                return ActionResultTemplates<MysteryFile>.InvalidInput;

            if (!property.used_in.PropertyType.Equals(typeof(MysteryFile)))
                return ActionResultTemplates<MysteryFile>.InvalidInput;

            MysteryFile file = (MysteryFile)property.retrive(content);

            if(file == null)
                return ActionResultTemplates<MysteryFile>.InvalidInput;

            var response = input.response;
            response.ClearHeaders();
            response.Clear();
            response.ClearContent();
            response.Buffer = true;
            var ext = Path.GetExtension(file.filename);
            response.ContentType = getMIMEType(ext);
            response.AppendHeader("Content-Disposition", getFileDownloadHeader(file.filename));

            //a file content property can be changed anytime
            //we can find a smarter way, but for now.
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            
            var repo = this.getGlobalObject<IFileRepository>();
            var stream = repo.GetFileStream(file);
            response.AppendHeader("content-length", stream.Length.ToString());
            stream.CopyTo(response.OutputStream);

            response.End();
            response.Flush();

            return file;

        }

        protected override bool AuthorizeImplementation()
        {
            if (input == null)
                return true;
            if (input.content_reference == null)
                return true;
            if (string.IsNullOrEmpty(input.property_name))
                return true;
            
            var content = this.input.content_reference.getContent();
            if (content == null)
                return true;

            return content.canAccess(user);


        }
    }
}