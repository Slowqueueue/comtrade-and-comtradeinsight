﻿using System.Runtime.CompilerServices;

namespace COMTRADE.Helpers;

/// <summary>
/// Defines a set of little-endian byte order interoperability functions.
/// </summary>
/// <remarks>
/// This class is setup to support aggressive in-lining of little endian conversions. Bounds
/// will not be checked as part of this function call, if bounds are violated, the exception
/// will be thrown at the <see cref="Array"/> level.
/// </remarks>
public static class LittleEndian
{
    #region [ To Value from Pointer ]

    /// <summary>
    /// Returns a <see cref="bool"/> value converted from one byte at a specified position in a byte array.
    /// </summary>
    /// <param name="buffer">An array of bytes.</param>
    /// <returns>true if the byte at startIndex in value is nonzero; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than zero or greater than the length of value minus 1.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool ToBoolean(byte* buffer)
    {
        return buffer[0] != 0;
    }

    /// <summary>
    /// Returns a Unicode character converted from two bytes, accounting for target endian-order, at a specified position in a byte array.
    /// </summary>
    /// <param name="buffer">An array of bytes (i.e., buffer containing binary image of value).</param>
    /// <returns>A character formed by two bytes beginning at startIndex.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than zero or greater than the length of value minus 1.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe char ToChar(byte* buffer)
    {
        return (char)ToInt16(buffer);
    }

    /// <summary>
    /// Returns a double-precision floating point number converted from eight bytes, accounting for target endian-order, at a specified position in a byte array.
    /// </summary>
    /// <param name="buffer">An array of bytes (i.e., buffer containing binary image of value).</param>
    /// <returns>A double-precision floating point number formed by eight bytes beginning at startIndex.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than zero or greater than the length of value minus 1.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe double ToDouble(byte* buffer)
    {
        long int64 = ToInt64(buffer);
        return *(double*)&int64;
    }

    /// <summary>
    /// Returns a 16-bit signed integer converted from two bytes, accounting for target endian-order, at a specified position in a byte array.
    /// </summary>
    /// <param name="buffer">An array of bytes (i.e., buffer containing binary image of value).</param>
    /// <returns>A 16-bit signed integer formed by two bytes beginning at startIndex.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than zero or greater than the length of value minus 1.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe short ToInt16(byte* buffer)
    {
        return (short)(buffer[0] | buffer[1] << 8);
    }

    /// <summary>
    /// Returns a 32-bit signed integer converted from four bytes, accounting for target endian-order, at a specified position in a byte array.
    /// </summary>
    /// <param name="buffer">An array of bytes (i.e., buffer containing binary image of value).</param>
    /// <returns>A 32-bit signed integer formed by four bytes beginning at startIndex.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than zero or greater than the length of value minus 1.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int ToInt32(byte* buffer)
    {
        return buffer[0] |
               buffer[1] << 8 |
               buffer[2] << 16 |
               buffer[3] << 24;
    }

    /// <summary>
    /// Returns a 64-bit signed integer converted from eight bytes, accounting for target endian-order, at a specified position in a byte array.
    /// </summary>
    /// <param name="buffer">An array of bytes (i.e., buffer containing binary image of value).</param>
    /// <returns>A 64-bit signed integer formed by eight bytes beginning at startIndex.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than zero or greater than the length of value minus 1.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe long ToInt64(byte* buffer)
    {
        return buffer[0] |
               (long)buffer[1] << 8 |
               (long)buffer[2] << 16 |
               (long)buffer[3] << 24 |
               (long)buffer[4] << 32 |
               (long)buffer[5] << 40 |
               (long)buffer[6] << 48 |
               (long)buffer[7] << 56;
    }

    /// <summary>
    /// Returns a single-precision floating point number converted from four bytes, accounting for target endian-order, at a specified position in a byte array.
    /// </summary>
    /// <param name="buffer">An array of bytes (i.e., buffer containing binary image of value).</param>
    /// <returns>A single-precision floating point number formed by four bytes beginning at startIndex.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than zero or greater than the length of value minus 1.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe float ToSingle(byte* buffer)
    {
        int int32 = ToInt32(buffer);
        return *(float*)&int32;
    }

