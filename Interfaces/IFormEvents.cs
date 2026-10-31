using System;

namespace BCC.Modules.BCCForms.Interfaces
{
    public interface IFormEvents
    {
        event Action RecordUpdated ;
        event Action RecordDeleted ; 
    }
}