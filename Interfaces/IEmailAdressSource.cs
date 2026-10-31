using System.Data;

namespace BCC.Modules.BCCForms.Interfaces
{
    public interface IEmailAdressSource
    {
        string GetEmailAddress(string fieldName, DataRow row);
    }
}