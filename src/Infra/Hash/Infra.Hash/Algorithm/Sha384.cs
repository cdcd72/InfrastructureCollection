﻿using System.Security.Cryptography;
using System.Text;
using Infra.Core.Hash.Abstractions;

namespace Infra.Hash.Algorithm;

public class Sha384 : IHashAlgorithm
{
    public string Hash(string text) => HashValue(text);

    public byte[] Hash(byte[] bytes) => HashBytes(bytes);

    public bool Verify(string text, string hashedText) => HashValue(text) == hashedText;

    public bool Verify(byte[] bytes, byte[] hashedBytes) => !HashBytes(bytes).Where((b, i) => b != hashedBytes[i]).Any();

    #region Private Method

    private static string HashValue(string text) => Convert.ToBase64String(HashBytes(Encoding.UTF8.GetBytes(text)));

    private static byte[] HashBytes(byte[] bytes)
    {
        using var sha384 = SHA384.Create();

        sha384.Initialize();

        return sha384.ComputeHash(bytes);
    }

    #endregion
}