    /// <summary>
    /// Returns a 16-bit unsigned integer converted from two bytes, accounting for target endian-order, at a specified position in a byte array.
    /// </summary>
    /// <param name="buffer">An array of bytes (i.e., buffer containing binary image of value).</param>
    /// <returns>A 16-bit unsigned integer formed by two bytes beginning at startIndex.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than zero or greater than the length of value minus 1.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe ushort ToUInt16(byte* buffer)
    {
        return (ushort)ToInt16(buffer);
    }

    /// <summary>
    /// Returns a 32-bit unsigned integer converted from four bytes, accounting for target endian-order, at a specified position in a byte array.
    /// </summary>
    /// <param name="buffer">An array of bytes (i.e., buffer containing binary image of value).</param>
    /// <returns>A 32-bit unsigned integer formed by four bytes beginning at startIndex.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than zero or greater than the length of value minus 1.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe uint ToUInt32(byte* buffer)
    {
        return (uint)ToInt32(buffer);
    }

    /// <summary>
    /// Returns a 64-bit unsigned integer converted from eight bytes, accounting for target endian-order, at a specified position in a byte array.
    /// </summary>
    /// <param name="value">An array of bytes (i.e., buffer containing binary image of value).</param>
    /// <returns>A 64-bit unsigned integer formed by eight bytes beginning at startIndex.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than zero or greater than the length of value minus 1.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe ulong ToUInt64(byte* value)
    {
        return (ulong)ToInt64(value);
    }

    /// <summary>
    /// Returns a 128-bit decimal converted from 16 bytes, accounting for target endian-order, at a specified position in a byte array.
    /// </summary>
    /// <param name="buffer">An array of bytes (i.e., buffer containing binary image of value).</param>
    /// <returns>A 128-bit decimal formed by 16 bytes beginning at startIndex.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than zero or greater than the length of value minus 1.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe decimal ToDecimal(byte* buffer)
    {
        if (BitConverter.IsLittleEndian)
            return *(decimal*)buffer;

        decimal returnValue;
        byte* destination = (byte*)&returnValue;
        //int flags
        destination[0] = buffer[3];
        destination[1] = buffer[2];
        destination[2] = buffer[1];
        destination[3] = buffer[0];
        //int high
        destination[4] = buffer[7];
        destination[5] = buffer[6];
        destination[6] = buffer[5];
        destination[7] = buffer[4];
        //int low
        destination[8] = buffer[11];
        destination[9] = buffer[10];
        destination[10] = buffer[9];
        destination[11] = buffer[8];
        //int mid
        destination[12] = buffer[15];
        destination[13] = buffer[14];
        destination[14] = buffer[13];
        destination[15] = buffer[12];
        return returnValue;
    }

    #endregion

    #region [ To Value from Byte Array ]

    /// <summary>
    /// Returns a <see cref="bool"/> value converted from one byte at a specified position in a byte array.
    /// </summary>
    /// <param name="buffer">An array of bytes.</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns>true if the byte at startIndex in value is nonzero; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than zero or greater than the length of value minus 1.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ToBoolean(byte[] buffer, int startIndex)
    {
        return buffer[startIndex] != 0;
    }

    /// <summary>
    /// Returns a Unicode character converted from two bytes, accounting for target endian-order, at a specified position in a byte array.
    /// </summary>
    /// <param name="buffer">An array of bytes (i.e., buffer containing binary image of value).</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns>A character formed by two bytes beginning at startIndex.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than zero or greater than the length of value minus 1.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char ToChar(byte[] buffer, int startIndex)
    {
        return (char)ToInt16(buffer, startIndex);
    }

    /// <summary>
    /// Returns a double-precision floating point number converted from eight bytes, accounting for target endian-order, at a specified position in a byte array.
    /// </summary>
    /// <param name="buffer">An array of bytes (i.e., buffer containing binary image of value).</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns>A double-precision floating point number formed by eight bytes beginning at startIndex.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than zero or greater than the length of value minus 1.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe double ToDouble(byte[] buffer, int startIndex)
    {
        long int64 = ToInt64(buffer, startIndex);
        return *(double*)&int64;
    }

    /// <summary>
    /// Returns a 16-bit signed integer converted from two bytes, accounting for target endian-order, at a specified position in a byte array.
    /// </summary>
    /// <param name="buffer">An array of bytes (i.e., buffer containing binary image of value).</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns>A 16-bit signed integer formed by two bytes beginning at startIndex.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than zero or greater than the length of value minus 1.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ToInt16(byte[] buffer, int startIndex)
    {
        return (short)(buffer[startIndex] | buffer[startIndex + 1] << 8);
    }

