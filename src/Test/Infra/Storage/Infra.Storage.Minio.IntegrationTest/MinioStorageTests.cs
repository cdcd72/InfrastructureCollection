using Infra.Core.Storage.Abstractions;
using NUnit.Framework;

namespace Infra.Storage.Minio.IntegrationTest;

public class MinioStorageTests
{
    private readonly IObjectStorage _storage;

    public MinioStorageTests()
    {
        var startup = new Startup();

        _storage = startup.GetService<IObjectStorage>();
    }

    #region Bucket

    [Test]
    public async Task BucketExistsSuccessAsync()
    {
        const string bucketName = "temp";

        Assert.That(await _storage.BucketExistsAsync(bucketName), Is.False);
    }

    [Test]
    public async Task MakeBucketSuccessAsync()
    {
        const string bucketName = "temp";

        await _storage.MakeBucketAsync(bucketName);

        Assert.That(await _storage.BucketExistsAsync(bucketName), Is.True);
    }

    [Test]
    public async Task RemoveBucketSuccessAsync()
    {
        const string bucketName = "temp";

        await _storage.MakeBucketAsync(bucketName);

        await _storage.RemoveBucketAsync(bucketName);

        Assert.That(await _storage.BucketExistsAsync(bucketName), Is.False);
    }

    [Test]
    public async Task ListBucketsSuccessAsync()
    {
        await _storage.MakeBucketAsync("temp1");
        await _storage.MakeBucketAsync("temp2");

        var bucketNames = await _storage.ListBucketsAsync();

        Assert.That(bucketNames, Has.Count.EqualTo(2));
    }

    #endregion

    #region Object

    // [Test]
    // public async Task ObjectExistsSuccessAsync()
    // {
    //     const string bucketName = "temp";
    //     const string objectName = "object";
    //
    //     Assert.That(await _storage.ObjectExistsAsync(bucketName, objectName), Is.False);
    // }
    //
    // [Test]
    // public async Task PutObjectSuccessAsync()
    // {
    //     const string bucketName = "temp";
    //     const string objectName = "object";
    //     var bytes = Array.Empty<byte>();
    //
    //     await _storage.PutObjectAsync(bucketName, objectName, new MemoryStream(bytes), bytes.LongLength);
    //
    //     Assert.That(await _storage.ObjectExistsAsync(bucketName, objectName), Is.True);
    // }

    #endregion

    [TearDown]
    public async Task TearDown()
    {
        var bucketNames = await _storage.ListBucketsAsync();

        foreach (var bucketName in bucketNames)
        {
            await _storage.RemoveBucketAsync(bucketName);
        }
    }
}
