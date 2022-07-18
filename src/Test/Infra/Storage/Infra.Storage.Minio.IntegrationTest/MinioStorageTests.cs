using System.Reflection;
using Infra.Core.Storage.Abstractions;
using NUnit.Framework;

namespace Infra.Storage.Minio.IntegrationTest;

public class MinioStorageTests
{
    private readonly IObjectStorage _storage;

    #region Properties

    private static string CurrentDirectory =>
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

    #endregion

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

        // Recovery
        await _storage.RemoveBucketAsync(bucketName);
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
        const string bucketName1 = "temp1";
        const string bucketName2 = "temp2";

        await _storage.MakeBucketAsync(bucketName1);
        await _storage.MakeBucketAsync(bucketName2);

        var bucketNames = await _storage.ListBucketsAsync();

        Assert.That(bucketNames, Has.Count.EqualTo(2));

        // Recovery
        await _storage.RemoveBucketAsync(bucketName1);
        await _storage.RemoveBucketAsync(bucketName2);
    }

    #endregion

    #region Object

    [Test]
    public async Task ObjectExistsSuccessAsync()
    {
        const string bucketName = "temp";
        const string objectName = "object";

        Assert.That(await _storage.ObjectExistsAsync(bucketName, objectName), Is.False);
    }

    [Test]
    public async Task PutObjectSuccessAsync()
    {
        const string bucketName = "temp";
        const string objectName = "object";

        await _storage.MakeBucketAsync(bucketName);

        var bytes =
            await File.ReadAllBytesAsync(Path.Combine(CurrentDirectory, "TestData", "Files", "test.jpg"));

        await _storage.PutObjectAsync(bucketName, objectName, new MemoryStream(bytes), bytes.Length);

        Assert.That(await _storage.ObjectExistsAsync(bucketName, objectName), Is.True);

        // Recovery
        await _storage.RemoveObjectAsync(bucketName, objectName);
        await _storage.RemoveBucketAsync(bucketName);
    }

    [Test]
    public async Task RemoveObjectSuccessAsync()
    {
        const string bucketName = "temp";
        const string objectName = "object";

        await _storage.MakeBucketAsync(bucketName);

        var bytes =
            await File.ReadAllBytesAsync(Path.Combine(CurrentDirectory, "TestData", "Files", "test.jpg"));

        await _storage.PutObjectAsync(bucketName, objectName, new MemoryStream(bytes), bytes.Length);

        await _storage.RemoveObjectAsync(bucketName, objectName);

        Assert.That(await _storage.ObjectExistsAsync(bucketName, objectName), Is.False);

        // Recovery
        await _storage.RemoveBucketAsync(bucketName);
    }

    [Test]
    public async Task GetObjectSuccessAsync()
    {
        const string bucketName = "temp";
        const string objectName = "object";

        await _storage.MakeBucketAsync(bucketName);

        var bytes =
            await File.ReadAllBytesAsync(Path.Combine(CurrentDirectory, "TestData", "Files", "test.jpg"));

        await _storage.PutObjectAsync(bucketName, objectName, new MemoryStream(bytes), bytes.Length);

        var stream = await _storage.GetObjectAsync(bucketName, objectName);

        Assert.That(stream, Has.Length.EqualTo(bytes.Length));

        // Recovery
        await _storage.RemoveObjectAsync(bucketName, objectName);
        await _storage.RemoveBucketAsync(bucketName);
    }

    [Test]
    public async Task ListObjectsSuccess()
    {
        const string bucketName = "temp";
        const string objectName = "object";

        await _storage.MakeBucketAsync(bucketName);

        var bytes =
            await File.ReadAllBytesAsync(Path.Combine(CurrentDirectory, "TestData", "Files", "test.jpg"));

        await _storage.PutObjectAsync(bucketName, objectName, new MemoryStream(bytes), bytes.Length);

        Assert.That(_storage.ListObjects(bucketName), Has.Count.EqualTo(1));

        // Recovery
        await _storage.RemoveObjectAsync(bucketName, objectName);
        await _storage.RemoveBucketAsync(bucketName);
    }

    #endregion
}
