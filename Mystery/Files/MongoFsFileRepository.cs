using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mystery.Register;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Mystery.Json;
using Mystery.Configuration;
using System.Security.Cryptography;

namespace Mystery.Files
{
    [GlobalAvalibleObjectImplementation(singleton = true)]
    [Serializable()]
    public class MongoFsFileRepositoryConfiguration
    {

        [NonSerialized()]

        private DirectoryInfo _files_root_folder;

        private string _files_root_folder_path;
        public string files_root_folder_path
        {
            get { return _files_root_folder_path; }
            set
            {
                _files_root_folder_path = value;
                if (!string.IsNullOrEmpty(_files_root_folder_path))
                {
                    _files_root_folder = new DirectoryInfo(_files_root_folder_path);
                }
            }
        }

        public DirectoryInfo files_root_folder
        {
            get { return _files_root_folder; }
        }


        [GlobalAvailableObjectConstructor()]
        public static MongoFsFileRepositoryConfiguration createDefaultInstance()
        {
            IConfigurationProvider provider = RegisterExstension.getGlobalObject<IConfigurationProvider>(null);
            return provider.getConfiguration<MongoFsFileRepositoryConfiguration>();
        }

    }


    class FileRecord {
        [BsonId]
        public Guid guid { get; set; } = Guid.NewGuid();

        public DateTime? expiration { get; set; }

        public string path { get; set; }

        public string hash { get; set; }
    }

    /// <summary>
    /// use mongo to store file references, files are actually keep as single copies
    /// </summary>
    [GlobalAvalibleObjectImplementation(implementation_of = typeof(IFileRepository), singleton = true)]
    public class MongoFsSingleCopyFileRepository : IFileRepository
    {
        //this is an experiment to see if I can avoid having the same file more than ones.

        private IMongoCollection<FileRecord> _collection;

        private DirectoryInfo _root;
            
        public MongoFsSingleCopyFileRepository() {
            _collection = this.getGlobalObject<MysteryMongoDb>()
                .content_db.GetCollection<FileRecord>(nameof(MongoFsSingleCopyFileRepository));
            _collection.Indexes.CreateOne(Builders<FileRecord>.IndexKeys.Hashed(x=>x.hash));
            _root = this.getGlobalObject<IConfigurationProvider>()
                .getConfiguration<MongoFsFileRepositoryConfiguration>()
                .files_root_folder;
            if (_root == null)
            {
                throw new Exception(
                    nameof(MongoFsFileRepositoryConfiguration.files_root_folder_path) 
                    + " not given in " + nameof(MongoFsFileRepositoryConfiguration));
            }
            if (!_root.Exists) {
                throw new DirectoryNotFoundException(_root.FullName + " not found");
            }
        }

        private FileRecord GetRecord(Guid guid) {
            return _collection.Find(x => x.guid == guid).FirstOrDefault();
        }
        private IEnumerable<FileRecord> GetRecords(string hash)
        {
            return (from x in _collection.AsQueryable()
                    where x.hash == hash
                    select x);
        }

        public MysteryFile CloneFile(MysteryFile file)
        {
            var record = GetRecord(file.guid);
            if (record == null)
                return null;
            var clone = record.Clone();
            clone.guid = Guid.NewGuid();
            clone.expiration = null;
            _collection.InsertOne(clone);
            return new MysteryFile() {filename=file.filename,guid = clone.guid };
        }

        public MysteryFile CloneInTmpFile(MysteryFile file, DateTime expiration)
        {
            var record = GetRecord(file.guid);
            if (record == null)
                return null;
            var clone = record.Clone();
            clone.guid = Guid.NewGuid();
            clone.expiration = expiration;
            _collection.InsertOne(clone);
            return new MysteryFile() { filename = file.filename, guid = clone.guid };
        }

        private string CheckCurrentFolder() {
            //make sure I have the folder for it
            var date = DateTime.Now;
            //we have a tree such as year/month/day/hour
            var path = Path.Combine(_root.FullName, date.Year.ToString());
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, date.Month.ToString());
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, date.Day.ToString());
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, date.Hour.ToString());
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        public MysteryFile CreateFile(string extension, Stream stream)
        {

            var md5 = new MD5CryptoServiceProvider();
            var checksum = BitConverter.ToString(md5.ComputeHash(stream));
            var exsisting = GetRecords(checksum).FirstOrDefault();
            if (exsisting != null) {
                var clone = exsisting.Clone();
                clone.guid = Guid.NewGuid();
                clone.expiration = null;
                _collection.InsertOne(clone);
                return new MysteryFile() { guid = clone.guid };
            }

            //all right, first time here
            //rewind!
            stream.Position = 0;
            
            var record = new FileRecord() { expiration = null, hash = checksum };
            record.path = Path.Combine(CheckCurrentFolder(), record.guid.ToString() + "." + extension);
            
            using (var filestream = new FileStream(record.path, FileMode.CreateNew)) {
                stream.CopyTo(filestream);
            }

            _collection.InsertOne(record);

            return new MysteryFile() {  guid = record.guid }; 
        }

        public MysteryFile CreateFile(string extension, byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            return CreateFile(extension, stream);
        }

        public MysteryFile CreateTmpFile(string extension, DateTime expiration, Stream stream)
        {
            var md5 = new MD5CryptoServiceProvider();
            var checksum = BitConverter.ToString(md5.ComputeHash(stream));
            var exsisting = GetRecords(checksum).FirstOrDefault();
            if (exsisting != null)
            {
                var clone = exsisting.Clone();
                clone.guid = Guid.NewGuid();
                clone.expiration = expiration;
                _collection.InsertOne(clone);
                return new MysteryFile() { guid = clone.guid };
            }

            //all right, first time here
            //rewind!
            stream.Position = 0;


            var record = new FileRecord() { expiration = expiration, hash = checksum};
            record.path = Path.Combine(CheckCurrentFolder(), record.guid.ToString() + "." + extension);
            

            using (var filestream = new FileStream(record.path, FileMode.CreateNew))
            {
                stream.CopyTo(filestream);
            }

            _collection.InsertOne(record);

            return new MysteryFile() { guid = record.guid };
        }

        public MysteryFile CreateTmpFile(string extension, DateTime expiration, byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            return CreateTmpFile(extension, expiration, stream);
        }

        public bool DeleteFile(MysteryFile file)
        {
            var record = GetRecord(file.guid);
            if (record == null)
                return false;
            //I have one, but is it the last copy?
            var different_copy = (from x in GetRecords(record.hash)
                                  where x.guid != record.guid
                                  select x).FirstOrDefault();

            _collection.DeleteOne(x => x.guid == record.guid);
            if (different_copy == null)
            {
                //last copy
                File.Delete(record.path);
            }
            else
            {
                //we don't we still have at least 1 reference out there.
            }
            //no matter if we still have a copy for somebody else we say we delete this one
            return true;
        }

        public byte[] GetFileBytes(MysteryFile file)
        {
            var record = GetRecord(file.guid);
            if (record == null)
                return null;
            using (var file_stream = new FileStream(record.path, FileMode.Open,FileAccess.Read,FileShare.Read)) {
                using (var mem = new MemoryStream()) {
                    file_stream.CopyTo(mem);
                    return mem.ToArray();
                }
            }
        }

        public Stream GetFileStream(MysteryFile file)
        {
            var record = GetRecord(file.guid);
            if (record == null)
                return null;
            var result = new FileStream(record.path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return result;
        }
    }
}
