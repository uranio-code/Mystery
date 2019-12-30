using Mystery.Register;
using System;
using System.Collections.Generic;
using System.Web;
using Mystery.Users;
using System.Security.Cryptography;
using System.IO;

namespace Mystery.Web
{
    public delegate void SessionEndEventHandler(MysterySession mystery_session);
    /// <summary>
    /// this class it is used instead of the SessionState in ASP.NET
    /// avoiding using session state avoid to lock the session on request allowing more than one request at the same time
    /// </summary>
    /// <remarks></remarks>
    [GlobalAvalibleObjectImplementation()]
    public class MysterySession
    {

        public string id { get; set; }

        public User authenticated_user { get; set; }

        public User user
        {
            get
            {
                if (authenticated_user == null) return null;
                if (authenticated_user.working_for == null) return authenticated_user;
                if (authenticated_user.working_for.value == null) return authenticated_user;
                return authenticated_user.working_for.value == null ? authenticated_user : authenticated_user.working_for.value;
            }
        }

        public string UserAgent { get; set; }

        public string HostAddress { get; set; }

        public string HostName { get; set; }

        public string UserName { get; set; }

        public System.DateTime LoginTime { get; set; }

        public string last_request_url { get; set; }

        public System.DateTime last_request_time { get; set; }



        private MysterySession()
        {
        }


        private static IDictionary<string, MysterySession> _sessions = new  Dictionary<string, MysterySession>(1000);

        private static object _lock = new object();

        private static int instanced = 0;
        public static event SessionEndEventHandler SessionEnd;
        

        public static MysterySession getSession(string session_id)
        {
            MysterySession session;
            _sessions.TryGetValue(session_id,out session);
            if (session != null)
            {
                session.last_request_time = System.DateTime.Now;
                return session;
            }

            //as we want anyway the session table to be unique we shall lock its creations to avoid 2 session to be created at the same time
            lock (_lock)
            {
                _sessions.TryGetValue(session_id, out session);
                if (session != null)
                {
                    return session;
                }
                if (HttpContext.Current != null)
                {
                    session = new MysterySession
                    {
                        id = session_id,
                        HostAddress = HttpContext.Current.Request.UserHostAddress,
                        HostName = HttpContext.Current.Request.UserHostName,
                        UserAgent = HttpContext.Current.Request.UserAgent,
                    };
                }
                else
                {
                    session = new MysterySession
                    {
                        id = session_id,
                        HostAddress = "local",
                        HostName = Environment.MachineName,
                        UserAgent = System.Security.Principal.WindowsIdentity.GetCurrent().Name
                    };
                }
                
                

                instanced += 1;
                if (instanced > 200)
                {
                    cleanUpSessions();
                }
                _sessions[session_id] = session;
                return session;
            }
        }

        public static void cleanUpSessions()
        {
            ICollection<MysterySession> toClean = new LinkedList<MysterySession>();

            lock (_lock)
            {
                foreach (MysterySession session in _sessions.Values)
                {
                    if (session.last_request_time.AddMinutes(30) < System.DateTime.Now)
                    {
                        toClean.Add(session);
                    }
                }
                foreach (MysterySession session in toClean)
                {
                    _sessions.Remove(session.id);
                }
                instanced = 0;
            }

            foreach (MysterySession session in toClean)
            {
                SessionEnd?.Invoke(session);
            }


        }

        public static void releaseSession(string session_id)
        {
            MysterySession mystery_session;
            _sessions.TryGetValue(session_id, out mystery_session);
            bool done = false;
            lock (_lock)
            {
                done = _sessions.Remove(session_id);
            }
            if (mystery_session != null && done)
            {
                SessionEnd?.Invoke(mystery_session);
            }
        }

        public static int Count
        {
            get { return _sessions.Count; }
        }


        private static string DecryptCoockieValue(string value)
        {
            try
            {
                string Decrypted = Decrypt(value);
                if (string.IsNullOrEmpty(Decrypted))
                {
                    return string.Empty;
                }
                if (Decrypted.StartsWith(HttpContext.Current.Request.UserHostAddress))
                {
                    return Decrypted.Replace(HttpContext.Current.Request.UserHostAddress, "");
                }
                else {
                    return string.Empty;
                }
            }
            catch 
            {
                return string.Empty;
            }
        }

        private static string CurrentSessionID
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return string.Empty;
                }
                HttpCookie cookie = HttpContext.Current.Request.Cookies["MysterySessionID"];
                if (cookie != null)
                {
                    string decrypted = DecryptCoockieValue(cookie.Value);
                    if (!string.IsNullOrEmpty(decrypted))
                    {
                        return decrypted;
                    }
                }
                string result = Guid.NewGuid().ToString().Replace("-", "");
                cookie = new HttpCookie("MysterySessionID", Encrypt(HttpContext.Current.Request.UserHostAddress + result));
                cookie.HttpOnly = true;

                HttpContext.Current.Response.Cookies.Add(cookie);
                //this request might not be done asking for the session
                //the coockie shall be added there to to avoid generating and change it
                HttpContext.Current.Request.Cookies.Add(cookie);
                return result;
            }
        }

        [GlobalAvailableObjectConstructor()]
        public static MysterySession getSession()
        {
            if (HttpContext.Current != null)
            {
                return getSession(CurrentSessionID);
            }
            else {
                return getSession("OutSideWebSession");
            }
        }

        private static RijndaelManaged RMCrypto;

        private static object _encrypt_lock = new object();

        private static void ensureRMCrypto() {
            if (RMCrypto != null)
                return;
            lock (_encrypt_lock)
            {
                if (RMCrypto != null)
                    return;
                RMCrypto = new RijndaelManaged();
                RMCrypto.GenerateIV();
                RMCrypto.GenerateKey();
            }
        }

        public static string Encrypt(string input)
        {
            ensureRMCrypto();
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] bites = input.getBytes();

                CryptoStream CryptStream = new CryptoStream(ms, RMCrypto.CreateEncryptor(RMCrypto.Key, RMCrypto.IV), CryptoStreamMode.Write);
                CryptStream.Write(bites, 0, bites.Length);
                CryptStream.FlushFinalBlock();
                CryptStream.Close();
                CryptStream.Dispose();
                return ms.ToArray().getBase64();
            }

        }


        public static string Decrypt(string input)
        {
            ensureRMCrypto();

            MemoryStream ms = new MemoryStream(input.getBytesBase64());
            CryptoStream CryptStream = new CryptoStream(ms, RMCrypto.CreateDecryptor(RMCrypto.Key, RMCrypto.IV), CryptoStreamMode.Read);

            MemoryStream output = new MemoryStream();
            byte[] buffer = new byte[1024];
            int length = buffer.Length;
            int read = CryptStream.Read(buffer, 0, length);
            while (read > 0)
            {
                output.Write(buffer, 0, read);
                read = CryptStream.Read(buffer, 0, length);
            }
            CryptStream.Flush();
            CryptStream.Dispose();
            ms.Dispose();

            return output.ToArray().getString();

        }
    }

}
