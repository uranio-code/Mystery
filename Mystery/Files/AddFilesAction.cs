using Mystery.MysteryAction;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;
using Mystery.Register;

namespace Mystery.Files
{

    public class AddFileInput {
        public string filename { get; set; }
        public Stream stream { get; set; }
    }
    public class AddFilesAction : BaseMysteryAction<List<AddFileInput>, List<MysteryFile>>
    {
        
        protected override ActionResult<List<MysteryFile>> ActionImplemetation()
        {
            var result = new List<MysteryFile>();
            var repo = this.getGlobalObject<IFileRepository>();
            foreach (var file in input) {
                var file_result = repo.CreateFile(Path.GetExtension(file.filename), file.stream);
                file_result.filename = file.filename;
                result.Add(file_result);
            }
            return new ActionResult<List<MysteryFile>>(result);
        }

        protected override bool AuthorizeImplementation()
        {
            //every logged person can upload files
            return true;
        }
    }
}