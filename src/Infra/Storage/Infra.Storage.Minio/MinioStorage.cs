using System.Reactive.Linq;
using Infra.Core.Storage.Abstractions;
using Infra.Storage.Minio.Configuration;
using Infra.Storage.Minio.Configuration.Validators;
using Microsoft.Extensions.Options;
using Minio;

namespace Infra.Storage.Minio;

public class MinioStorage : IObjectStorage
{
    private readonly Settings settings;
    private readonly MinioClient minioClient;

    public MinioStorage(IOptions<Settings> settings)
    {
        this.settings = SettingsValidator.TryValidate(settings.Value, out var validationException) ? settings.Value : throw validationException;
        minioClient = GetMinioClient(this.settings);
    }

    #region Bucket

    public async Task<bool> BucketExistsAsync(string name)
        => await minioClient.BucketExistsAsync(
            new BucketExistsArgs()
                .WithBucket(name));

    public async Task MakeBucketAsync(string name)
        => await minioClient.MakeBucketAsync(
            new MakeBucketArgs()
                .WithBucket(name));

    public async Task RemoveBucketAsync(string name)
        => await minioClient.RemoveBucketAsync(
            new RemoveBucketArgs()
                .WithBucket(name));

    public async Task<List<string>> ListBucketsAsync()
    {
        var result = await minioClient.ListBucketsAsync();

        return result.Buckets.Select(bucket => bucket.Name).ToList();
    }

    #endregion

    #region Object

    public async Task<bool> ObjectExistsAsync(string bucketName, string objectName)
    {
        try
        {
            await minioClient.StatObjectAsync(
                new StatObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName));

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task PutObjectAsync(string bucketName, string objectName, Stream data, long size)
        => await minioClient.PutObjectAsync(
            new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithStreamData(data)
                .WithObjectSize(size));

    public async Task RemoveObjectAsync(string bucketName, string objectName)
        => await minioClient.RemoveObjectAsync(
            new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName));

    public async Task<Stream> GetObjectAsync(string bucketName, string objectName)
    {
        Stream ms = new MemoryStream();

        await minioClient.GetObjectAsync(
            new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithCallbackStream(stream => stream.CopyTo(ms)));

        return ms;
    }

    public List<string> ListObjects(string bucketName, string prefix = null, bool recursive = true)
    {
        var result = new List<string>();

        var observable = minioClient.ListObjectsAsync(
            new ListObjectsArgs()
                .WithBucket(bucketName)
                .WithPrefix(prefix)
                .WithRecursive(recursive));

        observable.Subscribe(item => result.Add(item.Key));

        observable.DefaultIfEmpty().Wait();

        return result;
    }

    #endregion

    #region Private Method

    private static MinioClient GetMinioClient(Settings settings)
    {
        var minioClient = new MinioClient()
            .WithEndpoint(settings.Endpoint)
            .WithCredentials(settings.AccessKey, settings.SecretKey);

        if (settings.Timeout > 0)
            minioClient.WithTimeout(settings.Timeout);

        if (settings.EnableSsl)
            minioClient.WithSSL();

        return minioClient.Build();
    }

    #endregion
}
