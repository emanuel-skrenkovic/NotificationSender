namespace Notifications.Test.Common

module Common =

    [<StaticClass>]
    type Utility =
        static member DeserializeFromObject : obj -> 'T