    /// <summary>
    /// Returns a 32-bit signed integer converted from four bytes, accounting for target endian-order, at a specified position in a byte array.
    /// </summary>
    /// <param name="buffer">An array of bytes (i.e., buffer containing binary image of value).</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns>A 32-bit signed integer formed by four bytes beginning at startIndex.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than zero or greater than the length of value minus 1.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToInt32(byte[] buffer, int startIndex)
    {
        return buffer[startIndex + 0] |
               buffer[startIndex + 1] << 8 |
               buffer[startIndex + 2] << 16 |
               buffer[startIndex + 3] << 24;
    }

    /// <summary>
    /// Returns a 64-bit signed integer converted from eight bytes, accounting for target endian-order, at a specified position in a byte array.
    /// </summary>
    /// <param name="buffer">An array of bytes (i.e., buffer containing binary image of value).</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns>A 64-bit signed integer formed by eight bytes beginning at startIndex.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than zero or greater than the length of value minus 1.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ToInt64(byte[] buffer, int startIndex)
    {
        return buffer[startIndex + 0] |
               (long)buffer[startIndex + 1] << 8 |
               (long)buffer[startIndex + 2] << 16 |
               (long)buffer[startIndex + 3] << 24 |
               (long)buffer[startIndex + 4] << 32 |
               (long)buffer[startIndex + 5] << 40 |
               (long)buffer[startIndex + 6] << 48 |
               (long)buffer[startIndex + 7] << 56;
    }

    /// <summary>
    /// Returns a single-precision floating point number converted from four bytes, accounting for target endian-order, at a specified position in a byte array.
    /// </summary>
    /// <param name="buffer">An array of bytes (i.e., buffer containing binary image of value).</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns>A single-precision floating point number formed by four bytes beginning at startIndex.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than zero or greater than the length of value minus 1.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe float ToSingle(byte[] buffer, int startIndex)
    {
        int int32 = ToInt32(buffer, startIndex);
        return *(float*)&int32;
    }

    /// <summary>
    /// Returns a 16-bit unsigned integer converted from two bytes, accounting for target endian-order, at a specified position in a byte array.
    /// </summary>
    /// <param name="buffer">An array of bytes (i.e., buffer containing binary image of value).</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns>A 16-bit unsigned integer formed by two bytes beginning at startIndex.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than zero or greater than the length of value minus 1.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ToUInt16(byte[] buffer, int startIndex)
    {
        return (ushort)ToInt16(buffer, startIndex);
    }

    /// <summary>
    /// Returns a 32-bit unsigned integer converted from four bytes, accounting for target endian-order, at a specified position in a byte array.
    /// </summary>
    /// <param name="buffer">An array of bytes (i.e., buffer containing binary image of value).</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns>A 32-bit unsigned integer formed by four bytes beginning at startIndex.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than zero or greater than the length of value minus 1.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ToUInt32(byte[] buffer, int startIndex)
    {
        return (uint)ToInt32(buffer, startIndex);
    }

    /// <summary>
    /// Returns a 64-bit unsigned integer converted from eight bytes, accounting for target endian-order, at a specified position in a byte array.
    /// </summary>
    /// <param name="value">An array of bytes (i.e., buffer containing binary image of value).</param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <returns>A 64-bit unsigned integer formed by eight bytes beginning at startIndex.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than zero or greater than the length of value minus 1.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ToUInt64(byte[] value, int startIndex)
    {
        return (ulong)ToInt64(value, startIndex);
    }

    #endregion

    #region [ GetBytes from Value ]

