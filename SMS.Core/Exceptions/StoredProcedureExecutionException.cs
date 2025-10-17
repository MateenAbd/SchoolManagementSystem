using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.Exceptions
{
    public class StoredProcedureExecutionException : Exception
    {
        public StoredProcedureExecutionException(string errorMessage) : base($"Error Executing Stored Procedure. Error : {errorMessage}")
        {

        }
    }
}
