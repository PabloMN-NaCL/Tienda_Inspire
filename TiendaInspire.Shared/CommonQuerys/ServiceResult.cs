using System;
using System.Collections.Generic;
using System.Text;

namespace TiendaInspire.Shared.CommonQuerys
{
    
    /// Para representar resultados de servicios fuera de la capa de presentacion.
   
    public class ServiceResult
    {

        public bool Succeeded { get; set; }


        public string Message { get; set; } = string.Empty;

        public IEnumerable<string> Errors { get; set; } = new List<string>();

        public static ServiceResult Success(string message = "")
        {
            return new ServiceResult
            {
                Succeeded = true,
                Message = message
            };
        }

 
        public static ServiceResult Failure(IEnumerable<string> errors)
        {
            return new ServiceResult
            {
                Succeeded = false,
                Errors = errors
            };
        }


        public static ServiceResult Failure(string error)
        {
            return new ServiceResult
            {
                Succeeded = false,
                Errors = new[] { error }
            };
        }
    }


    public class ServiceResult<T> : ServiceResult
    {

        public T? Data { get; set; }

     
        public static ServiceResult<T> Success(T data, string message = "")
        {
            return new ServiceResult<T>
            {
                Succeeded = true,
                Data = data,
                Message = message
            };
        }

        public new static ServiceResult<T> Failure(IEnumerable<string> errors)
        {
            return new ServiceResult<T>
            {
                Succeeded = false,
                Errors = errors
            };
        }


        public new static ServiceResult<T> Failure(string error)
        {
            return new ServiceResult<T>
            {
                Succeeded = false,
                Errors = new[] { error }
            };
        }
    }
}