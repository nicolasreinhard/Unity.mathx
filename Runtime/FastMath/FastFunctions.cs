﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Burst;
using UnityEngine;

namespace Unity.Mathematics
{
    [BurstCompile]
    public static partial class Math
    {
        // https://gist.github.com/SaffronCR/b0802d102dd7f262118ac853cd5b4901#file-mathutil-cs-L24
        
        [StructLayout(LayoutKind.Explicit)]
        private struct FloatIntUnion
        {
            [FieldOffset(0)] public float f;
            [FieldOffset(0)] public int tmp;
        }

        /// <summary>Implementation of the fast inverse square root algorithm</summary>
        /// <remarks>https://gist.github.com/SaffronCR/b0802d102dd7f262118ac853cd5b4901#file-mathutil-cs-L24</remarks>
        [MethodImpl(INLINE)]
        public static float fsqrt(this float z)
        {
            if (z == 0) return 0;
            FloatIntUnion u;
            u.tmp = 0;
            u.f = z;
            u.tmp -= 1 << 23; // Subtract 2^m.
            u.tmp >>= 1; // Divide by 2.
            u.tmp += 1 << 29; // Add ((b + 1) / 2) * 2^m.
            return u.f;
        }

        
        /// <inheritdoc cref="fsqrt(float)"/>
        [MethodImpl(INLINE)]
        public static float4 fsqrt(this float4 f) => new(f.xy.fsqrt(), f.zw.fsqrt());
        /// <inheritdoc cref="fsqrt(float)"/>
        [MethodImpl(INLINE)]
        public static float3 fsqrt(this float3 f) => new(f.xy.fsqrt(), f.z.fsqrt());
        /// <inheritdoc cref="fsqrt(float)"/>
        [MethodImpl(INLINE)]
        public static float2 fsqrt(this float2 f) => new(f.x.fsqrt(), f.y.fsqrt()); // to never simplify to new float2(f.xy.fastsqrt())


        /// Returns the distance between a and b (fast but low accuracy)
        [MethodImpl(INLINE)]
        public static float fdistance(float4 a, float4 b) => fsqrt((a - b).lengthsq());
        /// <inheritdoc cref="fdistance(float4, float4)"/>
        [MethodImpl(INLINE)]
        public static float fdistance(float3 a, float3 b) => fsqrt((a - b).lengthsq());
        /// <inheritdoc cref="fdistance(float4, float4)"/>
        [MethodImpl(INLINE)]
        public static float fdistance(float2 a, float2 b) => fsqrt((a - b).lengthsq());
        

        /// Returns the length of the vector (fast but low accuracy)
        [MethodImpl(INLINE)]
        public static float flength(this float4 f) => fsqrt(f.lengthsq());
        /// <inheritdoc cref="flength(float4)"/>
        [MethodImpl(INLINE)]
        public static float flength(this float3 f) => fsqrt(f.lengthsq());
        /// <inheritdoc cref="flength(float4)"/>
        [MethodImpl(INLINE)]
        public static float flength(this float2 f) => fsqrt(f.lengthsq());
        
        /// <inheritdoc cref="flength(float4)"/>
        [MethodImpl(INLINE)]
        public static float flength(this Vector4 f) => fsqrt(f.lengthsq());
        /// <inheritdoc cref="flength(float4)"/>
        [MethodImpl(INLINE)]
        public static float flength(this Vector3 f) => fsqrt(f.lengthsq());
        /// <inheritdoc cref="flength(float4)"/>
        [MethodImpl(INLINE)]
        public static float flength(this Vector2 f) => fsqrt(f.lengthsq());
        
        // https://github.com/SunsetQuest/Fast-Integer-Log2 --------------------------
        [StructLayout(LayoutKind.Explicit)]
        private struct ConverterStruct2
        {
            [FieldOffset(0)] public ulong asLong;
            [FieldOffset(0)] public double asDouble;
        }

        // Same as Log2_SunsetQuest3 except it uses FP64.
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static int log2int(this int value)
        {
            ConverterStruct2 a;  a.asLong = 0; a.asDouble = (uint)value;
            return (int)((a.asLong >> 52) + 1) & 0xFF;
        }
        
        // MOD ---------------------------------------------------------------------

        /// fast mod function using the inverse Mod
        [MethodImpl(INLINE)]
        public static float fastmodinv(this int f, float invMod, float mod) => (f * invMod).frac() * mod;
        
        /// fast mod function
        /// <remarks>
        /// approx 5% faster than math.mod()
        /// It is also exact for negative values of x;
        /// </remarks>
        [MethodImpl(INLINE)]
        public static float4 mod(this float4 f, float4 mod) => (f / mod).frac() * mod;
        /// <inheritdoc cref="mod(float4, float4)"/>
        [MethodImpl(INLINE)]
        public static float3 mod(this float3 f, float3 mod) => (f / mod).frac() * mod;
        /// <inheritdoc cref="mod(float4, float4)"/>
        [MethodImpl(INLINE)]
        public static float2 mod(this float2 f, float2 mod) => (f / mod).frac() * mod;
        /// <inheritdoc cref="mod(float4, float4)"/>
        [MethodImpl(INLINE)]
        public static float mod(this float f, float mod) => (f / mod).frac() * mod;
        
