using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mystery.Register;

namespace Mystery.Files
{
    [GlobalAvalibleObject]
    public interface IFileRepository
    {

        Stream GetFileStream(MysteryFile file);

        byte[] GetFileBytes(MysteryFile file);
        /// <summary>
        /// create a new file
        /// </summary>
        /// <param name="extension">jpg, txt, etc.. without "."</param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        MysteryFile CreateFile(string extension, byte[] bytes);
        /// <summary>
        /// create a new file
        /// </summary>
        /// <param name="extension">jpg, txt, etc.. without "."</param>
        /// <param name="stream"></param>
        /// <returns></returns>
        MysteryFile CreateFile(string extension, Stream stream);
        /// <summary>
        /// create a file will persist at least to a give date
        /// </summary>
        /// <param name="extension">jpg, txt, etc.. without "."</param>
        /// <param name="expiration"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        MysteryFile CreateTmpFile(string extension, DateTime expiration, byte[] bytes);
        /// <summary>
        /// create a file will persist at least to a give date
        /// </summary>
        /// <param name="extension">jpg, txt, etc.. without "."</param>
        /// <param name="expiration"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        MysteryFile CreateTmpFile(string extension, DateTime expiration, Stream stream);

        bool DeleteFile(MysteryFile file);

        MysteryFile CloneFile(MysteryFile file);

        MysteryFile CloneInTmpFile(MysteryFile file, DateTime expiration);
    }
}
