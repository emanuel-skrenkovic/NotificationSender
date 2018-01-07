namespace Notifications.Test.Common

module Common =
    open Newtonsoft.Json

    type Utility() =
        static member DeserializeFromObject (serializedObj: obj) : 'T =
            let serialized: string = serializedObj :?> string
            JsonConvert.DeserializeObject<'T>(serialized)

