using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ProblemSolver.Configuration.Bot;
using ProblemSolver.Logic.BotServices.Interfaces;
using ProblemSolver.Shared.Bot.Dtos.Requests;
using ProblemSolver.Shared.Bot.Dtos.Responses;

namespace ProblemSolver.Logic.BotServices.Implementations
{
    public class BotService : IBotService
    {
        private readonly ITaskRequestConverter _converter;
        private readonly BotConnectionConfig _connectionConfig;
        private readonly ICodeExtractor _codeExtractor;

        public BotService(ITaskRequestConverter converter, IOptions<BotConnectionConfig> connectionConfig,
            ICodeExtractor codeExtractor)
        {
            _converter = converter;
            _connectionConfig = connectionConfig.Value;
            _codeExtractor = codeExtractor;
        }

        public async Task<TaskResponse> ProcessRequestAsync(TaskRequest request)
        {
            string response = string.Empty;
            using var webSocket = new ClientWebSocket();

            try
            {
                var connection = CreateWebSocketUri();
                await webSocket.ConnectAsync(connection, CancellationToken.None);
                var message = new
                {
                    message = _converter.ConvertToMessage(request),
                    value = request.UseBot.ToString(),
                    language = "English"
                };

                string jsonMessage = JsonSerializer.Serialize(message);
                Console.WriteLine(jsonMessage);
                await SendMessageAsync(webSocket, jsonMessage);

                response = await ReceiveMessageAsync(webSocket);

                if (webSocket.State == WebSocketState.Open)
                {
                    var clearContextMessage = new { action = "clear_context" };
                    string clearContextJson = JsonSerializer.Serialize(clearContextMessage);
                    await SendMessageAsync(webSocket, clearContextJson);
                }
            }
            catch (Exception e)
            {
                //TODO: switch to logger
                Console.WriteLine("WebSocket error: " + e.Message);
            }
            finally
            {
                if (webSocket.State == WebSocketState.Open || webSocket.State == WebSocketState.CloseReceived)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    Console.WriteLine("WebSocket connection closed.");
                }
            }

            if (response == string.Empty)
                return new TaskResponse(request.Language, "Failed");
            Console.WriteLine(response);
            string code = _codeExtractor.ExtractCode(response);

            return new TaskResponse(request.Language, code);
        }

        private Uri CreateWebSocketUri()
        {
            string clientId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            string webSocketUrl =
                $"{_connectionConfig.ConnectionString}/{clientId}"; // Replace [hostname] with the actual hostname

            return new Uri(webSocketUrl);
        }

        private static async Task SendMessageAsync(ClientWebSocket ws, string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            await ws.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true,
                CancellationToken.None);
        }

        private async Task<string> ReceiveMessageAsync(ClientWebSocket ws)
        {
            byte[] buffer = new byte[_connectionConfig.BufferSize];
            var fullResponse = new StringBuilder();
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(_connectionConfig.TimeOutInSeconds));

            try
            {
                while (ws.State == WebSocketState.Open)
                {
                    var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationTokenSource.Token);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);

                        break;
                    }

                    string chunk = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    fullResponse.Append(chunk);
                }
            }
            catch (OperationCanceledException)
            {
                //TODO: switch to logger
                Console.WriteLine("Receive operation timed out.");
            }
            catch (WebSocketException wse)
            {
                Console.WriteLine("WebSocket exception: " + wse.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("General exception: " + ex.Message);
            }

            return fullResponse.ToString();
        }
    }
}
