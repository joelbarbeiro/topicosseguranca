using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.models
{
    public class ValidationControllers
    {
        //FUNÇÃO PARA AUTENTICAÇÃO DO UTILIZADOR
        public static bool authUser(string user, string pass)
        {
            serverContext context = new serverContext();
            var query_result_alt = context.Users.Where(
            User =>
            User.user == user &&
            User.password == pass);
            if (query_result_alt.Count() == 0)
            {
                Console.WriteLine("Login Failed");
                return false;
            }
            Program.updateStateLoggin(query_result_alt.First().IdUser, true);

            return true;
        }
        //FUNÇÃO PARA CONFIRMAR O REGISTO
        public static bool userConfirmRegister(string user, string email)
        {
            serverContext context = new serverContext();

            //ENTITY FRAMEWORK
            var query_result_alt = context.Users.Where(
             User =>
             User.user == user &&
             User.email == email);

            if (query_result_alt.Count() == 0)
            {
                Console.WriteLine("Register Successful");
                return true;
            }
            Console.WriteLine("Register Failed");
            return false;

        }
        public static bool checkLoggedIn(int id)
        {
            serverContext context = new serverContext();
            var query_result_alt = context.Users.Where(
            User =>
            User.IdUser == id && User.isLoggedIn == true);
            if (query_result_alt.Count() == 0)
            {
                Console.WriteLine("Login Failed");
                return false;
            }
            return true;
        }
    }

}
