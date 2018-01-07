using Newtonsoft.Json;
using Notifications.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Service
{
    //public class TcpService : ISenderService
    //{
    //    public TcpService() { }

    //    public void Send(string message, string host, int port)
    //    {
    //        SendInternal(message, host, port);
    //    }
        
    //    public void Send<TMessage>(TMessage messageObj, string host, int port)
    //    {
    //        string serializedMsg = JsonConvert.SerializeObject(messageObj);

    //        SendInternal(serializedMsg, host, port);
    //    }

    //    public Task SendAsync(string message, string host, int port)
    //    {
    //        return SendInternalAsync(message, host, port);
    //    }

    //    public Task SendAsync<TMessage>(TMessage messageObj, string host, int port)
    //    {
    //        string serializedMsg = JsonConvert.SerializeObject(messageObj);

    //        return SendInternalAsync(serializedMsg, host, port);
    //    }

    //    public void SendBatch(string message, List<string> hosts, int port)
    //    {
    //        foreach (var host in hosts)
    //        {
    //            Send(message, host, port);
    //        }
    //    }

    //    public void SendBatch<TMessage>(TMessage messageObj, List<string> hosts, int port)
    //    {
    //        foreach (var host in hosts)
    //        {
    //            Send(messageObj, host, port);
    //        }
    //    }

    //    public Task SendBatchAsync(string message, List<string> hosts, int port)
    //    {
    //        var taskList = new List<Task>();

    //        foreach (var host in hosts)
    //        {
    //            taskList.Add(SendAsync(message, host, port));
    //        }

    //        return Task.WhenAll(taskList);
    //    }

    //    public Task SendBatchAsync<TMessage>(TMessage messageObj, List<string> hosts, int port)
    //    {
    //        var taskList = new List<Task>();

    //        foreach (var host in hosts)
    //        {
    //            taskList.Add(SendAsync(messageObj, host, port));
    //        }

    //        return Task.WhenAll(taskList);
    //    }

    //    private void SendInternal(string message, string host, int port)
    //    {
    //        using (var client = new TcpSender())
    //        {
    //            try
    //            {
    //                client.Send(message, host, port);
    //            }
    //            catch (Exception)
    //            {
    //                // if the ip doesn't respond then it doesn't have a client 
    //            }
    //        }
    //    }

    //    private async Task SendInternalAsync(string message, string host, int port)
    //    {
    //        using (var client = new TcpSender())
    //        {
    //            try
    //            {
    //                await client.SendAsync(message, host, port);
    //            }
    //            catch(Exception)
    //            {
    //                // if the ip doesn't respond then it doesn't have a client
    //            }
    //        }
    //    }
    //}
}
