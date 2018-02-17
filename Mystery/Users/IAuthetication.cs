using Mystery.Register;
namespace Mystery.Users
{

    [GlobalAvalibleObject()]
    public interface IAuthetication
    {

        /// <summary>
        /// return the user if found
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        User autheticate(string username, string password);

        string Hashstring(string input);

    } 
}