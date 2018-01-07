namespace Notifications.Test.Service

module Http =
    open System.Net.Http
    open Newtonsoft.Json
    open System
    open System.Threading.Tasks
    open System.Collections.Generic
    open System.Net

    type IClient =
        abstract member Send: string -> string -> int -> 'T 
        abstract member SendAsync: string -> string -> int -> Task<'T>

    type ISenderService =
        abstract member Send: string * string * int -> 'TResponse
        abstract member Send: 'TMessage * string * int -> 'TResponse

        abstract member SendAsync: string * string * int -> Task<'TResponse>
        abstract member SendAsync: 'TMessage * string * int -> Task<'TResponse>

        abstract member SendBatch: string * List<string> * int -> List<'TResponse>
        abstract member SendBatch: 'TMessage * List<string> * int -> List<'TResponse>

        abstract member SendBatchAsync: string * List<string> * int -> Task<List<'TResponse>>
        abstract member SendBatchAsync: 'TMessage * List<string> * int -> Task<List<'TResponse>>

    type HttpClient() = 
        let _client: System.Net.Http.HttpClient = new System.Net.Http.HttpClient()

        let CreateRequestMessage message requestUri httpMethod = 
            let response = new HttpRequestMessage (
                Content = new StringContent(message),
                RequestUri = new Uri("http://" + requestUri),
                Method = httpMethod
            ) 

            let headers = response.Headers
            headers.TryAddWithoutValidation("Content-Type", "application/json") |> ignore

            response
        
        let GetResponseObjAsync (httpResponse: HttpResponseMessage) =
            let content: HttpContent = httpResponse.Content

            let getSerializedResponse = 
                async {
                    let! stringContent = content.ReadAsStringAsync() |> Async.AwaitTask
                    return JsonConvert.DeserializeObject<'T>(stringContent)
                } 
            
            getSerializedResponse |> Async.StartAsTask
            
        member this.Send message host (port: int): 'TResponse = 
            let request = CreateRequestMessage message host HttpMethod.Post

            let getHttpResponse = async {
                return! _client.SendAsync(request) |> Async.AwaitTask
            }

            let httpResponse = getHttpResponse |> Async.RunSynchronously

            let getResult = async {
                return! GetResponseObjAsync(httpResponse) |> Async.AwaitTask
            }

            getResult |> Async.RunSynchronously

        member this.SendAsync message host (port: int): Task<'TResponse> = 
            let request = CreateRequestMessage message host HttpMethod.Post

            let getHttpResponse = async {
                return! _client.SendAsync(request) |> Async.AwaitTask
            }

            let httpResponse = getHttpResponse |> Async.RunSynchronously

            let result = async {
                return! GetResponseObjAsync(httpResponse) |> Async.AwaitTask
            }

            result |> Async.StartAsTask 

        interface IClient with
            member this.Send message host port = this.Send message host port
            member this.SendAsync message host port = this.SendAsync message host port

    type HttpService() =
        let _client = new HttpClient()

        member this.Send (message: string, host: string, port: int): 'TResponse =
            _client.Send message host port

        member this.Send (message: 'TMessage, host: string, port: int): 'TResponse =
            let serializedMessage = JsonConvert.SerializeObject(message)
            _client.Send serializedMessage host port

        member this.SendAsync (message, host, port): Task<'TResponse> =
            _client.SendAsync message host port

        member this.SendAsync (message, host, port): Task<'TResponse> =
            let serializedMessage = JsonConvert.SerializeObject(message)
            _client.SendAsync serializedMessage host port 
            
        member this.SendBatch (message: string, hosts: List<string>, port: int): List<'TResponse> =
            hosts 
            |> Seq.map (fun h -> this.Send<'TResponse>(message, h, port))
            |> ResizeArray<'TResponse>

        member this.SendBatch (message: 'TMessage, hosts: List<string>, port: int): List<'TResponse> =
            hosts 
            |> Seq.map (fun h -> this.Send<'TMessage, 'TResponse>(message, h, port))
            |> ResizeArray<'TResponse> 

        member this.SendBatchAsync (message: string, hosts: List<string>, port: int): Task<List<'TResponse>> =
            let getResponses = async {
                return hosts 
                |> Seq.map (fun h -> this.SendAsync<'TResponse>(message, h, port) |> Async.AwaitTask)
                |> Async.Parallel
                |> Async.RunSynchronously
                |> ResizeArray<'TResponse>
            }
            getResponses |> Async.StartAsTask

        member this.SendBatchAsync (message: 'TMessage, hosts: List<string>, port: int): Task<List<'TResponse>> =
            let getResponses = async {
                return hosts 
                |> Seq.map (fun h -> this.SendAsync<'TMessage, 'TResponse>(message, h, port) |> Async.AwaitTask)
                |> Async.Parallel
                |> Async.RunSynchronously
                |> ResizeArray<'TResponse>
            }
            getResponses |> Async.StartAsTask

        interface ISenderService with
            member this.Send(message: string, host: string, port: int) = this.Send(message, host, port)
            member this.Send(message: 'TMessage, host: string, port: int) = this.Send(message, host, port)
            
            member this.SendAsync(message: string, host: string, port: int) = this.SendAsync(message, host, port)
            member this.SendAsync(message: 'TMessage, host: string, port: int) = this.SendAsync(message, host, port)

            member this.SendBatch(message: string, hosts: List<string>, port: int): List<'TResponse> = this.SendBatch(message, hosts, port)
            member this.SendBatch(message: 'TMessage, hosts: List<string>, port: int): List<'TResponse> = this.SendBatch(message, hosts, port)

            member this.SendBatchAsync(message: string, hosts: List<string>, port: int): Task<List<'TResponse>> = this.SendBatchAsync(message, hosts, port)
            member this.SendBatchAsync(message: 'TMessage, hosts: List<string>, port: int): Task<List<'TResponse>> = this.SendBatchAsync(message, hosts, port)
     
    type HttpController(baseRoute: string) =
        let registeredRoutes: Dictionary<string, Delegate> = new Dictionary<string, Delegate>()
        let baseRoute: string = baseRoute

        let SanitizeRoute (route: string): string =
            if route.EndsWith("/") then 
                route
            else
                route + "/"
        
        let GetFullRouteName (route: string): string =
            let sanitizedRoute = SanitizeRoute(route)
            baseRoute + sanitizedRoute

        let FindRoute (route: string): Delegate =
            let fullRouteName = GetFullRouteName route
            registeredRoutes.[fullRouteName]
        
        member this.HandleRouting (route: string) (message: 'TMessage): obj =
            let action = FindRoute(route)
            action.DynamicInvoke(message)

        member this.RegisterRoute (route: string) (callback: Delegate) =
            let fullRouteName = GetFullRouteName route
            if not (registeredRoutes.ContainsKey fullRouteName)
                then registeredRoutes.Add(fullRouteName, callback)

    type HttpServer(ipAddress: string) =
        let baseUrl: string = "http//" + ipAddress 

        let httpListener: HttpListener = new HttpListener()
        let controller: HttpController = new HttpController(baseUrl)
