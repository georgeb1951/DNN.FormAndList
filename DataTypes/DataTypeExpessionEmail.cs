using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using DotNetNuke.Common;
using BCC.Modules.BCCForms.Components;
using BCC.Modules.BCCForms.Interfaces;
using DotNetNuke.Services.Mail;

namespace BCC.Modules.BCCForms.DataTypes
{
    #region DataType

    /// -----------------------------------------------------------------------------
    /// <summary>
    ///   MetaData and Formating for DataType "DataTypeExpessionEmail"
    /// </summary>
    /// -----------------------------------------------------------------------------
    public class DataTypeExpessionEmail : DataTypeExpression, IEmailAdressSource
    {
        public string GetEmailAddress(string fieldName, DataRow row)
        {
            return row[fieldName].AsString();
        }
    }

    #endregion
}