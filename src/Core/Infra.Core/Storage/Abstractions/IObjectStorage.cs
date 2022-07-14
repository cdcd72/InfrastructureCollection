using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Infra.Core.Storage.Abstractions
{
    public interface IObjectStorage
    {
        #region Bucket

        Task<bool> BucketExistsAsync(string name);

        Task MakeBucketAsync(string name);

        Task RemoveBucketAsync(string name);

        Task<List<string>> ListBucketsAsync();

        #endregion

        #region Object

        Task<bool> ObjectExistsAsync(string bucketName, string objectName);

        Task PutObjectAsync(string bucketName, string objectName, Stream data);

        Task RemoveObjectAsync(string bucketName, string objectName);

        Task<Stream> GetObjectAsync(string bucketName, string objectName);

        List<string> ListObjects(string bucketName, string prefix = null, bool recursive = true);

        #endregion
    }
}
