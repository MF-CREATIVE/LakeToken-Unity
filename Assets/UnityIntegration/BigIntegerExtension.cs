using System;
using System.Numerics;
public static class BigIntegerExtension
{
    public static readonly int ICXDecimals = 18;

    /// <summary>10^18</summary>
    public static readonly BigInteger ICXDivider = BigInteger.Pow(new BigInteger(10), ICXDecimals);

    public static readonly BigInteger MaxValue = BigInteger.Parse("115792089237316195423570985008687907853269984665640564039457584007913129639935");

    public static double DivideToDouble(this BigInteger x, BigInteger y)
    {
        double result = Math.Exp(BigInteger.Log(x) - BigInteger.Log(y));

        return result;
    }

    public static BigInteger MultiplyByDouble(this BigInteger x, double y)
    {
        BigInteger result = BigInteger.Multiply(x, (BigInteger)(y * 100000)) / 100000;

        return result;
    }

    public static double ToCoins(this BigInteger value, int decimals)
    {
        return value.DivideToDouble(BigInteger.Pow(10, decimals));
    }
}

public static class DoubleExtension
{
    public static BigInteger FromCoins(this double valueCoins, int decimals)
    {
        return BigInteger.Pow(10, decimals).MultiplyByDouble(valueCoins);
    }
}