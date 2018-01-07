namespace Notifications.Test.Service

module Http =
    [<Interface>]
    type IClient =
        abstract member Send: string -> string -> int -> 'T 
        abstract member SendAsync: string -> string -> int -> Task<'T>

    type HttpClient = 
        member Send : string -> string -> int -> HttpResponseMessage
        member SendAsync : string -> string -> int -> Task<HttpResponseMessage>