        /// <inheritdoc cref="mod(float4, float4)"/>
        [MethodImpl(INLINE)]
        public static float4 mod(this float4 f, int4 mod) => (f / mod).frac() * mod;
        /// <inheritdoc cref="mod(float4, float4)"/>
        [MethodImpl(INLINE)]
        public static float3 mod(this float3 f, int3 mod) => (f / mod).frac() * mod;
        /// <inheritdoc cref="mod(float4, float4)"/>
        [MethodImpl(INLINE)]
        public static float2 mod(this float2 f, int2 mod) => (f / mod).frac() * mod;
        /// <inheritdoc cref="mod(float4, float4)"/>
        [MethodImpl(INLINE)]
        public static float mod(this float f, int mod) => (f / mod).frac() * mod;
        /// <inheritdoc cref="mod(float4, float4)"/>
        [MethodImpl(INLINE)]
        public static float mod(this int f, int mod) => frac(f / mod) * mod;
        

        /// <inheritdoc cref="mod(float4, float4)"/>
        [MethodImpl(INLINE)]
        public static float2 mod(this float2 f, float mod) => (f / mod).frac() * mod;
        /// <inheritdoc cref="mod(float4, float4)"/>
        [MethodImpl(INLINE)]
        public static float3 mod(this float3 f, float mod) => (f / mod).frac() * mod;
        /// <inheritdoc cref="mod(float4, float4)"/>
        [MethodImpl(INLINE)]
        public static float4 mod(this float4 f, float mod) => (f / mod).frac() * mod;
        /// <inheritdoc cref="mod(float4, float4)"/>
        [MethodImpl(INLINE)]
        public static float2 mod(this int f, float mod) => (f / mod).frac() * mod;
        
        /// <inheritdoc cref="mod(float4, float4)"/>
        [MethodImpl(INLINE)]
        public static float4 mod(this float4 f, int mod) => (f / mod).frac() * mod;
        /// <inheritdoc cref="mod(float4, float4)"/>
        [MethodImpl(INLINE)]
        public static float3 mod(this float3 f, int mod) => (f / mod).frac() * mod;
        /// <inheritdoc cref="mod(float4, float4)"/>
        [MethodImpl(INLINE)]
        public static float2 mod(this float2 f, int mod) => (f / mod).frac() * mod;


        [StructLayout(LayoutKind.Explicit)]
        private struct FloatUInt32Union
        {
            [FieldOffset(0)] public float f;
            [FieldOffset(0)] public uint u;
        }

        private static FloatUInt32Union fiu;
        
        /// returns 1/x using fast math
        public static float frcp(this float x)
        {
            fiu.f = x;
            fiu.u = (0xbe6eb3beU - fiu.u) >> 1; // pow( x, -0.5 )
            return fiu.f * fiu.f; // pow( pow(x,-0.5), 2 ) = pow( x, -1 ) = 1.0 / x
        }
        
        /// returns 1/x using fast math
        public static float frcp(this int x)
        {
            fiu.f = x;
            fiu.u = (0xbe6eb3beU - fiu.u) >> 1; // pow( x, -0.5 )
            return fiu.f * fiu.f; // pow( pow(x,-0.5), 2 ) = pow( x, -1 ) = 1.0 / x
        }
        
        
        // frac --------------------------------------------------

        /// <summary>Returns the fractional part of a float value.</summary>
        /// <remarks>Fractional Remainder (f - (int)f) is x3 faster than math.frac() </remarks>
        [MethodImpl(INLINE)]
        public static float4 frac(this float4 f) => f - (int4)f;
        /// <inheritdoc cref="frac(float4)"/>
        [MethodImpl(INLINE)]
        public static float3 frac(this float3 f) => f - (int3)f;
        /// <inheritdoc cref="frac(float4)"/>
        [MethodImpl(INLINE)]
        public static float2 frac(this float2 f) => f - (int2)f;
        /// <inheritdoc cref="frac(float4)"/>
        [MethodImpl(INLINE)]
        public static float frac(this float f) => f - (int)f;
        
        /// <inheritdoc cref="frac(float4)"/>
        [MethodImpl(INLINE)]
        public static float4 frac(this Vector4 f) => f.cast() - f.asint();
        /// <inheritdoc cref="frac(float4)"/>
        [MethodImpl(INLINE)]
        public static float3 frac(this Vector3 f) => f.cast() - f.asint();
        /// <inheritdoc cref="frac(float4)"/>
        [MethodImpl(INLINE)]
        public static float2 frac(this Vector2 f) => f.cast() - f.asint();
        
        /// <inheritdoc cref="frac(float4)"/>
        [MethodImpl(INLINE)]
        public static double4 frac(this double4 f) => f - (int4)f;
        /// <inheritdoc cref="frac(float4)"/>
        [MethodImpl(INLINE)]
        public static double3 frac(this double3 f) => f - (int3)f;
        /// <inheritdoc cref="frac(float4)"/>
        [MethodImpl(INLINE)]
        public static double2 frac(this double2 f) => f - (int2)f;
        /// <inheritdoc cref="frac(float4)"/>
        [MethodImpl(INLINE)]
        public static double frac(this double f) => f - (int)f;
        
    }
}