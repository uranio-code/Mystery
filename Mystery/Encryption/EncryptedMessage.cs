using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using Mystery.Register;
using Mystery.Json;

namespace Mystery.Encryption
{
    /// <summary>
    /// Mystery message
    /// </summary>
    /// <remarks></remarks>
    public class EncryptedMessage
    {

        /// <summary>
        /// clear, identification the client application and the username
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Sender { get; set; }

        /// <summary>
        /// encrypted with receiver public, identification the current session
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public byte[] Token { get; set; }

        /// <summary>
        /// encrypted with receiver public, identification the current message
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public byte[] MessageID { get; set; }

        /// <summary>
        /// Hash of MessageID encrypted with sender private
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public byte[] MessageIDSignaure { get; set; }

        /// <summary>
        /// encrypted with receiver public, RijndaelManaged key of the message
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public byte[] Key { get; set; }
        /// <summary>
        /// encrypted with receiver public, RijndaelManaged IV of the message
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public byte[] IV { get; set; }

        /// <summary>
        /// encrypted with RijndaelManaged using the key and IV provided
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public byte[] Message { get; set; }

        /// <summary>
        /// Signature on the clear message using sender private
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public byte[] Signature { get; set; }


        public static EncryptedMessage GenerateMessage(string Sender, object message, RSACryptoServiceProvider receiver_rsa, string token)
        {
            RijndaelManaged RMCrypto = new RijndaelManaged();
            RMCrypto.GenerateIV();
            RMCrypto.GenerateKey();

            byte[] result_message = RMCrypto.getGlobalObject<MysteryJsonConverter>().getJson(message).getBytes();

            var ms = new MemoryStream();
            CryptoStream CryptStream = new CryptoStream(ms, RMCrypto.CreateEncryptor(RMCrypto.Key, RMCrypto.IV), CryptoStreamMode.Write);
            CryptStream.Write(result_message, 0, result_message.Length);
            CryptStream.FlushFinalBlock();

            byte[] message_id_bytes = Guid.NewGuid().ToString().getBytes();

            RSACryptoServiceProvider my_rsa = CryptStream.getGlobalObject<IMyRsaProvider>().getMySra();

            EncryptedMessage answer = new EncryptedMessage
            {
                Sender = Sender,
                IV = receiver_rsa.Encrypt(RMCrypto.IV, false),
                Key = receiver_rsa.Encrypt(RMCrypto.Key, false),
                Message = ms.ToArray(),
                Signature = my_rsa.SignData(result_message, new SHA1CryptoServiceProvider()),
                Token = receiver_rsa.Encrypt(token.getBytes(), false),
                MessageID = receiver_rsa.Encrypt(message_id_bytes, false),
                MessageIDSignaure = my_rsa.SignData(message_id_bytes, new SHA1CryptoServiceProvider())
            };
            return answer;
        }

        /// <summary>
        /// already received messages
        /// </summary>
        /// <remarks></remarks>
        private static HashSet<string> message_ids = new HashSet<string>();
        /// <summary>
        /// limit to message id keep in memory
        /// </summary>
        /// <remarks></remarks>

        private const int replay_store_size = 100000;

        public static BodyType readMessage<BodyType>(EncryptedMessage message, RSACryptoServiceProvider sender_rsa)
        {
            //VUNERABILTY
            //IN CASE OF client restart the fake server can resend the message, the token is still valid
            //sol: on boot reset all client token
            //this solve also reply attack as the message would all change token, so I need only 1 session message id
            //if the message id buffer if full receiver shall send an authenticate request along with the correct answer (to avoid answer to be to different from normal message)

            if (message == null)
                return default(BodyType);
            byte[] signature = message.Signature;

            RSACryptoServiceProvider my_rsa = message.getGlobalObject<IMyRsaProvider>().getMySra();

            //checking unique id
            byte[] message_id_bytes = my_rsa.Decrypt(message.MessageID, false);
            if (!sender_rsa.VerifyData(message_id_bytes, new SHA1CryptoServiceProvider(), message.MessageIDSignaure))
            {
                return default(BodyType);
            }
            string message_id = message_id_bytes.getString();
            if (message_ids.Contains(message_id))
                return default(BodyType);
            //replay attack!!!
            if (message_ids.Count > replay_store_size)
            {
                message_ids = new HashSet<string>();
            }
            message_ids.Add(message_id);

            byte[] Key = my_rsa.Decrypt(message.Key, false);
            byte[] IV = my_rsa.Decrypt(message.IV, false);

            //Create a new instance of the RijndaelManaged class
            //and decrypt the stream.
            RijndaelManaged RMCrypto = new RijndaelManaged();


            //Create an instance of the CryptoStream class, pass it the NetworkStream, and decrypt 
            //it with the Rijndael class using the key and IV.
            MemoryStream ms = new MemoryStream(message.Message);
            CryptoStream CryptStream = new CryptoStream(ms, RMCrypto.CreateDecryptor(Key, IV), CryptoStreamMode.Read);


            MemoryStream output = new MemoryStream();
            byte[] buffer = new byte[1024];
            int read = CryptStream.Read(buffer, 0, buffer.Length);
            while (read > 0)
            {
                output.Write(buffer, 0, read);
                read = CryptStream.Read(buffer, 0, buffer.Length);
            }
            CryptStream.Flush();
            CryptStream.Dispose();
            ms.Dispose();

            byte[] received_message = output.ToArray();

            if (!sender_rsa.VerifyData(received_message, new SHA1CryptoServiceProvider(), signature))
            {
                return default(BodyType);
            }

            //OK message certificated and decrypted!
            BodyType message_object = sender_rsa.getGlobalObject<MysteryJsonConverter>().readJson<BodyType>(received_message.getString());
            

            return message_object;

        }

    }


    [GlobalAvalibleObject()]
    public interface IMyRsaProvider 
    {
        RSACryptoServiceProvider getMySra();
    }

    [GlobalAvalibleObjectImplementation(implementation_of = typeof(IMyRsaProvider), overrides_exsisting = false, singleton = true)]
    public class DefaultMyRsaProvider : IMyRsaProvider
    {

        public RSACryptoServiceProvider getMySra()
        {
            // Create the CspParameters object and set the key container 
            //  name used to store the RSA key pair.
            CspParameters cp = new CspParameters();
            cp.KeyContainerName = "MysteryKey";

            // Create a new instance of RSACryptoServiceProvider that accesses
            // the key container MyKeyContainerName.
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cp);

            return rsa;
        }
    }

    public static class RsaExntesions
    {
        /// <summary>
        /// read a string encrypted with my public key
        /// </summary>
        /// <param name="encrypted"></param>
        /// <returns></returns>
        /// <remarks></remarks>

        public static string readAEncryptedString(this byte[] encrypted)
        {
            return encrypted.getGlobalObject<IMyRsaProvider>().getMySra().Decrypt(encrypted, false).getString();
        }
    }
}

