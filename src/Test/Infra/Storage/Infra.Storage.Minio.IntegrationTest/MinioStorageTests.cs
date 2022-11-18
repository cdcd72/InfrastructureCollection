using System.Reflection;
using Infra.Core.Storage.Abstractions;
using NUnit.Framework;

namespace Infra.Storage.Minio.IntegrationTest;

public class MinioStorageTests
{
    private readonly IObjectStorage storage;

    #region Properties

    private static string CurrentDirectory =>
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

    #endregion

    public MinioStorageTests()
    {
        var startup = new Startup();

        storage = startup.GetService<IObjectStorage>();
    }

    #region Bucket

    [Test]
    public async Task BucketExistsSuccessAsync()
    {
        const string bucketName = "temp";

        Assert.That(await storage.BucketExistsAsync(bucketName), Is.False);
    }

    [Test]
    public async Task MakeBucketSuccessAsync()
    {
        const string bucketName = "temp";

        await storage.MakeBucketAsync(bucketName);

        Assert.That(await storage.BucketExistsAsync(bucketName), Is.True);

        // Recovery
        await storage.RemoveBucketAsync(bucketName);
    }

    [Test]
    public async Task RemoveBucketSuccessAsync()
    {
        const string bucketName = "temp";

        await storage.MakeBucketAsync(bucketName);

        await storage.RemoveBucketAsync(bucketName);

        Assert.That(await storage.BucketExistsAsync(bucketName), Is.False);
    }

    [Test]
    public async Task ListBucketsSuccessAsync()
    {
        const string bucketName1 = "temp1";
        const string bucketName2 = "temp2";

        await storage.MakeBucketAsync(bucketName1);
        await storage.MakeBucketAsync(bucketName2);

        var bucketNames = await storage.ListBucketsAsync();

        Assert.That(bucketNames, Has.Count.EqualTo(2));

        // Recovery
        await storage.RemoveBucketAsync(bucketName1);
        await storage.RemoveBucketAsync(bucketName2);
    }

    #endregion

    #region Object

    [Test]
    public async Task ObjectExistsSuccessAsync()
    {
        const string bucketName = "temp";
        const string objectName = "object";

        Assert.That(await storage.ObjectExistsAsync(bucketName, objectName), Is.False);
    }

    [Test]
    public async Task PutObjectSuccessAsync()
    {
        const string bucketName = "temp";
        const string objectName = "object";

        await storage.MakeBucketAsync(bucketName);

        var bytes =
            await File.ReadAllBytesAsync(Path.Combine(CurrentDirectory, "TestData", "Files", "test.jpg"));

        await storage.PutObjectAsync(bucketName, objectName, new MemoryStream(bytes), bytes.Length);

        Assert.That(await storage.ObjectExistsAsync(bucketName, objectName), Is.True);

        // Recovery
        await storage.RemoveObjectAsync(bucketName, objectName);
        await storage.RemoveBucketAsync(bucketName);
    }

    [Test]
    public async Task RemoveObjectSuccessAsync()
    {
        const string bucketName = "temp";
        const string objectName = "object";

        await storage.MakeBucketAsync(bucketName);

        var bytes =
            await File.ReadAllBytesAsync(Path.Combine(CurrentDirectory, "TestData", "Files", "test.jpg"));

        await storage.PutObjectAsync(bucketName, objectName, new MemoryStream(bytes), bytes.Length);

        await storage.RemoveObjectAsync(bucketName, objectName);

        Assert.That(await storage.ObjectExistsAsync(bucketName, objectName), Is.False);

        // Recovery
        await storage.RemoveBucketAsync(bucketName);
    }

    [Test]
    public async Task GetObjectSuccessAsync()
    {
        const string bucketName = "temp";
        const string objectName = "object";

        await storage.MakeBucketAsync(bucketName);

        var bytes =
            await File.ReadAllBytesAsync(Path.Combine(CurrentDirectory, "TestData", "Files", "test.jpg"));

        await storage.PutObjectAsync(bucketName, objectName, new MemoryStream(bytes), bytes.Length);

        var stream = await storage.GetObjectAsync(bucketName, objectName);

        Assert.That(stream, Has.Length.EqualTo(bytes.Length));

        // Recovery
        await storage.RemoveObjectAsync(bucketName, objectName);
        await storage.RemoveBucketAsync(bucketName);
    }

    [Test]
    public async Task ListObjectsSuccess()
    {
        const string bucketName = "temp";
        const string objectName = "object";

        await storage.MakeBucketAsync(bucketName);

        var bytes =
            await File.ReadAllBytesAsync(Path.Combine(CurrentDirectory, "TestData", "Files", "test.jpg"));

        await storage.PutObjectAsync(bucketName, objectName, new MemoryStream(bytes), bytes.Length);

        Assert.That(storage.ListObjects(bucketName), Has.Count.EqualTo(1));

        // Recovery
        await storage.RemoveObjectAsync(bucketName, objectName);
        await storage.RemoveBucketAsync(bucketName);
    }

    #endregion
}
