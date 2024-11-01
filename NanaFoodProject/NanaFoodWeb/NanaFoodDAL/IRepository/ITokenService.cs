using NanaFoodDAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.IRepository
{
    public interface ITokenService
    {
        public string CreateToken(User user, IList<string> roles, bool keepLogined);
        public string CreateToken(User user, IList<string> roles);
    }
}
