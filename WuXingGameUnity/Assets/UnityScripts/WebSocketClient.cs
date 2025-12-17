using UnityEngine;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Concurrent;

public class WebSocketClient : MonoBehaviour
{
    private ClientWebSocket webSocket;
    private CancellationTokenSource cancellationTokenSource;
    private string serverUrl = "ws://localhost:5000/gameHub"; // SignalR hub endpoint
    
    // Thread-safe message queue for handling WebSocket messages on the main thread
    private ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();
    
    // Events for game events
    public event Action<string> OnBattleResultReceived;
    public event Action<string> OnPlayerJoined;
    public event Action<string> OnElementSelected;
    
    void Start()
    {
        ConnectToServer();
    }
    
    void Update()
    {
        // Process messages on the main thread
        while (messageQueue.TryDequeue(out string message))
        {
            ProcessMessage(message);
        }
    }
    
    async void ConnectToServer()
    {
        try
        {
            webSocket = new ClientWebSocket();
            cancellationTokenSource = new CancellationTokenSource();
            
            Debug.Log("Connecting to server: " + serverUrl);
            await webSocket.ConnectAsync(new Uri(serverUrl), cancellationTokenSource.Token);
            Debug.Log("Connected to server");
            
            // Send negotiation message for SignalR
            var negotiateMessage = "{\"protocol\":\"json\",\"version\":1}" + (char)0x1e;
            var negotiateBuffer = Encoding.UTF8.GetBytes(negotiateMessage);
            await webSocket.SendAsync(new ArraySegment<byte>(negotiateBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
            
            // Start listening for messages
            _ = ListenForMessages();
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to connect to server: " + ex.Message);
        }
    }
    
    async Task ListenForMessages()
    {
        var buffer = new byte[1024 * 4];
        
        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    // Remove SignalR end-of-message character if present
                    if (message.EndsWith(((char)0x1e).ToString()))
                    {
                        message = message.Substring(0, message.Length - 1);
                    }
                    // Queue message for processing on main thread
                    messageQueue.Enqueue(message);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error receiving message: " + ex.Message);
        }
    }
    
    void ProcessMessage(string message)
    {
        Debug.Log("Processing message: " + message);
        
        // Handle SignalR protocol messages
        if (message.Contains("\"protocol\":\"json\""))
        {
            Debug.Log("SignalR protocol established");
            return;
        }
        
        // Parse SignalR messages
        string target = JsonHelper.ExtractTarget(message);
        List<string> arguments = JsonHelper.ExtractArguments(message);
        
        // Trigger appropriate events based on target
        switch (target)
        {
            case "ReceiveBattleResult":
                if (arguments.Count > 0)
                    OnBattleResultReceived?.Invoke(arguments[0]);
                break;
            case "PlayerJoined":
                if (arguments.Count > 0)
                    OnPlayerJoined?.Invoke(arguments[0]);
                break;
            case "ElementSelected":
                if (arguments.Count > 0)
                    OnElementSelected?.Invoke(arguments[0]);
                break;
            default:
                Debug.Log("Unknown message target: " + target);
                break;
        }
    }
    
    public async void SendMessage(string message)
    {
        if (webSocket == null || webSocket.State != WebSocketState.Open)
        {
            Debug.LogWarning("WebSocket is not open. Current state: " + (webSocket?.State.ToString() ?? "null"));
            return;
        }
        
        try
        {
            // Add SignalR end-of-message character
            if (!message.EndsWith(((char)0x1e).ToString()))
            {
                message += (char)0x1e;
            }
            
            var buffer = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            Debug.Log("Sent message: " + message);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to send message: " + ex.Message);
        }
    }
    
    public void JoinGame(string playerId)
    {
        var joinHubMessage = $"{{\"type\":1,\"invocationId\":\"1\",\"target\":\"JoinGame\",\"arguments\":[\"{playerId}\"]}}";
        SendMessage(joinHubMessage);
    }
    
    public void SelectElement(string playerId, string element)
    {
        var selectMessage = $"{{\"type\":1,\"invocationId\":\"2\",\"target\":\"SelectElement\",\"arguments\":[\"{playerId}\",\"{element}\"]}}";
        SendMessage(selectMessage);
    }
    
    void OnDestroy()
    {
        if (cancellationTokenSource != null)
        {
            cancellationTokenSource.Cancel();
        }
        
        if (webSocket != null)
        {
            try
            {
                webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None).Wait();
            }
            catch (Exception ex)
            {
                Debug.LogError("Error closing WebSocket: " + ex.Message);
            }
            webSocket.Dispose();
        }
    }
}