using System.Diagnostics.CodeAnalysis;
using System.Dynamic;

namespace Maturaball_Tischreservierung.Models
{
    public class ServiceResult : IServiceResult
    {
        public InternalStatusCode Status { get; set; } = InternalStatusCode.Success;

        public static ServiceResult Completed => new();

        public Dictionary<string, List<string>> Errors { get; set; } = new Dictionary<string, List<string>>();

        public ServiceResult()
        {
        }

        public ServiceResult(string key, params string[] errors)
        {
            Status = InternalStatusCode.BadRequest;
            Errors.Add(key, errors.ToList());
        }

        public dynamic GetErrors()
        {
            var errorResult = new ExpandoObject() as IDictionary<string, object>;

            foreach (var item in Errors)
            {
                errorResult.Add(item.Key, item.Value);
            }

            return errorResult;
        }

        public bool Failed()
        {
            return Status != InternalStatusCode.Success;
        }
    }

    public class ServiceResult<T> : ServiceResult, IServiceResult<T>
    {
        public T? Value { get; set; }

        public ServiceResult(T result) : base()
        {
            Value = result;
        }

        public ServiceResult(string key, params string[] errors) : base(key, errors)
        {
        }

        [MemberNotNullWhen(false, nameof(Value))]
        public new bool Failed()
        {
            return Status != InternalStatusCode.Success;
        }
    }

    public enum InternalStatusCode
    {
        Success = 200,
        BadRequest = 400,
        InternalServerError = 500,
    }
}