    /// <summary>
    /// Returns the specified value as an array of bytes in the target endian-order.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>An array of bytes with length 1.</returns>
    /// <typeparam name="T">Native value type to get bytes for.</typeparam>
    /// <exception cref="ArgumentException"><paramref name="value"/> type is not primitive.</exception>
    /// <exception cref="InvalidOperationException">Cannot get bytes for <paramref name="value"/> type.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] GetBytes<T>(T value) where T : struct, IConvertible
    {
        if (!typeof(T).IsPrimitive)
            throw new ArgumentException("Value type is not primitive", nameof(value));

        IConvertible nativeValue = value;

        return nativeValue.GetTypeCode() switch
        {
            TypeCode.Char => GetBytes(nativeValue.ToChar(null)),
            TypeCode.Boolean => GetBytes(nativeValue.ToBoolean(null)),
            TypeCode.Int16 => GetBytes(nativeValue.ToInt16(null)),
            TypeCode.UInt16 => GetBytes(nativeValue.ToUInt16(null)),
            TypeCode.Int32 => GetBytes(nativeValue.ToInt32(null)),
            TypeCode.UInt32 => GetBytes(nativeValue.ToUInt32(null)),
            TypeCode.Int64 => GetBytes(nativeValue.ToInt64(null)),
            TypeCode.UInt64 => GetBytes(nativeValue.ToUInt64(null)),
            TypeCode.Single => GetBytes(nativeValue.ToSingle(null)),
            TypeCode.Double => GetBytes(nativeValue.ToDouble(null)),
            TypeCode.Decimal => GetBytes(nativeValue.ToDecimal(null)),
            _ => throw new InvalidOperationException($"Cannot get bytes for value type {nativeValue.GetTypeCode()}")
        };
    }

    /// <summary>
    /// Returns the specified <see cref="bool"/> value as an array of bytes in the target endian-order.
    /// </summary>
    /// <param name="value">The <see cref="bool"/> value to convert.</param>
    /// <returns>An array of bytes with length 1.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] GetBytes(bool value)
    {
        return new[] { value ? (byte)1 : (byte)0 };
    }

    /// <summary>
    /// Returns the specified Unicode character value as an array of bytes in the target endian-order.
    /// </summary>
    /// <param name="value">The Unicode character value to convert.</param>
    /// <returns>An array of bytes with length 2.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] GetBytes(char value)
    {
        return GetBytes((short)value);
    }

    /// <summary>
    /// Returns the specified double-precision floating point value as an array of bytes in the target endian-order.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 8.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe byte[] GetBytes(double value)
    {
        return GetBytes(*(long*)&value);
    }

    /// <summary>
    /// Returns the specified 16-bit signed integer value as an array of bytes.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 2.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] GetBytes(short value)
    {
        return new[]
        {
            (byte)value,
            (byte)(value >> 8)
        };
    }

    /// <summary>
    /// Returns the specified 32-bit signed integer value as an array of bytes.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 4.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] GetBytes(int value)
    {
        return new[]
        {
            (byte)value,
            (byte)(value >> 8),
            (byte)(value >> 16),
            (byte)(value >> 24)
        };
    }

    /// <summary>
    /// Returns the specified 64-bit signed integer value as an array of bytes.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 8.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] GetBytes(long value)
    {
        return new[]
        {
            (byte)value,
            (byte)(value >> 8),
            (byte)(value >> 16),
            (byte)(value >> 24),
            (byte)(value >> 32),
            (byte)(value >> 40),
            (byte)(value >> 48),
            (byte)(value >> 56)
        };
    }

    /// <summary>
    /// Returns the specified single-precision floating point value as an array of bytes in the target endian-order.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 4.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe byte[] GetBytes(float value)
    {
        return GetBytes(*(int*)&value);
    }

    /// <summary>
    /// Returns the specified 16-bit unsigned integer value as an array of bytes.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 2.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] GetBytes(ushort value)
    {
        return GetBytes((short)value);
    }

    /// <summary>
    /// Returns the specified 32-bit unsigned integer value as an array of bytes.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 4.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] GetBytes(uint value)
    {
        return GetBytes((int)value);
    }

    /// <summary>
    /// Returns the specified 64-bit unsigned integer value as an array of bytes.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 8.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] GetBytes(ulong value)
    {
        return GetBytes((long)value);
    }

    /// <summary>
    /// Returns the specified 128-bit decimal value as an array of bytes.
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>An array of bytes with length 16.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe byte[] GetBytes(decimal value)
    {
        byte[] destinationArray = new byte[16];
        fixed (byte* destination = destinationArray)
        {
            if (BitConverter.IsLittleEndian)
            {
                *(decimal*)destination = value;
            }
            else
            {
                byte* ptr = (byte*)&value;
                //int flags
                destination[0] = ptr[3];
                destination[1] = ptr[2];
                destination[2] = ptr[1];
                destination[3] = ptr[0];
                //int high
                destination[4] = ptr[7];
                destination[5] = ptr[6];
                destination[6] = ptr[5];
                destination[7] = ptr[4];
                //int low
                destination[8] = ptr[11];
                destination[9] = ptr[10];
                destination[10] = ptr[9];
                destination[11] = ptr[8];
                //int mid
                destination[12] = ptr[15];
                destination[13] = ptr[14];
                destination[14] = ptr[13];
                destination[15] = ptr[12];
            }
        }
        return destinationArray;
    }

