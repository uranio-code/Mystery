using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Mystery.Register;
using Mystery.Json;
using MongoDB.Driver;

namespace Mystery.TOTPAuthentication
{
    [GlobalAvalibleObjectImplementation(singleton =true)]
    public class TOTPAuthenticator
    {
        private const string allowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567"; // Due to Base32 encoding; https://code.google.com/p/vellum/wiki/GoogleAuthenticator
        private int validityPeriodSeconds = 30; // RFC6238 4.1; X represents the time step in seconds (default value X = 30 seconds) and is a system parameter.
        private int futureIntervals = 1; // How much time in the future can the client be; in validityPeriodSeconds intervals.
        private int pastIntervals = 1; // How much time in the past can the client be; in validityPeriodSeconds intervals.
        private int secretKeyLength = 16; // Must be a multiple of 8, iPhones accept up to 16 characters (apparently; didn't test it; don't own an iPhone)
        private readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc); // Beginning of time, according to Unix
        private IMongoCollection<UsedCodes> collection;

        public TOTPAuthenticator() {
            collection = this.getGlobalObject<MysteryMongoDb>().local_db.GetCollection<UsedCodes>(nameof(UsedCodes));
        }
        

        public string GenerateSecretKey()
        {
            Random random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
            return new string((new char[secretKeyLength]).Select(c => c = allowedCharacters[random.Next(0, allowedCharacters.Length)]).ToArray());
        }
        private long GetInterval(DateTime dateTime)
        {
            TimeSpan elapsedTime = dateTime.ToUniversalTime() - unixEpoch;
            return (long)elapsedTime.TotalSeconds / validityPeriodSeconds;
        }
        public string GetCode(string secretKey)
        {
            return GetCode(secretKey, DateTime.Now);
        }
        public string GetCode(string secretKey, DateTime when)
        {
            return GetCode(secretKey, GetInterval(when));
        }
        private string GetCode(string secretKey, long timeIndex)
        {
            var secretKeyBytes = Base32Encode(secretKey);
            //for (int i = secretKey.Length; i < secretKeyBytes.Length; i++) {secretKeyBytes[i] = 0;}
            HMACSHA1 hmac = new HMACSHA1(secretKeyBytes);
            byte[] challenge = BitConverter.GetBytes(timeIndex);
            if (BitConverter.IsLittleEndian) Array.Reverse(challenge);
            byte[] hash = hmac.ComputeHash(challenge);
            int offset = hash[19] & 0xf;
            int truncatedHash = hash[offset] & 0x7f;
            for (int i = 1; i < 4; i++)
            {
                truncatedHash <<= 8;
                truncatedHash |= hash[offset + i] & 0xff;
            }
            truncatedHash %= 1000000;
            return truncatedHash.ToString("D6");
        }
        public bool CheckCodeByKey(string secretKey, string code)
        {
            return CheckCode(secretKey, code, DateTime.Now);
        }
        private bool CheckCode(string secretKey, string code, DateTime when)
        {
            long currentInterval = GetInterval(when);
            bool success = false;
            for (long timeIndex = currentInterval - pastIntervals; timeIndex <= currentInterval + futureIntervals; timeIndex++)
            {
                string intervalCode = GetCode(secretKey, timeIndex);
                bool intervalCodeHasBeenUsed = false;// CodeIsUsed(upn, timeIndex);
                if (ConstantTimeEquals(intervalCode, code) && !intervalCodeHasBeenUsed)
                {
                    success = true;
                    // todo: add code here that adds the code for the user to codes used during an interval.
                    break;
                }
            }
            return success;
        }
        public bool CheckCode(string upn, string secretKey, string code)
        {
            return CheckCode(secretKey, code, upn, DateTime.Now);
        }
        private bool CheckCode(string secretKey, string code, string upn, DateTime when)
        {
            long currentInterval = GetInterval(when);
            bool success = false;
            for (long timeIndex = currentInterval - pastIntervals; timeIndex <= currentInterval + futureIntervals; timeIndex++)
            {
                string intervalCode = GetCode(secretKey, timeIndex);
                bool intervalCodeHasBeenUsed = CodeIsUsed(upn, timeIndex);
                if (!intervalCodeHasBeenUsed && ConstantTimeEquals(intervalCode, code))
                {
                    success = true;
                    SetUsedCode(upn, timeIndex);
                    break;
                }
            }
            return success;
        }
        private byte[] Base32Encode(string source)
        {
            var bits = source.ToUpper().ToCharArray().Select(c => Convert.ToString(allowedCharacters.IndexOf(c), 2).PadLeft(5, '0')).Aggregate((a, b) => a + b);
            return Enumerable.Range(0, bits.Length / 8).Select(i => Convert.ToByte(bits.Substring(i * 8, 8), 2)).ToArray();
        }
        protected bool ConstantTimeEquals(string a, string b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;

            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)a[i] ^ (uint)b[i];
            }

            return diff == 0;
        }

        
        private bool CodeIsUsed(string upn, long interval)
        {
            bool result = collection.Count(x=>x.upn==upn&&x.interval==interval)>0 ;
            // Housekeeping
            collection.DeleteMany(x => x.upn == upn && x.interval < interval);
            return result;
        }

        public class UsedCodes {
            public long interval { get; set; }
            public string upn { get; set; }
        }

        private void SetUsedCode(string upn, long interval)
        { 
            collection.InsertOne(new UsedCodes() { upn = upn, interval = interval });
        }
    }
}
