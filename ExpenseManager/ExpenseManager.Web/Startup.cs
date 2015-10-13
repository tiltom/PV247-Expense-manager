using System;
using ExpenseManager.BusinessLogic.DataTransferObjects;
using ExpenseManager.BusinessLogic.Services;
using ExpenseManager.Web;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof (Startup))]

namespace ExpenseManager.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);            
            var service = new UserService();
            var dto = new UserDTO
            {
                UserName = "Slavo",
                Email = "slavo@slavo.krupa",
                Password = "slavo.krupa",
                CreateTime = DateTime.Now
            };

            service.CreateUser(dto);
        }
    }
}