    #endregion

    #region [ Copy to Byte Array ]

    /// <summary>
    /// Copies the specified primitive type value as an array of bytes in the target endian-order to the destination array.
    /// </summary>
    /// <param name="value">The <see cref="bool"/> value to convert and copy.</param>
    /// <param name="destinationArray">The destination buffer.</param>
    /// <param name="destinationIndex">The byte offset into <paramref name="destinationArray"/>.</param>
    /// <typeparam name="T">Native value type to get bytes for.</typeparam>
    /// <exception cref="ArgumentException"><paramref name="value"/> type is not primitive.</exception>
    /// <exception cref="InvalidOperationException">Cannot get bytes for <paramref name="value"/> type.</exception>
    /// <returns>Length of bytes copied into array based on size of <typeparamref name="T"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CopyBytes<T>(T value, byte[] destinationArray, int destinationIndex) where T : struct, IConvertible
    {
        if (!typeof(T).IsPrimitive)
            throw new ArgumentException("Value type is not primitive", nameof(value));

        IConvertible nativeValue = value;

        return nativeValue.GetTypeCode() switch
        {
            TypeCode.Char => CopyBytes(nativeValue.ToChar(null), destinationArray, destinationIndex),
            TypeCode.Boolean => CopyBytes(nativeValue.ToBoolean(null), destinationArray, destinationIndex),
            TypeCode.Int16 => CopyBytes(nativeValue.ToInt16(null), destinationArray, destinationIndex),
            TypeCode.UInt16 => CopyBytes(nativeValue.ToUInt16(null), destinationArray, destinationIndex),
            TypeCode.Int32 => CopyBytes(nativeValue.ToInt32(null), destinationArray, destinationIndex),
            TypeCode.UInt32 => CopyBytes(nativeValue.ToUInt32(null), destinationArray, destinationIndex),
            TypeCode.Int64 => CopyBytes(nativeValue.ToInt64(null), destinationArray, destinationIndex),
            TypeCode.UInt64 => CopyBytes(nativeValue.ToUInt64(null), destinationArray, destinationIndex),
            TypeCode.Single => CopyBytes(nativeValue.ToSingle(null), destinationArray, destinationIndex),
            TypeCode.Double => CopyBytes(nativeValue.ToDouble(null), destinationArray, destinationIndex),
            TypeCode.Decimal => CopyBytes(nativeValue.ToDecimal(null), destinationArray, destinationIndex),
            _ => throw new InvalidOperationException($"Cannot copy bytes for value type {nativeValue.GetTypeCode()}")
        };
    }

    /// <summary>
    /// Copies the specified <see cref="bool"/> value as an array of 1 byte in the target endian-order to the destination array.
    /// </summary>
    /// <param name="value">The <see cref="bool"/> value to convert and copy.</param>
    /// <param name="destinationArray">The destination buffer.</param>
    /// <param name="destinationIndex">The byte offset into <paramref name="destinationArray"/>.</param>
    /// <returns>Length of bytes copied into array based on size of <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CopyBytes(bool value, byte[] destinationArray, int destinationIndex)
    {
        destinationArray[destinationIndex] = value ? (byte)1 : (byte)0;
        return 1;
    }

    /// <summary>
    /// Copies the specified Unicode character value as an array of 2 bytes in the target endian-order to the destination array.
    /// </summary>
    /// <param name="value">The Unicode character value to convert and copy.</param>
    /// <param name="destinationArray">The destination buffer.</param>
    /// <param name="destinationIndex">The byte offset into <paramref name="destinationArray"/>.</param>
    /// <returns>Length of bytes copied into array based on size of <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CopyBytes(char value, byte[] destinationArray, int destinationIndex)
    {
        return CopyBytes((short)value, destinationArray, destinationIndex);
    }

    /// <summary>
    /// Copies the specified double-precision floating point value as an array of 8 bytes in the target endian-order to the destination array.
    /// </summary>
    /// <param name="value">The number to convert and copy.</param>
    /// <param name="destinationArray">The destination buffer.</param>
    /// <param name="destinationIndex">The byte offset into <paramref name="destinationArray"/>.</param>
    /// <returns>Length of bytes copied into array based on size of <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int CopyBytes(double value, byte[] destinationArray, int destinationIndex)
    {
        return CopyBytes(*(long*)&value, destinationArray, destinationIndex);
    }

