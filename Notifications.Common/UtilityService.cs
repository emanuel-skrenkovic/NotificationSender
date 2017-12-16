using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Common
{
    public class UtilityService
    {

        public T DeserializeFromObject<T>(object serializedObj) where T : class
        {
            var serialized = serializedObj as string;

            if (serialized == null)
                return null;

            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}
