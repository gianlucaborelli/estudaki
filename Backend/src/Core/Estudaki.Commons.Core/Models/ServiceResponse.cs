using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Estudaki.Commons.Core.Models;

public class ServiceResponse
{
    public bool Success { get; protected set; }
    public string? Message { get; protected set; }

    protected ServiceResponse(bool success, string? message = null)
    {
        Success = success;
        Message = message;
    }

    public static ServiceResponse Ok(string? message = null)
        => new(true, message);

    public static ServiceResponse Fail(string message)
        => new(false, message);

}

public class ServiceResponse<T> : ServiceResponse
{
    public T? Data { get; private set; }

    private ServiceResponse(
        bool success,
        T? data,
        string? message)
        : base(success, message)
    {
        Data = data;
    }

    public static ServiceResponse<T> Ok(
        T data,
        string? message = null)
        => new(true, data, message );

    public static ServiceResponse<T> Fail(
        string message)
        => new(false, default, message);
}