    /// <summary>
    /// Copies the specified 16-bit signed integer value as an array of 2 bytes in the target endian-order to the destination array.
    /// </summary>
    /// <param name="value">The number to convert and copy.</param>
    /// <param name="destinationArray">The destination buffer.</param>
    /// <param name="destinationIndex">The byte offset into <paramref name="destinationArray"/>.</param>
    /// <returns>Length of bytes copied into array based on size of <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CopyBytes(short value, byte[] destinationArray, int destinationIndex)
    {
        destinationArray[destinationIndex] = (byte)value;
        destinationArray[destinationIndex + 1] = (byte)(value >> 8);

        return 2;
    }

    /// <summary>
    /// Copies the specified 32-bit signed integer value as an array of 4 bytes in the target endian-order to the destination array.
    /// </summary>
    /// <param name="value">The number to convert and copy.</param>
    /// <param name="destinationArray">The destination buffer.</param>
    /// <param name="destinationIndex">The byte offset into <paramref name="destinationArray"/>.</param>
    /// <returns>Length of bytes copied into array based on size of <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CopyBytes(int value, byte[] destinationArray, int destinationIndex)
    {
        destinationArray[destinationIndex + 0] = (byte)value;
        destinationArray[destinationIndex + 1] = (byte)(value >> 8);
        destinationArray[destinationIndex + 2] = (byte)(value >> 16);
        destinationArray[destinationIndex + 3] = (byte)(value >> 24);

        return 4;
    }

    /// <summary>
    /// Copies the specified 64-bit signed integer value as an array of 8 bytes in the target endian-order to the destination array.
    /// </summary>
    /// <param name="value">The number to convert and copy.</param>
    /// <param name="destinationArray">The destination buffer.</param>
    /// <param name="destinationIndex">The byte offset into <paramref name="destinationArray"/>.</param>
    /// <returns>Length of bytes copied into array based on size of <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CopyBytes(long value, byte[] destinationArray, int destinationIndex)
    {
        destinationArray[destinationIndex + 0] = (byte)value;
        destinationArray[destinationIndex + 1] = (byte)(value >> 8);
        destinationArray[destinationIndex + 2] = (byte)(value >> 16);
        destinationArray[destinationIndex + 3] = (byte)(value >> 24);
        destinationArray[destinationIndex + 4] = (byte)(value >> 32);
        destinationArray[destinationIndex + 5] = (byte)(value >> 40);
        destinationArray[destinationIndex + 6] = (byte)(value >> 48);
        destinationArray[destinationIndex + 7] = (byte)(value >> 56);

        return 8;
    }

    /// <summary>
    /// Copies the specified single-precision floating point value as an array of 4 bytes in the target endian-order to the destination array.
    /// </summary>
    /// <param name="value">The number to convert and copy.</param>
    /// <param name="destinationArray">The destination buffer.</param>
    /// <param name="destinationIndex">The byte offset into <paramref name="destinationArray"/>.</param>
    /// <returns>Length of bytes copied into array based on size of <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int CopyBytes(float value, byte[] destinationArray, int destinationIndex)
    {
        return CopyBytes(*(int*)&value, destinationArray, destinationIndex);
    }

    /// <summary>
    /// Copies the specified 16-bit unsigned integer value as an array of 2 bytes in the target endian-order to the destination array.
    /// </summary>
    /// <param name="value">The number to convert and copy.</param>
    /// <param name="destinationArray">The destination buffer.</param>
    /// <param name="destinationIndex">The byte offset into <paramref name="destinationArray"/>.</param>
    /// <returns>Length of bytes copied into array based on size of <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CopyBytes(ushort value, byte[] destinationArray, int destinationIndex)
    {
        return CopyBytes((short)value, destinationArray, destinationIndex);
    }

    /// <summary>
    /// Copies the specified 32-bit unsigned integer value as an array of 4 bytes in the target endian-order to the destination array.
    /// </summary>
    /// <param name="value">The number to convert and copy.</param>
    /// <param name="destinationArray">The destination buffer.</param>
    /// <param name="destinationIndex">The byte offset into <paramref name="destinationArray"/>.</param>
    /// <returns>Length of bytes copied into array based on size of <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CopyBytes(uint value, byte[] destinationArray, int destinationIndex)
    {
        return CopyBytes((int)value, destinationArray, destinationIndex);
    }

