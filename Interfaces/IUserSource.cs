using System.Data;
using DotNetNuke.Entities.Users;

namespace BCC.Modules.BCCForms.Interfaces
{
    public interface IUserSource
    {
        UserInfo GetUser(string fieldName, DataRow row);
    }
}