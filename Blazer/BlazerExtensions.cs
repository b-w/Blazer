namespace Blazer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class BlazerExtensions
    {
        public static T First<T>(IDbConnection connection, string command, object parameters = null)
        {
            using (var cmd = connection.CreateCommand())
            {
                
            }

            throw new NotImplementedException();
        }
    }
}