    /// <summary>
    /// Copies the specified 64-bit unsigned integer value as an array of 8 bytes in the target endian-order to the destination array.
    /// </summary>
    /// <param name="value">The number to convert and copy.</param>
    /// <param name="destinationArray">The destination buffer.</param>
    /// <param name="destinationIndex">The byte offset into <paramref name="destinationArray"/>.</param>
    /// <returns>Length of bytes copied into array based on size of <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CopyBytes(ulong value, byte[] destinationArray, int destinationIndex)
    {
        return CopyBytes((long)value, destinationArray, destinationIndex);
    }

    #endregion

    #region [ Copy to Byte Pointer ]

    /// <summary>
    /// Copies the specified primitive type value as an array of bytes in the target endian-order to the destination array.
    /// </summary>
    /// <param name="value">The <see cref="bool"/> value to convert and copy.</param>
    /// <param name="destination">The destination buffer.</param>
    /// <typeparam name="T">Native value type to get bytes for.</typeparam>
    /// <exception cref="ArgumentException"><paramref name="value"/> type is not primitive.</exception>
    /// <exception cref="InvalidOperationException">Cannot get bytes for <paramref name="value"/> type.</exception>
    /// <returns>Length of bytes copied into array based on size of <typeparamref name="T"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int CopyBytes<T>(T value, byte* destination) where T : struct, IConvertible
    {
        if (!typeof(T).IsPrimitive)
            throw new ArgumentException("Value type is not primitive", nameof(value));

        IConvertible nativeValue = value;

        return nativeValue.GetTypeCode() switch
        {
            TypeCode.Char => CopyBytes(nativeValue.ToChar(null), destination),
            TypeCode.Boolean => CopyBytes(nativeValue.ToBoolean(null), destination),
            TypeCode.Int16 => CopyBytes(nativeValue.ToInt16(null), destination),
            TypeCode.UInt16 => CopyBytes(nativeValue.ToUInt16(null), destination),
            TypeCode.Int32 => CopyBytes(nativeValue.ToInt32(null), destination),
            TypeCode.UInt32 => CopyBytes(nativeValue.ToUInt32(null), destination),
            TypeCode.Int64 => CopyBytes(nativeValue.ToInt64(null), destination),
            TypeCode.UInt64 => CopyBytes(nativeValue.ToUInt64(null), destination),
            TypeCode.Single => CopyBytes(nativeValue.ToSingle(null), destination),
            TypeCode.Double => CopyBytes(nativeValue.ToDouble(null), destination),
            TypeCode.Decimal => CopyBytes(nativeValue.ToDecimal(null), destination),
            _ => throw new InvalidOperationException($"Cannot copy bytes for value type {nativeValue.GetTypeCode()}")
        };
    }

    /// <summary>
    /// Copies the specified <see cref="bool"/> value as an array of 1 byte in the target endian-order to the destination array.
    /// </summary>
    /// <param name="value">The <see cref="bool"/> value to convert and copy.</param>
    /// <param name="destination">The destination buffer.</param>
    /// <returns>Length of bytes copied into array based on size of <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int CopyBytes(bool value, byte* destination)
    {
        destination[0] = value ? (byte)1 : (byte)0;
        return 1;
    }

    /// <summary>
    /// Copies the specified Unicode character value as an array of 2 bytes in the target endian-order to the destination array.
    /// </summary>
    /// <param name="value">The Unicode character value to convert and copy.</param>
    /// <param name="destination">The destination buffer.</param>
    /// <returns>Length of bytes copied into array based on size of <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int CopyBytes(char value, byte* destination)
    {
        return CopyBytes((short)value, destination);
    }

    /// <summary>
    /// Copies the specified double-precision floating point value as an array of 8 bytes in the target endian-order to the destination array.
    /// </summary>
    /// <param name="value">The number to convert and copy.</param>
    /// <param name="destination">The destination buffer.</param>
    /// <returns>Length of bytes copied into array based on size of <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int CopyBytes(double value, byte* destination)
    {
        return CopyBytes(*(long*)&value, destination);
    }

    /// <summary>
    /// Copies the specified 16-bit signed integer value as an array of 2 bytes in the target endian-order to the destination array.
    /// </summary>
    /// <param name="value">The number to convert and copy.</param>
    /// <param name="destination">The destination buffer.</param>
    /// <returns>Length of bytes copied into array based on size of <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int CopyBytes(short value, byte* destination)
    {
        destination[0] = (byte)value;
        destination[1] = (byte)(value >> 8);

        return 2;
    }

