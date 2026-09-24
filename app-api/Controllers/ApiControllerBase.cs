using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

public class ApiControllerBase : ControllerBase
{
    public ObjectResult ComputeObjectResultForDuplicateRequest(IdempotencyRecord previousRequest, string requestBodyHash)
    {
        if (requestBodyHash != previousRequest.RequestBodyHash)
        {
            return Conflict("An idempotency key was reused with a different request body");
        }
        else if (previousRequest.Status == IdempotencyRecordStatus.Processing)
        {
            return Conflict("An identical request is already being processed");
        }
        else if (previousRequest.ResponseStatusCode is not null && previousRequest.ResponseBody is not null)
        {
            return StatusCode(previousRequest.ResponseStatusCode.Value, JsonSerializer.Deserialize<object>(previousRequest.ResponseBody));
        }
        else
        {
            throw new Exception("Previous Request not in valid state!");
        }
    }
}