    /// <summary>
    /// Copies the specified 32-bit signed integer value as an array of 4 bytes in the target endian-order to the destination array.
    /// </summary>
    /// <param name="value">The number to convert and copy.</param>
    /// <param name="destination">The destination buffer.</param>
    /// <returns>Length of bytes copied into array based on size of <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int CopyBytes(int value, byte* destination)
    {
        destination[0] = (byte)value;
        destination[1] = (byte)(value >> 8);
        destination[2] = (byte)(value >> 16);
        destination[3] = (byte)(value >> 24);

        return 4;
    }

    /// <summary>
    /// Copies the specified 64-bit signed integer value as an array of 8 bytes in the target endian-order to the destination array.
    /// </summary>
    /// <param name="value">The number to convert and copy.</param>
    /// <param name="destination">The destination buffer.</param>
    /// <returns>Length of bytes copied into array based on size of <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int CopyBytes(long value, byte* destination)
    {
        destination[0] = (byte)value;
        destination[1] = (byte)(value >> 8);
        destination[2] = (byte)(value >> 16);
        destination[3] = (byte)(value >> 24);
        destination[4] = (byte)(value >> 32);
        destination[5] = (byte)(value >> 40);
        destination[6] = (byte)(value >> 48);
        destination[7] = (byte)(value >> 56);

        return 8;
    }

    /// <summary>
    /// Copies the specified single-precision floating point value as an array of 4 bytes in the target endian-order to the destination array.
    /// </summary>
    /// <param name="value">The number to convert and copy.</param>
    /// <param name="destination">The destination buffer.</param>
    /// <returns>Length of bytes copied into array based on size of <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int CopyBytes(float value, byte* destination)
    {
        return CopyBytes(*(int*)&value, destination);
    }

    /// <summary>
    /// Copies the specified 16-bit unsigned integer value as an array of 2 bytes in the target endian-order to the destination array.
    /// </summary>
    /// <param name="value">The number to convert and copy.</param>
    /// <param name="destination">The destination buffer.</param>
    /// <returns>Length of bytes copied into array based on size of <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int CopyBytes(ushort value, byte* destination)
    {
        return CopyBytes((short)value, destination);
    }

    /// <summary>
    /// Copies the specified 32-bit unsigned integer value as an array of 4 bytes in the target endian-order to the destination array.
    /// </summary>
    /// <param name="value">The number to convert and copy.</param>
    /// <param name="destination">The destination buffer.</param>
    /// <returns>Length of bytes copied into array based on size of <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int CopyBytes(uint value, byte* destination)
    {
        return CopyBytes((int)value, destination);
    }

    /// <summary>
    /// Copies the specified 64-bit unsigned integer value as an array of 8 bytes in the target endian-order to the destination array.
    /// </summary>
    /// <param name="value">The number to convert and copy.</param>
    /// <param name="destination">The destination buffer.</param>
    /// <returns>Length of bytes copied into array based on size of <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int CopyBytes(ulong value, byte* destination)
    {
        return CopyBytes((long)value, destination);
    }

    /// <summary>
    /// Copies the specified 128-bit decimal value as an array of 16 bytes in the target endian-order to the destination array.
    /// </summary>
    /// <param name="value">The number to convert and copy.</param>
    /// <param name="destination">The destination buffer.</param>
    /// <returns>Length of bytes copied into array based on size of <paramref name="value"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int CopyBytes(decimal value, byte* destination)
    {
        if (BitConverter.IsLittleEndian)
        {
            *(decimal*)destination = value;
        }
        else
        {
            byte* ptr = (byte*)&value;
            //int flags
            destination[0] = ptr[3];
            destination[1] = ptr[2];
            destination[2] = ptr[1];
            destination[3] = ptr[0];
            //int high
            destination[4] = ptr[7];
            destination[5] = ptr[6];
            destination[6] = ptr[5];
            destination[7] = ptr[4];
            //int low
            destination[8] = ptr[11];
            destination[9] = ptr[10];
            destination[10] = ptr[9];
            destination[11] = ptr[8];
            //int mid
            destination[12] = ptr[15];
            destination[13] = ptr[14];
            destination[14] = ptr[13];
            destination[15] = ptr[12];
        }
        return 16;
    }

    #endregion
}